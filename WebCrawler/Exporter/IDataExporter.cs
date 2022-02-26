using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Models;

namespace WebCrawler.Exporter
{
    public interface IDataExporter
    {
        public Task ExportProcessedPage(ProcessedPage page);
    }
}
