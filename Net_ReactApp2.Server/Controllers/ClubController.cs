using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UEFASwissFormatSelector.Models;
using UEFASwissFormatSelector.Services;

namespace Net_ReactApp2.Server.Controllers
{
    public class ClubController : Controller
    {
        private readonly IRepository repository;

        public ClubController(IRepository repository)
        {
            this.repository = repository;
        }

        public IActionResult GetAll()
        {
            return Ok(repository.Clubs.OrderBy(c => c.Name).ThenBy(c=>c.Country?.Name).ToList());
        }

        public IActionResult Get(Guid id)
        {
            return Ok(repository.Clubs.FirstOrDefault(c => c.Id == id));
        }

        [HttpPost]
        public IActionResult Create([FromBody] Club club)
        {
            if (ModelState.IsValid)
            {
                var clubCountry = repository.Countries.FirstOrDefault(c => club.CountryId == c.Id);
                if (clubCountry == null)
                {
                    return BadRequest(new { Error = "Country not found" });
                }
                club.Id = Guid.NewGuid();
                repository.Clubs.Add(club);
                return Ok("Club added successfully");
            }
            return BadRequest(new { Error = "Model not valid" });
        }

        [HttpPost]
        public ActionResult Edit([FromBody] Club club)
        {
            if (ModelState.IsValid)
            {
                var clubCountry = repository.Countries.FirstOrDefault(c => club.CountryId == c.Id);
                if (clubCountry == null)
                {
                    return BadRequest(new { Error = "Country not found" });
                }
                var repoCountry = repository.Clubs.Find(club.Id);
                if (repoCountry == null)
                    return NotFound();
                repository.Clubs.Update(club);
                return Ok();
            }
            return BadRequest(new { Error = "Model not valid" });
        }
    }
}
