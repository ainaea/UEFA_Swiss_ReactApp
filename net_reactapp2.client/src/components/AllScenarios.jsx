import Scenario from './Scenario';
import { Plus } from 'react-bootstrap-icons';

function AllScenarios({ scenarios, navigator }) {
    function addScenario() {
        navigator('/add-scenario');
    }
    return (scenarios == null ? <h3>No Scenario to display at the moment</h3> :
        <div className="bg-secondary pt-2 pb-2 row">
            {scenarios.map(sc => <Scenario key={sc.id} scenario={sc} navigator={navigator} />)}
            <div className="col-md-12">
            </div>
            <div className="col-md-11">
            </div>
            <div className="col-md-1 bg-light border rounded-pill d-flex justify-content-center ml-2">
                <Plus className="btn" onClick={addScenario} size={65} color="blue" />
            </div>
        </div>
    );
}

export default AllScenarios;