using UnityEditor;
using UnityEngine;

public static class AssetTypeDetector
{
    public static string GetAssetType(string assetPath)
    {
        Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
        if (asset != null)
        {
            return asset.GetType().Name;
        }
        return "Unknown";
    }
}
