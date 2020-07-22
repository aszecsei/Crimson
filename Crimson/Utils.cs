using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Crimson
{
    public static class Utils
    {
        #region Rectangle

        public static Rectangle Expand(this Rectangle rect, Rectangle other)
        {
            var result = new Rectangle {X = Mathf.Min(rect.X, other.X), Y = Mathf.Min(rect.Y, other.Y)};
            result.Width = Mathf.Max(rect.Right, other.Right) - result.X;
            result.Height = Mathf.Max(rect.Bottom, other.Bottom) - result.Y;
            return result;
        }

        #endregion

        public static string ReadNullTerminatedString(this BinaryReader stream)
        {
            var str = "";
            char ch;
            while ((ch = stream.ReadChar()) != 0) str = str + ch;

            return str;
        }

        #region Reflection

        public static Delegate? GetMethod<T>(object obj, string method) where T : class
        {
            var info = obj.GetType()
                .GetMethod(method, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (info == null)
                return null;
            return Delegate.CreateDelegate(typeof(T), obj, method);
        }

        #endregion

        public static RenderTarget2D CreateRenderTarget(int width, int height, bool hasDepth = false)
        {
            return new RenderTarget2D(Engine.Instance.GraphicsDevice, width, height, false, SurfaceFormat.Color,
                hasDepth ? DepthFormat.Depth24Stencil8 : DepthFormat.None);
        }

        #region Give Me

        public static T GiveMe<T>(int index, T a, T b)
        {
            switch (index)
            {
                default:
                    throw new Exception("Index was out of range!");
                case 0:
                    return a;
                case 1:
                    return b;
            }
        }

        public static T GiveMe<T>(int index, params T[] l)
        {
            if (index < 0 || index >= l.Length)
                throw new IndexOutOfRangeException("Index was out of range!");
            return l[index];
        }

        #endregion

        #region Enums

        public static int EnumLength(Type e)
        {
            return Enum.GetNames(e).Length;
        }

        public static int EnumLength<T>()
        {
            return Enum.GetNames(typeof(T)).Length;
        }

        public static T StringToEnum<T>(string str) where T : struct
        {
            if (Enum.IsDefined(typeof(T), str)) return (T) Enum.Parse(typeof(T), str);

            throw new Exception("The string cannot be converted to the enum type.");
        }

        public static T[] StringsToEnums<T>(string[] strs) where T : struct
        {
            var ret = new T[strs.Length];
            for (var i = 0; i < strs.Length; i++) ret[i] = StringToEnum<T>(strs[i]);

            return ret;
        }

        public static bool EnumHasString<T>(string str) where T : struct
        {
            return Enum.IsDefined(typeof(T), str);
        }

        public static T[] GetEnumValues<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        #endregion

        #region Strings

        public static bool StartsWith(this string str, string match)
        {
            return str.IndexOf(match) == 0;
        }

        public static bool EndsWith(this string str, string match)
        {
            return str.LastIndexOf(match) == str.Length - match.Length;
        }

        public static bool IsIgnoreCase(this string str, params string[] matches)
        {
            if (string.IsNullOrEmpty(str)) return false;

            foreach (var match in matches)
                if (str.Equals(match, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }

        public static string ToString(this int num, int minDigits)
        {
            var ret = num.ToString();
            while (ret.Length < minDigits) ret = "0" + ret;

            return ret;
        }
        
        [Pure]
        public static string ReplaceAt(this string str, int index, char replace)
        {
            char[] chars = str.ToCharArray();
            chars[index] = replace;
            return new string(chars);
        }

        #endregion

        #region Random

        public static Random Random = new Random();
        private static readonly Stack<Random> s_randomStack = new Stack<Random>();

        public static void PushRandom(int newSeed)
        {
            s_randomStack.Push(Random);
            Random = new Random(newSeed);
        }

        public static void PushRandom(Random random)
        {
            s_randomStack.Push(Random);
            Random = random;
        }

        public static void PushRandom()
        {
            s_randomStack.Push(Random);
            Random = new Random();
        }

        public static void PopRandom()
        {
            Random = s_randomStack.Pop();
        }

        #region Choose

        public static T Choose<T>(this Random random, T a, T b)
        {
            return GiveMe(random.Next(2), a, b);
        }

        public static T Choose<T>(this Random random, params T[] choices)
        {
            return choices[random.Next(choices.Length)];
        }

        public static T Choose<T>(this Random random, List<T> choices)
        {
            return choices[random.Next(choices.Count)];
        }

        #endregion

        #region Range

        /// <summary>
        ///     Returns a random integer between min (inclusive) and max (exclusive)
        /// </summary>
        /// <param name="random"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Range(this Random random, int min, int max)
        {
            return min + random.Next(max - min);
        }

        public static float Range(this Random random, float min, float max)
        {
            return min + random.NextFloat(max - min);
        }

        public static Vector2 Range(this Random random, Vector2 min, Vector2 max)
        {
            return min + new Vector2(random.NextFloat(max.X - min.X), random.NextFloat(max.Y - min.Y));
        }

        #endregion

        public static int Facing(this Random random)
        {
            return random.NextFloat() < 0.5f ? -1 : 1;
        }

        public static bool Chance(this Random random, float chance)
        {
            return random.NextFloat() < chance;
        }

        public static float NextFloat(this Random random)
        {
            return (float) random.NextDouble();
        }

        public static float NextFloat(this Random random, float max)
        {
            return random.NextFloat() * max;
        }

        public static float NextAngle(this Random random)
        {
            return random.NextFloat() * MathHelper.TwoPi;
        }

        private static readonly int[] s_shakeVectorOffsets = {-1, -1, 0, 1, 1};

        public static Vector2 ShakeVector(this Random random)
        {
            return new Vector2(random.Choose(s_shakeVectorOffsets), random.Choose(s_shakeVectorOffsets));
        }

        #endregion

        #region Time

        public static string ShortGameplayFormat(this TimeSpan time)
        {
            if (time.TotalHours >= 1) return time.Hours + ":" + time.ToString(@"mm\:ss\.fff");

            return time.ToString(@"m\:ss\.fff");
        }

        public static string LongGameplayFormat(this TimeSpan time)
        {
            var str = new StringBuilder();

            if (time.TotalDays >= 2)
            {
                str.Append((int) time.TotalDays);
                str.Append(" days, ");
            }
            else if (time.TotalDays >= 1)
            {
                str.Append("1 day, ");
            }

            str.Append((time.TotalHours - (int) time.TotalDays * 24).ToString("0.0"));
            str.Append(" hours");

            return str.ToString();
        }

        #endregion

        #region Vectors

        public static Vector2[] ParseVector2List(string list, char separator = '|')
        {
            var entries = list.Split(separator);
            var data = new Vector2[entries.Length];

            for (var i = 0; i < entries.Length; i++)
            {
                var sides = entries[i].Split(',');
                data[i] = new Vector2(Convert.ToInt32(sides[0]), Convert.ToInt32(sides[1]));
            }

            return data;
        }
        
        public static Vector3[] ParseVector3List(string list, char separator = '|')
        {
            var entries = list.Split(separator);
            var data = new Vector3[entries.Length];

            for (var i = 0; i < entries.Length; i++)
            {
                var sides = entries[i].Split(',');
                data[i] = new Vector3(Convert.ToInt32(sides[0]), Convert.ToInt32(sides[1]), Convert.ToInt32(sides[2]));
            }

            return data;
        }

        #endregion

        #region CSV

        public static int[,] ReadCSVIntGrid(string csv, int width, int height)
        {
            var data = new int[width, height];

            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                data[x, y] = -1;

            var lines = csv.Split('\n');
            for (var y = 0; y < height && y < lines.Length; y++)
            {
                var line = lines[y].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                for (var x = 0; x < width && x < line.Length; x++) data[x, y] = Convert.ToInt32(line[x]);
            }

            return data;
        }

        public static int[] ReadCSVInt(string csv)
        {
            if (csv == "") return new int[0];

            var values = csv.Split(',');
            var ret = new int[values.Length];

            for (var i = 0; i < values.Length; i++) ret[i] = Convert.ToInt32(values[i].Trim());

            return ret;
        }

        /// <summary>
        ///     Read positive-integer CSV with some added tricks.
        ///     Use - to add inclusive range. Ex: 3-6 = 3,4,5,6
        ///     Use * to add multiple values. Ex: 4*3 = 4,4,4
        /// </summary>
        /// <param name="csv"></param>
        /// <returns></returns>
        public static int[] ReadCSVIntWithTricks(string csv)
        {
            if (csv == "") return new int[0];

            var values = csv.Split(',');
            var ret = new List<int>();

            foreach (var val in values)
                if (val.IndexOf('-') != -1)
                {
                    var split = val.Split('-');
                    var a = Convert.ToInt32(split[0]);
                    var b = Convert.ToInt32(split[1]);

                    for (var i = a; i != b; i += Math.Sign(b - a)) ret.Add(i);

                    ret.Add(b);
                }
                else if (val.IndexOf('*') != -1)
                {
                    var split = val.Split('*');
                    var a = Convert.ToInt32(split[0]);
                    var b = Convert.ToInt32(split[1]);

                    for (var i = 0; i < b; i++) ret.Add(a);
                }
                else
                {
                    ret.Add(Convert.ToInt32(val));
                }

            return ret.ToArray();
        }

        public static string[] ReadCSV(string csv)
        {
            if (csv == "") return new string[0];

            var values = csv.Split(',');
            for (var i = 0; i < values.Length; i++) values[i] = values[i].Trim();

            return values;
        }

        public static string IntGridToCSV(int[,] data)
        {
            var str = new StringBuilder();

            var line = new List<int>();
            var newLines = 0;

            for (var y = 0; y < data.GetLength(1); y++)
            {
                var empties = 0;

                for (var x = 0; x < data.GetLength(0); x++)
                    if (data[x, y] == -1)
                    {
                        empties++;
                    }
                    else
                    {
                        for (var i = 0; i < newLines; i++) str.Append('\n');

                        for (var i = 0; i < empties; i++) line.Add(-1);

                        empties = newLines = 0;

                        line.Add(data[x, y]);
                    }

                if (line.Count > 0)
                {
                    str.Append(string.Join(",", line));
                    line.Clear();
                }

                newLines++;
            }

            return str.ToString();
        }

        #endregion

        #region Data Parse

        public static bool[,] GetBitData(string data, char rowSep = '\n')
        {
            var lengthX = 0;
            for (var i = 0; i < data.Length; i++)
                if (data[i] == '1' || data[i] == '0')
                    lengthX++;
                else if (data[i] == rowSep) break;

            var lengthY = data.Count(c => c == '\n') + 1;

            var bitData = new bool[lengthX, lengthY];
            var x = 0;
            var y = 0;
            for (var i = 0; i < data.Length; i++)
                switch (data[i])
                {
                    case '1':
                        bitData[x, y] = true;
                        x++;
                        break;

                    case '0':
                        bitData[x, y] = false;
                        x++;
                        break;

                    case '\n':
                        x = 0;
                        y++;
                        break;
                }

            return bitData;
        }

        public static void CombineBitData(bool[,] combineInto, string data, char rowSep = '\n')
        {
            var x = 0;
            var y = 0;
            for (var i = 0; i < data.Length; i++)
                switch (data[i])
                {
                    case '1':
                        combineInto[x, y] = true;
                        x++;
                        break;

                    case '0':
                        x++;
                        break;

                    case '\n':
                        x = 0;
                        y++;
                        break;
                }
        }

        public static void CombineBitData(bool[,] combineInto, bool[,] data)
        {
            for (var i = 0; i < combineInto.GetLength(0); i++)
            for (var j = 0; j < combineInto.GetLength(1); j++)
                if (data[i, j])
                    combineInto[i, j] = true;
        }

        public static int[] ConvertStringArrayToIntArray(string[] strings)
        {
            var ret = new int[strings.Length];
            for (var i = 0; i < strings.Length; i++) ret[i] = Convert.ToInt32(strings[i]);

            return ret;
        }

        public static float[] ConvertStringArrayToFloatArray(string[] strings)
        {
            var ret = new float[strings.Length];
            for (var i = 0; i < strings.Length; i++)
                ret[i] = Convert.ToSingle(strings[i], CultureInfo.InvariantCulture);

            return ret;
        }

        #endregion

        #region Save and Load Data

        public static string[] GetFiles(string directory)
        {
            return Directory.GetFiles(directory);
        }

        public static bool FileExists(string filename)
        {
            return File.Exists(filename);
        }

        public static bool SaveFile<T>(T obj, string filename) where T : new()
        {
            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, obj);
                stream.Close();
                return true;
            }
            catch
            {
                stream.Close();
                return false;
            }
        }

        public static bool LoadFile<T>(string filename, ref T data) where T : new()
        {
            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);

            try
            {
                var serializer = new XmlSerializer(typeof(T));
                var obj = (T) serializer.Deserialize(stream);
                stream.Close();
                data = obj;
                return true;
            }
            catch
            {
                stream.Close();
                return false;
            }
        }

        #endregion

        #region XML

        public static XmlDocument LoadContentXML(string filename)
        {
            var xml = new XmlDocument();
            xml.Load(TitleContainer.OpenStream(Path.Combine(Engine.Instance.Content.RootDirectory, filename)));
            return xml;
        }

        public static XmlDocument LoadXML(string filename)
        {
            var xml = new XmlDocument();
            using (var stream = File.OpenRead(filename))
            {
                xml.Load(stream);
            }

            return xml;
        }

        public static bool ContentXMLExists(string filename)
        {
            return File.Exists(Path.Combine(Engine.ContentDirectory, filename));
        }

        public static bool XMLExists(string filename)
        {
            return File.Exists(filename);
        }

        #region Attributes

        public static bool HasAttr(this XmlElement xml, string attributeName)
        {
            return xml.Attributes[attributeName] != null;
        }

        public static string Attr(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return xml.Attributes[attributeName].InnerText;
        }

        public static string Attr(this XmlElement xml, string attributeName, string defaultValue)
        {
            if (!xml.HasAttr(attributeName)) return defaultValue;

            return xml.Attributes[attributeName].InnerText;
        }

        public static int AttrInt(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToInt32(xml.Attributes[attributeName].InnerText);
        }

        public static int AttrInt(this XmlElement xml, string attributeName, int defaultValue)
        {
            if (!xml.HasAttr(attributeName)) return defaultValue;

            return Convert.ToInt32(xml.Attributes[attributeName].InnerText);
        }

        public static float AttrFloat(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToSingle(xml.Attributes[attributeName].InnerText, CultureInfo.InvariantCulture);
        }

        public static float AttrFloat(this XmlElement xml, string attributeName, float defaultValue)
        {
            if (!xml.HasAttr(attributeName)) return defaultValue;

            return Convert.ToSingle(xml.Attributes[attributeName].InnerText, CultureInfo.InvariantCulture);
        }

        public static Vector3 AttrVector3(this XmlElement xml, string attributeName)
        {
            var attr = xml.Attr(attributeName).Split(',');
            var x = float.Parse(attr[0].Trim(), CultureInfo.InvariantCulture);
            var y = float.Parse(attr[1].Trim(), CultureInfo.InvariantCulture);
            var z = float.Parse(attr[2].Trim(), CultureInfo.InvariantCulture);

            return new Vector3(x, y, z);
        }

        public static Vector2 AttrVector2(this XmlElement xml, string xAttributeName, string yAttributeName)
        {
            return new Vector2(xml.AttrFloat(xAttributeName), xml.AttrFloat(yAttributeName));
        }

        public static Vector2 AttrVector2(
            this XmlElement xml,
            string xAttributeName,
            string yAttributeName,
            Vector2 defaultValue
        )
        {
            return new Vector2(xml.AttrFloat(xAttributeName, defaultValue.X),
                xml.AttrFloat(yAttributeName, defaultValue.Y));
        }

        public static bool AttrBool(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToBoolean(xml.Attributes[attributeName].InnerText);
        }

        public static bool AttrBool(this XmlElement xml, string attributeName, bool defaultValue)
        {
            if (!xml.HasAttr(attributeName)) return defaultValue;

            return AttrBool(xml, attributeName);
        }

        public static char AttrChar(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToChar(xml.Attributes[attributeName].InnerText);
        }

        public static char AttrChar(this XmlElement xml, string attributeName, char defaultValue)
        {
            if (!xml.HasAttr(attributeName)) return defaultValue;

            return AttrChar(xml, attributeName);
        }

        public static T AttrEnum<T>(this XmlElement xml, string attributeName) where T : struct
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            if (Enum.IsDefined(typeof(T), xml.Attributes[attributeName].InnerText))
                return (T) Enum.Parse(typeof(T), xml.Attributes[attributeName].InnerText);

            throw new Exception("The attribute value cannot be converted to the enum type.");
        }

        public static T AttrEnum<T>(this XmlElement xml, string attributeName, T defaultValue) where T : struct
        {
            if (!xml.HasAttr(attributeName)) return defaultValue;

            return xml.AttrEnum<T>(attributeName);
        }

        public static Color AttrHexColor(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return ColorExt.HexToColor(xml.Attr(attributeName));
        }

        public static Color AttrHexColor(this XmlElement xml, string attributeName, Color defaultValue)
        {
            if (!xml.HasAttr(attributeName)) return defaultValue;

            return AttrHexColor(xml, attributeName);
        }

        public static Color AttrHexColor(this XmlElement xml, string attributeName, string defaultValue)
        {
            if (!xml.HasAttr(attributeName)) return ColorExt.HexToColor(defaultValue);

            return AttrHexColor(xml, attributeName);
        }

        public static Vector2 Position(this XmlElement xml)
        {
            return new Vector2(xml.AttrFloat("x"), xml.AttrFloat("y"));
        }

        public static Vector2 Position(this XmlElement xml, Vector2 defaultPosition)
        {
            return new Vector2(xml.AttrFloat("x", defaultPosition.X), xml.AttrFloat("y", defaultPosition.Y));
        }

        public static int X(this XmlElement xml)
        {
            return xml.AttrInt("x");
        }

        public static int X(this XmlElement xml, int defaultX)
        {
            return xml.AttrInt("x", defaultX);
        }

        public static int Y(this XmlElement xml)
        {
            return xml.AttrInt("y");
        }

        public static int Y(this XmlElement xml, int defaultY)
        {
            return xml.AttrInt("y", defaultY);
        }

        public static int Width(this XmlElement xml)
        {
            return xml.AttrInt("width");
        }

        public static int Width(this XmlElement xml, int defaultWidth)
        {
            return xml.AttrInt("width", defaultWidth);
        }

        public static int Height(this XmlElement xml)
        {
            return xml.AttrInt("height");
        }

        public static int Height(this XmlElement xml, int defaultHeight)
        {
            return xml.AttrInt("height", defaultHeight);
        }

        public static Rectangle Rect(this XmlElement xml)
        {
            return new Rectangle(xml.X(), xml.Y(), xml.Width(), xml.Height());
        }

        public static int ID(this XmlElement xml)
        {
            return xml.AttrInt("id");
        }

        #endregion

        #region Inner Text

        public static int InnerInt(this XmlElement xml)
        {
            return Convert.ToInt32(xml.InnerText);
        }

        public static float InnerFloat(this XmlElement xml)
        {
            return Convert.ToSingle(xml.InnerText, CultureInfo.InvariantCulture);
        }

        public static bool InnerBool(this XmlElement xml)
        {
            return Convert.ToBoolean(xml.InnerText);
        }

        public static T InnerEnum<T>(this XmlElement xml) where T : struct
        {
            if (Enum.IsDefined(typeof(T), xml.InnerText)) return (T) Enum.Parse(typeof(T), xml.InnerText);

            throw new Exception("The attribute value cannot be converted to the enum type.");
        }

        public static Color InnerHexColor(this XmlElement xml)
        {
            return ColorExt.HexToColor(xml.InnerText);
        }

        #endregion

        #region Child Inner Text

        public static bool HasChild(this XmlElement xml, string childName)
        {
            return xml[childName] != null;
        }

        public static string ChildText(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName].InnerText;
        }

        public static string ChildText(this XmlElement xml, string childName, string defaultValue)
        {
            if (xml.HasChild(childName)) return xml[childName].InnerText;

            return defaultValue;
        }

        public static int ChildInt(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName].InnerInt();
        }

        public static int ChildInt(this XmlElement xml, string childName, int defaultValue)
        {
            if (xml.HasChild(childName)) return xml[childName].InnerInt();

            return defaultValue;
        }

        public static float ChildFloat(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName].InnerFloat();
        }

        public static float ChildFloat(this XmlElement xml, string childName, float defaultValue)
        {
            if (xml.HasChild(childName)) return xml[childName].InnerFloat();

            return defaultValue;
        }

        public static bool ChildBool(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName].InnerBool();
        }

        public static bool ChildBool(this XmlElement xml, string childName, bool defaultValue)
        {
            if (xml.HasChild(childName)) return xml[childName].InnerBool();

            return defaultValue;
        }

        public static T ChildEnum<T>(this XmlElement xml, string childName) where T : struct
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            if (Enum.IsDefined(typeof(T), xml[childName].InnerText))
                return (T) Enum.Parse(typeof(T), xml[childName].InnerText);

            throw new Exception("The attribute value cannot be converted to the enum type.");
        }

        public static T ChildEnum<T>(this XmlElement xml, string childName, T defaultValue) where T : struct
        {
            if (xml.HasChild(childName))
            {
                if (Enum.IsDefined(typeof(T), xml[childName].InnerText))
                    return (T) Enum.Parse(typeof(T), xml[childName].InnerText);

                throw new Exception("The attribute value cannot be converted to the enum type.");
            }

            return defaultValue;
        }

        public static Color ChildHexColor(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return ColorExt.HexToColor(xml[childName].InnerText);
        }

        public static Color ChildHexColor(this XmlElement xml, string childName, Color defaultValue)
        {
            if (xml.HasChild(childName)) return ColorExt.HexToColor(xml[childName].InnerText);

            return defaultValue;
        }

        public static Color ChildHexColor(this XmlElement xml, string childName, string defaultValue)
        {
            if (xml.HasChild(childName)) return ColorExt.HexToColor(xml[childName].InnerText);

            return ColorExt.HexToColor(defaultValue);
        }

        public static Vector2 ChildPosition(this XmlElement xml, string childName)
        {
#if DEBUG
            if (!xml.HasChild(childName))
                throw new Exception("Cannot find child xml tag with name '" + childName + "'.");
#endif
            return xml[childName].Position();
        }

        public static Vector2 ChildPosition(this XmlElement xml, string childName, Vector2 defaultValue)
        {
            if (xml.HasChild(childName)) return xml[childName].Position(defaultValue);

            return defaultValue;
        }

        #endregion

        #region Add Stuff

        public static void SetAttr(this XmlElement xml, string attributeName, object setTo)
        {
            XmlAttribute attr;

            if (xml.HasAttr(attributeName))
            {
                attr = xml.Attributes[attributeName];
            }
            else
            {
                attr = xml.OwnerDocument.CreateAttribute(attributeName);
                xml.Attributes.Append(attr);
            }

            attr.Value = setTo.ToString();
        }

        public static void SetChild(this XmlElement xml, string childName, object setTo)
        {
            XmlElement ele;

            if (xml.HasChild(childName))
            {
                ele = xml[childName];
            }
            else
            {
                ele = xml.OwnerDocument.CreateElement(null, childName, xml.NamespaceURI);
                xml.AppendChild(ele);
            }

            ele.InnerText = setTo.ToString();
        }

        public static XmlElement CreateChild(this XmlDocument doc, string childName)
        {
            var ele = doc.CreateElement(null, childName, doc.NamespaceURI);
            doc.AppendChild(ele);
            return ele;
        }

        public static XmlElement CreateChild(this XmlElement xml, string childName)
        {
            var ele = xml.OwnerDocument.CreateElement(null, childName, xml.NamespaceURI);
            xml.AppendChild(ele);
            return ele;
        }

        #endregion

        #endregion

        #region Sorting

        public static int SortLeftToRight(Entity a, Entity b)
        {
            return (int) ((a.X - b.X) * 100);
        }

        public static int SortRightToLeft(Entity a, Entity b)
        {
            return (int) ((b.X - a.X) * 100);
        }

        public static int SortTopToBottom(Entity a, Entity b)
        {
            return (int) ((a.Y - b.Y) * 100);
        }

        public static int SortBottomToTop(Entity a, Entity b)
        {
            return (int) ((b.Y - a.Y) * 100);
        }

        public static int SortByDepth(Entity a, Entity b)
        {
            return a.Depth - b.Depth;
        }

        public static int SortByDepthReversed(Entity a, Entity b)
        {
            return b.Depth - a.Depth;
        }

        #endregion

        #region Debug

        public static void Log()
        {
            Debug.WriteLine("Log");
        }

        public static void LogError(params object[] obj)
        {
            Log(obj);
        }

        public static void TimeLog()
        {
            Debug.WriteLine(Engine.Scene?.RawTimeActive);
        }

        public static void Log(params object[] obj)
        {
            foreach (var o in obj)
                if (o == null)
                    Debug.WriteLine("null");
                else
                    Debug.WriteLine(o.ToString());
        }

        public static void TimeLog(object obj)
        {
            Debug.WriteLine(Engine.Scene?.RawTimeActive + " : " + obj);
        }

        public static void LogEach<T>(IEnumerable<T> collection)
        {
            foreach (var o in collection) Debug.WriteLine(o?.ToString());
        }

        public static void Dissect(object obj)
        {
            Debug.Write(obj.GetType().Name + " { ");
            foreach (var v in obj.GetType().GetFields()) Debug.Write(v.Name + ": " + v.GetValue(obj) + ", ");

            Debug.WriteLine(" }");
        }

        private static Stopwatch? s_stopwatch;

        public static void StartTimer()
        {
            s_stopwatch = new Stopwatch();
            s_stopwatch.Start();
        }

        public static void EndTimer()
        {
            if (s_stopwatch != null)
            {
                s_stopwatch.Stop();

                var message = "Timer: " +
                              s_stopwatch.ElapsedTicks + " ticks, or " +
                              TimeSpan.FromTicks(s_stopwatch.ElapsedTicks).TotalSeconds.ToString("00.0000000") +
                              " seconds";
                Debug.WriteLine(message);
#if DESKTOP && DEBUG
                //Commands.Trace(message);
#endif
                s_stopwatch = null;
            }
        }
        
        public static void Assert(bool condition)
        {
            Assert(condition);
        }
        
        public static void Assert(bool condition, string message)
        {
            Assert(condition, message);
        }

        public static void Assert(bool condition, string message, string detailMessage)
        {
            Assert(condition, message, detailMessage);
        }

        public static void Assert(bool condition, string message, string detailMessageFormat, params object[] args)
        {
            Assert(condition, message, detailMessageFormat, args);
        }

        #endregion
        
        public static bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
        {
            return potentialDescendant.IsSubclassOf(potentialBase)
                   || potentialDescendant == potentialBase;
        }
        
        public static string NormalizePath(string path)
        {
            unsafe
            {
                Span<char> temp = stackalloc char[path.Length];
                for (int i = 0; i < path.Length; i++)
                    temp[i] = path[i];
                return NormalizePath(temp).ToString();
            }
        }

        public static Span<char> NormalizePath(Span<char> path)
        {
            for (int i = 0; i < path.Length; i++)
                if (path[i] == '\\') path[i] = '/';

            int length = path.Length;
            for (int i = 1, t = 1, l = length; t < l; i++, t++)
            {
                if (path[t - 1] == '/' && path[t] == '/')
                {
                    i--;
                    length--;
                }
                else
                    path[i] = path[t];
            }

            return path.Slice(0, length);
        }
    }
}