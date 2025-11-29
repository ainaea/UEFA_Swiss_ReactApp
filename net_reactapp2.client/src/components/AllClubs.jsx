import Club from './Club';
import { Plus } from 'react-bootstrap-icons';

function AllClubs({ clubs, navigator }) {
    function addClub() {
        navigator('/add-club');
    }
    
    return ( clubs == null? <h3>No Club to display at the moment</h3> :
        <div className="bg-secondary pt-2 pb-2 row">
            {clubs.map(c => <Club key={c.id} club={c} navigator={navigator} />)}
            <div className="col-md-12">
            </div>
            <div className="col-md-11">
            </div>
            <div className="col-md-1 bg-light border rounded-pill d-flex justify-content-center ml-2">
                <Plus className="btn" onClick={addClub} size={65} color="blue" />
            </div>
        </div>
    )
}

export default AllClubs;