#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEditor;

namespace akira
{
    public class AutoAssetPrefix : AssetPostprocessor
    {
        #region Prefix Definitions

        private static readonly PrefixFileTypePair[] FileTypePairs =
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

        private static readonly PrefixAssetTypePair[] AssetTypePairs =
        {
            new(typeof(Material), "M"),
            new(typeof(AnimationClip), "AC"),
            new(typeof(GameObject), "FBX"),
            new(typeof(Texture2D), "SPR"),
            new(typeof(RuntimeAnimatorController), "CTRL"),
            new(typeof(TerrainLayer), "TL"),
            new(typeof(SceneAsset), "SCENE"),
            new(typeof(AudioClip), "SFX"),
            new(typeof(Shader), "SHADER"),
            new(typeof(ScriptableObject), "SO"),
            new(typeof(TerrainData), "TD"),
        };
        #endregion

        private static readonly HashSet<string> LoggedErrors = new();

        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
        )
        {
            foreach (string assetPath in importedAssets)
            {
                if (assetPath.EndsWith(".cs"))
                {
                    continue;
                }

                if (!assetPath.StartsWith("Assets/_Project"))
                {
                    LogErrorOnce($"Unknown asset type for path: {assetPath}");
                    continue;
                }

                Type assetType = GetAssetType(assetPath);
                if (assetType == null)
                {
                    LogErrorOnce($"Failed to determine asset type for path: {assetPath}");
                    continue;
                }

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

            LogErrorOnce($"Failed to load asset at path: {assetPath}");
            return null;
        }

        private static string GetNewName(string fileNameWithoutExtension, string fileExtension, Type assetType)
        {
            foreach (var pair in AssetTypePairs)
            {
                if (pair.AssetType == assetType)
                {
                    if (fileNameWithoutExtension.Split('_').First() == pair.Prefix)
                        return null;
                    return $"{pair.Prefix}_{fileNameWithoutExtension}.{fileExtension}";
                }
            }

            foreach (PrefixFileTypePair pair in FileTypePairs)
            {
                if (pair.FileType == fileExtension)
                {
                    if (fileNameWithoutExtension.Split('_').First() == pair.Prefix)
                        return null;
                    return $"{pair.Prefix}_{fileNameWithoutExtension}.{fileExtension}";
                }
            }

            LogErrorOnce($"Unknown file type: {fileExtension}");
            LogErrorOnce($"Unknown asset type for file: {fileNameWithoutExtension}.{fileExtension}");
            LogErrorOnce($"Unknown .asset file type: {assetType}");
            return null;
        }

        private static void LogErrorOnce(string message)
        {
            if (!LoggedErrors.Contains(message))
            {
                Debug.LogError(message);
                LoggedErrors.Add(message);
            }
        }
    }

    #region Editor
    struct PrefixAssetTypePair
    {
        public readonly Type AssetType;
        public readonly string Prefix;

        public PrefixAssetTypePair(Type assetType, string prefix)
        {
            AssetType = assetType;
            Prefix = prefix;
        }
    }

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
    #endregion
}
#endif
