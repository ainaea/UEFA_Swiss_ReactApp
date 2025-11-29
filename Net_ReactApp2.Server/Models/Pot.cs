namespace UEFASwissFormatSelector.Models
{
    public class Pot: Identifiable
    {
        public Guid ScenarioInstanceId { get; set; }
        public virtual IEnumerable<ClubInPot> ClubsInPot { get; set; }
        public bool IsOpponentPot { get; set; }
        public Pot(string name, int numberOfTeamsInPot)
        {
            ClubsInPot = new ClubInPot[numberOfTeamsInPot];
            Name = name;
        }

        /// <summary>
        /// To be used only for db purpose
        /// </summary>
        public Pot()
        {
            
        }
    }
}
