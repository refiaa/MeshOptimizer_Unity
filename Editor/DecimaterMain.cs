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

    //private string saveFolder = "Assets/";
    
    [MenuItem("Decimater/MeshDecimater")]
    public static void ShowWindow()
    {
        GetWindow<DecimaterMain>("Decimater for Unity");
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
        GUILayout.Label("Decimate Level", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        decimateLevel = EditorGUILayout.Slider("Decimate Level", decimateLevel, 0.1f, 1.0f);
        if (EditorGUI.EndChangeCheck())
        {
            // TODO:decimateLevelが変更された時の値の保存ロジックを追加したい。
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Apply Decimation"))
        {
            ApplyDecimation();
        }
        
        if (GUILayout.Button("Revert"))
        {
            RevertDecimation();
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
        meshPreviewer.UpdatePreviewMesh(selectedGameObject);
    }

    private void LoadShaders()
    {
        previewShader = Shader.Find("Refiaa/Wireframe");
    }

    private void UpdateSelection(GameObject newSelectedGameObject)
    {
        if (newSelectedGameObject != selectedGameObject)
        {
            decimateLevel = DEFAULT_DECIMATE_LEVEL;
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

    // public void SelectSaveFolder()
    // {
    //     string selectedPath = EditorUtility.OpenFolderPanel("Select saved folder", saveFolder, "");
    //     var match = Regex.Match(selectedPath, @"Assets/.*");
    //     saveFolder = match.Success ? match.Value : "Assets/";
    // }
}
