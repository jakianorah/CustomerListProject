using CustomerListProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CustomerListProject.Enums;

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
            try
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
            catch (Exception)
            {
                throw;
            }
            
            
        }

        public List<UploadViewModel> ParseData(List<string> lines)
        {
            try
            {
                var uploadResults = new List<UploadViewModel>();
                foreach (var line in lines)
                {
                    uploadResults.Add(ParseCustomer(line));
                }
                return uploadResults;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public UploadViewModel ParseCustomer(string contents)
        {
            try
            {
                var newContent = contents.Split(',', '|');
                var fullName = newContent[(int)CustomerDataEnum.FirstName] + " " + newContent[(int)CustomerDataEnum.LastName];
                var email = newContent[(int)CustomerDataEnum.Email];
                var vehicleType = newContent[(int)CustomerDataEnum.VehicleType];
                var vehicleName = newContent[(int)CustomerDataEnum.VehicleName];
                var vehicleLength = newContent[(int)CustomerDataEnum.VehicleLength];

                return new UploadViewModel { FullName = fullName, Email = email, VehicleType = vehicleType, VehicleName = vehicleName, VehicleLength = vehicleLength };
            }
            catch (Exception)
            {
                throw;
            }
           
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