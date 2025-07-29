using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReactApp1.Server.Pages
{
    public class AboutModel : PageModel
    {
        public DateTime CurrentTime { get; private set; }

        public void OnGet()
        {
            CurrentTime = DateTime.Now;
        }
    }
}
