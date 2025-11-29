namespace UEFASwissFormatSelector.Models
{
    public class Match: Identifiable
    {
        public virtual Club Home { get; set; }
        public virtual Club Away { get; set; }
        public Guid HomeId { get; set; }
        public Guid AwayId { get; set; }
        public int? Day { get; set; }
        public Guid ScenarioInstanceId { get; set; }
        public virtual ScenarioInstance ScenarioInstance { get; set; }
        public Match(Club homeside, Club awayside, ScenarioInstance scenarioInstance)
        {
            HomeId = homeside.Id;
            AwayId = awayside.Id;
            Home = homeside;
            Away = awayside;
            ScenarioInstanceId = scenarioInstance.Id;
            ScenarioInstance = scenarioInstance;

        }
    }
}
