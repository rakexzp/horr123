using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic; // Added this line

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
    }
 

    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Gallery()
    {
        var webRootPath = _webHostEnvironment.WebRootPath;
        var mediaPath = Path.Combine(webRootPath, "media");
        ViewBag.WebRootPath = mediaPath; // Pass path for debugging

        var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var imageFiles = new List<string>();

        if (Directory.Exists(mediaPath))
        {
            imageFiles = Directory.GetFiles(mediaPath, "*.*", SearchOption.AllDirectories)
                .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .Select(f => f.Replace(webRootPath, "").Replace("\\", "/"))
                .ToList();
        }
            
        return View(imageFiles);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}