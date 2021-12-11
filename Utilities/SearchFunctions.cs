﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusquedasRPI.Utilities
{
    public class SearchFunctions
    {

        public static String CleanString(String st)
        {
            String vReturn = st == null ? "" : st;

            vReturn = vReturn.ToLower().Trim();
            vReturn = vReturn.Replace("'", "");
            vReturn = vReturn.Replace("%", "");

            return vReturn;
        }

        public static String BuildClassCondition(String classes)
        {
            String vReturn = "";
            if (classes != null && classes.Trim() != "")
            {
                vReturn = "AND B.ClaseId IN ({2}) ";
            }
            return vReturn;
        }

        public static List<String> GetSearchWords(String searchString)
        {
            return searchString.Trim().Split(" ").ToList();
        }

        public static String GetSearchWordValue(Models.SearchWord word)
        {
            String vReturn;

            if (word.SearchReplace)
            {
                vReturn = "%" + AddOrthographyReplacementsRight(CleanString(word.SearchText)) + "%";
            } else
            {
                vReturn = "%" + CleanString(word.SearchText) + "%";
            }

            return vReturn;
        }

        private static void GetExactWords(List<String> words, String searchField, String searchWordCondition, int minSearchLength, ref Models.SearchCondition searchCondition)
        {
            foreach (var word in words)
            {
                //If OR then validate word length, if AND then do not validate word length
                if (
                    (word.Trim().Length >= minSearchLength && searchWordCondition.Trim().ToUpper() == "OR")
                    ||
                    (searchWordCondition.Trim().ToUpper() == "AND")
                )
                {
                    if (searchCondition.Words.Count > 0)
                    {
                        searchCondition.Condition += searchWordCondition.Trim().ToUpper() + " ";
                    }
                    searchCondition.Condition += searchField + " LIKE @SearchText" + searchCondition.Words.Count.ToString() + " ";
                    
                    Models.SearchWord thisWord = new();
                    thisWord.SearchText = CleanString(word);
                    thisWord.SearchReplace = false;
                    searchCondition.Words.Add(thisWord);
                }
            }
        }

        private static string ReverseString(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private static void GetReverseWords(List<String> words, String searchField, ref Models.SearchCondition searchCondition)
        {
            foreach (var word in words)
            {
                if (searchCondition.Words.Count > 0)
                {
                    searchCondition.Condition += "OR ";
                }
                searchCondition.Condition += searchField + " LIKE @SearchText" + searchCondition.Words.Count.ToString() + " ";

                Models.SearchWord thisWord = new();
                thisWord.SearchText = CleanString(ReverseString(word));
                thisWord.SearchReplace = false;
                searchCondition.Words.Add(thisWord);
            }
        }

        private static List<String> ExtractSubWords(String word, int minSearchLength)
        {
            List<String> vReturn = new();

            if (word.Length >= minSearchLength)
            {
                for (int start = 0; start < word.Length; start++)
                {
                    for (int end = minSearchLength; end <= word.Length; end++)
                    {
                        if ((start+end) <= word.Length) {
                            vReturn.Add(word.Substring(start, end));
                        }
                    }
                }
            }

            return vReturn;
        }

        private static void GetSubWords(List<String> words, String searchField, int minSearchLength, ref Models.SearchCondition searchCondition)
        {
            foreach (var word in words)
            {
                List<String> subwords = ExtractSubWords(CleanString(word), minSearchLength);

                foreach (var subword in subwords)
                {
                    if (searchCondition.Words.Count > 0)
                    {
                        searchCondition.Condition += "OR ";
                    }
                    searchCondition.Condition += searchField + " LIKE @SearchText" + searchCondition.Words.Count.ToString() + " ";

                    Models.SearchWord thisWord = new();
                    thisWord.SearchText = CleanString(subword);
                    thisWord.SearchReplace = false;
                    searchCondition.Words.Add(thisWord);
                }
            }
        }

        private static String AddOrthographyReplacementsLeft(String st)
        {
            String vReturn = "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                "REPLACE(" +
                    "LOWER(LTRIM(RTRIM(" + st + "))), " +
                "'ge', 'je')," +
                "'gi', 'ji')," +
                "'ce', 'se')," +
                "'ci', 'si')," +
                "'ke', 'que')," +
                "'ki', 'qui')," +
                "'qe', 'que')," +
                "'qi', 'qui')," +
                "'v', 'b')," +
                "'ll', 'y')," +
                "'sh', 'ch')," +
                "'h', '')," +
                "'z', 's')," +
                "'np', 'mp')," +
                "'nb', 'mb')," +
                "'nv', 'mb')," +
                "'rr', 'r')," +
                "'ka', 'ca')," +
                "'ko', 'co')," +
                "'ku', 'cu')," +
                "'ps', 'x')";

            return vReturn;
        }

        private static String AddOrthographyReplacementsRight(String st)
        {
            String vReturn = st;

            vReturn = vReturn.Trim()
                .ToLower()
                .Replace(" ", "%")
                .Replace("ge", "je")
                .Replace("gi", "ji")
                .Replace("ce", "se")
                .Replace("ci", "si")
                .Replace("ke", "que")
                .Replace("ki", "qui")
                .Replace("qe", "que")
                .Replace("qi", "qui")
                .Replace("v", "b")
                .Replace("ll", "y")
                .Replace("sh", "ch")
                .Replace("h", "")
                .Replace("z", "s")
                .Replace("np", "mp")
                .Replace("nb", "mb")
                .Replace("nv", "mb")
                .Replace("rr", "r")
                .Replace("ka", "ca")
                .Replace("ko", "co")
                .Replace("ku", "cu")
                .Replace("ps", "x");

            return vReturn;
        }

        private static void GetNoOrthographyWords(List<String> words, String searchField, String searchWordCondition, int minSearchLength, ref Models.SearchCondition searchCondition, String collation)
        {
            foreach (var word in words)
            {
                //If OR then validate word length, if AND then do not validate word length
                if (
                    (word.Trim().Length >= minSearchLength && searchWordCondition.Trim().ToUpper() == "OR")
                    ||
                    (searchWordCondition.Trim().ToUpper() == "AND")
                )
                {
                    if (searchCondition.Words.Count > 0)
                    {
                        searchCondition.Condition += searchWordCondition.Trim().ToUpper() + " ";
                    }

                    searchCondition.Condition += 
                        "((CONVERT(varchar(max), " + AddOrthographyReplacementsLeft(searchField) + " ) COLLATE " + collation + ") " +
                        " LIKE (CONVERT(varchar(max), @SearchText" + searchCondition.Words.Count.ToString() + " ) COLLATE " + collation + ") ) ";

                    Models.SearchWord thisWord = new();
                    thisWord.SearchText = CleanString(word);
                    thisWord.SearchReplace = true;
                    searchCondition.Words.Add(thisWord);
                }
            }
        }

        private static Models.SearchCondition DoBuildSearchCondition(List<String> words, String searchField, String searchWordCondition, bool searchSubstrings, int minSearchLength, String searchType, String collation)
        {
            Models.SearchCondition vReturn = new();
            vReturn.Condition = " ";
            vReturn.Words = new List<Models.SearchWord>();

            //Foneticas
            if (searchType == "0")
            {
                //No ortografia
                GetNoOrthographyWords(words, searchField, searchWordCondition, minSearchLength, ref vReturn, collation);
                //Reversas
                GetReverseWords(words, searchField, ref vReturn);
                //Subcadenas mayores o igual al minSearchLength
                if (searchSubstrings)
                {
                    GetSubWords(words, searchField, minSearchLength, ref vReturn);
                }
            }

            //Exactas y titulares
            if (searchType == "1" || searchType == "2")
            {
                //Exactas
                GetExactWords(words, searchField, searchWordCondition, minSearchLength, ref vReturn);
            }

            return vReturn;
        }

        public static Models.SearchCondition BuildSearchCondition(String searchString, String searchType, String searchWordCondition, bool searchSubstrings, int minSearchLength, String collation)
        {
            Models.SearchCondition vReturn = new();
            vReturn.Condition = " ";
            vReturn.Words = new List<Models.SearchWord>();

            if (searchString != null 
                && searchString.Trim() != "" 
                && searchString.Trim().Length >= minSearchLength)
            {
                List<String> words = GetSearchWords(searchString);
                String vSearchField = "";

                //Foneticas y Exactas
                if (searchType == "0" || searchType == "1")
                {
                    vSearchField = "B.Denominacion";
                    
                }

                //Titular
                if (searchType == "2")
                {
                    vSearchField = "B.TitularNombre";
                }

                //Build search
                vReturn = DoBuildSearchCondition(words, vSearchField, searchWordCondition, searchSubstrings, minSearchLength, searchType, collation);
            } 

            return vReturn;
        }
    }
}
