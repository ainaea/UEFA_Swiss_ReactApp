import { Pencil } from 'react-bootstrap-icons'
import { edit } from './fields';
function Country({ country, navigator }) {
    function editCountry() {
        edit.country = country;
        navigator('/edit-country');
    }
    return (
        <div className="bg-light col-md-3 bg-light border p-3 rounded-pill d-flex align-items-center">
            <h4 className="col-md-8">{country.name}</h4>
            <h4 className="col-md-2">({country.abbrevation})</h4>
            <Pencil className="btn col-md-2 fs-4" onClick={editCountry} size={55} color="blue" />
        </div>      
  );
}

export default Country;