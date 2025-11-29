import Country from './Country';
import { Plus } from 'react-bootstrap-icons';
function AllCountries({ countries, navigator }) {
    function addCountry() {
        navigator('/add-country');
    }
    return (countries == null ? <h3>No Country to display at the moment</h3> :
        <div className="bg-secondary pt-2 pb-2 row">
            {countries.map(c => <Country key={c.id} country={c} navigator={navigator }  />)}
            <div className="col-md-12">
            </div>
            <div className="col-md-11">
            </div>
            <div className="col-md-1 bg-light border rounded-pill d-flex justify-content-center ml-2">
                <Plus className="btn" onClick={addCountry} size={ 65} color="blue" />
            </div>
      </div>
  );
}

export default AllCountries;