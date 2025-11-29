using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Net_ReactApp2.Server.Controllers
{
    public class ClubController : Controller
    {
        // GET: ClubController
        public IActionResult Index()
        {
            return Ok();
        }

        // GET: ClubController/Details/5
        public ActionResult Details(int id)
        {
            return Ok();
        }

        // GET: ClubController/Create
        public ActionResult Create()
        {
            return Ok();
        }

        // POST: ClubController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Ok();
            }
        }

        // GET: ClubController/Edit/5
        public ActionResult Edit(int id)
        {
            return Ok();
        }

        // POST: ClubController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Ok();
            }
        }

        // GET: ClubController/Delete/5
        public ActionResult Delete(int id)
        {
            return Ok();
        }

        // POST: ClubController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Ok();
            }
        }
    }
}
