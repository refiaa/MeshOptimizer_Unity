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

        DisplayDetail("Vertices", originalVertexCount, newVertexCount);
        DisplayDetail("Triangles", originalTriangleCount, newTriangleCount);
        DisplayDetail("Submeshes", originalSubMeshCount, newSubMeshCount);
        DisplayDetail("Mesh Size (KB)", originalMeshSize, newMeshSize);
    }

    private void DisplayDetail(string label, int originalValue, int newValue)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{label}: {newValue}", GUILayout.Width(150));

        if (originalMesh != null && originalValue != newValue)
        {
            float reduction = CalculateReduction(originalValue, newValue);
            GUILayout.Label($"(-{originalValue - newValue}, -{reduction:F2}%)", GetReductionStyle());
        }

        GUILayout.EndHorizontal();
    }

    private void DisplayDetail(string label, float originalValue, float newValue)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{label}: {newValue:F2} KB", GUILayout.Width(150));

        if (originalMesh != null && !Mathf.Approximately(originalValue, newValue))
        {
            float reduction = CalculateReduction(originalValue, newValue);
            GUILayout.Label($"(-{originalValue - newValue:F2} KB, -{reduction:F2}%)", GetReductionStyle());
        }

        GUILayout.EndHorizontal();
    }

    private GUIStyle GetReductionStyle()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.normal.textColor = Color.green;
        return style;
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
