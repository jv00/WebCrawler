using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Crawler
{
    public class Scheduler
    {

        public Action<string> ProcessingStatusUpdateCallback { get; set; }

        private ICollection<string> _urlsForProcessing = new List<string>();
        private ICollection<string> _urlsBeingProcessed = new List<string>();
        private ICollection<string> _urlsProcessed = new List<string>();
        private IProcessor _processor;

        private int _maxNumberOfProcessing = 3;
        private readonly Random _random = new Random();

        public Scheduler(IProcessor processor)
        {
            _processor = processor;
        }

        public void ProcessFoundLinks(IEnumerable<string> links)
        {
            var linksForProcessing = links.Where(link => 
                                                   !_urlsProcessed.Contains(link) 
                                                && !_urlsBeingProcessed.Contains(link)
                                                && !_urlsForProcessing.Contains(link));

            foreach (var link in linksForProcessing)
                QueueUrlForProcessing(link);
        }

        public void QueueUrlForProcessing(string url)
        {
            if (_urlsBeingProcessed.Count < _maxNumberOfProcessing)
            {
                _urlsBeingProcessed.Add(url);
                Task.Run(() => StartProcessingUrl(url));
            }          
            else
                _urlsForProcessing.Add(url);
        }

        private void DequeueProcessedUrl(string url) 
        {
            _urlsBeingProcessed.Remove(url);
            _urlsProcessed.Add(url);

            if (_urlsForProcessing.Count > 0)
            {
                var nextUrl = _urlsForProcessing.First();
                _urlsForProcessing.Remove(nextUrl);
                _urlsBeingProcessed.Add(nextUrl);
                Task.Run(() => StartProcessingUrl(nextUrl));
            }
            else if(_urlsBeingProcessed.Count == 0)
            {
                ProcessingStatusUpdateCallback($"Crawling finished, total pages crawled: {_urlsProcessed.Count}");
            }
        }

        private async Task StartProcessingUrl(string url)
        {
            int sleepTime = _random.Next(1000, 4000);
            ProcessingStatusUpdateCallback($"Sleeping for {sleepTime} miliseconds...");
            await Task.Delay(sleepTime);

           await _processor.StartProcessing(url, ProcessFoundLinks, DequeueProcessedUrl);
        }
    }
}
