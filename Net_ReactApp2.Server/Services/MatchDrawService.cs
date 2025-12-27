using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UEFASwissFormatSelector.Models;

namespace UEFASwissFormatSelector.Services
{
    public class MatchDrawService : IMatchDrawService
    {
        public MatchDrawService()
        {
            //allOpponents = new Dictionary<Guid, IEnumerable<Pot>>();
        }
        public IEnumerable<Pot> PotTeam(ScenarioInstance scenarioInstance)
        {
            var scenario = scenarioInstance.Scenario;
            var clubsInScenarioInstance = scenarioInstance.ClubsInScenarioInstance;
            Pot[] pottedTeams = new Pot[scenario.NumberOfPot];
            clubsInScenarioInstance = clubsInScenarioInstance.OrderByDescending(c => c.Ranking).ToList();
            for (int i = 0; i < scenario.NumberOfPot; i++)
            {
                pottedTeams[i] = new Pot(GeneratePotName(i), scenario.NumberOfTeamsPerPot);
                var clubsInPot = clubsInScenarioInstance.Skip(i * scenario.NumberOfTeamsPerPot).Take(scenario.NumberOfTeamsPerPot).ToList();
                pottedTeams[i].ClubsInPot = clubsInPot.Select(c => new ClubInPot(c.ClubId, pottedTeams[i].Id) { Club = c.Club }).ToList();
                pottedTeams[i].ScenarioInstanceId = scenarioInstance.Id;
            }
            return pottedTeams;
        }
        private string GeneratePotName(int i) => $"Pot {Enum.GetName(typeof(PotEnum), i) ?? i.ToString()}";

        public IEnumerable<Pot> GenerateOpponentsForClub(ScenarioInstance scenarioInstance, Club club)
        {
            List<Pot> possibleOpponents = new List<Pot>();
            foreach (Pot pot in scenarioInstance.Pots)
            {
                var opponentsFromPot = new Pot(pot.Name, pot.ClubsInPot.Count()) { ScenarioInstanceId = scenarioInstance.Id, IsOpponentPot = true };
                var possibleOpponentsCIP = pot.ClubsInPot.Where(cp => cp.ClubId != club.Id && cp.Club?.CountryId != club.CountryId).ToList();
                var adjustedpossibleOpponentsCIP = new List<ClubInPot>();
                foreach (var cip in possibleOpponentsCIP)
                {
                    adjustedpossibleOpponentsCIP.Add(new ClubInPot { Club = cip.Club, ClubId = cip.ClubId, PotId = opponentsFromPot.Id });
                }
                opponentsFromPot.ClubsInPot = adjustedpossibleOpponentsCIP;
                possibleOpponents.Add(opponentsFromPot);
            }
            return possibleOpponents;
        }

        public Dictionary<Guid, IEnumerable<Pot>> GenerateOpponentsForAllClubs(ScenarioInstance scenarioInstance)
        {
            Dictionary<Guid, IEnumerable<Pot>> allOpponents = new Dictionary<Guid, IEnumerable<Pot>>();
            //Dictionary<Guid, IEnumerable<Pot>> allPossibleOpponents = new Dictionary<Guid, IEnumerable<Pot>>();
            foreach (ClubInScenarioInstance club in scenarioInstance.ClubsInScenarioInstance)
            {
                allOpponents[club.ClubId] = GenerateOpponentsForClub(scenarioInstance, club.Club!);
            }
            return allOpponents;
        }

        public IEnumerable<Club> PickOpponents(int numberOfOpponents, IEnumerable<Club> from)
        {
            var opponents = new List<Club>();
            if (from == null)
                return opponents;
            if (numberOfOpponents >= from!.Count())
                return from;
            for (int i = 0; i < numberOfOpponents; i++)
            {
                int choiceIndex = random.Next(0, from!.Count());
                opponents.Add(from!.ToList()[choiceIndex]);
                var newFrom = from!.ToList();
                newFrom.Remove(from!.ToList()[choiceIndex]);
                from = newFrom;
            }
            return opponents;
        }
        
        private List<Club> FindOpponents(int numberOfOpponent, Guid forClubId, List<Club> preferredOppponents, List<Club> allPot)
        {
            if (numberOfOpponent == preferredOppponents.Count())
            {
                return preferredOppponents;
            }
            else if (numberOfOpponent < preferredOppponents.Count())
            {
                return PickOpponents(numberOfOpponent, preferredOppponents).ToList();
            }
            else
            {
                //means number of preferredOppponents is not enough
                int remainderOppponents = numberOfOpponent - preferredOppponents.Count();
                var unselectedClubs = allPot.Where(c => c.Id != forClubId && !preferredOppponents.Contains(c)).ToList();
                var remainder = PickOpponents(remainderOppponents, unselectedClubs);
                var newList = new List<Club>();
                foreach (Club item in preferredOppponents)
                {
                    newList.Add(item);
                }
                foreach (Club item in remainder.ToList())
                {
                    newList.Add(item);
                }
                return newList;
            }

        }        
        private int FoundOpponentsInPot(string potName, Guid clubId, Dictionary<Guid, List<string>> fixedMatches)
        {
            //returns the number of opponents a club as found/booked for match in a pot
            return fixedMatches[clubId].Where(fm => fm.Contains(potName)).Count();
        }
        private string GetClubPotName(Guid clubId, IEnumerable<Pot> pots)
        {
            var choicePot = pots.FirstOrDefault(p => p.ClubsInPot.Any(cip => cip.ClubId == clubId));
            return choicePot == null ? string.Empty : choicePot.Name;
        }        
        private bool ClubPotFixtureFull(Guid clubId, string againstPotName, Dictionary<Guid, List<string>> fixedMatches, int target)
        {
            return ClubPotFixtureCount(clubId, againstPotName, fixedMatches) >= target;
        }
        private int ClubPotFixtureCount(Guid clubId, string againstPotName, Dictionary<Guid, List<string>> fixedMatches)
        {
            return fixedMatches[clubId].Where(s => s.Contains(GenerateClubPotName(null, againstPotName))).Count();
        }
        private bool ClubHasFixtureAgainst(Guid firstClubId, Guid secondClubId, Dictionary<Guid, List<string>> fixedMatches)
        {
            return fixedMatches[firstClubId].Any(fm => fm.Contains(secondClubId.ToString()));
        }
        private const char separator = '_';
        private string GenerateClubPotName(Guid? clubId, string potName)
        {
            return $"{clubId?.ToString() ?? string.Empty}{separator}{potName}";
        }
        private Guid ExtractClubId_Club_PotName(string str)
        {
            return new Guid(str.Split(separator)[0].ToString());
        }
        private Club GetClub(string str, IEnumerable<ClubInScenarioInstance> clubsInScenarioInstance)
        {
            var id = ExtractClubId_Club_PotName(str);
            return clubsInScenarioInstance.First(cisi => cisi.Club!.Id == id).Club!;
        }
        private string HomeAwayString(bool home) => $"{separator}{home}";        

        #region New Impelentation

