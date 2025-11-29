using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Net_ReactApp2.Server.Controllers
{
    public class ScenarioController : Controller
    {
        // GET: ScenarioController
        public ActionResult Index()
        {
            return Ok();
        }

        // GET: ScenarioController/Details/5
        public ActionResult Details(int id)
        {
            return Ok();
        }

        // GET: ScenarioController/Create
        public ActionResult Create()
        {
            return Ok();
        }

        // POST: ScenarioController/Create
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

        // GET: ScenarioController/Edit/5
        public ActionResult Edit(int id)
        {
            return Ok();
        }

        // POST: ScenarioController/Edit/5
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

        // GET: ScenarioController/Delete/5
        public ActionResult Delete(int id)
        {
            return Ok();
        }

        // POST: ScenarioController/Delete/5
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
