using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace akira
{
    public class AutoAssetPrefix : AssetPostprocessor
    {
        struct PrefixFileTypePair
        {
            public string fileType;
            public string prefix;

            public PrefixFileTypePair(string p_fileType, string p_prefix)
            {
                fileType = p_fileType;
                prefix = p_prefix;
            }
        }

        private static PrefixFileTypePair[] prefixInfo =
        {
            new PrefixFileTypePair("mat", "M"),
            new PrefixFileTypePair("anim", "AC"),
            new PrefixFileTypePair("fbx", "FBX"),
            new PrefixFileTypePair("png", "SPR"),
            new PrefixFileTypePair("controller", "CTRL"),
            new PrefixFileTypePair("prefab", "P"),
            new PrefixFileTypePair("terrainlayer", "TL"),
            new PrefixFileTypePair("unity", "SCENE"),
            new PrefixFileTypePair("mp3", "SFX"),
            new PrefixFileTypePair("wav", "SFX"),
            new PrefixFileTypePair("ogg", "SFX"),
            new PrefixFileTypePair("shader", "SHADER"),
        };

        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
        )
        {
            foreach (string str in importedAssets)
            {
                string[] splitFilePath = str.Split('/');
                string[] splitFileName = splitFilePath.Last().Split('.');
                string fileType = splitFileName.Last();

                string newName = "";

                foreach (PrefixFileTypePair ptp in prefixInfo)
                {
                    if (ptp.fileType == fileType)
                    {
                        if (splitFileName.First().Split('_').First() == ptp.prefix)
                            return;
                        newName = ptp.prefix + '_' + splitFileName.First();
                        break;
                    }
                }

                foreach (string assetPath in importedAssets)
                {
                    string assetType = AssetTypeDetector.GetAssetType(assetPath);
                    Debug.Log($"Imported asset: {assetPath}, Type: {assetType}");
                }

                AssetDatabase.RenameAsset(str, newName);
                AssetDatabase.Refresh();
            }
        }
    }
}
