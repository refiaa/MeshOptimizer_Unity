using UnityEngine;
using UnityEditor;

public class DecimaterMain : EditorWindow
{
    private GameObject selectedGameObject;
    private Mesh originalMesh;
    private Mesh decimatedMesh;
    private MeshPreviewer meshPreviewer;
    private MeshInfoDisplay meshInfoDisplay;
    private float decimateLevel = 1.0f;

    private Material previewMaterial;
    private Material wireframeMaterial;

    [MenuItem("Decimater/MeshDecimater")]
    public static void ShowWindow()
    {
        GetWindow<DecimaterMain>("Decimater for Unity");
    }

    private void OnEnable()
    {
        previewMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/MeshDecimater_Unity/Material/preview.mat");
        wireframeMaterial = new Material(Shader.Find("Refiaa/Wireframe"));

        meshPreviewer = new MeshPreviewer(previewMaterial, wireframeMaterial);
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
        decimateLevel = EditorGUILayout.Slider("Decimate Level", decimateLevel, 0.1f, 1.0f);

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

    private void UpdateSelection(GameObject newSelectedGameObject)
    {
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
                Selection.activeObject = originalMesh;
                Debug.Log($"Selected Mesh: {AssetDatabase.GetAssetPath(originalMesh)}");
                Repaint();
            }
        }
    }

    private void ApplyDecimation()
    {
        EnableReadWrite(decimatedMesh);
        MeshDecimaterUtility.DecimateMesh(decimatedMesh, decimateLevel);

        if (selectedGameObject.GetComponent<MeshFilter>() != null)
        {
            selectedGameObject.GetComponent<MeshFilter>().sharedMesh = decimatedMesh;
        }
        else if (selectedGameObject.GetComponent<SkinnedMeshRenderer>() != null)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = selectedGameObject.GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.sharedMesh = decimatedMesh;
            skinnedMeshRenderer.sharedMesh.bindposes = originalMesh.bindposes;
            skinnedMeshRenderer.sharedMesh.boneWeights = originalMesh.boneWeights;
        }

        meshPreviewer.UpdatePreviewMesh(selectedGameObject);
    }

    private void RevertDecimation()
    {
        if (selectedGameObject.GetComponent<MeshFilter>() != null)
        {
            selectedGameObject.GetComponent<MeshFilter>().sharedMesh = originalMesh;
        }
        else if (selectedGameObject.GetComponent<SkinnedMeshRenderer>() != null)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = selectedGameObject.GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.sharedMesh = originalMesh;
            skinnedMeshRenderer.sharedMesh.bindposes = originalMesh.bindposes;
            skinnedMeshRenderer.sharedMesh.boneWeights = originalMesh.boneWeights;
        }

        decimateLevel = 1.0f;
        meshPreviewer.UpdatePreviewMesh(selectedGameObject);
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
}
