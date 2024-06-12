using UnityEngine;
using UnityMeshSimplifier;

/*
using UnityMeshSimplifier from github.com/Whinarn/UnityMeshSimplifier
*/

public static class MeshDecimaterUtility
{
    public static void DecimateMesh(Mesh originalMesh, Mesh decimatedMesh, float decimateLevel)
    {
        MeshSimplifier meshSimplifier = new MeshSimplifier();

        meshSimplifier.Initialize(originalMesh);

        meshSimplifier.SimplifyMesh(decimateLevel);

        Mesh simplifiedMesh = meshSimplifier.ToMesh();

        decimatedMesh.Clear();
        decimatedMesh.vertices = simplifiedMesh.vertices;
        decimatedMesh.triangles = simplifiedMesh.triangles;
        decimatedMesh.normals = simplifiedMesh.normals;
        decimatedMesh.uv = simplifiedMesh.uv;
        decimatedMesh.bindposes = simplifiedMesh.bindposes;
        decimatedMesh.boneWeights = simplifiedMesh.boneWeights;

        CopyBlendShapes(originalMesh, simplifiedMesh, decimatedMesh);

        decimatedMesh.RecalculateBounds();
        decimatedMesh.RecalculateNormals();
    }

    private static void CopyBlendShapes(Mesh originalMesh, Mesh simplifiedMesh, Mesh targetMesh)
    {
        for (int i = 0; i < originalMesh.blendShapeCount; i++)
        {
            string shapeName = originalMesh.GetBlendShapeName(i);
            int frameCount = originalMesh.GetBlendShapeFrameCount(i);

            for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
            {
                float frameWeight = originalMesh.GetBlendShapeFrameWeight(i, frameIndex);
                Vector3[] deltaVertices = new Vector3[simplifiedMesh.vertexCount];
                Vector3[] deltaNormals = new Vector3[simplifiedMesh.vertexCount];
                Vector3[] deltaTangents = new Vector3[simplifiedMesh.vertexCount];

                SimplifyBlendShapeFrame(originalMesh, simplifiedMesh, i, frameIndex, deltaVertices, deltaNormals, deltaTangents);
                targetMesh.AddBlendShapeFrame(shapeName, frameWeight, deltaVertices, deltaNormals, deltaTangents);
            }
        }
    }

    private static void SimplifyBlendShapeFrame(Mesh originalMesh, Mesh simplifiedMesh, int shapeIndex, int frameIndex, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents)
    {
        Vector3[] originalVertices = originalMesh.vertices;
        Vector3[] simplifiedVertices = simplifiedMesh.vertices;
        Vector3[] originalDeltaVertices = new Vector3[originalMesh.vertexCount];
        Vector3[] originalDeltaNormals = new Vector3[originalMesh.vertexCount];
        Vector3[] originalDeltaTangents = new Vector3[originalMesh.vertexCount];

        originalMesh.GetBlendShapeFrameVertices(shapeIndex, frameIndex, originalDeltaVertices, originalDeltaNormals, originalDeltaTangents);

        for (int i = 0; i < simplifiedVertices.Length; i++)
        {
            int closestVertexIndex = FindClosestVertex(originalVertices, simplifiedVertices[i]);
            deltaVertices[i] = originalDeltaVertices[closestVertexIndex];
            deltaNormals[i] = originalDeltaNormals[closestVertexIndex];
            deltaTangents[i] = originalDeltaTangents[closestVertexIndex];
        }
    }

    private static int FindClosestVertex(Vector3[] vertices, Vector3 targetVertex)
    {
        int closestIndex = 0;
        float closestDistance = Vector3.Distance(vertices[0], targetVertex);

        for (int i = 1; i < vertices.Length; i++)
        {
            float distance = Vector3.Distance(vertices[i], targetVertex);
            if (distance < closestDistance)
            {
                closestIndex = i;
                closestDistance = distance;
            }
        }

        return closestIndex;
    }
}
