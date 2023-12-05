using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Text;

namespace Pdf_Conversion_Task.Services
{
    public static class HtmlToPdfConverter
    {
        public static async Task<byte[]> ConvertFile(IFormFile file)
        {
            try
            {


                var htmlString = new StringBuilder();
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                        htmlString.AppendLine(await reader.ReadLineAsync());
                }
                var html = htmlString.ToString();

                //file.CopyTo(memoryStream);
                //string html = Convert.ToBase64String(memoryStream.ToArray());


                //var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions()
                //{
                //    Path = Path.GetTempPath()
                //});

                //await browserFetcher.DownloadAsync(BrowserFetcher.DefaultRevision);

                //var launchOptions = new LaunchOptions()
                //{
                //    ExecutablePath = browserFetcher.RevisionInfo(BrowserFetcher.DefaultRevision).ExecutablePath,
                //    Headless = true
                //};




                var chromePath = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
                chromePath += @"Program Files\Google\Chrome\Application\chrome.exe";


                //var pdfOptions = new PdfOptions();
                //pdfOptions.Format = PuppeteerSharp.Media.PaperFormat.A4;

                using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    Browser = SupportedBrowser.Chrome,
                    //ExecutablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".local-chromium", "Win64-884014", "chrome-win", "UserData").Replace(@"\", @"\\"),
                    ExecutablePath = chromePath,
                }))
                {
                    using (var page = await browser.NewPageAsync())
                    {
                        await page.SetContentAsync(html);
                        var pdfOptions = new PdfOptions { DisplayHeaderFooter = true, Landscape = true, PrintBackground = true, Format = PaperFormat.A4, MarginOptions = new MarginOptions { Top = "1cm", Bottom = "1cm", Left = "1cm", Right = "1cm" }, Scale = (decimal)1.5 };
                        await page.PdfAsync($"{file.Name}.pdf", pdfOptions);

                        return await page.PdfDataAsync();
                        //var result = new FileStreamResult(stream, "application/pdf");
                        //return blob;
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            };
        }
    }
}
