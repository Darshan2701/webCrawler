using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webCrawler
{
    interface IHtmlParser
    {
        void Parser(string inputUrl);
        int DepthOfUrl { get; set; }
    }
}
