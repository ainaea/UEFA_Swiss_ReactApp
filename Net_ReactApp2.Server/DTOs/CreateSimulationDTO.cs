namespace Net_ReactApp2.Server.ViewModels
{
    public class CreateSimulationDTO
    {
        public string Name { get; set; }
        public Guid ScenarioId { get; set; }
        public IEnumerable<RankableClub> Clubs { get; set; }
    }
}
