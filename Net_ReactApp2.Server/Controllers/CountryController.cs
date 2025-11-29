using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using UEFASwissFormatSelector.Models;
using UEFASwissFormatSelector.Services;

namespace Net_ReactApp2.Server.Controllers
{
    //[Route("[controller]")]
    public class CountryController : Controller
    {
        private readonly IRepository repository;

        public CountryController(IRepository repository)
        {
            this.repository = repository;
        }
        public IActionResult GetAll()
        {
            return Ok(repository.Countries.OrderBy(c => c.Name).ToList());
        }

        public IActionResult Get(Guid id)
        {
            return Ok(repository.Countries.FirstOrDefault(c => c.Id == id));
        }

        [HttpPost]
        public IActionResult Create([FromBody]Country country)
        {
            if (ModelState.IsValid)
            {
                country.Id = Guid.NewGuid();
                repository.Countries.Add(country);
                return Ok("Country added successfully");
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
