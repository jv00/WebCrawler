using System;
using System.Threading.Tasks;
using WebCrawler.Crawler;
using WebCrawler.Exporter;

namespace WebCrawler
{
    public class CrawlerEntryPoint
    {
        public void InitializeAndStartCrawler(Action<string> processingStatusUpdateCallback, string baseUrl)
        {
            var processor = new Processor()
            {
                BaseUrl = baseUrl,
                ProcessingStatusUpdateCallback = processingStatusUpdateCallback,
                DataExporter = new DataExporterJsonFile()
            };

            Scheduler scheduler = new Scheduler(processor)
            {
                ProcessingStatusUpdateCallback = processingStatusUpdateCallback
            };

            processingStatusUpdateCallback("Crawling started!");
            Task.Run(() => scheduler.QueueUrlForProcessing(baseUrl));
        }
    }
}
