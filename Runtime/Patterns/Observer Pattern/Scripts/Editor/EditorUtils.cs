/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using System.IO;
using UnityEditor;
using UnityEngine;

namespace GKCore.Observers.Editors
{
    public static class EditorUtils
    {
        public const string Version = "1.2.0";
        
        private static string _assetPath;
        
        public static string AssetPath
        {
            get
            {
                if (_assetPath == null)
                {
                    string[] guids = AssetDatabase.FindAssets("t:asmdef ValueObserver-Editor");
                    if (guids.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        FileInfo info = new FileInfo(path);
                        _assetPath = info.Directory.Parent.Parent.FullName.Substring(Application.dataPath.Length - 6) + "/";
                    }
                    else
                    {
                        _assetPath = "Assets/Plugins/Infinity Code/Value Observer/";
                    }
                }

                return _assetPath;
            }
        }

        public static Texture2D LoadTexture(string name, string extension = "png")
        {
            string path = Path.Combine(AssetPath, "Textures", name + "." + extension);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
    }
}