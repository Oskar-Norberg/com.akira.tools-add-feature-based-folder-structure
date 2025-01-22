using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace akira
{
    public class AutoAssetPrefix : AssetPostprocessor
    {
        struct PrefixFileTypePair
        {
            public readonly string FileType;
            public readonly string Prefix;

            public PrefixFileTypePair(string pFileType, string pPrefix)
            {
                FileType = pFileType;
                Prefix = pPrefix;
            }
        }

        private static readonly PrefixFileTypePair[] PrefixInfo =
        {
            new("mat", "M"),
            new("anim", "AC"),
            new("fbx", "FBX"),
            new("png", "SPR"),
            new("controller", "CTRL"),
            new("prefab", "P"),
            new("terrainlayer", "TL"),
            new("unity", "SCENE"),
            new("mp3", "SFX"),
            new("wav", "SFX"),
            new("ogg", "SFX"),
            new("shader", "SHADER"),
            new("scenetemplate", "SCENE"),
        };

        private static readonly Dictionary<Type, string> TypePrefixMap = new()
        {
            { typeof(Material), "M" },
            { typeof(AnimationClip), "AC" },
            { typeof(GameObject), "FBX" },
            { typeof(Texture2D), "SPR" },
            { typeof(RuntimeAnimatorController), "CTRL" },
            { typeof(TerrainLayer), "TL" },
            { typeof(SceneAsset), "SCENE" },
            { typeof(AudioClip), "SFX" },
            { typeof(Shader), "SHADER" },
            { typeof(ScriptableObject), "SO" },
            { typeof(TerrainData), "TD" },
            { typeof(SubGraph), "SHADERSG" },
        };

        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
        )
        {
            foreach (string assetPath in importedAssets)
            {
                if (!assetPath.StartsWith("Assets/_Project"))
                {
                    continue;
                }

                Type assetType = GetAssetType(assetPath);
                //Debug.Log($"Imported asset: {assetPath}, Type: {assetType}");

                string[] splitFilePath = assetPath.Split('/');
                string[] splitFileName = splitFilePath.Last().Split('.');
                string fileNameWithoutExtension = splitFileName.First();
                string fileExtension = splitFileName.Last();

                string newName = GetNewName(fileNameWithoutExtension, fileExtension, assetType);
                if (!string.IsNullOrEmpty(newName))
                {
                    AssetDatabase.RenameAsset(assetPath, newName);
                    AssetDatabase.Refresh();

                    // Update the main object name to match the new filename
                    string newAssetPath =
                        $"{string.Join("/", splitFilePath.Take(splitFilePath.Length - 1))}/{newName}";
                    Object asset = AssetDatabase.LoadAssetAtPath<Object>(newAssetPath);
                    if (asset != null)
                    {
                        asset.name = newName.Split('/').Last().Split('.').First();
                        EditorUtility.SetDirty(asset);
                    }
                }
            }
        }

        private static Type GetAssetType(string assetPath)
        {
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            if (asset != null)
            {
                if (asset is ScriptableObject)
                {
                    return typeof(ScriptableObject);
                }
                return asset.GetType();
            }
            return null;
        }

        private static string GetNewName(
            string fileNameWithoutExtension,
            string fileExtension,
            Type assetType
        )
        {
            foreach (PrefixFileTypePair ptp in PrefixInfo)
            {
                if (ptp.FileType == fileExtension)
                {
                    if (fileNameWithoutExtension.Split('_').First() == ptp.Prefix)
                        return null;
                    return $"{ptp.Prefix}_{fileNameWithoutExtension}.{fileExtension}";
                }
            }

            if (TypePrefixMap.TryGetValue(assetType, out var prefix))
            {
                if (fileNameWithoutExtension.Split('_').First() == prefix)
                    return null;
                return $"{prefix}_{fileNameWithoutExtension}.{fileExtension}";
            }
            return null;
        }
    }
}
