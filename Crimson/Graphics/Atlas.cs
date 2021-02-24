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

        private readonly Dictionary<string, List<CTexture>> _orderedTexturesCache =
            new Dictionary<string, List<CTexture>>();

        private readonly Dictionary<string, CTexture> _textures =
            new Dictionary<string, CTexture>(StringComparer.OrdinalIgnoreCase);

        public List<Texture2D> Sources;

        public CTexture this[string id]
        {
            get => _textures[id];
            set => _textures[id] = value;
        }

        public static Atlas FromAtlas(string path, AtlasDataFormat format)
        {
            var atlas = new Atlas {Sources = new List<Texture2D>()};
            ReadAtlasData(atlas, path, format);
            return atlas;
        }

        private static void ReadAtlasData(Atlas atlas, string path, AtlasDataFormat format)
        {
            switch (format)
            {
                case AtlasDataFormat.TexturePackerSparrow:
                {
                    var xml = Utils.LoadContentXML(path);
                    var at = xml["TextureAtlas"];

                    var texturePath = at.Attr("imagePath", "");
                    var fileStream = new FileStream(Path.Combine(Path.GetDirectoryName(path), texturePath),
                        FileMode.Open,
                        FileAccess.Read);
                    var texture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, fileStream);
                    fileStream.Close();

                    var mTexture = new CTexture(texture);
                    atlas.Sources.Add(texture);

                    var subtextures = at.GetElementsByTagName("SubTexture");

                    foreach (XmlElement sub in subtextures)
                    {
                        var name = sub.Attr("name");
                        var clipRect = sub.Rect();
                        if (sub.HasAttr("frameX"))
                            atlas._textures[name] = new CTexture(mTexture, name, clipRect,
                                new Vector2(
                                    -sub.AttrInt("frameX"), -sub.AttrInt("frameY")),
                                sub.AttrInt("frameWidth"), sub.AttrInt("frameHeight"));
                        else
                            atlas._textures[name] = new CTexture(mTexture, name, clipRect);
                    }
                }
                    break;
                case AtlasDataFormat.CrunchXml:
                {
                    var xml = Utils.LoadContentXML(path);
                    var at = xml["atlas"];

                    foreach (XmlElement tex in at)
                    {
                        var texturePath = tex.Attr("n", "");
                        var fileStream = new FileStream(Path.Combine(Path.GetDirectoryName(path), texturePath + ".png"),
                            FileMode.Open, FileAccess.Read);
                        var texture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, fileStream);
                        fileStream.Close();

                        var mTexture = new CTexture(texture);
                        atlas.Sources.Add(texture);

                        foreach (XmlElement sub in tex)
                        {
                            var name = sub.Attr("n");
                            var clipRect = new Rectangle(sub.AttrInt("x"), sub.AttrInt("y"), sub.AttrInt("w"),
                                sub.AttrInt("h"));
                            if (sub.HasAttr("fx"))
                                atlas._textures[name] = new CTexture(mTexture, name, clipRect,
                                    new Vector2(-sub.AttrInt("fx"), -sub.AttrInt("fy")),
                                    sub.AttrInt("fw"), sub.AttrInt("fh"));
                            else
                                atlas._textures[name] = new CTexture(mTexture, name, clipRect);
                        }
                    }
                }
                    break;

                case AtlasDataFormat.CrunchBinary:
                    using (var stream = File.OpenRead(Path.Combine(Engine.ContentDirectory, path)))
                    {
                        var reader = new BinaryReader(stream);
                        var textures = reader.ReadInt16();

                        for (var i = 0; i < textures; i++)
                        {
                            var textureName = reader.ReadNullTerminatedString();
                            var texturePath = Path.Combine(Path.GetDirectoryName(path), textureName + ".png");
                            var fileStream = new FileStream(texturePath, FileMode.Open, FileAccess.Read);
                            var texture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, fileStream);
                            fileStream.Close();

                            atlas.Sources.Add(texture);

                            var mTexture = new CTexture(texture);
                            var subtextures = reader.ReadInt16();
                            for (var j = 0; j < subtextures; j++)
                            {
                                var name = reader.ReadNullTerminatedString();
                                var x = reader.ReadInt16();
                                var y = reader.ReadInt16();
                                var w = reader.ReadInt16();
                                var h = reader.ReadInt16();
                                var fx = reader.ReadInt16();
                                var fy = reader.ReadInt16();
                                var fw = reader.ReadInt16();
                                var fh = reader.ReadInt16();

                                atlas._textures[name] =
                                    new CTexture(mTexture, name, new Rectangle(x, y, w, h), new Vector2(-fx, -fy), fw,
                                        fh);
                            }
                        }
                    }

                    break;

                case AtlasDataFormat.CrunchBinaryNoAtlas:
                    using (var stream = File.OpenRead(Path.Combine(Engine.ContentDirectory, path + ".bin")))
                    {
                        var reader = new BinaryReader(stream);
                        var folders = reader.ReadInt16();

                        for (var i = 0; i < folders; i++)
                        {
                            var folderName = reader.ReadNullTerminatedString();
                            var folderPath = Path.Combine(Path.GetDirectoryName(path), folderName);

                            var subtextures = reader.ReadInt16();
                            for (var j = 0; j < subtextures; j++)
                            {
                                var name = reader.ReadNullTerminatedString();
                                var x = reader.ReadInt16();
                                var y = reader.ReadInt16();
                                var w = reader.ReadInt16();
                                var h = reader.ReadInt16();
                                var fx = reader.ReadInt16();
                                var fy = reader.ReadInt16();
                                var fw = reader.ReadInt16();
                                var fh = reader.ReadInt16();

                                var fileStream = new FileStream(Path.Combine(folderPath, name + ".png"), FileMode.Open,
                                    FileAccess.Read);
                                var texture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, fileStream);
                                fileStream.Close();

                                atlas.Sources.Add(texture);
                                atlas._textures[name] = new CTexture(texture, new Vector2(-fx, -fy), fw, fh);
                            }
                        }
                    }

                    break;

                case AtlasDataFormat.Packer:

                    using (var stream = File.OpenRead(Path.Combine(Engine.ContentDirectory, path + ".meta")))
                    {
                        var reader = new BinaryReader(stream);
                        reader.ReadInt32(); // version
                        reader.ReadString(); // args
                        reader.ReadInt32(); // hash

                        var textures = reader.ReadInt16();
                        for (var i = 0; i < textures; i++)
                        {
                            var textureName = reader.ReadString();
                            var texturePath = Path.Combine(Path.GetDirectoryName(path), textureName + ".data");
                            var fileStream = new FileStream(texturePath, FileMode.Open, FileAccess.Read);
                            var texture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, fileStream);
                            fileStream.Close();

                            atlas.Sources.Add(texture);

                            var mTexture = new CTexture(texture);
                            var subtextures = reader.ReadInt16();
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
                                    new CTexture(mTexture, name, new Rectangle(x, y, w, h), new Vector2(-fx, -fy), fw,
                                        fh);
                            }
                        }
                    }

                    break;

                case AtlasDataFormat.PackerNoAtlas:
                    using (var stream = File.OpenRead(Path.Combine(Engine.ContentDirectory, path + ".meta")))
                    {
                        var reader = new BinaryReader(stream);
                        reader.ReadInt32(); // version
                        reader.ReadString(); // args
                        reader.ReadInt32(); // hash

                        var folders = reader.ReadInt16();
                        for (var i = 0; i < folders; i++)
                        {
                            var folderName = reader.ReadString();
                            var folderPath = Path.Combine(Path.GetDirectoryName(path), folderName);

                            var subtextures = reader.ReadInt16();
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

                                var fileStream = new FileStream(Path.Combine(folderPath, name + ".data"), FileMode.Open,
                                    FileAccess.Read);
                                var texture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, fileStream);
                                fileStream.Close();

                                atlas.Sources.Add(texture);
                                atlas._textures[name] = new CTexture(texture, new Vector2(-fx, -fy), fw, fh);
                            }
                        }
                    }

                    break;

                case AtlasDataFormat.CrunchXmlOrBinary:

                    if (File.Exists(Path.Combine(Engine.ContentDirectory, path + ".bin")))
                        ReadAtlasData(atlas, path + ".bin", AtlasDataFormat.CrunchBinary);
                    else
                        ReadAtlasData(atlas, path + ".xml", AtlasDataFormat.CrunchXml);

                    break;

                case AtlasDataFormat.ImpactXml:
                {
                    var xml = Utils.LoadContentXML(path);
                    var at = xml["Atlas"];

                    foreach (XmlElement tex in at)
                    {
                        var texturePath = tex.Attr("n", "");
                        var texture =
                            Engine.Instance.Content.Load<Texture2D>(Path.Combine(Path.GetDirectoryName(path),
                                texturePath));

                        var mTexture = new CTexture(texture);
                        atlas.Sources.Add(texture);

                        foreach (XmlElement sub in tex)
                        {
                            var name = sub.Attr("n");
                            var clipRect = new Rectangle(sub.AttrInt("x"), sub.AttrInt("y"), sub.AttrInt("w"),
                                sub.AttrInt("h"));
                            if (sub.HasAttr("fx"))
                                atlas._textures[name] = new CTexture(mTexture, name, clipRect,
                                    new Vector2(-sub.AttrInt("fx"), -sub.AttrInt("fy")),
                                    sub.AttrInt("fw"), sub.AttrInt("fh"));
                            else
                                atlas._textures[name] = new CTexture(mTexture, name, clipRect);
                        }
                    }
                }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public static Atlas FromMultiAtlas(string rootPath, string[] dataPath, AtlasDataFormat format)
        {
            var atlas = new Atlas {Sources = new List<Texture2D>()};

            for (var i = 0; i < dataPath.Length; i++) ReadAtlasData(atlas, Path.Combine(rootPath, dataPath[i]), format);

            return atlas;
        }

        public static Atlas FromMultiAtlas(string rootPath, string filename, AtlasDataFormat format)
        {
            var atlas = new Atlas {Sources = new List<Texture2D>()};

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
            var atlas = new Atlas {Sources = new List<Texture2D>()};

            var contentDirectory = Engine.ContentDirectory;
            var contentDirectoryLength = contentDirectory.Length;
            var contentPath = Path.Combine(contentDirectory, path);
            var contentPathLength = contentPath.Length;

            foreach (var file in Directory.GetFiles(contentPath, "*", SearchOption.AllDirectories))
            {
                var ext = Path.GetExtension(file);
                if (ext != ".png" && ext != ".xnb") continue;

                // get path and load
                var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                var texture = Texture2D.FromStream(Engine.Instance.GraphicsDevice, fileStream);
                fileStream.Close();

                atlas.Sources.Add(texture);

                // make nice for dictionary
                var filepath = file.Substring(contentPathLength + 1);
                filepath = filepath.Substring(0, filepath.Length - 4);
                filepath = filepath.Replace('\\', '/');

                // load
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

        public void Dispose()
        {
            foreach (var texture in Sources) texture.Dispose();

            Sources.Clear();
            _textures.Clear();
        }
    }
}
