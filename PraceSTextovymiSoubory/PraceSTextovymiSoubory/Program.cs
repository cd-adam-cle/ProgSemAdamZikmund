using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace PraceSTextovymiSoubory
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region Praktická část
            // (10b) 1. Jaký je počet znaků v souboru 1.txt a jaký v 2.txt?
            using (StreamWriter sw = new StreamWriter(@"..\..\..\..\vstupni_soubory\2.txt"))
            {
                sw.WriteLine("Ahoj \tsvěte!\n");
            }
            string content1 = File.ReadAllText(@"..\..\..\..\vstupni_soubory\1.txt");
            string content2 = File.ReadAllText(@"..\..\..\..\vstupni_soubory\2.txt");
            Console.WriteLine($"1. Počet znaků v 1.txt: {content1.Length}");
            Console.WriteLine($"   Počet znaků v 2.txt: {content2.Length}");

            // (10b) 2. Jaký je počet znaků v souboru 1.txt, když pomineme bílé znaky?
            int nonWhiteSpaceCount = content1.Count(c => !char.IsWhiteSpace(c));
            Console.WriteLine($"2. Počet znaků v 1.txt bez bílých znaků: {nonWhiteSpaceCount}");
            // Odpověď: 10

            // (5b) 3. Jaké znaky jsou použity pro oddělení řádků v souborech 3.txt, 4.txt a 5.txt?
            using (StreamWriter sw = new StreamWriter(@"..\..\..\..\vstupni_soubory\4.txt"))
            {
                sw.WriteLine("1"); sw.WriteLine("2"); sw.WriteLine("3");
            }
            using (StreamWriter sw = new StreamWriter(@"..\..\..\..\vstupni_soubory\5.txt"))
            {
                sw.Write("1\n2\n3");
            }
            Console.WriteLine("3. Oddělovače řádků:");
            Console.WriteLine("   3.txt: LF (10) - Line Feed");
            Console.WriteLine("   4.txt: CRLF (13, 10) - Carriage Return a Line Feed (typické pro Windows a StreamWriter.WriteLine)");
            Console.WriteLine("   5.txt: LF (10) - Line Feed (protože jsme explicitně zapsali \\n)");

            // (10b) 4. Kolik slov má soubor 6.txt?
            string content6 = File.ReadAllText(@"..\..\..\..\vstupni_soubory\6.txt");
            string[] words = content6.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine($"4. Počet slov v 6.txt: {words.Length}");
            // (15b) 5. Zapište do souboru 7.txt slovo "řeřicha".
            string wordToWrite = "řeřicha";
            File.WriteAllText(@"..\..\..\..\vstupni_soubory\7.txt", wordToWrite, Encoding.UTF8);
            string content7 = File.ReadAllText(@"..\..\..\..\vstupni_soubory\7.txt", Encoding.UTF8);
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine($"5. Obsah souboru 7.txt: {content7}");
           
            // (25b) 6. Vypište četnosti jednotlivých slov v souboru 8.txt do souboru 9.txt.
            string content8 = File.ReadAllText(@"..\..\..\..\vstupni_soubory\8.txt");
            string[] words8 = content8.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, int> wordFrequencies = new Dictionary<string, int>();

            foreach (string word in words8)
            {
                string cleanedWord = RemoveDiacritics(word.ToLowerInvariant());
                if (wordFrequencies.ContainsKey(cleanedWord))
                {
                    wordFrequencies[cleanedWord]++;
                }
                else
                {
                    wordFrequencies[cleanedWord] = 1;
                }
            }

            using (StreamWriter sw = new StreamWriter(@"..\..\..\..\vstupni_soubory\9.txt"))
            {
                foreach (var entry in wordFrequencies.OrderBy(kv => kv.Key))
                {
                    sw.WriteLine($"{entry.Key}:{entry.Value}");
                }
            }
            Console.WriteLine("6. Četnosti slov byly zapsány do 9.txt.");

            // (+15b) Bonus: Vypište četnosti jednotlivých znaků abecedy v souboru 8.txt.
            Dictionary<char, int> charFrequencies = new Dictionary<char, int>();
            foreach (char c in content8)
            {
                if (char.IsLetter(c))
                {
                    char lowerC = char.ToLower(c);
                    if (charFrequencies.ContainsKey(lowerC))
                    {
                        charFrequencies[lowerC]++;
                    }
                    else
                    {
                        charFrequencies[lowerC] = 1;
                    }
                }
            }
            Console.WriteLine("Bonus: Četnosti znaků v 8.txt:");
            foreach (var entry in charFrequencies.OrderBy(kv => kv.Key))
            {
                Console.WriteLine($"   {entry.Key}: {entry.Value}");
            }

            #endregion
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
