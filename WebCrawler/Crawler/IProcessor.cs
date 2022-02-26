using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Crawler
{
    public interface IProcessor
    {
        public Task StartProcessing(string link, Action<IEnumerable<string>> processFoundLinks, Action<string> dequeueProcessedUrl);
    }
}
