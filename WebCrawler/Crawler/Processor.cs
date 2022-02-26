using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebCrawler.Models;
using System.Text.Json;
using WebCrawler.Exporter;
using System.Threading.Tasks;

namespace WebCrawler.Crawler
{
    public class Processor : IProcessor
    {
        public string BaseUrl { get; set; }
        public Action<string> ProcessingStatusUpdateCallback { get; set; }
        public IDataExporter DataExporter { get; set; }

        public async Task StartProcessing(string link, Action<IEnumerable<string>> processFoundLinks, Action<string> dequeueProcessedUrl)
        {
             HtmlWeb web = new HtmlWeb();
             var doc = await web.LoadFromWebAsync(link);

            var processedPage = new ProcessedPage();

            processedPage.Url = link;

            processedPage = GetPageTitle(doc, processedPage);

            processedPage = GetPageDescription(doc, processedPage);

            processedPage = GetPageImageLinks(doc, processedPage);

            processedPage = GetPageStylesheetLinks(doc, processedPage);

            processedPage = GetPageScriptLinks(doc, processedPage);

           
            var links = GetAllPageLinks(doc);

            processFoundLinks(links);
      
            await DataExporter.ExportProcessedPage(processedPage);

            ProcessingStatusUpdateCallback($"Result exported for URL: {processedPage.Url}");

            dequeueProcessedUrl(link);
        }

        private ProcessedPage GetPageTitle(HtmlDocument document, ProcessedPage processedPage)
        {
            processedPage.Title = document.DocumentNode.SelectSingleNode("//head/title")?.InnerText;

            return processedPage;
        }

        private ProcessedPage GetPageDescription(HtmlDocument document, ProcessedPage processedPage)
        {
            processedPage.Description = document.DocumentNode.Descendants("meta")
                                    .Where(e => e.GetAttributeValue("name", null) == "description")
                                    .Select(e => e.GetAttributeValue("content", null))
                                    .Where(s => !string.IsNullOrEmpty(s))
                                    .FirstOrDefault();

            return processedPage;
        }

        private ProcessedPage GetPageImageLinks(HtmlDocument document, ProcessedPage processedPage)
        {
            processedPage.ImageLinks = document.DocumentNode.Descendants("img")
                              .Select(e => e.GetAttributeValue("src", null))
                              .Where(s => !string.IsNullOrEmpty(s));

            return processedPage;
        }

        private ProcessedPage GetPageStylesheetLinks(HtmlDocument document, ProcessedPage processedPage)
        {
            processedPage.Stylesheets = document.DocumentNode.Descendants("link")
                                .Where(e => e.GetAttributeValue("rel", null) == "stylesheet")
                                .Select(e => e.GetAttributeValue("href", null))
                                .Where(s => !string.IsNullOrEmpty(s));


            return processedPage;
        }

        private ProcessedPage GetPageScriptLinks(HtmlDocument document, ProcessedPage processedPage)
        {
            processedPage.Scripts = document.DocumentNode.Descendants("script")
                              .Select(e => e.GetAttributeValue("src", null))
                              .Where(s => !string.IsNullOrEmpty(s));

            return processedPage;
        }

        private IEnumerable<string> GetAllPageLinks(HtmlDocument document)
        {
            return document.DocumentNode.Descendants("a")
                                  .Select(a => a.GetAttributeValue("href", null))
                                  .Where(u => !String.IsNullOrEmpty(u) && u.StartsWith(BaseUrl));
        }
    }
}
