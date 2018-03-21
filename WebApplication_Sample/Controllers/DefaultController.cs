using System.Web.Mvc;

namespace WebApplication_Sample.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }
    }
}