using UnityEngine;
using UnityEditor;

public class MeshInfoDisplay
{
    private Mesh originalMesh;
    private int originalVertexCount;
    private int originalTriangleCount;
    private int originalSubMeshCount;
    private float originalMeshSize;

    public void SetOriginalMesh(Mesh mesh)
    {
        originalMesh = mesh;
        originalVertexCount = mesh.vertexCount;
        originalTriangleCount = mesh.triangles.Length / 3;
        originalSubMeshCount = mesh.subMeshCount;
        originalMeshSize = CalculateMeshSize(mesh);
    }

    public void DisplayMeshInfo(Mesh mesh)
    {
        if (mesh == null)
        {
            EditorGUILayout.HelpBox("No mesh selected.", MessageType.Info);
            return;
        }

        GUILayout.BeginVertical("box");
        GUILayout.Label("Mesh Information", EditorStyles.boldLabel);

        DisplayMeshDetails(mesh);

        GUILayout.EndVertical();
    }

    private void DisplayMeshDetails(Mesh mesh)
    {
        int newVertexCount = mesh.vertexCount;
        int newTriangleCount = mesh.triangles.Length / 3;
        int newSubMeshCount = mesh.subMeshCount;
        float newMeshSize = CalculateMeshSize(mesh);

        GUILayout.Label(GetFormattedLabel("Vertices", originalVertexCount, newVertexCount));
        GUILayout.Label(GetFormattedLabel("Triangles", originalTriangleCount, newTriangleCount));
        GUILayout.Label(GetFormattedLabel("Submeshes", originalSubMeshCount, newSubMeshCount));
        GUILayout.Label(GetFormattedLabel("Mesh Size (KB)", originalMeshSize, newMeshSize));
    }

    private GUIStyle GetReductionStyle()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.normal.textColor = Color.green;
        return style;
    }

    private string GetFormattedLabel(string label, int originalValue, int newValue)
    {
        if (originalMesh == null || originalValue == newValue)
        {
            return $"{label}: {newValue}";
        }
        float reduction = CalculateReduction(originalValue, newValue);
        return $"{label}: {newValue} (-{originalValue - newValue}, -{reduction:F2}%)";
    }

    private string GetFormattedLabel(string label, float originalValue, float newValue)
    {
        if (originalMesh == null || Mathf.Approximately(originalValue, newValue))
        {
            return $"{label}: {newValue:F2} KB";
        }
        float reduction = CalculateReduction(originalValue, newValue);
        return $"{label}: {newValue:F2} KB (-{originalValue - newValue:F2} KB, -{reduction:F2}%)";
    }

    private float CalculateReduction(int original, int newValue)
    {
        if (original == 0) return 0;
        return ((float)(original - newValue) / original) * 100f;
    }

    private float CalculateReduction(float original, float newValue)
    {
        if (original == 0) return 0;
        return ((original - newValue) / original) * 100f;
    }

    private float CalculateMeshSize(Mesh mesh)
    {
        float size = 0;
        size += mesh.vertexCount * sizeof(float) * 3;
        size += mesh.normals.Length * sizeof(float) * 3;
        size += mesh.uv.Length * sizeof(float) * 2;
        size += mesh.tangents.Length * sizeof(float) * 4;
        size += mesh.triangles.Length * sizeof(int);

        return size / 1024;
    }
}
