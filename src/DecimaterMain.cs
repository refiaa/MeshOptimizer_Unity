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
            EditorGUILayout.HelpBox("No GameObject selected. Please select a GameObject with a MeshFilter component.", MessageType.Warning);
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
        meshInfoDisplay.DisplayMeshInfo(decimatedMesh);
    }

    private void UpdateSelection(GameObject newSelectedGameObject)
    {
        if (newSelectedGameObject != null && newSelectedGameObject.GetComponent<MeshFilter>() != null)
        {
            selectedGameObject = newSelectedGameObject;
            originalMesh = selectedGameObject.GetComponent<MeshFilter>().sharedMesh;
            decimatedMesh = Instantiate(originalMesh);
            Selection.activeObject = originalMesh;
            Debug.Log($"Selected Mesh: {AssetDatabase.GetAssetPath(originalMesh)}");
            Repaint();
        }
    }

    private void ApplyDecimation()
    {
        MeshDecimaterUtility.DecimateMesh(decimatedMesh, decimateLevel);
        selectedGameObject.GetComponent<MeshFilter>().sharedMesh = decimatedMesh;
        meshPreviewer.UpdatePreviewMesh(selectedGameObject);
    }

    private void RevertDecimation()
    {
        selectedGameObject.GetComponent<MeshFilter>().sharedMesh = originalMesh;
        decimateLevel = 1.0f;
        meshPreviewer.UpdatePreviewMesh(selectedGameObject);
    }
}
