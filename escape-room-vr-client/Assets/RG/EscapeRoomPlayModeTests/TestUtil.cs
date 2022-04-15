using System.IO;
using NUnit.Framework;

namespace RG.Tests
{
    public class TestUtil
    {
        public static string ReadTextFile(string path)
        {
            var pathToJson = PathToFile(path);
            return File.ReadAllText(pathToJson);
        }

        public static string PathToFile(string path)
        {
            var pathToJson = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory,
                "..",
                "..",
                path));
            return pathToJson;
        }
    }
}