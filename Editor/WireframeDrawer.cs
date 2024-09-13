using UnityEngine;
using UnityEditor;

public static class WireframeDrawer
{
    public static void DrawWireframe(Mesh mesh, Matrix4x4 matrix)
    {
        if (mesh == null)
            return;

        GL.PushMatrix();
        GL.MultMatrix(matrix);

        Material lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
        lineMaterial.SetPass(0);

        GL.Begin(GL.LINES);
        GL.Color(Color.green);

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] indices = mesh.GetIndices(i);
            for (int j = 0; j < indices.Length; j += 3)
            {
                Vector3 v1 = mesh.vertices[indices[j]];
                Vector3 v2 = mesh.vertices[indices[j + 1]];
                Vector3 v3 = mesh.vertices[indices[j + 2]];

                GL.Vertex(v1);
                GL.Vertex(v2);
                GL.Vertex(v2);
                GL.Vertex(v3);
                GL.Vertex(v3);
                GL.Vertex(v1);
            }
        }

        GL.End();
        GL.PopMatrix();
    }
}