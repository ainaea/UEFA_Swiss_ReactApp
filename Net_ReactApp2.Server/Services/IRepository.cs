using UEFASwissFormatSelector.Models;

namespace UEFASwissFormatSelector.Services
{
    public interface IRepository
    {
        IUnitRepository<Club> Clubs { get; set; }
        IUnitRepository<Country> Countries { get; set; }
        IUnitRepository<Scenario> Scenarios { get; set; }
        IUnitRepository<ScenarioInstance> ScenarioInstances { get; set; }
    }
}
