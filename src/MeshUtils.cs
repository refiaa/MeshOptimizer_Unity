using UnityEngine;

public static class MeshUtils
{
    public static float CalculateMeshSize(Mesh mesh)
    {
        float size = 0;
        size += mesh.vertexCount * sizeof(float) * 3; // vertices
        size += mesh.normals.Length * sizeof(float) * 3; // normals
        size += mesh.uv.Length * sizeof(float) * 2; // UVs
        size += mesh.triangles.Length * sizeof(int); // triangles

        return size / 1024; // convert to KB
    }
}
