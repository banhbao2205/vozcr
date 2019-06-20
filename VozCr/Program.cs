using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace VozCr
{
    class Program
    {
        public static string UrlHome = "https://forums.voz.vn/forumdisplay.php?f=17";
        public static string UrlTopic = "https://forums.voz.vn/showthread.php?t=";
        public static IDictionary<int, string> dictTopic = new Dictionary<int, string>();
        public static string topicId = "";
        public static int page = 0;

        static void Main(string[] args)
        {
            var cmd = Console.ReadLine();
            while(cmd != "e")
            {
                try
                {
                    var first = cmd.Substring(0, 1);
                    var page = 1;
                    switch (first)
                    {
                        case "s":
                            var num = int.Parse(cmd.Replace("s", ""));
                            topicId = num.ToString();
                            break;
                        case "p":
                            try
                            {
                                page = int.Parse(cmd.Replace("p", ""));
                            }
                            catch
                            {
                                page = 1;
                            }
                            GetPage(page);
                            break;
                        case "t":
                            try
                            {
                                page = int.Parse(cmd.Replace("t", ""));
                            } catch
                            {
                                page = 1;
                            }
                            GetListTopic(page);
                            break;
                        default:
                            topicId = dictTopic[int.Parse(cmd)].Split("-")[0];
                            GetPage(1);
                            break;
                    }
                    Console.WriteLine("--------DONE--------");
                }
                catch
                {
                    Console.WriteLine("--------Input Error--------");
                }
                cmd = Console.ReadLine();
            }
        }

        public static void GetListTopic(int page)
        {
            var url = UrlHome + "&order=desc&page=" + page;
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var topics = doc.GetElementbyId("threadslist").SelectNodes("//tbody//tr//td[contains(@class, 'alt1')]//div//a[contains(@id, 'thread_title')][not(@class='vozsticky')]").ToList();
            var index = 1;
            dictTopic = new Dictionary<int, string>();
            var content = "";
            topics.ForEach(o =>
            {
                var title = o.InnerText;
                var id = o.Attributes["href"].Value.Split("=")[1];
                //Console.WriteLine(index + "   " + id + "-" + title);
                content += index + "   " + id + "-" + title + Environment.NewLine;
                dictTopic.Add(index, id + "-" + title);
                if (index == 1)
                {
                    topicId = id;
                }
                index++;
            });
            File.WriteAllText(Directory.GetCurrentDirectory() + "//topic.txt", content);
        }

        public static void GetPage(int page)
        {
            var url = UrlTopic + topicId + "&page=" + page;
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var pageInfo = "";
            try
            {
                pageInfo = doc.DocumentNode.SelectNodes("//a[contains(@class, 'smallfont')][contains(@title, 'Show results')]").First().Attributes["title"].Value;
            }
            catch
            {

            }
            var posts = doc.GetElementbyId("posts").SelectNodes("//div[contains(@id, 'edit')]//div[contains(@class, 'voz-post-message')]").ToList();
            var content = "";
            posts.ForEach(post =>
            {
                content += post.InnerText.Trim();
            });
            string replacement = topicId + Environment.NewLine + pageInfo + Environment.NewLine + Regex.Replace(content, @"\t|\r", "") + Environment.NewLine + pageInfo;
            //Console.WriteLine(replacement);
            File.WriteAllText(Directory.GetCurrentDirectory() + "//content.txt", replacement);
        }
    }
}
