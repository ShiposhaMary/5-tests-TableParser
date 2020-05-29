using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TableParser
{
    [TestFixture]
    public class QuotedFieldTaskTests
    {
        [TestCase("''", 0, "", 2)]
        [TestCase("'a'", 0, "a", 3)]

        public void Test(string line, int startIndex, string expectedValue, int expectedLength)
        {
            var actualToken = My_QuotedFieldTask.ReadQuotedField(line, startIndex);
            Assert.AreEqual(actualToken, new Token(expectedValue, startIndex, expectedLength));
        }
        
        // Добавьте свои тесты
    }

    class My_QuotedFieldTask
    {
        public static Token ReadQuotedField(string line, int startIndex)
        {
            var endIndex = FindLastQuoteIndexOfToken(line, startIndex);
            string tokenValue;
            int tokenLengthInLine;

            if (endIndex == -1)
            {
                tokenLengthInLine = line.Length - startIndex;
                endIndex = line.Length;
                tokenValue = line.Substring(startIndex + 1, line.Length - startIndex - 1);
            }
            else
            {
                tokenLengthInLine = endIndex - startIndex + 1;
                tokenValue = line.Substring(startIndex + 1, endIndex - startIndex - 1);
            }

            tokenValue = RemoveEscapingSymbols(tokenValue);

            return new Token(tokenValue, startIndex, tokenLengthInLine);
        }

        public static int FindLastQuoteIndexOfToken(string line, int firstQuoteIndex)
        {
            var lastQuoteIndex = -1;
            var typeOfQuot = line[firstQuoteIndex];
            var countOfConsecutiveSlashes = 0;

            for (int i = firstQuoteIndex + 1; i < line.Length; i++)
            {
                if (line[i] == typeOfQuot && countOfConsecutiveSlashes % 2 == 0)
                {
                    lastQuoteIndex = i;
                    break;
                }
                if (line[i] == '\\')
                {
                    countOfConsecutiveSlashes++;
                }
                else
                {
                    countOfConsecutiveSlashes = 0;
                }
            }

            return lastQuoteIndex;
        }

        public static string RemoveEscapingSymbols(string line)
        {
            var lastSlashIndex = -2;
            var isPreviousSymbolIsNotRealSlash = false;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '\\')
                {
                    if (isPreviousSymbolIsNotRealSlash)
                    {
                        line = line.Remove(i, 1);
                        isPreviousSymbolIsNotRealSlash = false;
                    }
                    else
                    {
                        lastSlashIndex = i;
                        isPreviousSymbolIsNotRealSlash = true;
                    }
                }
                else if (IsQuote(line[i]) && i - lastSlashIndex == 1)
                {
                    line = line.Remove(lastSlashIndex, 1);
                }
            }
            return line;
        }

        public static bool IsQuote(char symbol)
        {
            return symbol == '\'' || symbol == '\"';
        }
    }
}