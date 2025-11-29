using System.ComponentModel.DataAnnotations;

namespace UEFASwissFormatSelector.Models
{
    public class Country: Identifiable
    {
        [Length(3, 3)]
        public string Abbrevation { get; set; } = "NIL";
        public string? Flag { get; set; }
    }
}
