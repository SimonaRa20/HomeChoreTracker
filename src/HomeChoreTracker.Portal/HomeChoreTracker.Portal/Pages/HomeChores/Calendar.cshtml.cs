using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using HomeChoreTracker.Portal.Constants;
using HomeChoreTracker.Portal.Models.HomeChore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using DayOfWeek = HomeChoreTracker.Portal.Constants.DayOfWeek;

namespace HomeChoreTracker.Portal.Pages.HomeChores
{
    public class CalendarModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public List<HomeChoreResponse> HomeChoreResponse { get; set; }

        public CalendarModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
        }

        [BindProperty]
        public int Id { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Id = id;
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = $"{_config["ApiUrl"]}/HomeChore/{id}";

                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    HomeChoreResponse = await response.Content.ReadFromJsonAsync<List<HomeChoreResponse>>();

                    var events = new List<object>();

                    foreach (var chore in HomeChoreResponse)
                    {
                        //var occurrences = CalculateOccurrences(chore);

                        //events.AddRange(occurrences.Select(occurrence => new
                        //{
                        //    id = chore.Id,
                        //    title = chore.Name,
                        //    start = occurrence.ToString("yyyy-MM-dd"),
                        //    end = occurrence.ToString("yyyy-MM-dd")
                        //}));
                    }

                    return Page();
                }
                else
                {
                    return BadRequest($"Failed to retrieve data: {response.ReasonPhrase}");
                }
            }
        }

        //private List<DateTime> CalculateOccurrences(HomeChoreResponse chore)
        //{
        //    var occurrences = new List<DateTime>();

        //    if (chore.Unit == RepeatUnit.Day)
        //    {
        //        // For daily repetition, add occurrences based on the interval
        //        for (var i = 0; i < chore.Interval; i++)
        //        {
        //            var occurrence = chore.StartDate.AddDays(i);
        //            if (chore.EndDate == null || occurrence <= chore.EndDate)
        //            {
        //                occurrences.Add(occurrence);
        //            }
        //        }
        //    }
        //    else if (chore.Unit == RepeatUnit.Week && chore.DaysOfWeek != null)
        //    {
        //        // For weekly repetition, add occurrences based on the interval and selected days
        //        for (var i = 0; i < chore.Interval * 7; i++)
        //        {
        //            var occurrence = chore.StartDate.AddDays(i);
        //            if (chore.EndDate != null && occurrence > chore.EndDate)
        //            {
        //                break;
        //            }

        //            if (chore.DaysOfWeek.Contains((DayOfWeek)occurrence.DayOfWeek))
        //            {
        //                occurrences.Add(occurrence);
        //            }
        //        }
        //    }
        //    // Add support for other repeat units (Month, Year) as needed

        //    return occurrences;
        //}
    }
}