        private Dictionary<Guid, List<string>> SwapFixtureFixing(ScenarioInstance scenarioInstance, int numberOfOpponentPerPot, Dictionary<Guid, List<string>> fixedMatches, int expectedMatchCount, List<string> potNames, int maxOpponenentFromADivision)
        {
            foreach (var kvp in fixedMatches.Where(kvp => kvp.Value.Count() < expectedMatchCount).OrderByDescending(kvp => kvp.Value.Count()))
            {
                if (kvp.Value.Count >= expectedMatchCount)
                    continue;
                Club thisClub = scenarioInstance.ClubsInScenarioInstance.FirstOrDefault(c => c.ClubId == kvp.Key)?.Club!;
                string clubPotName = GetClubPotName(thisClub.Id, scenarioInstance.Pots);
                foreach (string opponentPotname in potNames)
                {
                    int clubPotFixtureCount = ClubPotFixtureCount(kvp.Key, opponentPotname, fixedMatches);
                    if (clubPotFixtureCount >= numberOfOpponentPerPot)
                        continue;
                    int remainingOpponents = numberOfOpponentPerPot - clubPotFixtureCount;
                    for (int i = 0; i < remainingOpponents; i++)
                    {
                        //for thisClub not to be fully matchedup at this point, all possible opponents must have been fixed up except a club, selectedClubInOpponentPotWithIncompletePotFixtures = sCIOPWIPF that should have played thisClub but couldn't and that club will have an incomplete fixture too
                        var clubsInOpponentPot = scenarioInstance.Pots.First(p => p.Name == opponentPotname).ClubsInPot.Select(cip => cip.Club).ToList();
                        if (clubPotName == opponentPotname)
                        {
                            var this_Club = clubsInOpponentPot.First(c => c.Id == thisClub.Id);
                            clubsInOpponentPot.Remove(this_Club);
                        }
                        var clubsInOpponentPotWithIncompletePotFixtures = clubsInOpponentPot.Where(c => !ClubPotFixtureFull(c.Id, clubPotName, fixedMatches, numberOfOpponentPerPot)).ToList();
                        if (clubsInOpponentPotWithIncompletePotFixtures.Count() == 0 && clubPotName == opponentPotname)
                        {
                            //club is unable to find matchUp for itself because all other clubs are fully matched up
                            if (remainingOpponents % 2 == 1)
                                continue;
                            i++;
                        }
                        //selectedClubInOpponentPotWithIncompletePotFixtures = sCIOPWIPF
                        var sCIOPWIPF = clubsInOpponentPotWithIncompletePotFixtures.Count() == 0 && clubPotName == opponentPotname && remainingOpponents % 2 == 0 ? thisClub : FindOpponents(1, thisClub.Id, clubsInOpponentPotWithIncompletePotFixtures, clubsInOpponentPotWithIncompletePotFixtures).First();      //from opponentPotname

                        //this implementation was added because some earlier swap migth have made direct matching possible and maybe necessary.
                        var freeAndPlayableOpponents = clubsInOpponentPotWithIncompletePotFixtures.Where(c => c.CountryId != thisClub.CountryId && !ClubHasFixtureAgainst(c.Id, thisClub.Id, fixedMatches) && ThisClubCanPlayCountryClub(c, thisClub, maxOpponenentFromADivision, fixedMatches, scenarioInstance.ClubsInScenarioInstance)).ToList();
                        if (freeAndPlayableOpponents.Count > 0)
                        {
                            var selectedFAPO = FindOpponents(1, thisClub.Id, freeAndPlayableOpponents, freeAndPlayableOpponents).First();
                            fixedMatches[thisClub.Id].Add(GenerateClubPotName(selectedFAPO.Id, opponentPotname));
                            fixedMatches[selectedFAPO.Id].Add(GenerateClubPotName(thisClub.Id, clubPotName));
                            continue;
                        }

                        var potentialThisClubOpponents = clubsInOpponentPot.Where(c => thisClub.CountryId != c.CountryId && !ClubHasFixtureAgainst(thisClub.Id, c.Id, fixedMatches) && ThisClubCanPlayCountryClub(thisClub, c, maxOpponenentFromADivision, fixedMatches, scenarioInstance.ClubsInScenarioInstance));
                        var clubPotOpponent = new Dictionary<Guid, List<Club>>();
                        foreach (Club club in potentialThisClubOpponents)
                        {
                            clubPotOpponent[club.Id] = new List<Club>();
                            var clubPotName_Club_Clubs = fixedMatches[club.Id].Where(str => str.Contains(clubPotName)).Select(str => GetClub(str, scenarioInstance.ClubsInScenarioInstance)).ToList();  //all from clubPotName
                            foreach (var itemClub in clubPotName_Club_Clubs)
                            {
                                if (!ClubHasFixtureAgainst(itemClub.Id, sCIOPWIPF.Id, fixedMatches) && itemClub.CountryId != sCIOPWIPF.CountryId && ThisClubCanPlayCountryClub(itemClub, sCIOPWIPF, maxOpponenentFromADivision, fixedMatches, scenarioInstance.ClubsInScenarioInstance))
                                    clubPotOpponent[club.Id].Add(itemClub);
                            }
                        }
                        var acceptableThisClubOpponents = clubPotOpponent.Where(kvp => kvp.Value.Count() > 0).Select(kvp => scenarioInstance.ClubsInScenarioInstance.First(cisi => cisi.ClubId == kvp.Key).Club).ToList();
                        if (acceptableThisClubOpponents.Count() == 0)
                            continue;
                        var selectedThisClubOpponent = FindOpponents(1, thisClub.Id, acceptableThisClubOpponents, acceptableThisClubOpponents).First();      //from opponentPotname
                        var acceptablesCIOPWIPFClubOpponents = clubPotOpponent[selectedThisClubOpponent.Id];
                        if (thisClub.Id == sCIOPWIPF.Id)
                            acceptablesCIOPWIPFClubOpponents = acceptablesCIOPWIPFClubOpponents.Where(c => c.Id != selectedThisClubOpponent.Id).ToList();
                        //the condition above is intended for when a club cant match because other clubs have fully matched it up. Check var sCIOPWIPF = clubsInOpponentPotWithIncomple... 
                        if (acceptablesCIOPWIPFClubOpponents.Count() == 0)
                            continue;
                        var selectedsCIOPWIPFClubOpponent = FindOpponents(1, sCIOPWIPF.Id, acceptablesCIOPWIPFClubOpponents, acceptablesCIOPWIPFClubOpponents).First();    // from clubPotName
                        //selectedThisClubOpponent and selectedsCIOPWIPFClubOpponents are two clubs playing each other that can be rematched with sCIOPWIPF and thisClub after their mutual fixrture has been canceled
                        string str1 = fixedMatches[selectedThisClubOpponent.Id].First(str => str.Contains(selectedsCIOPWIPFClubOpponent.Id.ToString()));
                        fixedMatches[selectedThisClubOpponent.Id].Remove(str1);
                        string str2 = fixedMatches[selectedsCIOPWIPFClubOpponent.Id].First(str => str.Contains(selectedThisClubOpponent.Id.ToString()));
                        fixedMatches[selectedsCIOPWIPFClubOpponent.Id].Remove(str2);
                        List<string> fgh = new List<string>();
                        fixedMatches[thisClub.Id].Add(GenerateClubPotName(selectedThisClubOpponent.Id, opponentPotname));
                        fixedMatches[selectedThisClubOpponent.Id].Add(GenerateClubPotName(thisClub.Id, clubPotName));
                        fixedMatches[sCIOPWIPF.Id].Add(GenerateClubPotName(selectedsCIOPWIPFClubOpponent.Id, clubPotName));
                        fixedMatches[selectedsCIOPWIPFClubOpponent.Id].Add(GenerateClubPotName(sCIOPWIPF.Id, opponentPotname));                        
                    }
                }
            }
            return fixedMatches;
        }

