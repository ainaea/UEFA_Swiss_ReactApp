using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Net_ReactApp2.Server.Controllers
{
    public class SimulationController : Controller
    {
        // GET: SimulationController
        public ActionResult Index()
        {
            return Ok();
        }

        // GET: SimulationController/Details/5
        public ActionResult Details(int id)
        {
            return Ok();
        }

        // GET: SimulationController/Create
        public ActionResult Create()
        {
            return Ok();
        }

        // POST: SimulationController/Create
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

        // GET: SimulationController/Edit/5
        public ActionResult Edit(int id)
        {
            return Ok();
        }

        // POST: SimulationController/Edit/5
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

        // GET: SimulationController/Delete/5
        public ActionResult Delete(int id)
        {
            return Ok();
        }

        // POST: SimulationController/Delete/5
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
