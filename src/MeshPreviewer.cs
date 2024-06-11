using UnityEngine;
using UnityEditor;

public class MeshPreviewer
{
    private Material previewMaterial;
    private Material wireframeMaterial;
    private Editor cachedEditor;

    public MeshPreviewer(Material previewMaterial, Material wireframeMaterial)
    {
        this.previewMaterial = previewMaterial;
        this.wireframeMaterial = wireframeMaterial;
    }

    public void PreviewMesh(GameObject gameObject, Rect previewRect)
    {
        if (gameObject == null)
        {
            EditorGUI.DrawRect(previewRect, Color.gray);
            GUI.Label(previewRect, "No GameObject selected", EditorStyles.centeredGreyMiniLabel);
            return;
        }

        if (cachedEditor == null || cachedEditor.target != gameObject)
        {
            cachedEditor = Editor.CreateEditor(gameObject);
        }

        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            var originalMaterials = meshRenderer.sharedMaterials;
            meshRenderer.sharedMaterials = new Material[] { previewMaterial, wireframeMaterial };

            cachedEditor.OnPreviewGUI(previewRect, EditorStyles.whiteLabel);

            meshRenderer.sharedMaterials = originalMaterials;
        }
        else
        {
            cachedEditor.OnPreviewGUI(previewRect, EditorStyles.whiteLabel);
        }
    }
}
