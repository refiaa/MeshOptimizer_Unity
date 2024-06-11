using UnityEngine;
using UnityEditor;

public class MeshDecimater : EditorWindow
{
    private GameObject selectedGameObject;
    private Editor previewEditor;
    private Material previewMaterial;
    private Material wireframeMaterial;

    [MenuItem("Decimater/MeshDecimater")]
    public static void ShowWindow()
    {
        GetWindow<MeshDecimater>("Decimater for Unity");
    }

    private void OnEnable()
    {
        previewMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/MeshDecimater/Material/preview.mat");
        wireframeMaterial = new Material(Shader.Find("Refiaa/Wireframe"));
    }

    private void OnSelectionChange()
    {
        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<MeshFilter>() != null)
        {
            selectedGameObject = Selection.activeGameObject;
            previewEditor = Editor.CreateEditor(selectedGameObject);
            Repaint();
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Select a GameObject", EditorStyles.boldLabel);

        selectedGameObject = (GameObject)EditorGUILayout.ObjectField("GameObject", selectedGameObject, typeof(GameObject), true);

        if (selectedGameObject != null && selectedGameObject.GetComponent<MeshFilter>() == null)
        {
            EditorUtility.DisplayDialog("Invalid Selection", "The selected GameObject does not have a Meshcomponent.", "OK");
            selectedGameObject = null;
        }

        GUILayout.Space(10);

        GUILayout.Label("Mesh Preview", EditorStyles.boldLabel);

        Rect previewRect = GUILayoutUtility.GetRect(400, 400);
        if (selectedGameObject != null)
        {
            if (previewEditor == null || previewEditor.target != selectedGameObject)
            {
                previewEditor = Editor.CreateEditor(selectedGameObject);
            }

            if (previewMaterial != null && wireframeMaterial != null)
            {
                var meshRenderer = selectedGameObject.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    var originalMaterials = meshRenderer.sharedMaterials;
                    meshRenderer.sharedMaterials = new Material[] { previewMaterial, wireframeMaterial };

                    previewEditor.OnInteractivePreviewGUI(previewRect, EditorStyles.whiteLabel);

                    meshRenderer.sharedMaterials = originalMaterials;
                }
                else
                {
                    previewEditor.OnInteractivePreviewGUI(previewRect, EditorStyles.whiteLabel);
                }
            }
            else
            {
                previewEditor.OnInteractivePreviewGUI(previewRect, EditorStyles.whiteLabel);
            }
        }
        else
        {
            EditorGUI.DrawRect(previewRect, Color.gray);
            GUI.Label(previewRect, "No GameObject selected", EditorStyles.centeredGreyMiniLabel);
        }

        GUILayout.Space(10);

        if (selectedGameObject != null)
        {
            DisplayMeshInfo(selectedGameObject.GetComponent<MeshFilter>().sharedMesh);
        }
    }

    private void DisplayMeshInfo(Mesh mesh)
    {
        if (mesh == null) return;

        GUILayout.BeginVertical("box");
        GUILayout.Label("Mesh Information", EditorStyles.boldLabel);
        GUILayout.Label($"Vertices: {mesh.vertexCount}");
        GUILayout.Label($"Triangles: {mesh.triangles.Length / 3}");
        GUILayout.Label($"Submeshes: {mesh.subMeshCount}");
        GUILayout.Label($"Mesh Size: {CalculateMeshSize(mesh)} KB");
        GUILayout.EndVertical();
    }

    private float CalculateMeshSize(Mesh mesh)
    {
        float size = 0;
        size += mesh.vertexCount * sizeof(float) * 3; // vertices
        size += mesh.normals.Length * sizeof(float) * 3; // normals
        size += mesh.uv.Length * sizeof(float) * 2; // UVs
        size += mesh.triangles.Length * sizeof(int); // triangles

        return size / 1024; // convert to KB
    }

    private void OnDisable()
    {
        if (previewEditor != null)
        {
            DestroyImmediate(previewEditor);
        }
    }
}
