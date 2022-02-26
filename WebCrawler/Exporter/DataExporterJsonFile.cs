using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebCrawler.Models;

namespace WebCrawler.Exporter
{
    public class DataExporterJsonFile : IDataExporter
    {
        private string _resultFolderName;

        public async Task ExportProcessedPage(ProcessedPage page)
        {
            CreateResultFolderIfDoesNotExist();

            var fileName = GetFileName(page.Url);
            var content = JsonSerializer.Serialize(page);

            var newFilePath = $"{_resultFolderName}/{fileName}.json";

            CreateFile(newFilePath);

            await File.WriteAllTextAsync(newFilePath, content);
        }

        private void CreateResultFolderIfDoesNotExist()
        {
            if (string.IsNullOrEmpty(_resultFolderName))
            {
                _resultFolderName = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            }
        }

        private string GetFileName(string pageUrl)
        {
            using var sha = new SHA256Managed();
            byte[] textData = Encoding.UTF8.GetBytes(pageUrl);
            byte[] hash = sha.ComputeHash(textData);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        private void CreateFile(string path)
        {
            (new FileInfo(path)).Directory.Create();
            using (File.Create(path)) ;
        }
    }
}
