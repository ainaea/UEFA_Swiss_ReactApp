import { Pencil } from 'react-bootstrap-icons'
import { edit } from './fields';

function Scenario({ scenario, navigator }) {
    function editScenario() {
        edit.scenario = scenario;
        navigator('/edit-scenario');
    }

  return (
      <div className="bg-light col-md-4 bg-light border rounded-pill d-flex align-items-center">
          <div className="col-md-10 m-1 row justify-content-center">
              <h5 className="col-sm-12">{scenario.name}</h5>
              <h6 className="col-sm-6">Pots: {scenario.numberOfPot}</h6>
              <h6 className="col-sm-6">Teams per pot: {scenario.numberOfTeamsPerPot}</h6>
              <h6 className="col-sm-6">Games per pot: {scenario.numberOfGamesPerPot}</h6>
              <h6 className="col-sm-6">Home and Away: {scenario.homeAndAwayPerOpponent? "Yes": "No"}</h6>
          </div>          
          <Pencil className="btn col-md-2 fs-4 m-0" onClick={editScenario} size={55} color="blue" />
      </div>     
  );
}

export default Scenario;