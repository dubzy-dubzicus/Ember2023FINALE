using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace Ember2023.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBase = "http://localhost:2059";

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new System.Uri(_apiBase);
        }

        public IActionResult Index() => View();

        [HttpGet("News")]
        public IActionResult News() => View("News");

        [HttpGet("Search")]
        public IActionResult Search() => View("Search");

        [HttpGet("Profile/{username}")]
        public IActionResult Profile(string username) => View("Profile", (object)username);

        [HttpGet("Room/{id}")]
        public IActionResult Room(string id) => View("Room", (object)id);

        [HttpGet("Download")]
        public IActionResult Download() => View("Download");
    }
}
