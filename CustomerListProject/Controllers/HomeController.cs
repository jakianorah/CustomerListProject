using CustomerListProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CustomerListProject.Enums;
using System.IO;

namespace CustomerListProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost()]
        public IActionResult FileUpload(IFormFile uploadFile, int sortField)
        {
            var lines = ReadFileData(uploadFile);
            var uploadResults = ParseData(lines.Result);
            var sortedResults = sortField == (int)SortFieldEnum.FullName ? SortyByFullName(uploadResults) : SortyByVehicleType(uploadResults);
            return View("~/Views/Home/Index.cshtml", sortedResults);

        }

        public async Task<List<string>> ReadFileData(IFormFile uploadFile)
        {
            var filePath = Path.GetTempFileName();
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await uploadFile.CopyToAsync(stream);
                stream.Position = 0;
                using var sr = new StreamReader(stream);
                List<string> lines = new List<string>();

                string? line;

                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
                return lines;
            }
            
        }

        public List<UploadViewModel> ParseData(List<string> lines)
        {
            var uploadResults = new List<UploadViewModel>();
            foreach (var line in lines)
            {
                uploadResults.Add(ParseCustomer(line));
            }
            return uploadResults;
        }

        public UploadViewModel ParseCustomer(string contents)
        {

            var newContent = contents.Split(',', '|');
            var fullName = newContent[0] + " " + newContent[1];
            var email = newContent[2];
            var vehicleType = newContent[3];
            var vehicleName = newContent[4];
            var vehicleLength = newContent[5];

            return new UploadViewModel { FullName = fullName, Email = email, VehicleType = vehicleType, VehicleName = vehicleName, VehicleLength = vehicleLength };
        }
        public List<UploadViewModel> SortyByFullName(List<UploadViewModel> uploadData)
        {
            return uploadData.OrderBy(s => s.FullName).ToList();
        }

        public List<UploadViewModel> SortyByVehicleType(List<UploadViewModel> uploadData)
        {
            return uploadData.OrderBy(s => s.VehicleType).ToList();
        }



    }
}