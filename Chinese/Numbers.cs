using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Chinese
{
    

    public static class Numbers
    {
        private static Dictionary<int, char> _chineseNumbers = new Dictionary<int, char>
        {
            {1, '一'},
            { 2, '二'},
            { 3, '三'},
            { 4, '四'},
            { 5, '五'},
            { 6, '六'},
            { 7, '七'},
            { 8, '八'},
            { 9, '九'},
            { 10, '十'},
            { 100, '百'},
            { 1000, '千'},
            { 10000, '萬'}
        };

        private static Dictionary<int, string> _englishnumbers = new Dictionary<int, string>
        {
            {1, "one" },
            {2, "two" },
            {3, "three" },
            {4, "four" },
            {5, "five" },
            {6, "six" },
            {7, "seven" },
            {8, "eight" },
            {9, "nine" },
            {10, "ten" },
            {100, "one hundred" },
            {1000, "one thousand" },
            {10000, "ten thousand" }
        };

        private static Dictionary<int, string> _englishOrdinals = new Dictionary<int, string>
        {
            { 1, "first" },
            { 2, "second" },
            { 3, "third" },
            {4, "fourth" },
            {5, "fifth" },
            {6, "sixth" },
            {7, "seventh" },
            {8, "eighth" },
            {9, "ninth" },
            {10, "tenth" },
            {11, "eleventh" },
            {12, "twelfth" },
            {13, "thirteenth" },
            {14, "fourteenth" },
            {15, "fifteenth" },
            {16, "sixteenth" },
            {17, "seventeenth" },
            {18, "eighteenth" },
            {19, "ninteenth" },
            {20, "twentieth" },
            {30, "thirtieth" },
            {40, "fortieth" },
            {50, "fiftieth" },
            {60, "sixtieth" },
            {70, "seventieth" },
            {80, "eightieth" },
            {90, "nintieth" },
            {100, "hundredth" },
            {1000, "thousandth" }
        };

        public static string ToChinese(int i)
        {
            if(i < 11)
                return SingleDigit(i);
            if(i < 100)
                return TwoDigits(i);
            if(i < 1000)
                return ThreeDigits(i);

            throw new Exception("Negative numbers and numbers > 9999 not yet implemented.");
        }

        public static string ToEnglish(string chinese)
        {
            if (IsOrdinal(chinese))
                return TranslateChineseOrdinal(chinese);

            int value = ConvertChineseToInteger(chinese);
            return GetEnglishNumberString(value);
        }

        public static int ToValue(string chinese)
        {
            return ConvertChineseToInteger(chinese);
        }

        private static string TranslateChineseOrdinal(string chinese)
        {
            string number = chinese.Substring(1);
            int value = ConvertChineseToInteger(number);

            // if the ordinal is below 21 or divides evenly by 10 and under 101, then look up the English
            if (value < 21 || (value <= 100 && value % 10 == 0) )
                return _englishOrdinals[value];

            // if the ordinal is above 100, then add the hundreds place english
            string english = "";
            if (value > 100)
            {
                int hundreds = value / 100;
                english = _englishnumbers[hundreds] + " hundred and ";
                value = value - (hundreds * 100);
            }

            int tens = (value / 10) * 10;
            english += _englishOrdinals[tens];
            return english + "-" + _englishOrdinals[value % tens];
        }

        public static bool IsOrdinal(string chinese)
        {
            if (chinese.Length < 2)
                return false;
            return chinese.Substring(0, 1).Equals("第") && IsChineseNumber(chinese.Substring(1, 1)[0]);
        }

        public static bool IsChineseNumber(char chinese)
        {
            return _chineseNumbers.ContainsValue(chinese);
        }

        #region Helper Methods
        private static string SingleDigit(int i)
        {
            if (i == 0)
                return "";
            else if (_chineseNumbers.ContainsKey(i))
                return _chineseNumbers[i].ToString();
            else
                throw new Exception("Integer " + i + " not found in list of Chinese numbers.");
        }

        private static string TwoDigits(int i)
        {
            if (i == 0)
                return "";
            if (i < 11 || i > 99)
                throw new Exception("i must be between 11 and 99");
            
            int tensDigit = i / 10;
            int onesDigit = i % 10;

            if (tensDigit == 1)
            {
                return SingleDigit(10) + SingleDigit(onesDigit);
            }
            else
            {
                return SingleDigit(tensDigit) + SingleDigit(10) + SingleDigit(onesDigit);
            }
        }

        private static string ThreeDigits(int i)
        {
            if (i < 100 || i > 999)
                throw new Exception("i must be between 100 and 999.");

            int hundredsDigit = i / 100;
            i = i % 100;

            return SingleDigit(hundredsDigit) + SingleDigit(100) + TwoDigits(i);
        }

        private static int ConvertChineseToInteger(string chinese)
        {
            int value = 0;
            var numbers = new List<int>();

            foreach(char chNumber in chinese)
            {
                numbers.Add(ChineseCharToInt(chNumber));
            }

            while(numbers.Count > 0)
            {
                if(numbers[0] > 9)
                {
                    value += numbers[0];
                    numbers.RemoveAt(0);
                }
                else
                {
                    if (numbers.Count > 1 && numbers[1] > 9)
                    {
                        value += numbers[0] * numbers[1];
                        numbers.RemoveAt(0);
                        numbers.RemoveAt(0);
                    }
                    else
                    {
                        value += numbers[0];
                        numbers.RemoveAt(0);
                    }
                }
            }

            return value;
        }

        private static int ChineseCharToInt(char chinese)
        {
            if (_chineseNumbers.ContainsValue(chinese))
                return _chineseNumbers.First(k => k.Value.Equals(chinese)).Key;
            else
                throw new ArgumentException("Chinese character " + chinese + " is not a number or not supported yet");
        }

        private static string GetEnglishNumberString(int number)
        {
            if(_englishnumbers.ContainsKey(number))
            {
                return _englishnumbers[number];
            }
            else
            {
                return string.Format("{0:n0}", number);
            }
        }
        #endregion
    }
}
