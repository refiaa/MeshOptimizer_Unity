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

    private float decimateLevel = 1.0f;

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
            Mesh currentMesh = GetCurrentMesh();
            if (currentMesh != null)
            {
                MeshRevertManager.StoreDecimateLevel(currentMesh, decimateLevel);
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

        EnableReadWrite(decimatedMesh);
        MeshDecimaterUtility.DecimateMesh(originalMesh, decimatedMesh, decimateLevel, isSkinnedMeshRenderer, originalSubmeshCount);

        SaveDecimatedMesh();

        MeshRevertManager.StoreOriginalMesh(decimatedMesh, originalMesh);

        MeshRevertManager.StoreDecimateLevel(decimatedMesh, decimateLevel);

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
            skinnedMeshRenderer.sharedMesh = decimatedMesh;
            skinnedMeshRenderer.sharedMaterials = originalMaterials;
        }

        meshPreviewer.UpdatePreviewMesh(selectedGameObject);

        // errorめんどいの
        if (isFirstDecimation)
        {
            isFirstDecimation = false;
            Debug.LogWarning("First decimation performed. Applying decimation again to prevent mesh data mismatch error.");
            ApplyDecimation();
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

        decimateLevel = 1.0f;
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

            decimateLevel = 1.0f;
            MeshRevertManager.StoreDecimateLevel(originalMeshFromManager, decimateLevel);
        }
        else
        {
            Debug.LogWarning("Original mesh not found.");

            decimateLevel = 1.0f;
            MeshRevertManager.StoreDecimateLevel(currentMesh, decimateLevel);
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
            MeshFilter meshFilter = newSelectedGameObject.GetComponent<MeshFilter>();
            SkinnedMeshRenderer skinnedMeshRenderer = newSelectedGameObject.GetComponent<SkinnedMeshRenderer>();

            if (meshFilter != null)
            {
                selectedGameObject = newSelectedGameObject;
                originalMesh = meshFilter.sharedMesh;
                EnableReadWrite(originalMesh);

                decimateLevel = MeshRevertManager.GetDecimateLevel(originalMesh);

                decimatedMesh = Instantiate(originalMesh);
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
            else if (skinnedMeshRenderer != null)
            {
                selectedGameObject = newSelectedGameObject;
                originalMesh = skinnedMeshRenderer.sharedMesh;
                EnableReadWrite(originalMesh);

                decimateLevel = MeshRevertManager.GetDecimateLevel(originalMesh);

                decimatedMesh = Instantiate(originalMesh);
                meshInfoDisplay.SetOriginalMesh(originalMesh);

                originalMaterials = skinnedMeshRenderer.sharedMaterials;
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

    private void SaveDecimatedMesh()
    {
        string actualMeshName = GetActualMeshName();

        string originalPath = AssetDatabase.GetAssetPath(originalMesh);
        string directory = Path.GetDirectoryName(originalPath);
        string newFileName = $"{actualMeshName}{MESH_SUFFIX}.asset";
        string newPath = Path.Combine(directory, newFileName);

        Mesh existingMesh = AssetDatabase.LoadAssetAtPath<Mesh>(newPath);
        if (existingMesh != null)
        {
            EditorUtility.CopySerialized(decimatedMesh, existingMesh);
            AssetDatabase.SaveAssets();
            Debug.Log($"Decimated mesh updated: {newPath}");
        }
        else
        {
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
            return meshFilter.sharedMesh.name;
        }

        SkinnedMeshRenderer skinnedMeshRenderer = selectedGameObject.GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null)
        {
            return skinnedMeshRenderer.sharedMesh.name;
        }

        return "Unknown";
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
            modelImporter.isReadable = true;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
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
