using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net_ReactApp2.Server.ViewModels;
using UEFASwissFormatSelector.Models;
using UEFASwissFormatSelector.Services;

namespace Net_ReactApp2.Server.Controllers
{
    public class SimulationController : Controller
    {
        private readonly IRepository repository;

        public SimulationController(IRepository repository)
        {
            this.repository = repository;
        }
        public IActionResult GetAll()
        {
            return Ok(repository.ScenarioInstances.ToList());
        }

        public IActionResult Get(Guid id)
        {
            return Ok(repository.ScenarioInstances.FirstOrDefault(c => c.Id == id));
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateSimulationVM vm)
        {
            if (ModelState.IsValid)
            {
                var scenario = repository.Scenarios.FirstOrDefault(s => s.Id == vm.ScenarioId);
                if (scenario == null)
                    return BadRequest("Scenario not found");
                int expectedClubCount = scenario.NumberOfPot * scenario.NumberOfTeamsPerPot;
                if (vm.Clubs.Count() != expectedClubCount)
                    return BadRequest("Club count is off");
                var scenarioInstace = new ScenarioInstance(scenario);
                var clubsInSI = new List<ClubInScenarioInstance>();
                foreach (RankableClub rankedCLub in vm.Clubs)
                {
                    var club = repository.Clubs.FirstOrDefault(c=> c.Id == rankedCLub.Id);
                    if(club == null || clubsInSI.Any(cisi => cisi.ClubId == rankedCLub.Id)) //to detect invalid club id and dublication of clubs
                        return BadRequest($"Club {rankedCLub.Id} with name {rankedCLub.Name} is off");
                    clubsInSI.Add(new ClubInScenarioInstance(rankedCLub.Id, scenarioInstace.Id));
                }
                scenarioInstace.ClubsInScenarioInstance = clubsInSI;
                repository.ScenarioInstances.Add(scenarioInstace);
                return Ok("Simulation added successfully");
            }
            return BadRequest("Model not valid");
        }

        [HttpPost]
        public ActionResult Edit([FromBody] Country country)
        {
            if (ModelState.IsValid)
            {
                var repoCountry = repository.Countries.Find(country.Id);
                if (repoCountry == null)
                    return NotFound();
                repository.Countries.Update(country);
                return Ok();
            }
            return BadRequest("Model not valid");
        }
    }
}
