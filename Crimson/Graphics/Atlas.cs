using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Crimson
{
    public class Atlas
    {
        public enum AtlasDataFormat
        {
            TexturePackerSparrow,
            CrunchXml,
            CrunchBinary,
            CrunchXmlOrBinary,
            CrunchBinaryNoAtlas,
            Packer,
            PackerNoAtlas,
            ImpactXml
        }

        public List<VirtualTexture> Sources;

        private Dictionary<string, CTexture> _textures =
            new Dictionary<string, CTexture>(StringComparer.OrdinalIgnoreCase);

        private Dictionary<string, List<CTexture>> _orderedTexturesCache = new Dictionary<string, List<CTexture>>();
        private Dictionary<string, string>         _links                = new Dictionary<string, string>();

        public CTexture this[string id]
        {
            get => _textures[id];
            set => _textures[id] = value;
        }

        public static Atlas FromAtlas(string path, AtlasDataFormat format)
        {
            var atlas = new Atlas {Sources = new List<VirtualTexture>()};
            ReadAtlasData(atlas, path, format);
            return atlas;
        }

        private static void ReadAtlasData(Atlas atlas, string path, AtlasDataFormat format)
        {
            switch (format)
            {
                case AtlasDataFormat.TexturePackerSparrow:
                {
                    XmlElement xmlElement = Utils.LoadContentXML(path)["TextureAtlas"];
                    VirtualTexture texture =
                        VirtualContent.CreateTexture(
                            Path.Combine(Path.GetDirectoryName(path), xmlElement.Attr("imagePath", "")));
                    CTexture cTexture = new CTexture(texture);
                    atlas.Sources.Add(texture);
                    foreach ( XmlElement sub in xmlElement.GetElementsByTagName("SubTexture") )
                    {
                        string    name     = sub.Attr("name");
                        Rectangle clipRect = sub.Rect();
                        if ( sub.HasAttr("frameX") )
                        {
                            atlas._textures[name] = new CTexture(cTexture, name, clipRect,
                                                                 new Vector2(
                                                                     -sub.AttrInt("frameX", -sub.AttrInt("frameY"))),
                                                                 sub.AttrInt("frameWidth"), sub.AttrInt("frameHeight"));
                        }
                        else
                        {
                            atlas._textures[name] = new CTexture(cTexture, name, clipRect);
                        }
                    }
                    break;
                }
                case AtlasDataFormat.CrunchXml:
                {
                    foreach ( XmlElement item in Utils.LoadContentXML(path)["atlas"] )
                    {
                        VirtualTexture texture =
                            VirtualContent.CreateTexture(
                                Path.Combine(Path.GetDirectoryName(path), item.Attr("n", "") + ".png"));
                        CTexture cTexture = new CTexture(texture);
                        atlas.Sources.Add(texture);
                        foreach ( XmlElement sub in item )
                        {
                            string name = sub.Attr("n");
                            Rectangle clipRect =
                                new Rectangle(sub.AttrInt("x"), sub.AttrInt("y"), sub.AttrInt("w"), sub.AttrInt("h"));
                            if ( sub.HasAttr("fx") )
                            {
                                atlas._textures[name] = new CTexture(cTexture, name, clipRect,
                                                                     new Vector2(
                                                                         -sub.AttrInt("fx"), -sub.AttrInt("fy")),
                                                                     sub.AttrInt("fw"), sub.AttrInt("fh"));
                            }
                            else
                            {
                                atlas._textures[name] = new CTexture(cTexture, name, clipRect);
                            }
                        }
                    }
                    break;
                }
                case AtlasDataFormat.CrunchBinary:
                {
                    using (FileStream input = File.OpenRead(Path.Combine(Engine.ContentDirectory, path)))
                    {
                        BinaryReader reader   = new BinaryReader(input);
                        short        textures = reader.ReadInt16();
                        for (int i = 0; i < textures; i++)
                        {
                            string textureName = reader.ReadNullTerminatedString();
                            VirtualTexture texture = VirtualContent.CreateTexture(Path.Combine(Path.GetDirectoryName(path), textureName + ".png"));
                            atlas.Sources.Add(texture);
                            CTexture cTexture    = new CTexture(texture);
                            short    subtextures = reader.ReadInt16();
                            for (int j = 0; j < subtextures; j++)
                            {
                                string name = reader.ReadNullTerminatedString();
                                short x = reader.ReadInt16();
                                short y = reader.ReadInt16();
                                short w = reader.ReadInt16();
                                short h = reader.ReadInt16();
                                short fx = reader.ReadInt16();
                                short fy = reader.ReadInt16();
                                short fw = reader.ReadInt16();
                                short fh = reader.ReadInt16();
                                atlas._textures[name] = new CTexture(cTexture, name, new Rectangle(x, y, w, h), new Vector2(-fx, -fy), fw, fh);
                            }
                        }
                    }
                    break;
                }
                case AtlasDataFormat.CrunchBinaryNoAtlas:
                {
                    using (FileStream input = File.OpenRead(Path.Combine(Engine.ContentDirectory, path + ".bin")))
                    {
                        BinaryReader reader  = new BinaryReader(input);
                        short        folders = reader.ReadInt16();
                        for (int i = 0; i < folders; i++)
                        {
                            string folderName  = reader.ReadNullTerminatedString();
                            string folderPath  = Path.Combine(Path.GetDirectoryName(path), folderName);
                            short  subtextures = reader.ReadInt16();
                            for (int j = 0; j < subtextures; j++)
                            {
                                string name = reader.ReadNullTerminatedString();
                                reader.ReadInt16();
                                reader.ReadInt16();
                                reader.ReadInt16();
                                reader.ReadInt16();
                                short fx = reader.ReadInt16();
                                short fy = reader.ReadInt16();
                                short fw = reader.ReadInt16();
                                short fh = reader.ReadInt16();
                                VirtualTexture texture = VirtualContent.CreateTexture(Path.Combine(folderPath, name + ".png"));
                                atlas.Sources.Add(texture);
                                atlas._textures[name] = new CTexture(texture, new Vector2(-fx, -fy), fw, fh);
                            }
                        }
                    }
                    break;
                }
                case AtlasDataFormat.Packer:
                {
                    using (FileStream input = File.OpenRead(Path.Combine(Engine.ContentDirectory, path + ".meta")))
                    {
                        var reader = new BinaryReader(input);
                        reader.ReadInt32(); // version
                        reader.ReadString(); // args
                        reader.ReadInt32(); // hash

                        short textures = reader.ReadInt16();
                        for (var i = 0; i < textures; i++)
                        {
                            var textureName = reader.ReadString();
                            VirtualTexture texture =
                                VirtualContent.CreateTexture(
                                    Path.Combine(Path.GetDirectoryName(path), textureName + ".data"));
                            atlas.Sources.Add(texture);
                            CTexture cTexture    = new CTexture(texture);
                            short    subtextures = reader.ReadInt16();
                            for (var j = 0; j < subtextures; j++)
                            {
                                var name = reader.ReadString().Replace('\\', '/');
                                var x = reader.ReadInt16();
                                var y = reader.ReadInt16();
                                var w = reader.ReadInt16();
                                var h = reader.ReadInt16();
                                var fx = reader.ReadInt16();
                                var fy = reader.ReadInt16();
                                var fw = reader.ReadInt16();
                                var fh = reader.ReadInt16();

                                atlas._textures[name] =
                                    new CTexture(cTexture, name, new Rectangle(x, y, w, h), new Vector2(-fx, -fy), fw,
                                        fh);
                            }
                        }

                        if ( input.Position < input.Length && reader.ReadString() == "LINKS" )
                        {
                            short links = reader.ReadInt16();
                            for ( int k = 0; k < links; ++k )
                            {
                                string key = reader.ReadString();
                                string val = reader.ReadString();
                                atlas._links.Add(key, val);
                            }
                        }
                    }

                    break;
                }
                case AtlasDataFormat.PackerNoAtlas:
                {
                    using (FileStream input = File.OpenRead(Path.Combine(Engine.ContentDirectory, path + ".meta")))
                    {
                        BinaryReader reader = new BinaryReader(input);
                        reader.ReadInt32();  // version
                        reader.ReadString(); // args
                        reader.ReadInt32();  // hash

                        short folders = reader.ReadInt16();
                        for (var i = 0; i < folders; i++)
                        {
                            string folderName  = reader.ReadString();
                            string folderPath  = Path.Combine(Path.GetDirectoryName(path), folderName);
                            short  subtextures = reader.ReadInt16();
                            for (var j = 0; j < subtextures; j++)
                            {
                                var name = reader.ReadString().Replace('\\', '/');
                                var x    = reader.ReadInt16();
                                var y    = reader.ReadInt16();
                                var w    = reader.ReadInt16();
                                var h    = reader.ReadInt16();
                                var fx   = reader.ReadInt16();
                                var fy   = reader.ReadInt16();
                                var fw   = reader.ReadInt16();
                                var fh   = reader.ReadInt16();

                                VirtualTexture texture =
                                    VirtualContent.CreateTexture(Path.Combine(folderPath, name + ".data"));
                                atlas.Sources.Add(texture);
                                atlas._textures[name]           = new CTexture(texture, new Vector2(-fx, -fy), fw, fh);
                                atlas._textures[name].AtlasPath = name;
                            }

                            if ( input.Position < input.Length && reader.ReadString() == "LINKS" )
                            {
                                short links = reader.ReadInt16();
                                for ( int m = 0; m < links; ++m )
                                {
                                    string key = reader.ReadString();
                                    string val = reader.ReadString();
                                    atlas._links.Add(key, val);
                                }
                            }
                        }
                    }

                    break;
                }
                case AtlasDataFormat.CrunchXmlOrBinary:
                {
                    if (File.Exists(Path.Combine(Engine.ContentDirectory, path + ".bin")))
                        ReadAtlasData(atlas, path + ".bin", AtlasDataFormat.CrunchBinary);
                    else
                        ReadAtlasData(atlas, path + ".xml", AtlasDataFormat.CrunchXml);

                    break;
                }
                case AtlasDataFormat.ImpactXml:
                {
                    foreach ( XmlElement item in Utils.LoadContentXML(path)["Atlas"] )
                    {
                        VirtualTexture texture =
                            VirtualContent.CreateTexture(
                                Path.Combine(Path.GetDirectoryName(path), item.Attr("n", "") + ".xnb"));
                        CTexture cTexture = new CTexture(texture);
                        atlas.Sources.Add(texture);
                        foreach ( XmlElement sub in item )
                        {
                            string name = sub.Attr("n");
                            Rectangle clipRect =
                                new Rectangle(sub.AttrInt("x"), sub.AttrInt("y"), sub.AttrInt("w"), sub.AttrInt("h"));
                            if ( sub.HasAttr("fx") )
                            {
                                atlas._textures[name] = new CTexture(cTexture, name, clipRect,
                                                                     new Vector2(
                                                                         -sub.AttrInt("fx"), -sub.AttrInt("fy")),
                                                                     sub.AttrInt("fw"), sub.AttrInt("fh"));
                            }
                            else
                            {
                                atlas._textures[name] = new CTexture(cTexture, name, clipRect);
                            }
                        }
                    }
                    break;
                }
                default:
                    throw new NotImplementedException();
            }
        }

        public static Atlas FromMultiAtlas(string rootPath, string[] dataPath, AtlasDataFormat format)
        {
            var atlas = new Atlas {Sources = new List<VirtualTexture>()};

            for (var i = 0; i < dataPath.Length; i++)
                ReadAtlasData(atlas, Path.Combine(rootPath, dataPath[i]), format);

            return atlas;
        }

        public static Atlas FromMultiAtlas(string rootPath, string filename, AtlasDataFormat format)
        {
            var atlas = new Atlas {Sources = new List<VirtualTexture>()};

            var index = 0;
            while (true)
            {
                var dataPath = Path.Combine(rootPath, filename + index + ".xml");

                if (!File.Exists(Path.Combine(Engine.ContentDirectory, dataPath))) break;

                ReadAtlasData(atlas, dataPath, format);
                index++;
            }

            return atlas;
        }

        public static Atlas FromDirectory(string path)
        {
            var atlas = new Atlas {Sources = new List<VirtualTexture>()};

            var      contentDirectory       = Engine.ContentDirectory;
            var      contentDirectoryLength = contentDirectory.Length;
            var      contentPath            = Path.Combine(contentDirectory, path);
            var      contentPathLength      = contentPath.Length;
            string[] files                  = Directory.GetFiles(contentPath, "*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                string ext = Path.GetExtension(file);
                if (ext != ".png" && ext != ".xnb") continue;

                VirtualTexture texture = VirtualContent.CreateTexture(file.Substring(contentDirectoryLength + 1));
                atlas.Sources.Add(texture);
                string filepath = file.Substring(contentPathLength + 1);
                filepath = filepath.Substring(0, filepath.Length - 4);
                filepath = filepath.Replace("\\", "/");
                atlas._textures.Add(filepath, new CTexture(texture));
            }

            return atlas;
        }

        public bool Has(string id)
        {
            return _textures.ContainsKey(id);
        }

        public CTexture GetOrDefault(string id, CTexture defaultTexture)
        {
            if (string.IsNullOrEmpty(id) || !Has(id)) return defaultTexture;

            return _textures[id];
        }

        public List<CTexture> GetAtlasSubtextures(string key)
        {
            if (!_orderedTexturesCache.TryGetValue(key, out var list))
            {
                list = new List<CTexture>();

                var index = 0;
                while (true)
                {
                    var texture = GetAtlasSubtextureFromAtlasAt(key, index);
                    if (texture != null)
                        list.Add(texture);
                    else
                        break;

                    index++;
                }

                _orderedTexturesCache.Add(key, list);
            }

            return list;
        }

        private CTexture GetAtlasSubtextureFromCacheAt(string key, int index)
        {
            return _orderedTexturesCache[key][index];
        }

        private CTexture GetAtlasSubtextureFromAtlasAt(string key, int index)
        {
            if (index == 0 && _textures.ContainsKey(key)) return _textures[key];

            var indexString = index.ToString();
            var startLength = indexString.Length;
            while (indexString.Length < startLength + 6)
            {
                if (_textures.TryGetValue(key + indexString, out var result)) return result;

                indexString = "0" + indexString;
            }

            return null;
        }

        public CTexture GetAtlasSubtexturesAt(string key, int index)
        {
            if (_orderedTexturesCache.TryGetValue(key, out var list)) return list[index];

            return GetAtlasSubtextureFromAtlasAt(key, index);
        }

        public CTexture GetLinkedTexture(string key)
        {
            if ( key != null && _links.TryGetValue(key, out var other) &&
                 _textures.TryGetValue(other, out var texture) )
            {
                return texture;
            }

            return null;
        }

        public void Dispose()
        {
            foreach (VirtualTexture texture in Sources) texture.Dispose();

            Sources.Clear();
            _textures.Clear();
        }
    }
}