        public (Dictionary<Guid, List<Club>>, Dictionary<Guid, List<string>>) DoMatchUps(ScenarioInstance scenarioInstance, int numberOfOpponentPerPot)
        {
            return PotXPotDoMatchUps(scenarioInstance, numberOfOpponentPerPot);
            //int loopCount = 0;
            //int expectedMatchCount = scenarioInstance.Pots.Count() * numberOfOpponentPerPot;
            //Dictionary<Guid, List<string>> fixedMatches = new Dictionary<Guid, List<string>>();     //Guid is for clubId and string is the concatination of format "opponentCludId_potname"
            //Dictionary<Guid, List<Club>> fixedMatchesFull = new Dictionary<Guid, List<Club>>();
            //do
            //{
            //    (fixedMatchesFull, fixedMatches) = LocalDoMatchUps(scenarioInstance, numberOfOpponentPerPot);
            //    //loopCount++;
            //} 
            //while (loopCount < 3 && !fixedMatches.Any(kvp=> kvp.Value.Count() != expectedMatchCount));
            //return (fixedMatchesFull, fixedMatches);
        }

        private ModifiedDictionary<IEnumerable<Pot>>? UpdateAllOpponents(ModifiedDictionary<IEnumerable<Pot>>? allOpponent, Dictionary<Guid, List<string>> fixedMatches, ScenarioInstance scenarioInstance, int numberOfOpponentPerPot, int maxOpponenentFromADivision)
        {
            ModifiedDictionary<IEnumerable<Pot>> newAllOpponents = new ModifiedDictionary<IEnumerable<Pot>>();
            foreach (KeyValuePair<Guid, IEnumerable<Pot>> opponentKvp in allOpponent.GetAsDictionary())
            {
                Club thisClub = scenarioInstance.ClubsInScenarioInstance.FirstOrDefault(c => c.ClubId == opponentKvp.Key)?.Club!;
                string clubPotName = GetClubPotName(thisClub.Id, scenarioInstance.Pots);
                var kvpPots = new List<Pot>();
                foreach (Pot pot in opponentKvp.Value)
                {
                    var fixtureFreeOpponents = pot.ClubsInPot.Where(cip => !ClubHasFixtureAgainst(cip.ClubId, thisClub.Id, fixedMatches) && !ClubPotFixtureFull(cip.ClubId, clubPotName, fixedMatches, numberOfOpponentPerPot)).Select(cip => cip.Club).ToList();
                    var divisionAndFixtureFreeOpponentIds = fixtureFreeOpponents.Where(countryClub => ThisClubCanPlayCountryClub(countryClub, thisClub, maxOpponenentFromADivision, fixedMatches, scenarioInstance.ClubsInScenarioInstance)).Select(c=>c.Id).ToList();
                    var prefferedCIP = pot.ClubsInPot.Where(cip => divisionAndFixtureFreeOpponentIds.Contains(cip.ClubId)).ToList();
                    //pot.ClubsInPot = prefferedCIP;
                    kvpPots.Add(new Pot(pot.Name, pot.ClubsInPot.Count())
                    {
                        Id = pot.Id,
                        IsOpponentPot = pot.IsOpponentPot,
                        ScenarioInstanceId = pot.ScenarioInstanceId,
                        ClubsInPot = prefferedCIP
                    });
                }
                newAllOpponents[opponentKvp.Key] = kvpPots;
            }
            return newAllOpponents;
        }
        private (bool, Dictionary<Guid, List<string>>, ModifiedDictionary<IEnumerable<Pot>>?) IsFixtureAllowed(Club thisClub, string clubPotName, Club opponent, string oppositionPotName, Dictionary<Guid, List<string>> fixMatches, ScenarioInstance scenarioInstance, int numberOfOpponentPerPot, int maxOpponenentFromADivision, ModifiedDictionary<IEnumerable<Pot>>? allOpponent)
        {
            var fixedMatches = new Dictionary<Guid, List<string>>(fixMatches);
            var a1 = fixedMatches[thisClub.Id].ToList();
            a1.Add(GenerateClubPotName(opponent.Id, oppositionPotName));
            fixedMatches[thisClub.Id] = a1;
            //fixedMatches[thisClub.Id].Add(GenerateClubPotName(opponent.Id, oppositionPotName));
            var b1 = fixedMatches[opponent.Id].ToList();
            b1.Add(GenerateClubPotName(thisClub.Id, clubPotName));
            fixedMatches[opponent.Id] = b1;
            //fixedMatches[opponent.Id].Add(GenerateClubPotName(thisClub.Id, clubPotName));

            var newAllOpponent = UpdateAllOpponents(allOpponent, fixedMatches, scenarioInstance, numberOfOpponentPerPot, maxOpponenentFromADivision);

            foreach (KeyValuePair<Guid, IEnumerable<Pot>> opponentKvp in newAllOpponent.GetAsDictionary())
            {
                Club thisKvpClub = scenarioInstance.ClubsInScenarioInstance.FirstOrDefault(c => c.ClubId == opponentKvp.Key)?.Club!;
                string kvpClubPotName = GetClubPotName(thisKvpClub.Id, scenarioInstance.Pots);
                var kvpPots = new List<Pot>();
                foreach (Pot pot in opponentKvp.Value)
                {
                    int remainingOpponents = numberOfOpponentPerPot - FoundOpponentsInPot(pot.Name, thisClub.Id, fixedMatches);
                    if (remainingOpponents == 0)
                        continue;
                    var prefferedCIP = pot.ClubsInPot.ToList();
                    if (remainingOpponents > prefferedCIP.Count() && prefferedCIP.Count()!= allOpponent[opponentKvp.Key].First(p=>p.Name == pot.Name).ClubsInPot.Count())
                    {
                        //a situation of unmatchable clubs will arise.
                        return ((false, null, null));
                    }
                }
            }
            return ((true, fixedMatches, newAllOpponent));
        }
        private bool ThisClubCanPlayCountryClub(Club thisClub, Club countryClub, int maxDivisionCount,  Dictionary<Guid, List<string>> fixedMatches, IEnumerable<ClubInScenarioInstance> clubsInScenarioInstance)
        {
            Dictionary<Guid, List<Club>> fixedMatchesFull = new Dictionary<Guid, List<Club>>();
            GeneratePartialFixedMatchesFull(ref fixedMatchesFull, fixedMatches, clubsInScenarioInstance, new List<Club> { thisClub, countryClub});
            int thisClubCountryCount = fixedMatchesFull[thisClub.Id].Where(c => c.CountryId == countryClub.CountryId).Count();
            int thisCountryClubCountryCount = fixedMatchesFull[countryClub.Id].Where(c => c.CountryId == thisClub.CountryId).Count();
            return thisClubCountryCount < maxDivisionCount && thisCountryClubCountryCount < maxDivisionCount;
        }

        private bool ThisClubIsPlayingDivision(Club thisClub, Club countryClub, Dictionary<Guid, List<string>> fixedMatches, IEnumerable<ClubInScenarioInstance> clubsInScenarioInstance)
        {
            return !ThisClubCanPlayCountryClub(thisClub, countryClub, 1, fixedMatches, clubsInScenarioInstance);
        }

        private Dictionary<Guid, List<Club>> GenerateFixedMatchesFull(Dictionary<Guid, List<string>> fixedMatches, IEnumerable<ClubInScenarioInstance> clubsInScenarioInstance)
        {
            Dictionary<Guid, List<Club>> fixedMatchesFull = new Dictionary<Guid, List<Club>>();
            var clubs = clubsInScenarioInstance.Select(cisi => cisi.Club as Club).ToList();
            GeneratePartialFixedMatchesFull(ref fixedMatchesFull, fixedMatches, clubsInScenarioInstance, clubs);
            return fixedMatchesFull;
        }
        private void GeneratePartialFixedMatchesFull(ref Dictionary<Guid, List<Club>> fixedMatchesFull, Dictionary<Guid, List<string>> fixedMatches, IEnumerable<ClubInScenarioInstance> clubsInScenarioInstance, List<Club> clubs)
        {
            foreach (var kvp_key in clubs.Select(c => c.Id))
            {
                fixedMatchesFull[kvp_key] = new List<Club>();
                foreach (var valueVal in fixedMatches[kvp_key])
                {
                    if (fixedMatchesFull.ContainsKey(kvp_key))
                        fixedMatchesFull[kvp_key].Add(GetClub(valueVal, clubsInScenarioInstance));
                    else
                        fixedMatchesFull[kvp_key] = new List<Club>() { GetClub(valueVal, clubsInScenarioInstance) };
                }
            }
        }

