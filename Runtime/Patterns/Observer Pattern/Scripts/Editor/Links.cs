﻿/*           INFINITY CODE          */
/*     https://infinity-code.com    */

using UnityEngine;

namespace GKCore.Observers.Editors
{
    public static class Links
    {
        public const string AssetStore = "https://assetstore.unity.com/packages/slug/266922";
        public const string Discord = "https://discord.gg/2XRWwPgZK4";
        public const string Documentation = "https://infinity-code.com/documentation/value-observer.html";
        public const string Forum = "https://forum.infinity-code.com";
        public const string Homepage = "https://infinity-code.com/assets/value-observer";
        public const string Reviews = AssetStore + "/reviews";
        public const string Support = "mailto:support@infinity-code.com?subject=Value%20Observer";
        private const string Aid = "?aid=1100liByC";

        public static void Open(string url)
        {
            Application.OpenURL(url);
        }

        public static void OpenAssetStore()
        {
            Open(AssetStore + Aid);
        }

        public static void OpenDiscord()
        {
            Open(Discord);
        }

        public static void OpenDocumentation()
        {
            OpenDocumentation(null);
        }

        public static void OpenDocumentation(string anchor)
        {
            string url = Documentation;
            if (!string.IsNullOrEmpty(anchor)) url += "#" + anchor;
            Open(url);
        }

        public static void OpenForum()
        {
            Open(Forum);
        }

        public static void OpenHomepage()
        {
            Open(Homepage);
        }

        public static void OpenLocalDocumentation()
        {
            string url = EditorUtils.AssetPath + "Documentation/Content/Documentation-Content.html";
            Application.OpenURL(url);
        }

        public static void OpenReviews()
        {
            Open(Reviews + Aid);
        }

        public static void OpenSupport()
        {
            Open(Support);
        }
    }
}