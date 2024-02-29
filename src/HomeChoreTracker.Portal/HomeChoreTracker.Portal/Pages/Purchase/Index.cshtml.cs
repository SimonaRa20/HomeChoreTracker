using HomeChoreTracker.Portal.Models.HomeChoreBase;
using HomeChoreTracker.Portal.Models.Purchase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HomeChoreTracker.Portal.Pages.Purchase
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;


        [BindProperty]
        public int Id { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
        }

        public void OnGetAsync(int id)
        {
            Id = id;           
        }

    }
}
