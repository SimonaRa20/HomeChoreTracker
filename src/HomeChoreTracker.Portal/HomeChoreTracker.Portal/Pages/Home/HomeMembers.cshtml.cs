using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomeChoreTracker.Portal.Pages.Home
{
    public class HomeMembersModel : PageModel
    {
        [BindProperty]
        public int Id { get; set; }
        public void OnGet(int id)
        {
            Id = id;
            // You can use the id parameter as needed
        }
    }
}
