import { Eye } from 'react-bootstrap-icons'
import { edit } from './fields';
function SimulationCard({ simulation, navigator }) {
    function viewDetail() {
        edit.simulation = simulation;
        navigator('/view-simulation')
    };
    return (
      <div className="bg-light col-md-3 bg-light border p-2 rounded-pill d-flex align-items-center">
            <div className="col-md-9">
                <h5>{simulation.name}</h5>
                <h6>{simulation.scenarioName}</h6>
            </div>
            <Eye className="btn col-md-3" onClick={viewDetail} size={40} color="blue" />
        </div>
  );
}

export default SimulationCard;