/* Program Name: NgramMker.cs
 * Date: Oct 15, 2022
 */

using System.Text;

namespace Ngram
{
    public class NgramMaker
    {
        private const Int32 BUFFER_SIZE = 1024;
        private const Int16 MIN_NGRAM_LEVEL = 2;
        private const Int16 MAX_NGRAM_LEVEL = 4;
        private bool _writeHeader = true;
        private const string DEBUG_FILENAME = "./debug.txt";
        private Dictionary<string, string> _ngramNounsDataList; // <token, data>
        private Dictionary<string, string> _ngramNounsIndexList; // <ngram, token>

        public NgramMaker(string ngramDataFilename, string ngramIndexFilename)
        {
            _ngramNounsDataList = readNgramDbFile(ngramDataFilename);
            _ngramNounsIndexList = readNgramDbFile(ngramIndexFilename);
        }

        public void Generate(string filename)
        {
            string sentence = readSentenceFile(filename);

            string[] words = sentence.TrimEnd('.').Split(" ");

            clearFile();

            for (short level = MIN_NGRAM_LEVEL; level <= MAX_NGRAM_LEVEL; ++level)
            {
                Ngram ngram = new Ngram(sentence);

                generateNgram(ngram, words, level);

                Dictionary<int, string> tokensDict = getNgramToken(ngram.NgramsList());

                parseNgramList(ngram, tokensDict);

                List<string> fileContent = createFileContent(ngram, sentence, level);

                writeToDebugFile(fileContent);

                if (_writeHeader)
                    _writeHeader = false;
            }
        }

        private void generateNgram(Ngram? ngram, string[] words, int ngramLevel)
        {
            if (words.Length == ngramLevel - 1)
                return;

            string ngramWord = "";

            for (int i = 0; i < words.Length; ++i)
            {
                if (i < ngramLevel)
                    ngramWord += $"{words[i]}_";
                else
                    break;
            }

            ngram?.AddNgram(ngramWord.TrimEnd('_'));

            words = words.Skip(1).ToArray();

            generateNgram(ngram, words, ngramLevel);
        }

        private Dictionary<int, string> getNgramToken(List<string> ngramsList)
        {
            Dictionary<int, string> tokensDict = new Dictionary<int, string>();

            foreach (var item in ngramsList.Select((value, index) => new { index, value }))
            {
                string ngram = item.value.ToLower();
                int index = item.index;

                try
                {
                    if (_ngramNounsIndexList.ContainsKey(ngram))
                    {
                        tokensDict.Add(index, _ngramNounsIndexList[ngram]);
                    }
                }
                catch (Exception)
                {
                }
            }

            return tokensDict;
        }

        private string getNgramData(string nounIndex)
        {
            string data = "";

            if (_ngramNounsDataList.ContainsKey(nounIndex))
            {
                data = _ngramNounsDataList[nounIndex];
            }

            return data;
        }

        private void parseNgramList(Ngram ngram, Dictionary<int, string> tokensDict)
        {
            List<string> ngramsList = ngram.NgramsList();

            for (int index = 0; index < ngramsList.Count(); ++index)
            {
                if (!ngramsList[index].Contains(','))
                {
                    ngramsList[index] += ',';
                }
            }

            foreach (var item in tokensDict)
            {
                int ngramIndex = item.Key;
                string[] indexes = item.Value.Split(',');

                string data = parseData(indexes);

                ngramsList[ngramIndex] += $" {data}";

            }
        }

        private string parseData(string[] indexes)
        {
            string data = "";

            if (indexes.Count() > 1)
            {
                foreach (string index in indexes)
                    data += $"{getNgramData(index)}<< and >> ";
            }
            else
            {
                data = getNgramData(indexes[0]);
            }

            return data.Contains(';') ? data.Replace(";", "<< and >>") : data;
        }

        private string readSentenceFile(string filename)
        {
            if (!File.Exists(filename))
                throw new Exception("File not found");

            try
            {
                return File.ReadAllLines(filename)[0];
            }
            catch (IOException iox)
            {
                Console.WriteLine(iox);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return "";
        }

        private Dictionary<string, string> readNgramDbFile(string filename)
        {
            if (!File.Exists(filename))
                throw new Exception("File not found");

            Dictionary<string, string> dataDict = new Dictionary<string, string>();

            try
            {
                using (var fileStream = File.OpenRead(filename))
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BUFFER_SIZE))
                    {
                        string? line;

                        while ((line = streamReader.ReadLine()) != null)
                        {
                            string[] words = line.Split("|");

                            dataDict.Add(words[0], words[1]);
                        }
                    }
            }
            catch (IOException iox)
            {
                Console.WriteLine(iox);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return dataDict;
        }

        private List<string> createFileContent(Ngram ngram, string sentence, short ngramLevel)
        {
            List<string> fileContent = new List<string>();

            if (_writeHeader)
            {
                fileContent.Add($"{sentence}\n\n{ngram.NgramSentence()}");
            }

            fileContent.Add($"\n{ngramLevel} level n-gram\n");

            foreach (string ng in ngram.NgramsList())
                fileContent.Add($"{ng}");

            return fileContent;
        }

        private void clearFile()
        {
            Console.WriteLine(DateTime.Now.ToString());
            Console.WriteLine($"Generating file -> {DEBUG_FILENAME}");
            File.WriteAllText(DEBUG_FILENAME, string.Empty);
        }

        private void writeToDebugFile(List<string> contentList)
        {
            using (StreamWriter sw = File.AppendText(DEBUG_FILENAME))
            {
                foreach (string content in contentList)
                {
                    sw.WriteLine(content);
                }
            }
        }
    }
}
