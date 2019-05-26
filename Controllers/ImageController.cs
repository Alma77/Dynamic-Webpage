using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HW_06.Controllers
{
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public ImageController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: /<controller>/
        [HttpGet("{ImageName}")]
        public string Get(string imageName) 
        {
            string path = _hostingEnvironment.WebRootPath + "/images/" + imageName;
            byte[] b = System.IO.File.ReadAllBytes(path);
            string base64Encoded = Convert.ToBase64String(b);
            return "data:image/png;base64," + base64Encoded;
        }
    }
}
