using System.IO;
using UnityEngine;

namespace akira
{
    public static class Folders
    {
        public static void CreateDirectories(string root, params string[] dirs)
        {
            var fullPath = Path.Combine(Application.dataPath, root);
            foreach (var dir in dirs)
            {
                var path = Path.Combine(fullPath, dir.Replace('>', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(path);
            }
        }
    }
}
