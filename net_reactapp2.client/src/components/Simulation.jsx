import Club from './Club';
import { edit } from './fields';
import PotTeams from './PotTeams'

function Simulation({ navigator }) {
    const simulation = edit.simulation;
    const clubs_Ranking = [];
    const pots = simulation.potsFixtures.length;
    const clubsPerPot = simulation.potsFixtures[0].clubsFixtures.length;
    function resimulate() {
        simulation.potsFixtures.map((pf, i) => pf.clubsFixtures
            .map((cf, j) => clubs_Ranking.push({ id: cf.mainClub.id, rank: clubsPerPot * (pots -1-i)+(clubsPerPot -j) })));
        console.log(simulation);
        edit.simulationClubs_Ranking = clubs_Ranking;
        edit.scenario = { numberOfPot: pots, numberOfTeamsPerPot: clubsPerPot, id: simulation.scenarioId, name: simulation.scenarioName };
        
        navigator('/resimulate');
    }
    return (
      
        <div>
            <h4>{simulation.name} of { simulation.scenarioName}</h4>
            {simulation.potsFixtures.map((potfix) =>
                <div key={potfix.potId } className="rankableCollectionDiv">
                    <h5>{potfix.potName}</h5>
                    <PotTeams clubsFixtures={potfix.clubsFixtures} />
                </div>
            )}
            <button className="btn btn-primary mx-auto d-block m-2" onClick={resimulate }>Resimulate</button>
        </div>
  );
}

export default Simulation;