        #endregion

        #region PotToPotApproachTopDown
        private int alternator = 0;
        private bool GetAlternatingBool => alternator++ % 2 == 0;
        private (Dictionary<Guid, List<Club>>, Dictionary<Guid, List<string>>) PotXPotDoMatchUps(ScenarioInstance scenarioInstance, int numberOfOpponentPerPot)
        {
            int expectedMatchCount = scenarioInstance.Pots.Count() * numberOfOpponentPerPot;
            Dictionary<Guid, List<string>> fixedMatches = new Dictionary<Guid, List<string>>();     //Guid is for clubId and string is the concatination of format "opponentCludId_potname"
            Dictionary<Guid, List<Club>> fixedMatchesFull = new Dictionary<Guid, List<Club>>();
            foreach (Club club in scenarioInstance.ClubsInScenarioInstance.OrderByDescending(cisi => cisi.Ranking).Select(c => c.Club).ToList())
            {
                fixedMatches[club!.Id] = new List<string>();
                fixedMatchesFull[club.Id] = new List<Club>();
            }
            Dictionary<string, List<Guid>> stats = new Dictionary<string, List<Guid>>();
            foreach (Pot pot in scenarioInstance.Pots)
            {
                stats[pot.Name] = new List<Guid>();
                var currStat = stats[pot.Name];
                foreach (ClubInPot cip in pot.ClubsInPot)
                {
                    if (!currStat.Contains(cip.Club.CountryId))
                        currStat.Add(cip.Club.CountryId);
                }
            }
            var potNames = stats.OrderBy(kvp => kvp.Value.Count()).Select(kvp => kvp.Key).ToList();
            int maxOpponenentFromADivision = 2;
            var allOpponent = scenarioInstance.Opponents;
            int potClubsCount = scenarioInstance.Pots.First().ClubsInPot.Count();

            int clubsPerPot = scenarioInstance.Pots.First().ClubsInPot.Count();

            var countryClubStats = new Dictionary<Guid, int>();
            foreach (Club club in scenarioInstance.ClubsInScenarioInstance.Select(cisi => cisi.Club).ToList())
            {
                if (countryClubStats.ContainsKey(club.CountryId))
                    countryClubStats[club.CountryId] += 1;
                else
                    countryClubStats[club.CountryId] = 1;
            }
            int maxDivCount = countryClubStats.Max(kvp => kvp.Value);
            var maxDivisions = countryClubStats.Where(kvp => kvp.Value == maxDivCount).Select(kvp => kvp.Key).ToList();
            var firstClubs = scenarioInstance.ClubsInScenarioInstance.Where(cisi => maxDivisions.Contains(cisi.Club.CountryId)).Select(cisi => cisi.Club);
            var orderedFirstClubs = firstClubs.OrderByDescending(c => countryClubStats[c.CountryId]).ToList();
            var primaryStuckClubPots = new List<string>();
            //priority clubs fixture fixing
            for (int i = 0; i < orderedFirstClubs.Count; i++)
            {
                foreach (string secondPotname in potNames)
                {
                    int loopSafetyCounter = 0;
                    bool flowControl = false;
                    do
                    {
                        loopSafetyCounter++;

                        Club thisClub = orderedFirstClubs[i];
                        string clubPotName = GetClubPotName(thisClub.Id, scenarioInstance.Pots);
                        if (primaryStuckClubPots.Contains(GenerateClubPotName(thisClub.Id, secondPotname)))
                            continue;

                        int remainingOpponents = numberOfOpponentPerPot - FoundOpponentsInPot(secondPotname, thisClub.Id, fixedMatches);
                        if (remainingOpponents > 0)
                        {
                            var divisionAndFixtureFreeOpponents = allOpponent[thisClub.Id].First(p => p.Name == secondPotname).ClubsInPot.Select(cip => cip.Club).ToList();

                            if (divisionAndFixtureFreeOpponents.Count() == 0)
                                continue;

                            var priorityClubs = new List<Club>();
                            if (divisionAndFixtureFreeOpponents.Count() > 1)
                            {
                                priorityClubs = divisionAndFixtureFreeOpponents.Where(c => !ThisClubIsPlayingDivision(thisClub, c, fixedMatches, scenarioInstance.ClubsInScenarioInstance)).ToList();
                                if (priorityClubs.Count() == 0)
                                {
                                    var selectedClubCountries = fixedMatches[thisClub.Id].Select(str => GetClub(str, scenarioInstance.ClubsInScenarioInstance));
                                    var selectedClubCountryIds = selectedClubCountries.Select(c => c.CountryId);
                                    //clubs that thisClub is not already playing a club from their division
                                    priorityClubs = divisionAndFixtureFreeOpponents.Where(c => !selectedClubCountryIds.Contains(c.CountryId)).ToList();
                                    if (priorityClubs.Count() > 1)
                                    {
                                        //filter out possible opponents not already playing a club from thisClub's division
                                        var possibleOpponentsNotPlayingThisClubDivisiom = divisionAndFixtureFreeOpponents.Where(c => !(fixedMatches[c.Id].Select(str => GetClub(str, scenarioInstance.ClubsInScenarioInstance)).Any(c => c.CountryId == thisClub.CountryId))).ToList();
                                        if (possibleOpponentsNotPlayingThisClubDivisiom.Count() > 0)
                                            priorityClubs = possibleOpponentsNotPlayingThisClubDivisiom.ToList();
                                    }
                                }
                                priorityClubs = priorityClubs.OrderBy(c => FoundOpponentsInPot(clubPotName, c.Id, fixedMatches)).ThenBy(c => allOpponent[c.Id].First(p => p.Name == clubPotName).ClubsInPot.Count()).ThenByDescending(c => countryClubStats[c.CountryId]).Take(numberOfOpponentPerPot > 1 ? numberOfOpponentPerPot : 2).ToList();
                            }
                            if (priorityClubs.Count() == 0)
                                priorityClubs = divisionAndFixtureFreeOpponents.OrderBy(c => FoundOpponentsInPot(clubPotName, c.Id, fixedMatches)).ThenBy(c => allOpponent[c.Id].First(p => p.Name == clubPotName).ClubsInPot.Count()).Take(numberOfOpponentPerPot + 1).ToList();
                            if (priorityClubs.Count > 1)
                            {
                                int minFound = priorityClubs.Min(c => FoundOpponentsInPot(clubPotName, c.Id, fixedMatches));
                                priorityClubs = priorityClubs.Where(c => FoundOpponentsInPot(clubPotName, c.Id, fixedMatches) == minFound).ToList();
                            }

                            if (priorityClubs.Count + divisionAndFixtureFreeOpponents.Count == 0)
                                continue;
                            var opponentsForClub = new List<Club>();
                            bool opponentAllowed = false;
                            int cnt = 0;
                            int maxCount = divisionAndFixtureFreeOpponents.Count();
                            ModifiedDictionary<IEnumerable<Pot>>? temporaryAllOpponent = default;
                            Dictionary<Guid, List<string>> temporaryFixedMatches = default;
                            //int loopCounter = clubsPerPot * numberOfOpponentPerPot;
                            do
                            {
                                cnt++;
                                opponentsForClub = FindOpponents(/*remainingOpponents*/1, thisClub.Id, priorityClubs!, /*allPot!*/divisionAndFixtureFreeOpponents!);
                                if (opponentsForClub.Count() == 0)
                                    continue;
                                (opponentAllowed, temporaryFixedMatches, temporaryAllOpponent) = /*true;//*/IsFixtureAllowed(thisClub, clubPotName, opponentsForClub.First(), secondPotname, fixedMatches.ToDictionary(), scenarioInstance, numberOfOpponentPerPot, maxOpponenentFromADivision, allOpponent);
                                if (opponentsForClub.Count() == 0)
                                    continue;
                                if (!opponentAllowed)
                                {
                                    //removal of less than ideal club from possible choices
                                    if (priorityClubs.Count() > 0)
                                    {
                                        if (opponentsForClub.Count() == 0)
                                            continue;
                                        var opp = priorityClubs.First(c => c.Id == opponentsForClub.First().Id);
                                        priorityClubs.Remove(opp);
                                    }
                                    if (divisionAndFixtureFreeOpponents.Count() > 0)
                                    {
                                        if (opponentsForClub.Count() == 0)
                                            continue;
                                        var opp = divisionAndFixtureFreeOpponents.First(c => c.Id == opponentsForClub.First().Id);
                                        divisionAndFixtureFreeOpponents.Remove(opp);
                                    }
                                    if (divisionAndFixtureFreeOpponents.Count() == 0)
                                        primaryStuckClubPots.Add(GenerateClubPotName(thisClub.Id, secondPotname));
                                }

                            } while (!opponentAllowed && priorityClubs.Count + divisionAndFixtureFreeOpponents.Count > 0 && cnt < maxCount);
                            if (!opponentAllowed)
                                continue;
                            //if opponent is playeable without blocking others
                            if (opponentsForClub != null)
                            {
                                (flowControl, fixedMatches, allOpponent) = (true, temporaryFixedMatches, temporaryAllOpponent);
                            }
                        }
                    }
                    while (loopSafetyCounter < clubsPerPot/* * numberOfOpponentPerPot*/ && flowControl);

                }
            }
            //priority clubs swap fixture fixing
            var firstClubsId = firstClubs.Select(c=>c.Id).ToList();
            if (fixedMatches.Any(kvp =>firstClubsId.Contains(kvp.Key) && kvp.Value.Count() != expectedMatchCount))
            {
                foreach (var kvp in fixedMatches.Where(kvp => firstClubsId.Contains(kvp.Key) && kvp.Value.Count() < expectedMatchCount).OrderByDescending(kvp => kvp.Value.Count()))
                {
                    if (kvp.Value.Count >= expectedMatchCount)
                        continue;
                    Club thisClub = scenarioInstance.ClubsInScenarioInstance.FirstOrDefault(c => c.ClubId == kvp.Key)?.Club!;
                    string clubPotName = GetClubPotName(thisClub.Id, scenarioInstance.Pots);
                    foreach (string opponentPotname in potNames)
                    {
                        int clubPotFixtureCount = ClubPotFixtureCount(kvp.Key, opponentPotname, fixedMatches);
                        if (clubPotFixtureCount >= numberOfOpponentPerPot)
                            continue;
                        int remainingOpponents = numberOfOpponentPerPot - clubPotFixtureCount;
                        for (int i = 0; i < remainingOpponents; i++)
                        {
                            //for thisClub not to be fully matchedup at this point, all ideal opponents must have been fixed up except a club, selectedClubInOpponentPotWithIncompletePotFixtures = sCIOPWIPF that should have played thisClub but couldn't and that club will have an incomplete fixture too
                            var clubsInOpponentPot = scenarioInstance.Pots.First(p => p.Name == opponentPotname).ClubsInPot.Select(cip => cip.Club).ToList();
                            if (clubPotName == opponentPotname)
                            {
                                var this_Club = clubsInOpponentPot.First(c => c.Id == thisClub.Id);
                                clubsInOpponentPot.Remove(this_Club);
                            }
                            var clubsInOpponentPotWithIncompletePotFixtures = clubsInOpponentPot.Where(c => !ClubPotFixtureFull(c.Id, clubPotName, fixedMatches, numberOfOpponentPerPot)).ToList();
                            if (clubsInOpponentPotWithIncompletePotFixtures.Count() == 0 && clubPotName == opponentPotname)
                            {
                                //club is unable to find matchUp for itself because all other clubs are fully matched up
                                if (remainingOpponents % 2 == 1)
                                    continue;
                                i++;
                            }
                            //selectedClubInOpponentPotWithIncompletePotFixtures = sCIOPWIPF
                            var sCIOPWIPF = clubsInOpponentPotWithIncompletePotFixtures.Count() == 0 && clubPotName == opponentPotname && remainingOpponents % 2 == 0 ? thisClub : FindOpponents(1, thisClub.Id, clubsInOpponentPotWithIncompletePotFixtures, clubsInOpponentPotWithIncompletePotFixtures).First();      //from opponentPotname

                            //this implementation was added because some earlier swap migth have made direct matching possible and maybe necessary.
                            var freeAndPlayableOpponents = clubsInOpponentPotWithIncompletePotFixtures.Where(c => /*c.CountryId != thisClub.CountryId &&*/ !ClubHasFixtureAgainst(c.Id, thisClub.Id, fixedMatches) && ThisClubCanPlayCountryClub(c, thisClub, maxOpponenentFromADivision, fixedMatches, scenarioInstance.ClubsInScenarioInstance)).ToList();
                            if (freeAndPlayableOpponents.Count > 0)
                            {
                                var selectedFAPO = FindOpponents(1, thisClub.Id, freeAndPlayableOpponents.Where(c =>c.CountryId != thisClub.CountryId).ToList(), freeAndPlayableOpponents).First();
                                fixedMatches[thisClub.Id].Add(GenerateClubPotName(selectedFAPO.Id, opponentPotname));
                                fixedMatches[selectedFAPO.Id].Add(GenerateClubPotName(thisClub.Id, clubPotName));
                                continue;
                            }

                            var potentialThisClubOpponents = clubsInOpponentPot.Where(c => /*thisClub.CountryId != c.CountryId &&*/ !ClubHasFixtureAgainst(thisClub.Id, c.Id, fixedMatches) && ThisClubCanPlayCountryClub(thisClub, c, maxOpponenentFromADivision, fixedMatches, scenarioInstance.ClubsInScenarioInstance));
                            var clubPotOpponent = new Dictionary<Guid, List<Club>>();
                            foreach (Club club in potentialThisClubOpponents)
                            {
                                clubPotOpponent[club.Id] = new List<Club>();
                                var clubPotName_Club_Clubs = fixedMatches[club.Id].Where(str => str.Contains(clubPotName)).Select(str => GetClub(str, scenarioInstance.ClubsInScenarioInstance)).ToList();  //all club fixture from clubPotName
                                foreach (var itemClub in clubPotName_Club_Clubs)
                                {
                                    if (!ClubHasFixtureAgainst(itemClub.Id, sCIOPWIPF.Id, fixedMatches) /*&& itemClub.CountryId != sCIOPWIPF.CountryId*/ && ThisClubCanPlayCountryClub(itemClub, sCIOPWIPF, maxOpponenentFromADivision, fixedMatches, scenarioInstance.ClubsInScenarioInstance))
                                        clubPotOpponent[club.Id].Add(itemClub);
                                }
                            }
                            var acceptableThisClubOpponents = clubPotOpponent.Where(kvp => kvp.Value.Count() > 0).Select(kvp => scenarioInstance.ClubsInScenarioInstance.First(cisi => cisi.ClubId == kvp.Key).Club).ToList();
                            if (acceptableThisClubOpponents.Count() == 0)
                                continue;
                            var selectedThisClubOpponent = FindOpponents(1, thisClub.Id, acceptableThisClubOpponents.Where(c => c.CountryId != thisClub.CountryId).ToList(), acceptableThisClubOpponents).First();      //from opponentPotname
                            var acceptablesCIOPWIPFClubOpponents = clubPotOpponent[selectedThisClubOpponent.Id];
                            if (thisClub.Id == sCIOPWIPF.Id)
                                acceptablesCIOPWIPFClubOpponents = acceptablesCIOPWIPFClubOpponents.Where(c => c.Id != selectedThisClubOpponent.Id).ToList();
                            //the condition above is intended for when a club cant match because other clubs have fully matched it up. Check var sCIOPWIPF = clubsInOpponentPotWithIncomple... 
                            if (acceptablesCIOPWIPFClubOpponents.Count() == 0)
                                continue;
                            var selectedsCIOPWIPFClubOpponent = FindOpponents(1, sCIOPWIPF.Id, acceptablesCIOPWIPFClubOpponents.Where(c => c.CountryId!= sCIOPWIPF.CountryId).ToList(), acceptablesCIOPWIPFClubOpponents).First();    // from clubPotName
                                                                                                                                                                               //selectedThisClubOpponent and selectedsCIOPWIPFClubOpponents are two clubs playing each other that can be rematched with sCIOPWIPF and thisClub after their mutual fixrture has been canceled
                            string str1 = fixedMatches[selectedThisClubOpponent.Id].First(str => str.Contains(selectedsCIOPWIPFClubOpponent.Id.ToString()));
                            fixedMatches[selectedThisClubOpponent.Id].Remove(str1);
                            string str2 = fixedMatches[selectedsCIOPWIPFClubOpponent.Id].First(str => str.Contains(selectedThisClubOpponent.Id.ToString()));
                            fixedMatches[selectedsCIOPWIPFClubOpponent.Id].Remove(str2);
                            List<string> fgh = new List<string>();
                            fixedMatches[thisClub.Id].Add(GenerateClubPotName(selectedThisClubOpponent.Id, opponentPotname));
                            fixedMatches[selectedThisClubOpponent.Id].Add(GenerateClubPotName(thisClub.Id, clubPotName));
                            fixedMatches[sCIOPWIPF.Id].Add(GenerateClubPotName(selectedsCIOPWIPFClubOpponent.Id, clubPotName));
                            fixedMatches[selectedsCIOPWIPFClubOpponent.Id].Add(GenerateClubPotName(sCIOPWIPF.Id, opponentPotname));
                        }
                    }
                }
            }
            //other clubs fixture fixing
            for (int currentNnumberOfOpponentPerPot = numberOfOpponentPerPot; currentNnumberOfOpponentPerPot <= numberOfOpponentPerPot; currentNnumberOfOpponentPerPot++)
            {
                List<string> stuckClubPots = new List<string>();

                foreach (string potname in potNames)
                {
                    var firstPot = scenarioInstance.Pots.First(p => p.Name == potname);
                    var firstPotCLubs = firstPot.ClubsInPot.Select(cip => cip.Club).ToList();
                    foreach (string secondPotname in potNames)
                    {
                        if (potNames.IndexOf(potname) > potNames.IndexOf(secondPotname))
                            continue;
                        int loopSafetyCounter = 0;
                        bool flowControl = false;
                        do
                        {
                            loopSafetyCounter++;
                            var orderedFirstPot = firstPotCLubs.Where(c => (FoundOpponentsInPot(secondPotname, c.Id, fixedMatches) < currentNnumberOfOpponentPerPot) && !stuckClubPots.Contains(GenerateClubPotName(c.Id, secondPotname))).OrderByDescending(c => countryClubStats[c.CountryId]).ThenBy(c => FoundOpponentsInPot(secondPotname, c.Id, fixedMatches)).ThenBy(c => allOpponent[c.Id].First(p => p.Name == secondPotname).ClubsInPot.Count()).ToList();

                            if (orderedFirstPot.Count() == 0)
                                break;

                            Club thisClub = orderedFirstPot[0];
                            string clubPotName = potname;
                            if (stuckClubPots.Contains(GenerateClubPotName(thisClub.Id, secondPotname)))
                                continue;

                            int remainingOpponents = currentNnumberOfOpponentPerPot - FoundOpponentsInPot(secondPotname, thisClub.Id, fixedMatches);
                            if (remainingOpponents > 0)
                            {
                                var divisionAndFixtureFreeOpponents = allOpponent[thisClub.Id].First(p => p.Name == secondPotname).ClubsInPot.Select(cip => cip.Club).ToList();

                                if (divisionAndFixtureFreeOpponents.Count() == 0)
                                    continue;

                                var priorityClubs = new List<Club>();
                                if (divisionAndFixtureFreeOpponents.Count() > 1)
                                {
                                    priorityClubs = divisionAndFixtureFreeOpponents.Where(c => !ThisClubIsPlayingDivision(thisClub, c, fixedMatches, scenarioInstance.ClubsInScenarioInstance)).ToList();
                                    if (priorityClubs.Count() == 0)
                                    {
                                        var selectedClubCountries = fixedMatches[thisClub.Id].Select(str => GetClub(str, scenarioInstance.ClubsInScenarioInstance));
                                        var selectedClubCountryIds = selectedClubCountries.Select(c => c.CountryId);
                                        //clubs that thisClub is not already playing a club from their division
                                        priorityClubs = divisionAndFixtureFreeOpponents.Where(c => !selectedClubCountryIds.Contains(c.CountryId)).ToList();
                                        if (priorityClubs.Count() > 1)
                                        {
                                            //filter out possible opponents not already playing a club from thisClub's division
                                            var possibleOpponentsNotPlayingThisClubDivisiom = divisionAndFixtureFreeOpponents.Where(c => !(fixedMatches[c.Id].Select(str => GetClub(str, scenarioInstance.ClubsInScenarioInstance)).Any(c => c.CountryId == thisClub.CountryId))).ToList();
                                            if (possibleOpponentsNotPlayingThisClubDivisiom.Count() > 0)
                                                priorityClubs = possibleOpponentsNotPlayingThisClubDivisiom.ToList();
                                            if (priorityClubs.Count() > 0)
                                                stuckClubPots.Any();
                                        }
                                    }
                                    priorityClubs = priorityClubs.OrderBy(c => FoundOpponentsInPot(potname, c.Id, fixedMatches)).ThenBy(c => allOpponent[c.Id].First(p => p.Name == potname).ClubsInPot.Count()).ThenByDescending(c => countryClubStats[c.CountryId]).Take(currentNnumberOfOpponentPerPot > 1 ? currentNnumberOfOpponentPerPot : 2).ToList();
                                }
                                if (priorityClubs.Count() == 0)
                                    priorityClubs = divisionAndFixtureFreeOpponents.OrderBy(c => FoundOpponentsInPot(potname, c.Id, fixedMatches)).ThenBy(c => allOpponent[c.Id].First(p => p.Name == potname).ClubsInPot.Count()).Take(currentNnumberOfOpponentPerPot + 1).ToList();

                                if (priorityClubs.Count > 1)
                                {
                                    int minFound = priorityClubs.Min(c => FoundOpponentsInPot(clubPotName, c.Id, fixedMatches));
                                    priorityClubs = priorityClubs.Where(c => FoundOpponentsInPot(clubPotName, c.Id, fixedMatches) == minFound).ToList();
                                }
                                if (priorityClubs.Count + divisionAndFixtureFreeOpponents.Count == 0)
                                    continue;
                                var opponentsForClub = new List<Club>();
                                bool opponentAllowed = false;
                                int cnt = 0;
                                int maxCount = divisionAndFixtureFreeOpponents.Count();
                                ModifiedDictionary<IEnumerable<Pot>>? temporaryAllOpponent = default;
                                Dictionary<Guid, List<string>> temporaryFixedMatches = default;
                                //int loopCounter = clubsPerPot * numberOfOpponentPerPot;
                                do
                                {
                                    cnt++;
                                    opponentsForClub = FindOpponents(/*remainingOpponents*/1, thisClub.Id, priorityClubs!, /*allPot!*/divisionAndFixtureFreeOpponents!);
                                    if (opponentsForClub.Count() == 0)
                                        continue;
                                    (opponentAllowed, temporaryFixedMatches, temporaryAllOpponent) = /*true;//*/IsFixtureAllowed(thisClub, clubPotName, opponentsForClub.First(), secondPotname, fixedMatches.ToDictionary(), scenarioInstance, currentNnumberOfOpponentPerPot, maxOpponenentFromADivision, allOpponent);
                                    if (opponentsForClub.Count() == 0)
                                        continue;
                                    if (!opponentAllowed)
                                    {
                                        //removal of less than ideal club from possible choices
                                        if (priorityClubs.Count() > 0)
                                        {
                                            if (opponentsForClub.Count() == 0)
                                                continue;
                                            var opp = priorityClubs.First(c => c.Id == opponentsForClub.First().Id);
                                            priorityClubs.Remove(opp);
                                        }
                                        if (divisionAndFixtureFreeOpponents.Count() > 0)
                                        {
                                            if (opponentsForClub.Count() == 0)
                                                continue;
                                            var opp = divisionAndFixtureFreeOpponents.First(c => c.Id == opponentsForClub.First().Id);
                                            divisionAndFixtureFreeOpponents.Remove(opp);
                                        }
                                        if (divisionAndFixtureFreeOpponents.Count() == 0)
                                            stuckClubPots.Add(GenerateClubPotName(thisClub.Id, secondPotname));
                                    }

                                } while (!opponentAllowed && priorityClubs.Count + divisionAndFixtureFreeOpponents.Count > 0 && cnt < maxCount);
                                if (!opponentAllowed)
                                    continue;
                                //if opponent is playeable without blocking others
                                if (opponentsForClub != null)
                                {
                                    (flowControl, fixedMatches, allOpponent) = (true, temporaryFixedMatches, temporaryAllOpponent);
                                }
                            }
                        }
                        while (loopSafetyCounter < clubsPerPot/* * numberOfOpponentPerPot*/ && flowControl);

                    }
                }
            }
            //other clubs swap fixture fixing
            for (int i = 0; i < 3; i++)
            {
                scenarioInstance.Opponents.RePopulate(GenerateOpponentsForAllClubs(scenarioInstance));
                if (fixedMatches.Any(kvp => kvp.Value.Count() != expectedMatchCount))
                {
                    fixedMatches = SwapFixtureFixing(scenarioInstance, numberOfOpponentPerPot, fixedMatches, expectedMatchCount, potNames, maxOpponenentFromADivision);
                }
                else
                    break;
            }
            fixedMatchesFull = GenerateFixedMatchesFull(fixedMatches, scenarioInstance.ClubsInScenarioInstance);
            if (!scenarioInstance.Scenario.HomeAndAwayPerOpponent)
            {
                int minHomeMatchCount = scenarioInstance.Scenario.NumberOfGamesPerPot / 2;
                int maxHomeMatchCount = minHomeMatchCount + scenarioInstance.Scenario.NumberOfGamesPerPot % 2;

                if (numberOfOpponentPerPot % 2 != 1)
                {
                    foreach (string potname in potNames)
                    {
                        var firstPot = scenarioInstance.Pots.First(p => p.Name == potname);
                        var firstPotCLubs = firstPot.ClubsInPot.Select(cip => cip.Club).ToList();
                        foreach (string secondPotname in potNames)
                        {
                            if (potNames.IndexOf(potname) > potNames.IndexOf(secondPotname))
                                continue;
                            int loopSafetyCounter = 0;
                            bool flowControl = false;
                            var stuckClub_Pots = new List<string>();
                            Club nextThisClub = null;
                            do
                            {
                                loopSafetyCounter++;
                                var orderedFirstPot = firstPotCLubs.Where(c => fixedMatches[c.Id].Any(str => str.Contains(secondPotname) && str.Split(separator).Length < 3) && !stuckClub_Pots.Contains(GenerateClubPotName(c.Id, secondPotname))).OrderByDescending(c => fixedMatches[c.Id].Where(str => str.Contains(secondPotname) && str.Split(separator).Length == 3).ToList().Count).ToList();

                                if (orderedFirstPot.Count() == 0)
                                    break;

                                Club thisClub = orderedFirstPot[0];

                                if (nextThisClub != null)
                                    thisClub = nextThisClub;

                                var clubPotFixtures = fixedMatches[thisClub.Id].Where(str => str.Contains(secondPotname)).ToList();

                                int existingHomeFixture = clubPotFixtures.Where(str => str.EndsWith(HomeAwayString(true))).ToList().Count;
                                int existingAwayFixture = clubPotFixtures.Where(str => str.EndsWith(HomeAwayString(false))).ToList().Count;

                                bool prioritizeHome = maxHomeMatchCount == minHomeMatchCount ? existingHomeFixture <= existingAwayFixture : existingHomeFixture <= existingAwayFixture && fixedMatches[thisClub.Id].Where(fix => fix.EndsWith(HomeAwayString(true))).ToList().Count() <= fixedMatches[thisClub.Id].Where(fix => fix.EndsWith(HomeAwayString(false))).ToList().Count();

                                int remainingHomeFixtures = (prioritizeHome ? maxHomeMatchCount : minHomeMatchCount) - existingHomeFixture;
                                int remainingAwayFixtures = (!prioritizeHome ? maxHomeMatchCount : minHomeMatchCount) - existingAwayFixture;

                                int priorityRemainingFixtures = prioritizeHome ? remainingHomeFixtures : remainingAwayFixtures;

                                var possibleHomeFixtures = clubPotFixtures.Where(fix => fix.Split(separator).Length < 3 && fixedMatches[ExtractClubId_Club_PotName(fix)].Where(oppFix => oppFix.Contains(potname) && oppFix.EndsWith(HomeAwayString(false))).ToList().Count < maxHomeMatchCount).OrderBy(fix => fixedMatches[ExtractClubId_Club_PotName(fix)].Where(f => f.Contains(potname) && f.Split(separator).Length < 3).Count()).ToList();
                                if (/*false &&*/ possibleHomeFixtures.Count > 1)
                                {
                                    int minPHFCount = possibleHomeFixtures.Max(fix => fixedMatches[ExtractClubId_Club_PotName(fix)].Where(f => f.Contains(potname) && f.Split(separator).Length < 3).Count());
                                    possibleHomeFixtures = possibleHomeFixtures.Where(fix => fixedMatches[ExtractClubId_Club_PotName(fix)].Where(f => f.Contains(potname) && f.Split(separator).Length < 3).Count() == minPHFCount).ToList();
                                }

                                var possibleAwayFixtures = clubPotFixtures.Where(fix => fix.Split(separator).Length < 3 && fixedMatches[ExtractClubId_Club_PotName(fix)].Where(oppFix => oppFix.Contains(potname) && oppFix.EndsWith(HomeAwayString(true))).ToList().Count < maxHomeMatchCount).OrderBy(fix => fixedMatches[ExtractClubId_Club_PotName(fix)].Where(f => f.Contains(potname) && f.Split(separator).Length < 3).Count()).ToList();
                                if (/*false &&*/ possibleAwayFixtures.Count > 1)
                                {
                                    int minPAFCount = possibleAwayFixtures.Max(fix => fixedMatches[ExtractClubId_Club_PotName(fix)].Where(f => f.Contains(potname) && f.Split(separator).Length < 3).Count());
                                    possibleAwayFixtures = possibleAwayFixtures.Where(fix => fixedMatches[ExtractClubId_Club_PotName(fix)].Where(f => f.Contains(potname) && f.Split(separator).Length < 3).Count() == minPAFCount).ToList();
                                }
                                //.OrderBy(fix => fix.Where(oppFix => fixedMatches[ExtractClubId_Club_PotName(oppFix)].Where(str =>))
                                var priorityPossibleFixtures = prioritizeHome ? possibleHomeFixtures : possibleAwayFixtures;

                                if (/*remainingHomeFixtures < 1*/ priorityRemainingFixtures < 1 || /*possibleHomeFixtures.Count() < 1*/ priorityPossibleFixtures.Count() < 1)
                                {
                                    var ee = fixedMatches[thisClub.Id];
                                    stuckClub_Pots.Add(GenerateClubPotName(thisClub.Id, secondPotname));
                                    continue;
                                }
                                //var selectedHomeOpp = possibleHomeFixtures[GetRandomIndex(possibleHomeFixtures.Count)];
                                var prioritySelectedOpp = priorityPossibleFixtures[GetRandomIndex(priorityPossibleFixtures.Count)];
                                var prioritySelectedOppId = ExtractClubId_Club_PotName(/*selectedHomeOpp*/prioritySelectedOpp);

                                UpdateLocationOnly(thisClub.Id, fixedMatches, prioritySelectedOppId, prioritizeHome);
                                //decide location in a chain order
                                if (potname != secondPotname)
                                {
                                    var possibleNextThisClubs = fixedMatches[prioritySelectedOppId].Where(fix => fix.Contains(potname) && fix.Split(separator).Length < 3).OrderByDescending(fix => fixedMatches[ExtractClubId_Club_PotName(fix)].Where(f => f.Contains(secondPotname) && f.Split(separator).Length < 3).Count()).ToList();
                                    if (possibleNextThisClubs.Count > 0)
                                    {
                                        nextThisClub = GetClub(possibleNextThisClubs[0], scenarioInstance.ClubsInScenarioInstance);
                                        continue;
                                    }
                                }
                                nextThisClub = null;
                            }
                            while (loopSafetyCounter < numberOfOpponentPerPot * clubsPerPot/* && flowControl*/);
                        }
                    }
                }
                else
                {
                    int matchCount = 0;
                    var maxMatchCount = expectedMatchCount * scenarioInstance.ClubsInScenarioInstance.Count();
                    var stuckClub_Pots = new List<string>();
                    Dictionary<Guid, List<string>> fixedMatchesSorted = new Dictionary<Guid, List<string>>();
                    foreach (var kvp in fixedMatches)
                    {
                        fixedMatchesSorted[kvp.Key] = kvp.Value.OrderBy(fix => fix.Split(separator)[1]).ToList();
                    }
                    do
                    {
                        matchCount++;
                        var orderedFirstPot = fixedMatchesSorted.Where(kvp => kvp.Value.Where(fix => fix.Split(separator).Count() == 3).Count() < expectedMatchCount).OrderByDescending(kvp => kvp.Value.Where(fix => fix.Split(separator).Length == 3).Count()).ThenByDescending(kvp => Math.Max(kvp.Value.Where(fix => fix.EndsWith(HomeAwayString(true))).ToList().Count, kvp.Value.Where(fix => fix.EndsWith(HomeAwayString(false))).ToList().Count)).ThenBy(fix => GetClubPotName(fix.Key, scenarioInstance.Pots)).ToList();

                        if (orderedFirstPot.Count() == 0)
                            break;

                        Club thisClub = GetClub(orderedFirstPot[0].Key.ToString(), scenarioInstance.ClubsInScenarioInstance);

                        //foreach (var potname in potNames)
                        if (true)
                        {
                            var clubPotFixtures = fixedMatchesSorted[thisClub.Id]/*.Where(fix => fix.Split(separator)[1] == potname)*/.ToList();
                            var clubPotFreeFixtures = clubPotFixtures.Where(fix => fix.Split(separator).Length != 3).ToList();
                            int freePotFixture = clubPotFreeFixtures.Count();
                            if (freePotFixture == 0)
                                continue;

                            int existingHomeFixture = clubPotFixtures.Where(str => str.EndsWith(HomeAwayString(true))).ToList().Count;
                            int existingAwayFixture = clubPotFixtures.Where(str => str.EndsWith(HomeAwayString(false))).ToList().Count;

                            bool prioritizeHome = existingHomeFixture != existingAwayFixture ? existingHomeFixture < existingAwayFixture : fixedMatchesSorted[thisClub.Id].Where(fix => fix.EndsWith(HomeAwayString(true))).ToList().Count() != fixedMatchesSorted[thisClub.Id].Where(fix => fix.EndsWith(HomeAwayString(false))).ToList().Count() ? /*existingHomeFixture < existingAwayFixture ||*/ fixedMatchesSorted[thisClub.Id].Where(fix => fix.EndsWith(HomeAwayString(true))).ToList().Count() < fixedMatchesSorted[thisClub.Id].Where(fix => fix.EndsWith(HomeAwayString(false))).ToList().Count() : GetAlternatingBool;

                            int remainingHomeFixtures = (prioritizeHome ? maxHomeMatchCount : minHomeMatchCount) - existingHomeFixture;
                            int remainingAwayFixtures = (!prioritizeHome ? maxHomeMatchCount : minHomeMatchCount) - existingAwayFixture;

                            int priorityRemainingFixtures = prioritizeHome ? remainingHomeFixtures : remainingAwayFixtures;

                            var prioritySelectedOpp = clubPotFreeFixtures[0];
                            var prioritySelectedOppId = ExtractClubId_Club_PotName(/*selectedHomeOpp*/prioritySelectedOpp);

                            UpdateLocationOnly(thisClub.Id, fixedMatchesSorted, prioritySelectedOppId, prioritizeHome);
                        }
                    }
                    while (matchCount < maxMatchCount && fixedMatchesSorted.Where(kvp => kvp.Value.Any(fix => fix.Split(separator).Length < 3)).Count() > 0);
                    var incomp = fixedMatchesSorted.Where(kvp => kvp.Value.Any(fix => fix.Split(separator).Length < 3)).ToList();
                    if (incomp.Count > 0)
                    {

                    }
                    else
                    {
                        fixedMatches = fixedMatchesSorted;
                    }
                }
            }
            return (fixedMatchesFull, fixedMatches);
        }
        private void UpdateLocationOnly(Guid clubId, Dictionary<Guid, List<string>> fixedMatches, Guid selectedOpponent, bool prioritizeHome)
        {
            var primaryFixture = fixedMatches[clubId].First(fix => fix.Contains(selectedOpponent.ToString()));
            var primaryFixtureIndex = fixedMatches[clubId].IndexOf(primaryFixture);
            fixedMatches[clubId][primaryFixtureIndex] = $"{primaryFixture}{HomeAwayString(prioritizeHome)}";

            var secondaryFixture = fixedMatches[selectedOpponent].First(fix => fix.Contains(clubId.ToString()));
            var secondaryFixtureIndex = fixedMatches[selectedOpponent].IndexOf(secondaryFixture);
            fixedMatches[selectedOpponent][secondaryFixtureIndex] = $"{secondaryFixture}{HomeAwayString(!prioritizeHome)}";
        }
        private Random random = new Random();
        private int GetRandomIndex(int max) => random.Next(max);
        #endregion

    }
}
