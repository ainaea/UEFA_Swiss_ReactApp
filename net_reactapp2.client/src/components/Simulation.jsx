import { edit } from './fields';
import PotTeams from './PotTeams'

function Simulation() {
    const simulation = edit.simulation;
    return (
      
        <div>
            <h4>{simulation.name} of { simulation.scenarioName}</h4>
            {simulation.potsFixtures.map((potfix) =>
                <div key={potfix.potId } className="rankableCollectionDiv">
                    <h5>{potfix.potName}</h5>
                    <PotTeams clubsFixtures={potfix.clubsFixtures} />
                </div>
            )}
            
        </div>
  );
}

export default Simulation;