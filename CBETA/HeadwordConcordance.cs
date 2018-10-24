using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Utilities.Chinese;
using Utilities.IOFunctions;

namespace Utilities.CBETA
{
    public class HeadwordConcordance : IFileOutput
    {
        private Database _db;
        private string _inputPath;
        private List<string> _lines;
        private Dictionary<string, List<string>> _concordance;
        private HashSet<string> _nonCjk = 
            new HashSet<string> { "　", " ", "N", "o", "s", "", "", "",
                ".", "。", "？", "！", ",", "，", "、", "：", "；", "＊", "*", "-",
                "[", "]", "(", ")", "「", "」", "〈", "〉",
                "『", "』",
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
        private HashSet<string> _dbTerms;
        private string _termSelectStatement = "SELECT term_chinese, headword.radical_id, radical_string, hword_strokecount," +
                    " speech_fullname, speech_abbrev, reading_english, reading_plural, reading_notes " +
                "FROM headword JOIN term ON headword.hword_id = term.hword_id " +
                      "JOIN reading ON reading.term_id = term.term_id " +
                      "JOIN radical ON headword.radical_id = radical.radical_id " +
                      "JOIN part_of_speech ON reading.speech_id = part_of_speech.speech_id;";


        public HeadwordConcordance(Database database, string inputPath = null, string outputPath = null) 
        {
            _db = database;
            _dbTerms = FetchTermsListFromDB();
            SetIOPaths(inputPath, outputPath);

            _lines = LoadSourcetexts();

            _concordance = BuildConcordanceOfHeadwords(GetKeywordsFor("T02n0124"));
        }

        private HashSet<string> FetchTermsListFromDB()
        {
            var data = _db.Select(_termSelectStatement);
            var termSet = new HashSet<string>();

            foreach(object[] record in data)
            {
                string term = (string)record[0];
                termSet.Add(term);
            }

            return termSet;
        }

        public string OutputDirectory { get; private set; }
        public string OutputFilename { get; set; }

        private void SetIOPaths(string input, string output)
        {
            if (string.IsNullOrEmpty(output))
                OutputDirectory = @"C:\Users\Charlie\Desktop\Translation\swan_song_project\lexicon\";
            else
                OutputDirectory = output;

            if (string.IsNullOrEmpty(input))
                _inputPath = @"C:\Users\Charlie\Desktop\Translation\swan_song_project\";
            else
                _inputPath = input;

            OutputFilename = "headwords_concordance.txt";
        }

        #region Loading Source Texts
        private List<string> LoadSourcetexts()
        {
            List<string> subDirs = Directory.EnumerateDirectories(_inputPath, "T*").ToList();
            List<string> sourceLines = new List<string>();
            foreach(string dir in subDirs)
            {
                sourceLines.AddRange(LoadSourceFiles(dir));
            }

            return sourceLines;
        }

        private IEnumerable<string> LoadSourceFiles(string dir)
        {
            List<string> files = Directory.EnumerateFiles(dir, "T????f???.txt").ToList();
            List<string> otherFiles = Directory.EnumerateFiles(dir, "T???f???.txt").ToList();
            files.AddRange(otherFiles);

            List<string> lines = new List<string>();
            foreach (string file in files)
            {
                lines.AddRange(LoadFiles.TextFile(file));
            }

            return lines;
        }
        #endregion


        #region Building Concordance
        private Dictionary<string, List<string>> BuildConcordanceOfHeadwords()
        {
            var concordance = new Dictionary<string, List<string>>();
            string[] lineSplit;
            foreach(string line in _lines)
            {
                lineSplit = line.Split('║');
                if (lineSplit.Length < 2)
                    continue;

                string[] content = lineSplit[1].Split(' ');

                foreach (string cjk in content)
                {
                    if(!_dbTerms.Contains(cjk))
                    {
                        foreach(string c in Utf8.AsCodePoints(cjk))
                        {
                            if (!_nonCjk.Contains(c))
                            {
                                if (!concordance.ContainsKey(c))
                                    concordance[c] = new List<string>();
                                concordance[c].Add(line);
                            }
                        }
                    }
                    else
                    {
                        if (!_nonCjk.Contains(cjk))
                        {
                            if (!concordance.ContainsKey(cjk))
                                concordance[cjk] = new List<string>();
                            concordance[cjk].Add(line);
                        }
                    }
                }
            }
            return concordance;
        }

        private Dictionary<string, List<string>> BuildConcordanceOfHeadwords(HashSet<string> keywords)
        {
            var concordance = new Dictionary<string, List<string>>();
            string[] lineSplit;
            var lines = new List<string>(_lines);
            for(int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                string nextLine = "";
                if (i + 1 <= lines.Count - 1)
                    nextLine = lines[i + 1];

                lineSplit = line.Split('║');
                if (lineSplit.Length < 2)
                    continue;

                string[] content = lineSplit[1].Split(' ');
                bool straddle;
                string cleanCjk;
                foreach (string cjk in content)
                {
                    if (cjk.Length < 1)
                        continue;
                    cleanCjk = cjk;
                    straddle = false;

                    if(cjk[cjk.Length - 1].Equals('-'))
                    {
                        straddle = true;
                        cleanCjk = cjk.TrimEnd('-');
                        string firstWord = GetFirstWordOf(nextLine);
                        lines[i + 1] = RemoveFirstWordFrom(nextLine, firstWord);
                        cleanCjk += firstWord;
                    }

                    cleanCjk = RemoveNonCjkCharsFrom(cleanCjk);
                    if (!_dbTerms.Contains(cleanCjk))
                    {
                        foreach (string c in Utf8.AsCodePoints(cleanCjk))
                        {
                            if (!_nonCjk.Contains(c) && keywords.Contains(c))
                            {
                                if (!concordance.ContainsKey(c))
                                    concordance[c] = new List<string>();
                                concordance[c].Add(_lines[i]);
                            }
                        }
                    }
                    else
                    {
                        if (keywords.Contains(cleanCjk))
                        {
                            if (!concordance.ContainsKey(cleanCjk))
                                concordance[cleanCjk] = new List<string>();
                            concordance[cleanCjk].Add(_lines[i]);
                            if (straddle)
                                concordance[cleanCjk].Add(_lines[i + 1]);
                        }
                    }
                }
            }
            return concordance;
        }

        private HashSet<string> GetKeywordsFor(string taisho)
        {
            var taishoLines = _lines.FindAll(l => l.Substring(0, 8).Equals(taisho));
            var keywords = new HashSet<string>();
            string[] lineSplit;
            for(int i = 0; i < taishoLines.Count; i++)
            {
                string line = taishoLines[i];
                string nextLine = "";
                if (i + 1 <= taishoLines.Count - 1)
                    nextLine = taishoLines[i + 1];

                lineSplit = line.Split('║');
                if (lineSplit.Length < 2)
                    continue;

                string[] content = lineSplit[1].Split(' ');

                foreach (string cjk in content)
                {
                    if (cjk.Length == 0)
                        continue;

                    string cleanCjk;
                    if(cjk[cjk.Length - 1].Equals("-"))
                    {
                        cleanCjk = cjk.Substring(0, cjk.Length - 1);
                        string firstWord = GetFirstWordOf(nextLine);
                        nextLine = RemoveFirstWordFrom(nextLine, firstWord);
                        cleanCjk += firstWord;
                    }

                    cleanCjk = RemoveNonCjkCharsFrom(cjk);
                    if (!_dbTerms.Contains(cleanCjk))
                    {
                        foreach (string c in Utf8.AsCodePoints(cleanCjk))
                        {
                            if (!_nonCjk.Contains(c))
                                keywords.Add(c);
                        }
                    }
                    else
                        keywords.Add(cleanCjk);
                }
            }
            return keywords;
        }

        private string RemoveFirstWordFrom(string nextLine, string firstWord)
        {
            firstWord += " ";
            return nextLine.Remove(18, firstWord.Length);
        }

        private string GetFirstWordOf(string nextLine)
        {
            string[] nextSplit = nextLine.Split('║');
            string[] nextWords = nextSplit[1].Split(' ');
            return nextWords[0];
        }
        #endregion


        #region Creating Output File Contents
        public List<string> GetFileOutput()
        {
            var lines = new List<string>();

            lines.AddRange(OutputHeader());
            lines.AddRange(OutputBody());

            return lines;
        }

        private List<string> OutputHeader()
        {
            return new List<string>
            {
                "\t\t\t\t SWAN SONG PROJECT",
                "",
                "HEADWORDS CONCORDANCE",
                "",
                ""
            };
        }

        private List<string> OutputBody()
        {
            var lines = new List<string>();

            var data = new List<Tuple<string, List<string>>>();
            foreach(KeyValuePair<string, List<string>> pair in _concordance)
            {
                data.Add(new Tuple<string, List<string>>(pair.Key, pair.Value));
            }
            data = data.OrderByDescending(d => d.Item2.Count).ToList();

            foreach(Tuple<string, List<string>> datum in data)
            {
                lines.Add(">>" + datum.Item1 + "  (" + datum.Item2.Count + ")");
                List<string> concord = datum.Item2;
                concord.Sort();
                foreach(string line in concord)
                {
                    lines.Add("\t" + line);
                } 
            }

            return lines;
        }
        #endregion

        #region Helper Functions
        private string RemoveNonCjkCharsFrom(string term)
        {
            string[] termSplit = Regex.Split(term, string.Empty);
            term = "";
            foreach(string c in termSplit)
            {
                if (!_nonCjk.Contains(c))
                    term += c;
            }

            return term;
        }
        
        #endregion
    }
}