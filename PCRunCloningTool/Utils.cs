using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace PCRunCloningTool
{
    internal class Utils
    {
        //[ThreadStatic]
        //private static Dictionary<string, Stopwatch> map = new Dictionary<string, Stopwatch>();
        private static ThreadLocal<Dictionary<string, Stopwatch>> map = new ThreadLocal<Dictionary<string, Stopwatch>>(() => { return new Dictionary<string, Stopwatch>(); });

        public static void Unzip(string source, string target)
        {
            if (Directory.Exists(target))
            {
                Directory.Delete(target, true);
            }
            ZipFile.ExtractToDirectory(source, target);
        }

        public static void StartMeasure(string name)
        {
            map.Value.Add(name, Stopwatch.StartNew());
        }

        public static void StopMeasure(string name)
        {
            Stopwatch watch;
            if (map.Value.TryGetValue(name, out watch))
            {
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                map.Value.Remove(name);
                Logger.Log.Info("Measure(" + name + "): " + elapsedMs);
            } else
            {
                Logger.Log.Error("Incorrect measure instance name: " + name);
            }
            
        }
    }
}