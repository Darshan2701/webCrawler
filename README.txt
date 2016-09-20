Problem Statement:

Write a web crawler

A crawler is a program that starts with a url on the web (ex: http://python.org), fetches the web-page corresponding to that url, and parses all the links on that page into a repository of links. Next, it fetches the contents of any of the url from the repository just created, parses the links from this new content into the repository and continues this process for all links in the repository until stopped or after a given number of links are fetched.



The solution to the mentioned problem is written using C# language with .NET 4.5 using Visual Studio 2012.
Following scenarios are considered while designing the solution.

1. User input's a valid url with correct depth to be crawl
2. If the above case is false, a default URL called "https://google.com" will be crawled infinitly.
3. If the depth is equal 1, the crawling is done infinitly.
4. The solution is designed to make use of async calls.
5. Multi threads are created when the program is executed.
6. Crawling of same URls are skipped, if found while crawling.
7. HtmlAgility Package is made use to parse the html body as this package is more efficient than Regex way of crawling.
8. Made use of Cancellation Tokens to stop the execution of the tasks.
9. Dependecy Injection concept has been used.
10.Interface programming concept is used.




Following are the steps to be followed.
1. Go to the folder containing webCrawler.exe folder
2.Following are the valid commmands.
	a. webCrawler.exe (default url is crawled infinitly,Press S to terminate)
	b. webCrawler.exe https://google.com or webCrawler.exe https://google.com 1 (crawls infinitly) 
	c. webCrawler.exe https://google.com 5 (crawls through first 5 URl's including https://google.com)
	
