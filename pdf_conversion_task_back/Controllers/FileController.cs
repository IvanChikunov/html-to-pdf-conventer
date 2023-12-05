using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdf_Conversion_Task.Services;
using System.Net;

namespace Pdf_Conversion_Task.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class FileController : ControllerBase
    {
        [HttpPost("ConvertToPdf")]
        public async Task<IActionResult> ConvertToPdf(IFormFile file)
        {
            var result = await HtmlToPdfConverter.ConvertFile(file);
            return File(result, "application/pdf", file.FileName + ".pdf");
            //return File(result, "application/pdf");
            //return result;
        }
    }
}
