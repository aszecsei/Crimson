using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Crimson
{
    public static class Logger
    {
        private static bool ShouldLog(string tag, LogLevel level)
        {
            // TODO
            return true;
        }

        public static void Log(string tag, string str) => Log(LogLevel.Verbose, tag, str);

        public static void Log(LogLevel level, string tag, string str)
        {
            if ( ShouldLog(tag, level) )
            {
                Console.Write("(");
                Console.Write(DateTime.Now);
                Console.Write(") [SectorFive] [");
                Console.Write(level.ToString());
                Console.Write("] [");
                Console.Write(tag);
                Console.Write("] ");
                Console.WriteLine(str);
            }
        }

        public static void LogDetailed(string tag, string str) => LogDetailed(LogLevel.Verbose, tag, str);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void LogDetailed(LogLevel level, string tag, string str)
        {
            if ( ShouldLog(tag, level) )
            {
                Log(level, tag, str);
                Console.WriteLine(new StackTrace(1, true).ToString());
            }
        }

        /// <summary>
        /// Print the exception to the console, including extended loading / reflection data.
        /// </summary>
        public static void LogDetailed(this Exception e, string tag = null) {
            if (tag == null) {
                Console.WriteLine("--------------------------------");
                Console.WriteLine("Detailed exception log:");
            }
            for (Exception e_ = e; e_ != null; e_ = e_.InnerException) {
                Console.WriteLine("--------------------------------");
                Console.WriteLine(e_.GetType().FullName + ": " + e_.Message + "\n" + e_.StackTrace);
                if (e_ is ReflectionTypeLoadException rtle) {
                    for (int i = 0; i < rtle.Types.Length; i++) {
                        Console.WriteLine("ReflectionTypeLoadException.Types[" + i + "]: " + rtle.Types[i]);
                    }
                    for (int i = 0; i < rtle.LoaderExceptions.Length; i++) {
                        LogDetailed(rtle.LoaderExceptions[i], tag + (tag == null ? "" : ", ") + "rtle:" + i);
                    }
                }
                if (e_ is TypeLoadException) {
                    Console.WriteLine("TypeLoadException.TypeName: " + ((TypeLoadException) e_).TypeName);
                }
                if (e_ is BadImageFormatException) {
                    Console.WriteLine("BadImageFormatException.FileName: " + ((BadImageFormatException) e_).FileName);
                }
            }
        }
    }

    public enum LogLevel
    {
        Verbose,
        Debug,
        Info,
        Warn,
        Error,
    }
}
