using UEFASwissFormatSelector.Models;

namespace UEFASwissFormatSelector.Services
{
    public interface IMatchDrawService
    {
        IEnumerable<Pot> PotTeam(ScenarioInstance scenarioInstance);
        IEnumerable<Pot> GenerateOpponentsForClub(ScenarioInstance scenarioInstance, Club club);
        Dictionary<Guid, IEnumerable<Pot>> GenerateOpponentsForAllClubs(ScenarioInstance scenarioInstance);
        IEnumerable<Club> PickOpponents(int numberOfOpponents, IEnumerable<Club> from);
        (Dictionary<Guid, List<Club>>, Dictionary<Guid, List<string>>) DoMatchUps(ScenarioInstance scenarioInstance, int numberOfOpponentPerPot);        
    }
}
