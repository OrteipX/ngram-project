/* Program Name: Program.cs
 * Date: Oct 15, 2022
 */

using System.Runtime.InteropServices;

namespace Ngram
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
                throw new Exception("A file must be passed in as first command arg");
            else if (args.Length > 1)
                throw new Exception("Only one argument must be passed in cli");

            string nounsData = (getOsType() == "linux")
                ? "./NounsData.txt"
                : $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\..\\NounsData.txt";

            string nounsIndex = (getOsType() == "linux")
                ? "./NounsIndex.txt"
                : $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\..\\NounsIndex.txt";

            NgramMaker ngram = new NgramMaker(nounsData, nounsIndex);

            string filename = (getOsType() == "linux")
                ? args[0]
                : $"{AppDomain.CurrentDomain.BaseDirectory}..\\..\\..\\{args[0]}";

            ngram.Generate(filename);
        }

        private static string getOsType()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "windows";
            else
                return "linux";
        }
    }
}
