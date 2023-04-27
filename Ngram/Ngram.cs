/* Program Name: Ngram.cs
 * Date: Oct 15, 2022
 */

namespace Ngram
{
    public class Ngram
    {
        private List<string> _ngramsList = new List<string>();
        private static string? _ngramSentence;

        public Ngram(string sentence)
        {
            _ngramSentence = sentence.TrimEnd('.');
        }

        public string NgramSentence()
        {
            return string.IsNullOrEmpty(_ngramSentence) ? "" : _ngramSentence;
        }

        public List<string> NgramsList()
        {
            return _ngramsList;
        }

        public void AddNgram(string ngram)
        {
            _ngramsList.Add(ngram);
        }
    }
}
