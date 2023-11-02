using HCIHelp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using HCIHelp.wwwroot;

namespace HCIHelp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private IWebHostEnvironment _hostingEnvironment;

        public HomeController(IWebHostEnvironment environment)
        {
            _hostingEnvironment = environment;
        }

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(ComplaintModel model)
        {
            //if (ModelState.IsValid)
            //{
            //    // Handle the form submission...
            //    return RedirectToAction("Success");
            //}

            //// If we got this far, something failed; redisplay the form.
            //return View(model);
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }


        // GET: Home  
        public ActionResult UploadFiles()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UploadData()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UploadData(string name, string email, string complaint, IFormFile[] files)
        {

            string CommentLabel = "";
            string CatchyTweetLine = "";
            bool IsEligibleImage = false;
            string imgBase64String = "";
            //Ensure model state is valid  
            if (ModelState.IsValid)
            {

                IFormFile file = files?.FirstOrDefault();
                if (file != null)
                {
                    var InputFileName = Path.GetFileName(file.FileName);
                    var ServerSavePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", InputFileName);
                    //Save file to server folder  
                    using (Stream fileStream = new FileStream(ServerSavePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    imgBase64String = GetBase64StringForImage(ServerSavePath);
                }

                string GSettingsPath = Path.Combine(_hostingEnvironment.WebRootPath, "GoogleSetting.json");
                string bearerToken = await WebHelper.GetAccessToken(GSettingsPath);

                string promptContent = "Classify the text as one of the following categories:- Eathquake- Tornado - Flood - Wildifire -Drought. If the text doesn't fit any categories, classify it as the following: - customer service Text: " + complaint;

                CommentLabel = await WebHelper.GetCommentLabel(bearerToken, promptContent);
                string hashTags = "";
                CommentLabel = CommentLabel.Trim().ToString().ToLower();


                if (CommentLabel == "earthquake" || CommentLabel == "tornado" || CommentLabel == "flood" || CommentLabel == "wildifire" || CommentLabel == "drought")
                {
                    if (!string.IsNullOrEmpty(imgBase64String))
                    {
                        IsEligibleImage = WebHelper.GetImageLabels(imgBase64String, bearerToken, out hashTags);
                    }

                    CatchyTweetLine = await WebHelper.GetHashTags(bearerToken, complaint);
                
                    if(IsEligibleImage)
                        CatchyTweetLine += Environment.NewLine + hashTags;
                }
                else
                {
                    CommentLabel = "customer service";
                }


                ThankYouModel tm = new ThankYouModel();
                tm.complaintClassification = CommentLabel;
                tm.embededTweetString = CatchyTweetLine;
                tm.isEligibleImage = IsEligibleImage;
                tm.imageSource = "~/uploads/" + file?.FileName;
                ViewBag.isEligibleImage = tm.isEligibleImage;
                return View("ThankYou", tm);
            }
            else
            {
                return View();
            }
        }
        public static string GetBase64StringForImage(string imgPath)
        {
            byte[] imageBytes = System.IO.File.ReadAllBytes(imgPath);
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}