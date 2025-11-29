using UEFASwissFormatSelector.Models;

namespace UEFASwissFormatSelector.Services
{
    public interface IMatchDrawService
    {
        IEnumerable<Pot> PotTeam(ScenarioInstance scenarioInstance);
        IEnumerable<Pot> GenerateOpponentsForClub(ScenarioInstance scenarioInstance, Club club);
        Dictionary<Guid, IEnumerable<Pot>> GenerateOpponentsForAllClubs(ScenarioInstance scenarioInstance);
        IEnumerable<Club> PickOpponents(int numberOfOpponents, IEnumerable<Club> from);
        /*Dictionary<Guid, List<Club>>*/
        (Dictionary<Guid, List<Club>>, Dictionary<Guid, List<string>>) DoMatchUps(ScenarioInstance scenarioInstance, int numberOfOpponentPerPot);
        Dictionary<Guid, Club[]> FixMatches(IEnumerable<Club> opponents, Club club, Dictionary<Guid, Club[]> matchLineUp, Dictionary<Guid, IEnumerable<Pot>> allOpponents);
        void RemoveFromPossibleOpponents(IEnumerable<Club> opponents, Club club, Dictionary<Guid, IEnumerable<Pot>> allOpponents);
    }
}
