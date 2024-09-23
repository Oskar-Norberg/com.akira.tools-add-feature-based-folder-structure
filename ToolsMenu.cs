using UnityEditor;
using UnityEngine;
using static UnityEditor.AssetDatabase;

namespace akira
{
    public static class ToolsMenu
    {
        [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            Folders.CreateDirectories("_Project", "_Scrips", "Audio", "Prefabs", "Resources", "Scenes");
            Refresh();
        }

        [MenuItem("Tools/Setup/Load New Manifest")]
        static async void LoadNewManifest() =>
            await Packages.ReplacePackageFromGist("7b3e5fd1f6bd7d18fd23b382765b938b");

        [MenuItem("Tools/Setup/Packages/New Input System")]
        static void AddNewInputSystem() => Packages.InstallUnityPackage("inputsystem");

        [MenuItem("Tools/Setup/Packages/Post Processing")]
        static void AddPostProcessing() => Packages.InstallUnityPackage("postprcessing");

        [MenuItem("Tools/Setup/Packages/Cinemachine")]
        static void AddCinemachine() => Packages.InstallUnityPackage("cinemachine");
    }
}
