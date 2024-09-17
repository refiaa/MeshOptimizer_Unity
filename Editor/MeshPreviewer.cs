using UnityEngine;
using UnityEditor;

public class MeshPreviewer
{
    private Material previewMaterial;
    private Editor cachedEditor;

    public MeshPreviewer(Material previewMaterial)
    {
        this.previewMaterial = previewMaterial;
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
        var skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();

        if (meshRenderer != null)
        {
            var originalMaterials = meshRenderer.sharedMaterials;
            int subMeshCount = originalMaterials.Length;

            Material[] tempMaterials = new Material[subMeshCount];

            for (int i = 0; i < subMeshCount; i++)
            {
                tempMaterials[i] = previewMaterial;
            }

            meshRenderer.sharedMaterials = tempMaterials;

            cachedEditor.OnPreviewGUI(previewRect, EditorStyles.whiteLabel);

            meshRenderer.sharedMaterials = originalMaterials;
        }
        else if (skinnedMeshRenderer != null)
        {
            var originalMaterials = skinnedMeshRenderer.sharedMaterials;
            int subMeshCount = originalMaterials.Length;

            Material[] tempMaterials = new Material[subMeshCount];

            for (int i = 0; i < subMeshCount; i++)
            {
                tempMaterials[i] = previewMaterial;
            }

            skinnedMeshRenderer.sharedMaterials = tempMaterials;

            cachedEditor.OnPreviewGUI(previewRect, EditorStyles.whiteLabel);

            skinnedMeshRenderer.sharedMaterials = originalMaterials;
        }
        else
        {
            cachedEditor.OnPreviewGUI(previewRect, EditorStyles.whiteLabel);
        }
    }

    public void UpdatePreviewMesh(GameObject gameObject)
    {
        if (cachedEditor != null && cachedEditor.target == gameObject)
        {
            Object.DestroyImmediate(cachedEditor);
            cachedEditor = Editor.CreateEditor(gameObject);
        }
    }
}
