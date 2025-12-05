using UEFASwissFormatSelector.Models;

namespace Net_ReactApp2.Server.ViewModels
{
    public class OpponentDTO
    {
        public OpponentDTO(Club opponent, string pot, bool home)
        {
            Opponent = opponent;
            PotName = pot;
            Home = home;
        }
        public Club Opponent { get; set; }
        public String PotName { get; set; }
        public bool Home { get; set; }
    }
}
