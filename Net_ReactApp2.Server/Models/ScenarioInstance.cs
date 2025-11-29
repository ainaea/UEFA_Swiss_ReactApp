using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using UEFASwissFormatSelector.Services;

namespace UEFASwissFormatSelector.Models
{
    public class ScenarioInstance: Identifiable
    {
        public Guid ScenarioId { get; set; }
        public Scenario Scenario { get; set; }
        public virtual IEnumerable<Pot> Pots { get; set; }
        public virtual IEnumerable<ClubInScenarioInstance> ClubsInScenarioInstance { get; set; }
        //[NotMapped]
        //public virtual Dictionary<Guid, IEnumerable<Pot>>? Opponents { get; set; }
        private ModifiedDictionary<IEnumerable<Pot>>? opponents;
        public virtual ModifiedDictionary<IEnumerable<Pot>>? Opponents
        {
            get => opponents;
            set
            {
                if (value == null && opponents == null)
                    return;
                else
                {
                    if (opponents == null || opponents.Id == Guid.Empty)
                    {
                        value.Id = Guid.NewGuid();
                    }
                    else
                    {
                        value.Id = opponents.Id;
                    }
                    opponents = value;
                    opponents.ScenarioInstanceId = this.Id;
                }                
            }
        }
        public DbModifiedDictionaryEntity<Pot>? DbOpponents { get; set; }
        //[NotMapped]
        //public virtual Dictionary<Guid, List<Club>>? MatchUps { get; set; }
        private ModifiedDictionary<List<Club>>? matchUps;
        public virtual ModifiedDictionary<List<Club>>? MatchUps
        {
            get => matchUps;
            set
            {
                if (value == null && matchUps == null)
                    return;
                if (matchUps == null || matchUps.Id == Guid.Empty)
                {
                    value.Id = Guid.NewGuid();
                }
                else
                {
                    value.Id = matchUps.Id;
                }
                matchUps = value;
                MatchUps.ScenarioInstanceId = this.Id;
            }
        }
        public DbModifiedDictionaryEntity<Club>? DbMatchUps { get; set; }
        //[NotMapped]
        //public virtual Dictionary<Guid, List<String>>? MatchUpSkeleton { get; set; }
        private ModifiedDictionary<List<String>>? matchUpSkeleton;
        public virtual ModifiedDictionary<List<String>>? MatchUpSkeleton
        {
            get => matchUpSkeleton;
            set
            {
                if (value == null && matchUpSkeleton == null)
                    return;
                if (matchUpSkeleton == null || matchUpSkeleton.Id == Guid.Empty)
                {
                    value.Id = Guid.NewGuid();
                }
                else
                {
                    value.Id = matchUpSkeleton.Id;
                }
                matchUpSkeleton = value;
                matchUpSkeleton.ScenarioInstanceId = this.Id;
            }
        }
        [NotMapped]
        public List<DbModifiedDictionaryEntity<Pot>>? GetEquivalentPotDbEntities { get; }
        [NotMapped]
        public List<DbModifiedDictionaryEntity<Club>>? GetEquivalentClubDbEntities { get; }
        public ScenarioInstance(Scenario scenario)
        {
            Scenario = scenario;
            ScenarioId = scenario.Id;
            //Pots = new Pot[scenario.NumberOfPot];
            //ClubsInScenarioInstance = new ClubInScenarioInstance[scenario.NumberOfPot * scenario.NumberOfTeamsPerPot];
        }
        /// <summary>
        /// To be used only for db purpose
        /// </summary>
        public ScenarioInstance()
        {
            
        }
    }
}
