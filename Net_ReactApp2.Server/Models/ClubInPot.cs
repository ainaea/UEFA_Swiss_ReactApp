namespace UEFASwissFormatSelector.Models
{
    public class ClubInPot
    {
        public Guid ClubId { get; set; }
        public Guid PotId { get; set; }
        public virtual Club? Club { get; set; }
        public virtual Pot? Pot { get; set; }
        public ClubInPot(Guid clubId, Guid potId)
        {
            ClubId = clubId;
            PotId = potId;
        }

        public ClubInPot()
        {
            
        }
    }
}
