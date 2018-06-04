using System.IO;
using System.IO.Compression;

namespace PCRunCloningTool
{
    internal class Utils
    {
        public static void Unzip(string source, string target)
        {
            if (Directory.Exists(target))
            {
                Directory.Delete(target, true);
            }
            ZipFile.ExtractToDirectory(source, target);
        }

    }
}