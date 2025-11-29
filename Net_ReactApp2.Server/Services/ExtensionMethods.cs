using Microsoft.EntityFrameworkCore;
using UEFASwissFormatSelector.Models;

namespace UEFASwissFormatSelector.Services
{
    public static class ExtensionMethods
    {
        public static void SeedDBData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>().HasData( MockRepository.SetupCountries() );
            modelBuilder.Entity<Scenario>().HasData(MockRepository.SetupScenarios());
            modelBuilder.Entity<Club>().HasData(MockRepository.SetupClubs());
        }
    }
}
