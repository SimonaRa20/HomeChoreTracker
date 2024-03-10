using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HomeChoreTracker.Portal.Pages.HomeChores
{
    public class IndexModel : PageModel
    {
		[BindProperty]
		public int Id { get; set; }
		public void OnGet(int id)
		{
			Id = id;
		}
	}
}
