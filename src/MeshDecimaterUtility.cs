using UnityEngine;
using UnityMeshSimplifier;

/*
using UnityMeshSimplifier from github.com/Whinarn/UnityMeshSimplifier
*/

public static class MeshDecimaterUtility
{
    public static void DecimateMesh(Mesh mesh, float decimateLevel)
    {
        MeshSimplifier meshSimplifier = new MeshSimplifier();

        meshSimplifier.Initialize(mesh);

        meshSimplifier.SimplifyMesh(decimateLevel);

        Mesh simplifiedMesh = meshSimplifier.ToMesh();

        mesh.Clear();
        mesh.vertices = simplifiedMesh.vertices;
        mesh.triangles = simplifiedMesh.triangles;
        mesh.normals = simplifiedMesh.normals;
        mesh.uv = simplifiedMesh.uv;
        mesh.bindposes = simplifiedMesh.bindposes;
        mesh.boneWeights = simplifiedMesh.boneWeights;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
