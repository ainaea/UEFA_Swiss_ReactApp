using UEFASwissFormatSelector.Models;

namespace Net_ReactApp2.Server.ViewModels
{
    public class ScenarioInstanceDTO: Identifiable
    {
        public ScenarioInstanceDTO(ScenarioInstance scenarioInstance)
        {
            Id = scenarioInstance.Id;
            Name = scenarioInstance.Name;
            ScenarioId = scenarioInstance.ScenarioId;
            ScenarioName = scenarioInstance.Scenario.Name;
            var potsFixtures = new List<PotClubsFixturesDTO>();
            foreach (Pot pot in scenarioInstance.Pots.Where(p=>!p.IsOpponentPot).ToList())
            {
                potsFixtures.Add(new PotClubsFixturesDTO(pot, scenarioInstance.MatchUps, scenarioInstance.MatchUpSkeleton));
            }
            PotsFixtures = potsFixtures.ToArray();
        }
        public Guid ScenarioId { get; set; }
        public string ScenarioName { get; set; }
        public IEnumerable<PotClubsFixturesDTO> PotsFixtures { get; set; }
    }
}
