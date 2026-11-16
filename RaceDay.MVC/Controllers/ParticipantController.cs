using Microsoft.AspNetCore.Mvc;

namespace RaceDay.MVC.Controllers
{
    public class ParticipantController : Controller
    {
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("JWToken") == null)
            {
                return RedirectToAction("Login", "Account");
            }


            // Only organisers may access this page
            if (HttpContext.Session.GetString("Role") != "Participant")
            {
                return RedirectToAction("Index", "Events");
            }
            
            return View();
        }
    }
}
