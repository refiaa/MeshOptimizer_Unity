using UnityEngine;

public static class MeshUtils
{
    public static float CalculateMeshSize(Mesh mesh)
    {
        float size = 0;
        size += mesh.vertexCount * sizeof(float) * 3;
        size += mesh.normals.Length * sizeof(float) * 3; 
        size += mesh.uv.Length * sizeof(float) * 2;
        size += mesh.triangles.Length * sizeof(int);

        return size / 1024;
    }
}