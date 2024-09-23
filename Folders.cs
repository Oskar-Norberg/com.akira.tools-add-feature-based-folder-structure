using System.IO;
using UnityEngine;

namespace akira
{
    public static class Folders
    {
        public static void CreateDirectories(string root, params string[] dir)
        {
            var fullPath = Path.Combine(Application.dataPath, root);
            foreach (var newDirectory in dir)
            {
                Directory.CreateDirectory(Path.Combine(fullPath, newDirectory));
            }
        }
    }
}
