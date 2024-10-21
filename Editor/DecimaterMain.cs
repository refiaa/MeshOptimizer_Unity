using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class DecimaterMain : EditorWindow
{
    private const float DEFAULT_DECIMATE_LEVEL = 1.0f;
    private const string MESH_SUFFIX = "_decimated";

    private GameObject selectedGameObject;
    private Mesh originalMesh;
    private Mesh decimatedMesh;

    private MeshPreviewer meshPreviewer;
    private MeshInfoDisplay meshInfoDisplay;

    private Material[] originalMaterials;
    private int[] originalSubmeshCount;
    private Material previewMaterial;
    private Shader previewShader;

    private float decimateLevel = DEFAULT_DECIMATE_LEVEL;

    private bool isFirstDecimation = true;

    [MenuItem("MeshOptimizer/Mesh Optimizer GUI")]
    public static void ShowWindow()
    {
        GetWindow<DecimaterMain>("Mesh Optimizer GUI");
    }

    private void OnEnable()
    {
        LoadShaders();
        previewMaterial = CreatePreviewMaterial();

        meshPreviewer = new MeshPreviewer(previewMaterial);
        meshInfoDisplay = new MeshInfoDisplay();
    }

    private void OnSelectionChange()
    {
        UpdateSelection(Selection.activeGameObject);
    }

    private void OnGUI()
    {
        GUILayout.Label("Select a GameObject", EditorStyles.boldLabel);

        GameObject newSelectedGameObject = (GameObject)EditorGUILayout.ObjectField("GameObject", selectedGameObject, typeof(GameObject), true);
        if (newSelectedGameObject != selectedGameObject)
        {
            UpdateSelection(newSelectedGameObject);
        }

        if (selectedGameObject == null)
        {
            EditorGUILayout.HelpBox("No GameObject selected. Please select a GameObject with a MeshFilter or SkinnedMeshRenderer component.", MessageType.Warning);
            return;
        }

        GUILayout.Space(10);
        GUILayout.Label("Mesh Preview", EditorStyles.boldLabel);
        Rect previewRect = GUILayoutUtility.GetRect(400, 400);
        meshPreviewer.PreviewMesh(selectedGameObject, previewRect);

        GUILayout.Space(10);
        GUILayout.Label("Optimize Level", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        decimateLevel = EditorGUILayout.Slider("Optimize Level", decimateLevel, 0.1f, 1.0f);
        if (EditorGUI.EndChangeCheck())
        {
            if (originalMesh != null)
            {
                MeshRevertManager.StoreDecimateLevel(originalMesh, decimateLevel);
            }
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Apply Optimization"))
        {
            ApplyDecimation();
        }

        if (GUILayout.Button("Revert"))
        {
            RevertDecimation();
        }

        if (GUILayout.Button("Revert to Original"))
        {
            RevertToOriginalMesh();
        }

        GUILayout.Space(10);
        meshInfoDisplay.DisplayMeshInfo(GetCurrentMesh());
    }

    private void ApplyDecimation()
    {
        bool isSkinnedMeshRenderer = selectedGameObject.GetComponent<SkinnedMeshRenderer>() != null;
        int boneCount = 0;

        if (isSkinnedMeshRenderer)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = selectedGameObject.GetComponent<SkinnedMeshRenderer>();
            boneCount = skinnedMeshRenderer.bones.Length;
        }

        EnableReadWrite(originalMesh);

        MeshDecimaterUtility.DecimateMesh(originalMesh, decimatedMesh, decimateLevel, isSkinnedMeshRenderer, originalSubmeshCount, boneCount);

        decimatedMesh.RecalculateNormals();
        decimatedMesh.RecalculateBounds();

        SaveDecimatedMesh();

        MeshRevertManager.StoreOriginalMesh(decimatedMesh, originalMesh);

        MeshRevertManager.StoreDecimateLevel(originalMesh, decimateLevel);

        EditorApplication.delayCall += () =>
        {
            ApplyMeshToRenderer(isSkinnedMeshRenderer);
        };

        meshPreviewer.UpdatePreviewMesh(selectedGameObject);

        isFirstDecimation = false;
    }

    private void ApplyMeshToRenderer(bool isSkinnedMeshRenderer)
    {
        if (selectedGameObject == null) return;

        if (selectedGameObject.GetComponent<MeshFilter>() != null)
        {
            MeshFilter meshFilter = selectedGameObject.GetComponent<MeshFilter>();
            meshFilter.sharedMesh = decimatedMesh;
            MeshRenderer meshRenderer = selectedGameObject.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterials = originalMaterials;
        }
        else if (isSkinnedMeshRenderer)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = selectedGameObject.GetComponent<SkinnedMeshRenderer>();

            skinnedMeshRenderer.sharedMesh = null;

            skinnedMeshRenderer.sharedMesh = decimatedMesh;
            skinnedMeshRenderer.sharedMaterials = originalMaterials;

            skinnedMeshRenderer.updateWhenOffscreen = true;

            Animator animator = selectedGameObject.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Rebind();
            }

            ValidateBoneWeights(skinnedMeshRenderer);
        }
    }

    private void ValidateBoneWeights(SkinnedMeshRenderer skinnedMeshRenderer)
    {
        Mesh mesh = skinnedMeshRenderer.sharedMesh;
        int boneCount = skinnedMeshRenderer.bones.Length;

        for (int i = 0; i < mesh.boneWeights.Length; i++)
        {
            BoneWeight bw = mesh.boneWeights[i];
            if (bw.boneIndex0 >= boneCount || bw.boneIndex1 >= boneCount || bw.boneIndex2 >= boneCount || bw.boneIndex3 >= boneCount)
            {
                Debug.LogError($"Invalid bone index detected in boneWeights at vertex {i}: boneIndex0={bw.boneIndex0}, boneIndex1={bw.boneIndex1}, boneIndex2={bw.boneIndex2}, boneIndex3={bw.boneIndex3}");
            }
        }
    }

    private void RevertDecimation()
    {
        if (selectedGameObject.GetComponent<MeshFilter>() != null)
        {
            MeshFilter meshFilter = selectedGameObject.GetComponent<MeshFilter>();
            meshFilter.sharedMesh = originalMesh;
            MeshRenderer meshRenderer = selectedGameObject.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterials = originalMaterials;
        }
        else if (selectedGameObject.GetComponent<SkinnedMeshRenderer>() != null)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = selectedGameObject.GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.sharedMesh = originalMesh;
            skinnedMeshRenderer.sharedMaterials = originalMaterials;
        }

        decimateLevel = DEFAULT_DECIMATE_LEVEL;
        MeshRevertManager.StoreDecimateLevel(originalMesh, decimateLevel);

        meshPreviewer.UpdatePreviewMesh(selectedGameObject);

        isFirstDecimation = true;
    }

    private void RevertToOriginalMesh()
    {
        Mesh currentMesh = GetCurrentMesh();
        if (currentMesh == null)
        {
            Debug.LogWarning("No mesh to revert.");
            return;
        }

        Mesh originalMeshFromManager = MeshRevertManager.GetOriginalMesh(currentMesh);
        if (originalMeshFromManager != null)
        {
            if (selectedGameObject.GetComponent<MeshFilter>() != null)
            {
                MeshFilter meshFilter = selectedGameObject.GetComponent<MeshFilter>();
                meshFilter.sharedMesh = originalMeshFromManager;
                MeshRenderer meshRenderer = selectedGameObject.GetComponent<MeshRenderer>();
                meshRenderer.sharedMaterials = originalMaterials;
            }
            else if (selectedGameObject.GetComponent<SkinnedMeshRenderer>() != null)
            {
                SkinnedMeshRenderer skinnedMeshRenderer = selectedGameObject.GetComponent<SkinnedMeshRenderer>();
                skinnedMeshRenderer.sharedMesh = originalMeshFromManager;
                skinnedMeshRenderer.sharedMaterials = originalMaterials;
            }
            Debug.Log("Reverted to original mesh.");

            decimateLevel = DEFAULT_DECIMATE_LEVEL;
            MeshRevertManager.StoreDecimateLevel(originalMeshFromManager, decimateLevel);
            meshInfoDisplay.SetOriginalMesh(originalMeshFromManager);
        }
        else
        {
            Debug.LogWarning("Original mesh not found.");

            decimateLevel = DEFAULT_DECIMATE_LEVEL;
            MeshRevertManager.StoreDecimateLevel(currentMesh, decimateLevel);
            meshInfoDisplay.SetOriginalMesh(currentMesh);
        }

        meshPreviewer.UpdatePreviewMesh(selectedGameObject);

        isFirstDecimation = true;
    }

    private void LoadShaders()
    {
        previewShader = Shader.Find("Refiaa/Wireframe");
    }

    private void UpdateSelection(GameObject newSelectedGameObject)
    {
        if (newSelectedGameObject != selectedGameObject)
        {
            isFirstDecimation = true;
        }

        if (newSelectedGameObject != null)
        {
            Mesh original = GetOriginalMesh(newSelectedGameObject);
            if (original != null)
            {
                selectedGameObject = newSelectedGameObject;
                originalMesh = original;
                EnableReadWrite(originalMesh);

                decimateLevel = MeshRevertManager.GetDecimateLevel(originalMesh);

                string actualMeshName = GetActualMeshName();
                string originalPath = AssetDatabase.GetAssetPath(originalMesh);
                string directory = Path.GetDirectoryName(originalPath);
                string newFileName = $"{actualMeshName}{MESH_SUFFIX}.asset";
                string newPath = $"{directory}/{newFileName}";

                Mesh existingDecimatedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(newPath);
                if (existingDecimatedMesh != null)
                {
                    decimatedMesh = existingDecimatedMesh;
                }
                else
                {
                    decimatedMesh = new Mesh();
                    decimatedMesh.name = $"{actualMeshName}{MESH_SUFFIX}";
                    AssetDatabase.CreateAsset(decimatedMesh, newPath);
                    AssetDatabase.SaveAssets();
                }

                meshInfoDisplay.SetOriginalMesh(originalMesh);

                Renderer renderer = newSelectedGameObject.GetComponent<Renderer>();
                originalMaterials = renderer.sharedMaterials;
                originalSubmeshCount = new int[originalMesh.subMeshCount];
                for (int i = 0; i < originalMesh.subMeshCount; i++)
                {
                    originalSubmeshCount[i] = originalMesh.GetTriangles(i).Length / 3;
                }

                Selection.activeObject = originalMesh;
                Debug.Log($"Selected Mesh: {AssetDatabase.GetAssetPath(originalMesh)}");
                Repaint();
            }
        }
    }

    private Mesh GetOriginalMesh(GameObject gameObject)
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            Mesh mesh = meshFilter.sharedMesh;
            if (mesh.name.EndsWith(MESH_SUFFIX))
            {
                Mesh original = MeshRevertManager.GetOriginalMesh(mesh);
                return original != null ? original : mesh;
            }
            return mesh;
        }

        SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null)
        {
            Mesh mesh = skinnedMeshRenderer.sharedMesh;
            if (mesh.name.EndsWith(MESH_SUFFIX))
            {
                Mesh original = MeshRevertManager.GetOriginalMesh(mesh);
                return original != null ? original : mesh;
            }
            return mesh;
        }

        return null;
    }

    private void SaveDecimatedMesh()
    {
        string actualMeshName = GetActualMeshName();

        string originalPath = AssetDatabase.GetAssetPath(originalMesh);
        string directory = Path.GetDirectoryName(originalPath);
        string newFileName = $"{actualMeshName}{MESH_SUFFIX}.asset";

        if (actualMeshName.EndsWith(MESH_SUFFIX))
        {
            newFileName = $"{RemoveDecimatedSuffix(actualMeshName)}{MESH_SUFFIX}.asset";
        }

        string newPath = $"{directory}/{newFileName}";

        Mesh existingMesh = AssetDatabase.LoadAssetAtPath<Mesh>(newPath);
        if (existingMesh != null)
        {
            EditorUtility.CopySerialized(decimatedMesh, existingMesh);
            AssetDatabase.SaveAssets();
            Debug.Log($"Decimated mesh updated: {newPath}");
        }
        else
        {
            decimatedMesh.name = $"{RemoveDecimatedSuffix(actualMeshName)}{MESH_SUFFIX}";
            AssetDatabase.CreateAsset(decimatedMesh, newPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"Decimated mesh saved: {newPath}");
        }

        decimatedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(newPath);
    }

    private string GetActualMeshName()
    {
        if (selectedGameObject == null) return "Unknown";

        MeshFilter meshFilter = selectedGameObject.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            return RemoveCloneAndDecimatedSuffix(meshFilter.sharedMesh.name);
        }

        SkinnedMeshRenderer skinnedMeshRenderer = selectedGameObject.GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null)
        {
            return RemoveCloneAndDecimatedSuffix(skinnedMeshRenderer.sharedMesh.name);
        }

        return "Unknown";
    }

    private string RemoveCloneAndDecimatedSuffix(string meshName)
    {
        // erase "(Clone)" 
        if (meshName.EndsWith("(Clone)"))
        {
            meshName = meshName.Substring(0, meshName.Length - "(Clone)".Length);
        }
        // erase "_decimated" 
        if (meshName.EndsWith(MESH_SUFFIX))
        {
            meshName = meshName.Substring(0, meshName.Length - MESH_SUFFIX.Length);
        }
        return meshName;
    }

    private string RemoveDecimatedSuffix(string meshName)
    {
        if (meshName.EndsWith(MESH_SUFFIX))
        {
            return meshName.Substring(0, meshName.Length - MESH_SUFFIX.Length);
        }
        return meshName;
    }

    private Mesh GetCurrentMesh()
    {
        if (selectedGameObject != null)
        {
            if (selectedGameObject.GetComponent<MeshFilter>() != null)
            {
                return selectedGameObject.GetComponent<MeshFilter>().sharedMesh;
            }
            else if (selectedGameObject.GetComponent<SkinnedMeshRenderer>() != null)
            {
                return selectedGameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
            }
        }
        return null;
    }

    private void EnableReadWrite(Mesh mesh)
    {
        string path = AssetDatabase.GetAssetPath(mesh);
        if (string.IsNullOrEmpty(path)) return;

        ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
        if (modelImporter != null)
        {
            if (!modelImporter.isReadable)
            {
                modelImporter.isReadable = true;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }
    }

    private Material CreatePreviewMaterial()
    {
        if (previewShader == null)
        {
            Debug.LogError("Preview shader not found!");
            return null;
        }

        Material material = new Material(previewShader)
        {
            name = "Preview Material"
        };

        material.SetFloat("_Glossiness", 0.0f);
        material.SetFloat("_Metallic", 0.0f);
        material.SetColor("_Color", Color.white);
        material.SetColor("_EmissionColor", Color.white);

        material.EnableKeyword("_EMISSION");

        material.SetColor("_WireColor", Color.black);

        return material;
    }
}
