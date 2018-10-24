using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.IOFunctions;
using Utilities.Strings;

namespace Utilities.CBETA
{
    public class EnglishDraft : IFileOutput
    {
        private string _inputPath;
        private Database _db;
        private string _selectTermReadings;
        private List<string> _sourcetext;
        private List<string> _draftTranslation;
        private HashSet<string> _punctuation = new HashSet<string> { ",", ".", ";", ":", "\"", "'", "?", "!"};
        private HashSet<string> _sentenceFinalPunct = new HashSet<string> { ".", "!", "?" };
        private string _newParagraph = "{p}";

        public EnglishDraft(Database db, string selectReadings, string sourcePath)
        {
            _db = db;
            _selectTermReadings = selectReadings;
            SetIOPaths(sourcePath);
            _sourcetext = LoadFiles.TextFile(sourcePath);
            _draftTranslation = ProcessSourcetext();
        }

        

        private void SetIOPaths(string inputPath)
        {
            _inputPath = inputPath;
            OutputDirectory = inputPath.Truncate(13);
            OutputFilename = "english_draft.txt";
        }

        public string OutputDirectory { get; set; }

        public string OutputFilename { get; set; }

        private List<string> ProcessSourcetext()
        {
            var draftTranslation = new List<string>();
            var source = SplitSourceLinesIntoItems();
            string draft = "";

            for (int i = 0; i < _sourcetext.Count; i++)
            {
                if (source[i].Count == 1)
                    continue;

                List<string> lineItems = source[i];
                List<string> nextItems = new List<string>();
                if (i < _sourcetext.Count - 1)
                    nextItems = source[i + 1];

                List<string> engItems = new List<string>();
                foreach(string item in lineItems)
                {
                    if(item.Contains("_p"))
                        continue;

                    if(item.Equals(_newParagraph))
                    {
                        string pageRef = "[" + lineItems[0].Substring(10) + "]";
                        engItems.Add("\n");
                        engItems.Add(pageRef);
                        continue;
                    }
                    engItems.Add(Translate(item, nextItems, out nextItems));
                }
                engItems.RemoveAll(item => item.Equals("∅"));
                
                for(int j = 0; j < engItems.Count; j++)
                {
                    var first = "";
                    if(engItems[j].Length > 0)
                        first = engItems[j].Substring(0, 1);
                    var nextFirst = "";
                    if(j < engItems.Count - 1 && engItems[j + 1].Length > 0)
                        nextFirst = engItems[j + 1].Substring(0, 1);

                    if (engItems[j].Equals("\n"))
                    {
                        if (engItems.Count > 2)
                        {
                            draftTranslation.Add(draft);
                            draft = "";
                            draft += engItems[j];
                        }
                        else
                        {
                            draftTranslation.Add(draft);
                            draft = "";
                            break;
                        }
                    }
                    else if (engItems[j].Length == 0)
                        continue;
                    else if (j == engItems.Count - 1)
                        draft += engItems[j] + " ";
                    else if ((!_punctuation.Contains(first) && _punctuation.Contains(nextFirst)) ||
                        (_punctuation.Contains(first) && _punctuation.Contains(nextFirst)))
                    {
                        draft += engItems[j];
                    }
                    else if ((engItems[j].Equals("\"") || engItems[j].Equals("'")) && _sentenceFinalPunct.Contains(engItems[j - 1]))
                        draft += engItems[j] + " ";
                    else if ((engItems[j].Equals("\"") || engItems[j].Equals("'")) && !_punctuation.Contains(nextFirst))
                        draft += " " + engItems[j];
                    else
                        draft += engItems[j] + " ";
                }
            }
            draftTranslation.Add(draft);
            return draftTranslation;
        }

        private List<List<string>> SplitSourceLinesIntoItems()
        {
            var source = new List<List<string>>();
            foreach (string line in _sourcetext)
            {
                List<string> lineItems = new List<string>();
                string[] lineSplit = line.Split('║');
                lineItems.Add(lineSplit[0]);

                if (lineSplit.Length < 2)
                {
                    source.Add(lineItems);
                    continue;
                }

                lineItems.AddRange(lineSplit[1].Split(' '));
                source.Add(lineItems);
            }
            return source;
        }

        private string Translate(string item, List<string> next, out List<string> nextItems)
        {
            nextItems = next;
            if (item.Length < 1)
                return "";

            int readingNumber;
            string translation = "";

            if (char.IsDigit(item[item.Length - 1]))
            {
                string number = item[item.Length - 1].ToString();
                item = item.Truncate(1);
                if (item.Length > 1 && char.IsDigit(item[item.Length - 1]))
                {
                    number = number.Insert(0, item[item.Length - 1].ToString());
                    item = item.Truncate(1);
                }
                readingNumber = int.Parse(number);
                translation = GetTermReading(item, readingNumber);
            }
            else if(item[item.Length - 1].Equals('-'))
            {
                item = item.Truncate(1);
                nextItems[1] = item + nextItems[1];
            }
            else
                return item;

            return translation;
        }

        private string GetTermReading(string term, int readingNumber)
        {
            string select = _selectTermReadings + term + "';";
            var readings = _db.Select(select);
            if(readings.Count >= readingNumber)
            {
                return (string)readings[readingNumber - 1][0];
            }
            else
            {
                return "<NO READING>";
            }
        }

        public List<string> GetFileOutput()
        {
            return _draftTranslation;
        }
    }
}
