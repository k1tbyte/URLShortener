using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReactApp1.Server.Pages
{
    public class ContactModel : PageModel
    {
        public DateTime VisitTime { get; private set; }

        public void OnGet()
        {
            VisitTime = DateTime.Now;
        }
    }
}
