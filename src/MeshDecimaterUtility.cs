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

        CopyMeshData(simplifiedMesh, mesh);
    }

    private static void CopyMeshData(Mesh sourceMesh, Mesh targetMesh)
    {
        targetMesh.Clear();
        targetMesh.vertices = sourceMesh.vertices;
        targetMesh.triangles = sourceMesh.triangles;
        targetMesh.normals = sourceMesh.normals;
        targetMesh.uv = sourceMesh.uv;
        targetMesh.bindposes = sourceMesh.bindposes;
        targetMesh.boneWeights = sourceMesh.boneWeights; 

        for (int i = 0; i < sourceMesh.blendShapeCount; i++)
        {
            string shapeName = sourceMesh.GetBlendShapeName(i);
            int frameCount = sourceMesh.GetBlendShapeFrameCount(i);

            for (int j = 0; j < frameCount; j++)
            {
                float frameWeight = sourceMesh.GetBlendShapeFrameWeight(i, j);
                Vector3[] deltaVertices = new Vector3[sourceMesh.vertexCount];
                Vector3[] deltaNormals = new Vector3[sourceMesh.vertexCount];
                Vector3[] deltaTangents = new Vector3[sourceMesh.vertexCount];

                sourceMesh.GetBlendShapeFrameVertices(i, j, deltaVertices, deltaNormals, deltaTangents);
                targetMesh.AddBlendShapeFrame(shapeName, frameWeight, deltaVertices, deltaNormals, deltaTangents);
            }
        }

        targetMesh.RecalculateBounds();
        targetMesh.RecalculateNormals();
    }
}
