import { Pencil } from 'react-bootstrap-icons'
import { edit } from './fields';

function Club({ club, navigator }) {
    function editClub() {
        edit.club = club;
        navigator('/edit-club')
    };

    return (
        <div className="bg-light col-md-3 bg-light border p-3 rounded-pill d-flex align-items-center">
            <div className="col-md-10">
                <h5>{club.name}</h5>
                <h6>{club.country.name} ({club.country.abbrevation})</h6>
            </div>
            <Pencil className="btn col-md-2 fs-4" onClick={editClub} size={55} color="blue" />
        </div>
    )
}

export default Club;