using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace akira
{
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

    public class AutoAssetPrefix : AssetPostprocessor
    {
        private static Dictionary<string, string> typePrefixMap = new Dictionary<string, string>
        {
            { "Material", "M" },
            { "AnimationClip", "AC" },
            { "Model", "FBX" },
            { "Texture2D", "SPR" },
            { "AnimatorController", "CTRL" },
            { "Prefab", "P" },
            { "TerrainLayer", "TL" },
            { "SceneAsset", "SCENE" },
            { "AudioClip", "SFX" },
            { "Shader", "SHADER" },
            { "ScriptableObject", "SO" },
        };

        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths,
            string[] movedToAssetPaths
        )
        {
            foreach (string assetPath in importedAssets)
            {
                string assetType = AssetTypeDetector.GetAssetType(assetPath);
                Debug.Log($"Imported asset: {assetPath}, Type: {assetType}");

                if (typePrefixMap.TryGetValue(assetType, out string prefix))
                {
                    string[] splitFilePath = assetPath.Split('/');
                    string[] splitFileName = splitFilePath.Last().Split('.');
                    string fileType = splitFileName.Last();

                    string newName = prefix + "_" + splitFileName.First();

                    if (splitFileName.First().Split('_').First() != prefix)
                    {
                        AssetDatabase.RenameAsset(assetPath, newName);
                        AssetDatabase.Refresh();
                    }
                }
            }
        }
    }
}
