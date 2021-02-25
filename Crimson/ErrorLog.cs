using System;
using System.IO;
using System.Text;

namespace Crimson
{
    public static class ErrorLog
    {
        public const           string Marker   = "==========================================";
        public static readonly string Filename = "error.txt";

        public static void Open()
        {
            if ( File.Exists(Filename) )
            {
                // Process.Start(Filename);
            }
        }

        public static void Write(Exception e)
        {
            Write(e.ToString());
        }

        public static void Write(string str)
        {
            StringBuilder s       = new StringBuilder();
            string        content = "";

            if ( File.Exists(Filename) )
            {
                StreamReader streamReader = new StreamReader(Filename);
                content = streamReader.ReadToEnd();
                streamReader.Close();
                if ( !content.Contains(Marker) )
                {
                    content = "";
                }
            }

            if ( Engine.Instance != null )
            {
                s.Append(Engine.Instance.Title);
            }
            else
            {
                s.Append("Crimson Engine");
            }

            s.AppendLine(" Error Log");
            s.AppendLine(Marker);
            s.AppendLine();

            if ( Engine.Instance != null && Engine.Instance.Version != null )
            {
                s.Append("Version ");
                s.AppendLine(Engine.Instance.Version.ToString());
            }

            s.AppendLine(DateTime.Now.ToString());
            s.Append(str);
            if ( content != "" )
            {
                int    at    = content.IndexOf(Marker) + Marker.Length;
                string after = content.Substring(at);
                s.AppendLine(after);
            }

            StreamWriter streamWriter = new StreamWriter(Filename, append: false);
            streamWriter.Write(s.ToString());
            streamWriter.Close();
        }
    }
}
