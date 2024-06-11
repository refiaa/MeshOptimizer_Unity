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

        for (int i = 0; i < simplifiedMesh.blendShapeCount; i++)
        {
            string shapeName = simplifiedMesh.GetBlendShapeName(i);
            int frameCount = simplifiedMesh.GetBlendShapeFrameCount(i);

            for (int j = 0; j < frameCount; j++)
            {
                float frameWeight = simplifiedMesh.GetBlendShapeFrameWeight(i, j);
                Vector3[] deltaVertices = new Vector3[simplifiedMesh.vertexCount];
                Vector3[] deltaNormals = new Vector3[simplifiedMesh.vertexCount];
                Vector3[] deltaTangents = new Vector3[simplifiedMesh.vertexCount];

                simplifiedMesh.GetBlendShapeFrameVertices(i, j, deltaVertices, deltaNormals, deltaTangents);
                mesh.AddBlendShapeFrame(shapeName, frameWeight, deltaVertices, deltaNormals, deltaTangents);
            }
        }

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
