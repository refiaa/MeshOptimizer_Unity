using UnityEngine;
using UnityEditor;

public class MeshInfoDisplay
{
    public void DisplayMeshInfo(Mesh mesh)
    {
        if (mesh == null) return;

        GUILayout.BeginVertical("box");
        GUILayout.Label("Mesh Information", EditorStyles.boldLabel);
        GUILayout.Label($"Vertices: {mesh.vertexCount}");
        GUILayout.Label($"Triangles: {mesh.triangles.Length / 3}");
        GUILayout.Label($"Submeshes: {mesh.subMeshCount}");
        GUILayout.Label($"Mesh Size: {MeshUtils.CalculateMeshSize(mesh)} KB");
        GUILayout.EndVertical();
    }
}
