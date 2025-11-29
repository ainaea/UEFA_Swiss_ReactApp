using System.ComponentModel.DataAnnotations;

namespace UEFASwissFormatSelector.Models
{
    public class Scenario : Identifiable
    {
        [Display(Name = "Number of pots")]
        [Range(1, int.MaxValue)]
        public int NumberOfPot { get; set; }
        [Display(Name = "Number of Teams in a pot")]
        [Range(1, int.MaxValue)]
        public int NumberOfTeamsPerPot { get; set; }
        [Display(Name = "Number of Opponents from a pot")]
        [Range(1, int.MaxValue)]
        public int NumberOfGamesPerPot { get; set; }
        [Display(Name = "Play Each Match Home and Away")]
        public bool HomeAndAwayPerOpponent { get; set; }
    }
}
