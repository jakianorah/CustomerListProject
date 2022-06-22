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
            var linesInFile = ReadFileData(uploadFile);
            var uploadedDataResult = ParseData(linesInFile.Result);
            var sortedResults = sortField == (int)SortFieldEnum.FullName ? SortyByFullName(uploadedDataResult) : SortyByVehicleType(uploadedDataResult);
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
                    stream.Position = 0; //set stream to beginning
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
                    uploadResults.Add(GetCustomerData(line));
                }
                return uploadResults;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public UploadViewModel GetCustomerData(string customerString)
        {
            try
            {
                var customerFields = customerString.Split(',', '|');
                var fullName = customerFields[(int)CustomerDataEnum.FirstName] + " " + customerFields[(int)CustomerDataEnum.LastName];
                var email = customerFields[(int)CustomerDataEnum.Email];
                var vehicleType = customerFields[(int)CustomerDataEnum.VehicleType];
                var vehicleName = customerFields[(int)CustomerDataEnum.VehicleName];
                var vehicleLength = customerFields[(int)CustomerDataEnum.VehicleLength];

                return new UploadViewModel { 

                    FullName = fullName, 
                    Email = email, 
                    VehicleType = vehicleType, 
                    VehicleName = vehicleName, 
                    VehicleLength = vehicleLength 
                };
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