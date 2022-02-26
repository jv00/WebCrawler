using System;
using System.Collections.Generic;
using System.Text;

namespace WebCrawler.Models
{
    public class ProcessedPage
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> ImageLinks { get; set; }
        public IEnumerable<string> Stylesheets { get; set; }
        public IEnumerable<string> Scripts { get; set; }
        public IEnumerable<Link> Links { get; set; }
        public IEnumerable<string> Meta { get; set; }
        public ICollection<Form> Forms { get; set; }
    }
}
