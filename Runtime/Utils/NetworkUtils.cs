#if FISHNET_V4
    using System.Collections;
    using System.Collections.Generic;
    using FishNet.Serializing;
    using UnityEngine;

    public static class NetworkWriterExtensions
    {
        public static void WriteSpriteWithName(this Writer writer, Sprite sprite)
        {
            writer.WriteSpriteWithName(sprite.name);
        }
        public static void WriteSpriteWithName(this Writer writer, string spriteName)
        {
            writer.WriteString(spriteName);
        }
        public static Sprite ReadSpriteByName(this Reader reader)
        {
            return Resources.Load<Sprite>("Images/"+reader.ReadString());
        }
    }
#endif