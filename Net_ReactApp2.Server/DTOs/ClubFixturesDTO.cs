using Microsoft.AspNetCore.Routing.Patterns;
using UEFASwissFormatSelector.Models;

namespace Net_ReactApp2.Server.ViewModels
{
    public class ClubFixturesDTO
    {
        private const char separator = '_';
        public ClubFixturesDTO(Club club, List<string> clubfixturesSkeleton, List<Club> clubfixtures)
        {
            MainClub = club;
            var clubFixtures = new List<OpponentDTO>();
            for (int i = 0; i < clubfixturesSkeleton.Count; i++)
            {
                clubFixtures.Add(new OpponentDTO(clubfixtures[i], clubfixturesSkeleton[i].Split(separator)[1], clubfixturesSkeleton[i].Split(separator)[2] == true.ToString() ));
            }
            fixtures = clubFixtures;
        }
        public Club MainClub { get; set; }
        private IEnumerable<OpponentDTO> fixtures;
        public IEnumerable<OpponentDTO> Fixtures { get => fixtures.OrderBy(fx => fx.PotName).ThenByDescending(fx => fx.Home).ToList(); }
    }
}
