using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace webCrawler
{
    class Program
    {
        #region Fields
        public static string enteredUrl = string.Empty;
        public static int depthOfUrl = 1;
        public static string myUrl = "https://google.com";
        public static IMessageService messageService = ConsoleLogger.Instance;

        #endregion
        
        static void Main(string[] args)
        {
            IHtmlParser parser = new HtmlParser(messageService);

            try
            {
                switch (args.Length)
                {
                    case 1:
                        enteredUrl = args[0];
                        messageService.ShowMessage("Crawling" + " " + enteredUrl + " " + "with no interuption.Please press 'S' to stop the execution");
                        parser.DepthOfUrl = depthOfUrl;
                        parser.Parser(enteredUrl);
                        break;

                    case 2:
                        enteredUrl = args[0];
                        depthOfUrl = Int32.Parse(args[1]);
                        messageService.ShowMessage("Crawling" + " " + enteredUrl + " " + "with" + " " + depthOfUrl + " " + "depth");
                        parser.DepthOfUrl = depthOfUrl;
                        parser.Parser(enteredUrl);
                        messageService.ShowMessage("Press any key to terminate as the crawling has reached its depth");
                        break;

                    default:
                        messageService.ShowMessage("You entered nothing,so crawling the default URL 'http://www.google.com' with no interuption");
                        parser.DepthOfUrl = depthOfUrl;
                        parser.Parser(myUrl);
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
