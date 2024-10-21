using UnityEngine;
using UnityMeshSimplifier;
using System.Collections.Generic;
using System.Linq;

/*
using UnityMeshSimplifier from github.com/Whinarn/UnityMeshSimplifier
*/

public static class MeshDecimaterUtility
{
	public static void DecimateMesh(Mesh originalMesh, Mesh decimatedMesh, float decimateLevel, bool isSkinnedMeshRenderer, int[] originalSubmeshCount, int boneCount)
	{
		try
		{
			MeshSimplifier meshSimplifier = new MeshSimplifier();
			meshSimplifier.Initialize(originalMesh);

			meshSimplifier.PreserveBorderEdges = true;
			meshSimplifier.PreserveUVSeamEdges = true;

			meshSimplifier.SimplifyMesh(decimateLevel);

			Mesh simplifiedMesh = meshSimplifier.ToMesh();

			decimatedMesh.Clear();
			decimatedMesh.vertices = simplifiedMesh.vertices;
			decimatedMesh.normals = simplifiedMesh.normals;
			decimatedMesh.uv = simplifiedMesh.uv;
			decimatedMesh.tangents = simplifiedMesh.tangents;

			decimatedMesh.subMeshCount = originalMesh.subMeshCount;
			for (int i = 0; i < originalMesh.subMeshCount; i++)
			{
				int[] triangles = simplifiedMesh.GetTriangles(i);
				int targetTriangleCount = Mathf.CeilToInt(originalSubmeshCount[i] * decimateLevel);
				if (triangles.Length > targetTriangleCount * 3)
				{
					System.Array.Resize(ref triangles, targetTriangleCount * 3);
				}
				decimatedMesh.SetTriangles(triangles, i);
			}

			if (isSkinnedMeshRenderer)
			{
				if (originalMesh.bindposes != null && originalMesh.boneWeights != null && originalMesh.boneWeights.Length == originalMesh.vertexCount)
				{
					decimatedMesh.bindposes = originalMesh.bindposes;
					decimatedMesh.boneWeights = CopyBoneWeights(originalMesh, simplifiedMesh, boneCount);
				}

				CopyBlendShapes(originalMesh, simplifiedMesh, decimatedMesh);
			}
			else
			{
				CopyBlendShapes(originalMesh, simplifiedMesh, decimatedMesh);
			}

			decimatedMesh.RecalculateBounds();
			decimatedMesh.RecalculateNormals();
			decimatedMesh.RecalculateTangents();
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"Decimation failed: {ex.Message}");
		}
	}

	private static void CopyBlendShapes(Mesh sourceMesh, Mesh simplifiedMesh, Mesh targetMesh)
	{
		Vector3[] originalVertices = sourceMesh.vertices;
		Vector3[] simplifiedVertices = simplifiedMesh.vertices;
		int[] closestVertexMap = new int[simplifiedVertices.Length];

		for (int i = 0; i < simplifiedVertices.Length; i++)
		{
			closestVertexMap[i] = FindClosestVertex(originalVertices, simplifiedVertices[i]);
		}

		for (int i = 0; i < sourceMesh.blendShapeCount; i++)
		{
			string shapeName = sourceMesh.GetBlendShapeName(i);
			int frameCount = sourceMesh.GetBlendShapeFrameCount(i);

			for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
			{
				float frameWeight = sourceMesh.GetBlendShapeFrameWeight(i, frameIndex);
				Vector3[] deltaVertices = new Vector3[simplifiedVertices.Length];
				Vector3[] deltaNormals = new Vector3[simplifiedVertices.Length];
				Vector3[] deltaTangents = new Vector3[simplifiedVertices.Length];

				SimplifyBlendShapeFrame(sourceMesh, i, frameIndex, closestVertexMap, deltaVertices, deltaNormals, deltaTangents);
				targetMesh.AddBlendShapeFrame(shapeName, frameWeight, deltaVertices, deltaNormals, deltaTangents);
			}
		}
	}

	private static void SimplifyBlendShapeFrame(Mesh originalMesh, int shapeIndex, int frameIndex, int[] closestVertexMap, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents)
	{
		Vector3[] originalDeltaVertices = new Vector3[originalMesh.vertexCount];
		Vector3[] originalDeltaNormals = new Vector3[originalMesh.vertexCount];
		Vector3[] originalDeltaTangents = new Vector3[originalMesh.vertexCount];

		originalMesh.GetBlendShapeFrameVertices(shapeIndex, frameIndex, originalDeltaVertices, originalDeltaNormals, originalDeltaTangents);

		for (int i = 0; i < closestVertexMap.Length; i++)
		{
			int closestVertexIndex = closestVertexMap[i];
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

	private static BoneWeight[] CopyBoneWeights(Mesh originalMesh, Mesh simplifiedMesh, int boneCount)
	{
		Vector3[] originalVertices = originalMesh.vertices;
		BoneWeight[] originalBoneWeights = originalMesh.boneWeights;
		Vector3[] simplifiedVertices = simplifiedMesh.vertices;
		BoneWeight[] simplifiedBoneWeights = new BoneWeight[simplifiedVertices.Length];

		float influenceRadius = 0.1f;

		for (int i = 0; i < simplifiedVertices.Length; i++)
		{
			Vector3 simplifiedVertex = simplifiedVertices[i];
			List<BoneWeight> nearbyBoneWeights = new List<BoneWeight>();

			for (int j = 0; j < originalVertices.Length; j++)
			{
				float distance = Vector3.Distance(simplifiedVertex, originalVertices[j]);
				if (distance <= influenceRadius)
				{
					nearbyBoneWeights.Add(originalBoneWeights[j]);
				}
			}

			if (nearbyBoneWeights.Count == 0)
			{
				simplifiedBoneWeights[i] = new BoneWeight();
			}
			else
			{
				Dictionary<int, float> boneWeightDict = new Dictionary<int, float>();

				foreach (var bw in nearbyBoneWeights)
				{
					AddBoneWeight(boneWeightDict, bw.boneIndex0, bw.weight0, boneCount);
					AddBoneWeight(boneWeightDict, bw.boneIndex1, bw.weight1, boneCount);
					AddBoneWeight(boneWeightDict, bw.boneIndex2, bw.weight2, boneCount);
					AddBoneWeight(boneWeightDict, bw.boneIndex3, bw.weight3, boneCount);
				}

				var sortedBones = boneWeightDict.OrderByDescending(bw => bw.Value).Take(4).ToList();
				BoneWeight newBoneWeight = new BoneWeight();
				float weightSum = sortedBones.Sum(bw => bw.Value);

				for (int k = 0; k < sortedBones.Count; k++)
				{
					int boneIndex = sortedBones[k].Key;
					float normalizedWeight = sortedBones[k].Value / weightSum;

					if (boneIndex >= boneCount)
					{
						Debug.LogWarning($"Bone index {boneIndex} is out of range ({boneCount}). Clamping to 0.");
						boneIndex = 0;
					}

					switch (k)
					{
						case 0:
							newBoneWeight.boneIndex0 = boneIndex;
							newBoneWeight.weight0 = normalizedWeight;
							break;
						case 1:
							newBoneWeight.boneIndex1 = boneIndex;
							newBoneWeight.weight1 = normalizedWeight;
							break;
						case 2:
							newBoneWeight.boneIndex2 = boneIndex;
							newBoneWeight.weight2 = normalizedWeight;
							break;
						case 3:
							newBoneWeight.boneIndex3 = boneIndex;
							newBoneWeight.weight3 = normalizedWeight;
							break;
					}
				}

				simplifiedBoneWeights[i] = newBoneWeight;
			}
		}

		return simplifiedBoneWeights;
	}

	private static void AddBoneWeight(Dictionary<int, float> dict, int boneIndex, float weight, int boneCount)
	{
		if (boneIndex < 0 || boneIndex >= boneCount) return;

		if (dict.ContainsKey(boneIndex))
		{
			dict[boneIndex] += weight;
		}
		else
		{
			dict[boneIndex] = weight;
		}
	}
}
