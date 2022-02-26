using System;
using System.Collections.Generic;
using System.Text;

namespace WebCrawler.Models
{
    public class Form
    {
        public string Id { get; set; }
        public ICollection<FormElement> FormElements { get; set; }
    }

    public class FormElement
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Placeholder { get; set; }
    }
}
