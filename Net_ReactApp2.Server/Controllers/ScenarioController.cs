using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UEFASwissFormatSelector.Models;
using UEFASwissFormatSelector.Services;

namespace Net_ReactApp2.Server.Controllers
{
    public class ScenarioController : Controller
    {
        private readonly IRepository repository;

        public ScenarioController(IRepository repository)
        {
            this.repository = repository;
        }
        public IActionResult GetAll()
        {
            return Ok(repository.Scenarios.OrderBy(c => c.Name).ToList());
        }

        public IActionResult Get(Guid id)
        {
            return Ok(repository.Scenarios.FirstOrDefault(c => c.Id == id));
        }

        [HttpPost]
        public IActionResult Create([FromBody] Scenario scenario)
        {
            if (ModelState.IsValid)
            {
                scenario.Id = Guid.NewGuid();
                repository.Scenarios.Add(scenario);
                return Ok("Scenario added successfully");
            }
            return BadRequest("Model not valid");
        }

        [HttpPost]
        public ActionResult Edit([FromBody] Scenario scenario)
        {
            if (ModelState.IsValid)
            {
                var repoScenario = repository.Scenarios.Find(scenario.Id);
                if (repoScenario == null)
                    return NotFound();
                repository.Scenarios.Update(scenario);
                return Ok();
            }
            return BadRequest("Model not valid");
        }
    }
}
