import Country from './Country';
import { Plus } from 'react-bootstrap-icons';
//import { useNavigate } from 'react-router-dom';
function AllCountries({ countries, navigator }) {
    //const navigate = useNavigate();
    function addCountry() {
        navigator('/add-country');
    }
    return (
      <div className="bg-secondary pt-2 row">
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