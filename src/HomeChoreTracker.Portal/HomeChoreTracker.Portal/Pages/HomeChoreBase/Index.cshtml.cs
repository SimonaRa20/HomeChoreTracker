using HomeChoreTracker.Portal.Constants;
using HomeChoreTracker.Portal.Models.HomeChoreBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using DayOfWeek = HomeChoreTracker.Portal.Constants.DayOfWeek;

namespace HomeChoreTracker.Portal.Pages.HomeChoreBase
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public List<HomeChoreBaseResponse> HomeChoreBases { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;

        [BindProperty]
        public HomeChoreBaseEditRequest EditHomeChore { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _config = configuration;
            EditHomeChore = new HomeChoreBaseEditRequest();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                int pageSize = PageSize;
                int skip = (CurrentPage - 1) * pageSize;
                var apiUrl = $"{_config["ApiUrl"]}/HomeChoreBase/skip{skip}/take{pageSize}";
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    HomeChoreBases = await response.Content.ReadFromJsonAsync<List<HomeChoreBaseResponse>>();
                    var apiUrlAll = _config["ApiUrl"] + "/HomeChoreBase";
                    var totalCountResponse = await httpClient.GetAsync(apiUrlAll);
                    if (totalCountResponse.IsSuccessStatusCode)
                    {
                        List<HomeChoreBaseResponse> list = await totalCountResponse.Content.ReadFromJsonAsync<List<HomeChoreBaseResponse>>();
                        Count = list.Count;
                        TotalPages = (int)Math.Ceiling((double)Count / pageSize);
                    }

                    return Page();
                }
                else
                {
                    return BadRequest($"Failed to retrieve data: {response.ReasonPhrase}");
                }
            }
        }

        public async Task<IActionResult> OnGetDetailAsync(int id)
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = $"{_config["ApiUrl"]}/HomeChoreBase/{id}";
                var response = await httpClient.GetAsync(apiUrl);

                var choreDetails = new HomeChoreBaseResponse();
                if (response.IsSuccessStatusCode)
                {
                    choreDetails = await response.Content.ReadFromJsonAsync<HomeChoreBaseResponse>();

                    if (choreDetails != null)
                    {
                        var daysOfWeekList = choreDetails.DaysOfWeek;
                        string choreTypeText = ((HomeChoreType)choreDetails.ChoreType).ToString();
                        string choreTimeText = ((TimeLong)choreDetails.Time).ToString();
                        string choreLevelTypeText = ((LevelType)choreDetails.LevelType).ToString();
                        string choreUnitText = ((RepeatUnit)choreDetails.Unit).ToString();
                        string choreMonthlyRepeatTypeText = ((MonthlyRepeatType)choreDetails.MonthlyRepeatType).ToString();
                        string descriptionText = choreDetails.Description ?? "-";
                        string repeatingDataText = GetRepeatingDataText(choreDetails);

                        return new JsonResult(new
                        {
                            id = choreDetails.Id,
                            name = choreDetails.Name,
                            choreType = choreTypeText,
                            description = descriptionText,
                            points = choreDetails.Points,
                            time = choreTimeText,
                            levelType = choreLevelTypeText,
                            interval = choreDetails.Interval,
                            unit = choreUnitText,
                            dayOfMonth = choreDetails.DayOfMonth,
                            monthlyRepeatType = choreMonthlyRepeatTypeText,
                            repeatingData = repeatingDataText,
                            daysOfWeek = daysOfWeekList
                        });
                    }
                    else
                    {
                        return NotFound("Chore details not found.");
                    }
                }
                else
                {
                    return BadRequest($"Failed to retrieve data: {response.ReasonPhrase}");
                }
            }
        }


        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            ClearFieldErrors(key => key == "EditHomeChore.Name");
            ClearFieldErrors(key => key == "Name");
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            try
            {
                var token = User.FindFirstValue("Token");
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var apiUrl = $"{_config["ApiUrl"]}/HomeChoreBase/{id}";
                    var val = Request.Form["editDaysOfWeek"];
                    EditHomeChore.Name = Request.Form["editName"];
                    EditHomeChore.ChoreType = (int)Enum.Parse<HomeChoreType>(Request.Form["editChoreType"]);
                    EditHomeChore.Description = Request.Form["editDescription"];
                    EditHomeChore.Points = int.Parse(Request.Form["editPoints"]);
                    EditHomeChore.LevelType = (int)Enum.Parse<LevelType>(Request.Form["editLevelType"]);
                    EditHomeChore.Time = (int)Enum.Parse<TimeLong>(Request.Form["editTimeLong"]);
                    EditHomeChore.Interval = int.Parse(Request.Form["editInterval"]);
                    EditHomeChore.Unit = (int)Enum.Parse<RepeatUnit>(Request.Form["editRepeatUnit"]);
                    EditHomeChore.DaysOfWeek = Request.Form["editDaysOfWeek"].SelectMany(s => s.Split(',')).Select(int.Parse).ToList();
                    EditHomeChore.DayOfMonth = int.Parse(Request.Form["editDayOfMonth"]);
                    EditHomeChore.MonthlyRepeatType = (int)Enum.Parse<MonthlyRepeatType>(Request.Form["editMonthlyRepeatType"]);
                    var response = await httpClient.PutAsJsonAsync(apiUrl, EditHomeChore);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToPage();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Failed to update chore: {response.StatusCode}");
                        return Page();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                await OnGetAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var token = User.FindFirstValue("Token");
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = $"{_config["ApiUrl"]}/HomeChoreBase/{id}";

                var response = await httpClient.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Failed to delete chore: {response.StatusCode}");
                    return Page();
                }
            }
        }

        private void ClearFieldErrors(Func<string, bool> predicate)
        {
            foreach (var field in ModelState)
            {
                if (field.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                {
                    if (predicate(field.Key))
                    {
                        field.Value.ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                    }
                }
            }
        }

        private string GetRepeatingDataText(HomeChoreBaseResponse choreDetails)
        {
            if (choreDetails.Unit == RepeatUnit.Day)
            {
                return "Every day";
            }
            else if (choreDetails.Unit == RepeatUnit.Week)
            {
                if (choreDetails.DaysOfWeek != null && choreDetails.DaysOfWeek.Any())
                {
                    return "Every week on " + string.Join(", ", choreDetails.DaysOfWeek.Select(d => ((DayOfWeek)d).ToString()));
                }
                else
                {
                    return "Every week";
                }
            }
            else if (choreDetails.Unit == RepeatUnit.Month)
            {
                if (choreDetails.DayOfMonth.HasValue)
                {
                    return "Every month on day " + choreDetails.DayOfMonth;
                }
                else if (choreDetails.MonthlyRepeatType.HasValue)
                {
                    return "Every month on the " + choreDetails.MonthlyRepeatType + " " + string.Join(", ", choreDetails.DaysOfWeek.Select(d => ((DayOfWeek)d).ToString()));
                }
                else
                {
                    return "";
                }
            }
            else if (choreDetails.Unit == RepeatUnit.Year)
            {
                if (choreDetails.DayOfMonth.HasValue)
                {
                    return "Every year on day " + choreDetails.DayOfMonth;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
    }
}