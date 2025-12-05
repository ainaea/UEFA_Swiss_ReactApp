using UEFASwissFormatSelector.Models;
using UEFASwissFormatSelector.Services;

namespace Net_ReactApp2.Server.ViewModels
{
    public class PotClubsFixturesDTO
    {
        public PotClubsFixturesDTO(Pot pot, ModifiedDictionary<List<Club>> matchUps, ModifiedDictionary<List<string>>? matchUpsSkeleton)
        {
            PotId = pot.Id;
            PotName = pot.Name;
            var potClubsFixtures = new List<ClubFixturesDTO>();
            foreach (Club club in pot.ClubsInPot.Select(cip=> cip.Club))
            {
                potClubsFixtures.Add(new ClubFixturesDTO(club, matchUpsSkeleton[club.Id], matchUps[club.Id] ));
            }
            ClubsFixtures = potClubsFixtures;
        }
        public Guid PotId { get; set; }
        public string PotName { get; set; }
        public IEnumerable<ClubFixturesDTO> ClubsFixtures { get; set; }
    }
}
