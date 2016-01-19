using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class AtlasCoordinate
{
    public Vector4 location;
    public string name;
    public Vector2 textureSize;

    public AtlasCoordinate(string name, Vector4 location, Vector2 textureSize)
    {
        this.name = name;
        this.location = location;
        this.textureSize = textureSize;
    }

    public Vector2[] ToUvMap()
    {
        return new Vector2[]
        {
            new Vector2((location.x)/textureSize.x,(location.y)/textureSize.y),
            new Vector2((location.x + location.z)/textureSize.x,(location.y)/textureSize.y),
            new Vector2((location.x)/textureSize.x,(location.y + location.w)/textureSize.y),
            new Vector2((location.x + location.z)/textureSize.x,(location.y + location.w)/textureSize.y)
        };
    }
}

public class TextureAtlas
{
    private static readonly int textureWidth = 16;
    private static readonly int textureHeight = 16;

    private static List<AtlasCoordinate> _coords = new List<AtlasCoordinate>();
    private static Texture2D atlas;

    public static void RegisterAtlas()
    {
        List<string> _textures = new List<string>();

        string[] filenames = Directory.GetFiles(FileManager.blockTexturesDir);
        foreach (string s in filenames)
        {
            if (s.EndsWith(".png"))
            {
                _textures.Add(s);
            }
        }
        int widthHeight = (int)Mathf.Ceil(Mathf.Sqrt(Mathf.Ceil(_textures.Count)));
        widthHeight *= textureWidth;
        Texture2D atlasTemp = new Texture2D(widthHeight, widthHeight, TextureFormat.ARGB32, false);


        int x = 0;
        int y = 0;
        foreach (string loc in _textures)
        {
            if (x > (int)Mathf.Ceil(Mathf.Sqrt(Mathf.Ceil(_textures.Count))) - 1)
            {
                x = 0;
                y += 1;
            }

            Texture2D texTemp = new Texture2D(0, 0, TextureFormat.ARGB32, false);
            texTemp.LoadImage(File.ReadAllBytes(loc));
            if (texTemp.width == textureWidth && texTemp.height == textureHeight)
            {
                atlasTemp.SetPixels(x * textureWidth, y * textureHeight, textureWidth, textureHeight, texTemp.GetPixels());
                string[] locat = loc.Split('/');
                string path = locat[locat.Length - 1];
                path = path.Split('.')[0];
                _coords.Add(new AtlasCoordinate(
                    path,
                    new Vector4(x * textureWidth, y * textureHeight, textureWidth, textureHeight),
                    new Vector2(atlasTemp.width, atlasTemp.height))
                    );
                x++;
            }
            else
            {
                Logger.Log(loc + " texture size greater than supported");
            }
        }
        File.WriteAllBytes("atlas.png", atlasTemp.EncodeToPNG());

        atlas = atlasTemp;
    }

    public static AtlasCoordinate GetCoordinate(string name)
    {
        if(_coords.Count < 1)
        {
            RegisterAtlas();
        }

        foreach (AtlasCoordinate coord in _coords)
        {
            if (coord.name.Equals(name))
            {
                return coord;
            }
        }

        Logger.Log(new System.Exception("Atlas texture cannot be found: " + name).StackTrace);
        return _coords[0];
    }

    public static Texture2D GetAtlas()
    {
        Texture2D texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
        texture.LoadImage(atlas.EncodeToPNG());
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        return texture;

    }
}
