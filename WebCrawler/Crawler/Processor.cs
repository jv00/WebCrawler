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

            processedPage = GetMetaTags(doc, processedPage);

            processedPage = GetForms(doc, processedPage);

            var links = GetAllPageLinks(doc);

            processedPage.Links = links;

            processFoundLinks(links.Where(l => l.Href.StartsWith(BaseUrl)).Select(l => l.Href));

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

        private IEnumerable<Link> GetAllPageLinks(HtmlDocument document)
        {
            return document.DocumentNode.Descendants("a")
                                    .Select(l => new Link() { Href = l.GetAttributeValue("href", null), Title = l.InnerText.Trim() })
                                    .Where(l => !string.IsNullOrEmpty(l.Href));
        }

        private ProcessedPage GetMetaTags(HtmlDocument document, ProcessedPage processedPage)
        {
            processedPage.Meta = document.DocumentNode.SelectNodes("//meta")
                            .Select(e => e.GetAttributeValue("content", null))
                            .Where(m => !string.IsNullOrEmpty(m));

            return processedPage;
        }

        private ProcessedPage GetForms(HtmlDocument document, ProcessedPage processedPage)
        {
            ICollection<Form> forms = new List<Form>();
            var formNodes = document.DocumentNode.SelectNodes("//form");

            if (formNodes == null)
                return processedPage;

            foreach(var formNode in formNodes)
            {
                var form = new Form()
                {
                    Id = formNode.GetAttributeValue("id", null),
                    FormElements = new List<FormElement>()
                };

                var formElementNodes = formNode.SelectNodes("//input");

                foreach(var formElementNode in formElementNodes)
                {
                    form.FormElements.Add(new FormElement()
                    {
                        Name = formElementNode.GetAttributeValue("name", null),
                        Placeholder = formElementNode.GetAttributeValue("placeholder", null),
                        Type = formElementNode.GetAttributeValue("type", null)
                    });
                }

                forms.Add(form);
            }

            processedPage.Forms = forms;

            return processedPage;
        }
    }
}
