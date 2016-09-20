using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace webCrawler
{
    class HtmlParser : IHtmlParser
    {
        public int DepthOfUrl { get; set; }
        IMessageService messageService;

        #region constructor
        public HtmlParser(IMessageService messageService)
        {
            DepthOfUrl = 1;
            this.messageService = messageService;
        }
        #endregion

        #region Fields
        public int depthOfUrlCount = 0;
        public List<string> crawledUrls = new List<string>();
        public ConcurrentQueue<string> enteredUrls = new ConcurrentQueue<string>();
        public ConcurrentQueue<Tuple<string, HtmlDocument>> parsedBody = new ConcurrentQueue<Tuple<string, HtmlDocument>>();
        public CancellationTokenSource cancellationTokenSource;
        public List<string> crawledUrlsList = new List<string>();
        #endregion

        public void Parser(string enteredUrl)
        {
            crawledUrlsList.Add(enteredUrl);

            cancellationTokenSource = new CancellationTokenSource();

            enteredUrls.Enqueue(enteredUrl);                                                //adding the input urls to the inputUrl's Concurrent queue
            Task task = Task.Factory.StartNew(FetchLinks, cancellationTokenSource.Token);   //Start two tasks parallely.
            Task parser = Task.Factory.StartNew(ParseBody, cancellationTokenSource.Token);

            if (DepthOfUrl != 1)
            {
                Task.WaitAll(task, parser);                                                 //Wait for the complete execution for given depth to crawl
                messageService.ShowMessage("The depth has reached. Programs is terminated.Press any key to exit");
            }
            else
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.S)
                {
                    messageService.ShowMessage("Please wait........");
                    cancellationTokenSource.Cancel(true);
                }
                messageService.ShowMessage("I am in Main. Press any key to exit");
            }
            Console.ReadKey();
        }

        /// <summary>
        /// Fetches the links from the response body of the URL's
        /// </summary>
        void FetchLinks()
        {
            while (true)
            {
                if (cancellationTokenSource.IsCancellationRequested || depthOfUrlCount >= DepthOfUrl)
                    return;

                // Check if the first concurrent string queue has any item
                if (!enteredUrls.IsEmpty)
                // If has any item, deque and call FetchResponse();
                {
                    string crawledUrl = string.Empty;

                    if (enteredUrls.TryDequeue(out crawledUrl) && !crawledUrl.StartsWith("/") && !crawledUrl.StartsWith("#"))
                    {
                        messageService.ShowMessage("Crawling through: " + crawledUrl);
                        if (DepthOfUrl != 1)
                            depthOfUrlCount++;
                        var task = FetchResponse(crawledUrl);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the status Code is OK and awaits till the Response Body is fetched and parsed.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        async Task<HttpResponseMessage> FetchResponse(string url)
        {
            var client = new HttpClient();
            HttpResponseMessage result = await client.GetAsync(url, cancellationTokenSource.Token);  //Send a GET request to the specified Uri with async call

            if (result.IsSuccessStatusCode)
            {
                HtmlAgilityPack.HtmlDocument doc = await GetBody(result);       //Gets the body from the given URL
                // Create tuple  with url and doc now
                // add to the second concurrent tuple queue 
                parsedBody.Enqueue(Tuple.Create<string, HtmlDocument>(url, doc));

            }
            return result;
        }

        /// <summary>
        /// Gets the Response Body from the given Url
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        async Task<HtmlAgilityPack.HtmlDocument> GetBody(HttpResponseMessage response)
        {
            string result = await response.Content.ReadAsStringAsync();    //Write the HTTP content to a string as an asynchronous operation.
            if (!response.IsSuccessStatusCode)
                throw new FileNotFoundException("Can't find the document");

            var bodyDocument = new HtmlAgilityPack.HtmlDocument();
            bodyDocument.LoadHtml(result);
            return bodyDocument;
        }

        /// <summary>
        /// Gets the href attribute values from the response body and add to queue.
        /// </summary>
        void ParseBody()
        {
            while (true)
            {
                if (cancellationTokenSource.IsCancellationRequested || depthOfUrlCount >= DepthOfUrl)
                    return;

                try
                {
                    // Read the second concurrent tuple queue
                    Tuple<string, HtmlDocument> a = null;

                    // If has items, parse the body, find links and add to the first string queue
                    if (parsedBody.TryDequeue(out a))
                    {
                        HtmlNodeCollection aTags = a.Item2.DocumentNode.SelectNodes("//a[@href]"); // Gets all the anchor tags contaning href.

                        if (aTags == null)
                            continue;

                        IEnumerable<string> hrefValuesToBeCrawled = aTags.Select(t => t.GetAttributeValue("href", null));

                        foreach (var item in hrefValuesToBeCrawled)
                        {
                            if (!crawledUrlsList.Contains(item))
                            {
                                crawledUrlsList.Add(item);
                                enteredUrls.Enqueue(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    messageService.ShowMessage("Exception occured: " + ex.Message);
                    throw;
                }
            }
        }

    }
}

