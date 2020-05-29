using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace TableParser
{
    [TestFixture]
    public class FieldParserTaskTests
    {   [TestCase("text", new[] { "text" })]
        [TestCase("hello world", new[] { "hello", "world" })]
        [TestCase("", new string[0])]
        [TestCase("'x y'", new[] { "x y" })]
        [TestCase("\"a 'b' 'c' d\"", new[] { "a 'b' 'c' d" })]
        [TestCase("'\"1\"", new[] { "\"1\"" })]
        [TestCase("a\"b c d e\"", new[] { "a", "b c d e" })]
        [TestCase("\"b c d e\"f", new[] { "b c d e", "f" })]
        [TestCase(" 1 ", new[] { "1" })]
        [TestCase(@"""abc ", new[] { "abc " })]
        [TestCase("''", new[] { "" })]
        [TestCase("\"a \\\"c\"", new[] { "a \"c" })]
        [TestCase("\'a \\\'c\'", new[] { "a \'c" })]
        [TestCase("\'a\'  \'b\'", new[] { "a", "b" })]
        [TestCase(@"""\\""", new[] { "\\" })]

        public static void RunTests(string input, string[] expectedResult)
        {
            var actualResult = My_FieldsParserTask.ParseLine(input);
            Assert.AreEqual(expectedResult.Length, actualResult.Count);
            for (int i = 0; i < expectedResult.Length; ++i)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i].Value);
            }
        }  
       
    }

    public class My_FieldsParserTask
    {
        public static List<Token> ParseLine(string line)
        {
            if (line.Replace(" ", string.Empty) == string.Empty) return new List<Token> {};
            if (!line.Contains("\"") && !line.Contains("'")) return SimpleField(line);
            else return QuotedField(line);
  
        }
        
        private static Token ReadField(string line, int startIndex)
        {
            return new Token(line, 0, line.Length);
        }
        public static List<Token> QuotedField(string line)
        {
            var token = new Token(line, 0, line.Length);
            List<Token> quotedToken = new List<Token>();
            int simple=0;
            for (int i = 0; i < line.Length; i++)                  
            {
               // simple++;
                if ((line[i] == '\\' && line[i + 1] == '\'') || (line[i] == '\\' && line[i + 1] == '"'))
                {   if(simple>0)
                    { string field = line.Substring(i-simple, simple);
                        token = ReadField(field, 0);
                        if (token.Value != " ") quotedToken.Add(token);
                    }


                    token = ReadQuotedField(line, i + 1);
                    quotedToken.Add(token);
                    i = token.GetIndexNextToToken()-1;
                    simple = 0;
                    continue;
                }
                if ((line[i] == '\'') || (line[i] == '"'))
                {   if (simple>0)
                    {
                        string field = line.Substring(i - simple, simple);
                        token = ReadField(field, 0);
                        if (token.Value != " ") quotedToken.Add(token);
                    }
                    token = ReadQuotedField(line, i);
                    quotedToken.Add(token);
                    i = token.GetIndexNextToToken()-1;
                    simple = 0;
                    continue;
                }
                simple++;
               // string field = line.Substring(i,simple);

                //if (!field.Contains("\"") && !field.Contains("'"))

                //{
                //    token = ReadField(field, 0);
                //    if(token.Value!=" ") quotedToken.Add(token);
                //    simple = 0;
                //    continue;
                //}  
                
            }

            return quotedToken;
        }
        public static List<Token> SimpleField(string line)
        {
            var token = new Token(line, 0, line.Length);
            List<string> simpleList = line.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            List<Token> simpleToken = new List<Token>();
            for (int i = 0; i < simpleList.Count; i++)
            {
                token = ReadField(simpleList[i], 0);
                simpleToken.Add(token);

            }

            return simpleToken;
        }
        public static Token ReadQuotedField(string line, int startIndex)
        {
            return My_QuotedFieldTask.ReadQuotedField(line, startIndex);
        }
    }
}