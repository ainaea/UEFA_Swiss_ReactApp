import { Pencil } from 'react-bootstrap-icons'
import { edit } from './fields';

function Scenario({ scenario, navigator }) {
    function editScenario() {
        edit.scenario = scenario;
        navigator('/edit-scenario');
    }

  return (
      <div className="row bg-light col-md-4 border rounded-pill d-flex p-0 m-0">
          <div className="align-items-center col-md-12 row p-0 m-0">
              <div className="col-md-10 m-0 row justify-content-center">
                  <h4 className="col-sm-12 text-center">{scenario.name}</h4>
                  <h6 className="col-sm-6">Pots: {scenario.numberOfPot}</h6>
                  <h6 className="col-sm-6">Teams per pot: {scenario.numberOfTeamsPerPot}</h6>
                  <h6 className="col-sm-6">Games per pot: {scenario.numberOfGamesPerPot}</h6>
                  <h6 className="col-sm-6">Home & Away: {scenario.homeAndAwayPerOpponent ? "Yes" : "No"}</h6>
              </div>
              <Pencil className="btn col-md-2 fs-4 m-0" onClick={editScenario} size={55} color="blue" />
          </div>  
          <div className="col-md-12 row" >
              <div className="col-md-3"/>
              <h5 className="col-md-5 btn btn-primary border text-center">Run simulation</h5>
          </div>
      </div>     
  );
}

export default Scenario;