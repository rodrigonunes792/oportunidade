using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;

namespace MinutoSegurosWordCount
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var reader = System.Xml.XmlReader.Create("https://www.minutoseguros.com.br/blog/feed/"))
            {
                var feed = SyndicationFeed.Load(reader);
                var listOfWords = new List<String>();

                Console.WriteLine("-- Quantidade de palavras por tópico --");

                foreach (var feedItem in feed.Items)
                {
                    var summary = RemovePrepositions(feedItem.Summary.Text);
                    var words = summary.Split(' ').Where(p => !String.IsNullOrEmpty(p));
                    listOfWords.AddRange(words);

                    Console.WriteLine("{0}: {1}",feedItem.Title.Text, words.Count());                    
                }

                Console.WriteLine("\n-- Dez principais palavras abordadas --");

                var wordCount = from word in listOfWords
                                  group word by word.ToLower()
                                  into g                                  
                                  orderby g.Count() descending                                 
                                  select new { Word = g.Key, Count = g.Count() };

                foreach(var word in wordCount.Take(10))
                {
                    Console.WriteLine("{0}: {1} ocorrências", word.Word, word.Count);
                }

                Console.ReadLine();
            }
        }

        public static string RemovePrepositions(string textItem)
        {
            var regexp = new Regex(@"(?i)(<[^>]*?>|No related posts|&nbsp;|\(foto: divulgação\)" +
                           @"|\ba\b|\bante\b|\baté\b|\bapós\b|\bcom\b|\bcontra\b|\bde\b" +
                           @"|\bdesde\b|\bem\b|\bentre\b|\bpara\b|\bper\b|\bperante\b" +
                           @"|\bpor\b|\bsem\b|\bsob\b|\bsobre\b|\btrás\b|\bo\b|\ba\b" +
                           @"|\bos\b|\basb\|\bum\b|\buma\b|\buns\b|\bumas\b|\be\b|\bum\b" +
                           @"|\bdos\b|\bé\b|\bque\b|\bnão\b|\bsão\b|\bmais\b|\bna\b|\bdo\b" +
                           @"|\besse\b|\bno\b|\bda\b|\bse\b|\bseu\b|\bou\b|\bas\b|\bdas\b|\bcom\b" +
                           @"|\t|\n|\r|\.|\bcomo\b|\bser\b|\bsua|\b\d+?\b|\bessa\b)");

            textItem = regexp.Replace(textItem, "");
            textItem = Regex.Replace(textItem, @"\s+", " ");

            return textItem;
        }
    }
}
