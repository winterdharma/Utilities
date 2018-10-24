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
        private string _newline = "\n";
        private string _silentReading = "∅";

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
                // skip this iteration if it's an empty line
                if (SourcetextLineIsHeaderOnly(source[i]))
                    continue;

                // setup variables for this line and the next line
                List<string> lineItems = source[i];
                List<string> nextItems = new List<string>();
                if (IsNotLastItem(i, _sourcetext))
                    nextItems = source[i + 1];

                // build a list of english equivalents
                List<string> englishItems = TranslateCbetaItemsIntoEnglishItems(lineItems, nextItems);

                // build a string for the translation of each line
                string item, nextItem, previousItem;
                for(int j = 0; j < englishItems.Count; j++)
                {
                    item = nextItem = previousItem = "";
                    item = englishItems[j];

                    // don't do any more work if item is empty
                    if (string.IsNullOrEmpty(item))
                        continue;

                    // handle newline characters
                    if (item.Equals(_newline))
                    {
                        if (englishItems.Count > 2)
                        {
                            draftTranslation.Add(draft);
                            draft = "";
                            draft += item;
                        }
                        else
                        {
                            draftTranslation.Add(draft);
                            draft = "";
                            break;
                        }
                    }

                    if (j > 0)
                        previousItem = englishItems[j - 1];
                    if (j < englishItems.Count - 1)
                        nextItem = englishItems[j + 1];

                    // add whitespace for various situations
                    item = AddWhitespaceToEnglishItemAsNeeded(item, previousItem, nextItem);

                    // append the english item to the draft of this paragraph
                    draft += item;
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
            if (string.IsNullOrEmpty(item))
                return item;

            int readingNumber;
            string translation = "";

            if (item.Length > 1 && char.IsDigit(item.Last()))
            {
                string number = item.Last().ToString();
                item = item.Truncate(1);
                if (item.Length > 1 && char.IsDigit(item.Last()))
                {
                    number = number.Insert(0, item.Last().ToString());
                    item = item.Truncate(1);
                }
                readingNumber = int.Parse(number);
                translation = GetTermReading(item, readingNumber);
            }
            else if(item.Last().Equals('-'))
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

        #region Helper Methods

        #region Predicates
        private bool SourcetextLineIsHeaderOnly(List<string> splitLine)
        {
            return splitLine.Count == 1;
        }

        private bool IsNotLastItem(int i, List<string> list)
        {
            return i < list.Count - 1;
        }

        private bool IsCbetaLineHeader(string str)
        {
            return str.Contains("_p");
        }

        private bool IsNewParagraphSymbol(string str)
        {
            return str.Equals(_newParagraph);
        }

        private bool IsNotAnEmptyString(string str)
        {
            return string.IsNullOrEmpty(str);
        }

        private bool IsQuotationMark(string str)
        {
            return str.Equals("\"") || str.Equals("'");
        }

        private bool IsSentenceFinalPunct(string str)
        {
            return _sentenceFinalPunct.Contains(str);

        }
        #endregion

        #region Process Sourcetext Helpers
        private List<string> TranslateCbetaItemsIntoEnglishItems(List<string> thisLineItems, List<string> nextLineItems)
        {
            List<string> engItems = new List<string>();
            foreach (string item in thisLineItems)
            {
                if (IsCbetaLineHeader(item))
                    continue;

                if (IsNewParagraphSymbol(item))
                {
                    engItems = AddParagraphHeaderToEnglishItems(thisLineItems[0], engItems);
                    continue;
                }

                engItems.Add(Translate(item, nextLineItems, out nextLineItems));
            }
            engItems.RemoveAll(item => item.Equals(_silentReading));

            return engItems;
        }

        private List<string> AddParagraphHeaderToEnglishItems(string lineHeader, List<string> engItems)
        {
            string pageRef = "[" + lineHeader.Substring(10) + "]";
            engItems.Add("\n");
            engItems.Add(pageRef);
            return engItems;
        }

        private string AddWhitespaceToEnglishItemAsNeeded(string item, string previousItem, string nextItem)
        {
            string itemFirstChar = item.Substring(0, 1);
            
            string nextItemFirstChar = "";
            if (IsNotAnEmptyString(nextItem))
                nextItemFirstChar = nextItem.Substring(0, 1);

            if (string.IsNullOrEmpty(nextItem) || (IsQuotationMark(item) && IsSentenceFinalPunct(previousItem)) )
                
                item += " ";
            else if ((!_punctuation.Contains(itemFirstChar) && _punctuation.Contains(nextItemFirstChar)) ||
                (_punctuation.Contains(itemFirstChar) && _punctuation.Contains(nextItemFirstChar)))
            {
                
            }
            else if (IsQuotationMark(item) && !_punctuation.Contains(nextItemFirstChar))
                item = " " + item;
            else
                item += " ";

            return item;
        }
        #endregion

        #endregion
    }
}
