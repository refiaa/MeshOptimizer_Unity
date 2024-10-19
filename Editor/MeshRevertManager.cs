using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class MeshRevertManager
{
    private static Dictionary<string, Mesh> originalMeshMap = new Dictionary<string, Mesh>();

    public static void StoreOriginalMesh(Mesh decimatedMesh, Mesh originalMesh)
    {
        string key = GetMeshKey(decimatedMesh);
        if (!originalMeshMap.ContainsKey(key))
        {
            originalMeshMap.Add(key, originalMesh);
        }
    }

    public static Mesh GetOriginalMesh(Mesh decimatedMesh)
    {
        string key = GetMeshKey(decimatedMesh);
        if (originalMeshMap.TryGetValue(key, out Mesh originalMesh))
        {
            return originalMesh;
        }
        return null;
    }

    private static string GetMeshKey(Mesh mesh)
    {
        string assetPath = AssetDatabase.GetAssetPath(mesh);
        if (string.IsNullOrEmpty(assetPath))
        {
            return mesh.name;
        }
        else
        {
            return $"{assetPath}:{mesh.name}";
        }
    }
}
