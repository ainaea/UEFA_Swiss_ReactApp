//import './App.css'
import { useState, useEffect } from 'react'; 
import Layout from './components/Layout';
import { Route } from 'react-router';
import SimpleTest from './components/SimpleTest';
import 'bootstrap/dist/css/bootstrap.css';
import AllCountries from './components/AllCountries'
import AllClubs from './components/AllClubs'
import AllScenarios from './components/AllScenarios'
import AllSimulations from './components/AllSimulations'
import { useNavigate } from 'react-router-dom';
import AddCountry from './components/AddCountry';


function App() {
    const navigator = useNavigate();
    const [countries, setCountries] = useState([]);
    useEffect(() => updateCountries, []);
    
    async function updateCountries() {
        const response = await fetch(`api/country/getall`);
        //console.log(response);
        const data = await response.json();
        setCountries(data);
        //console.log(data);
    }
  return (
    <>    
      <Layout>
              <Route exact path='/' element={<AllCountries countries={countries} navigator={navigator} />} />
            <Route path='/counter' element={<AllClubs />} />
            <Route path='/rank-items' element={<AllScenarios />} />
              <Route path='/rank-items' element={<AllSimulations />} />
              <Route path='/rank-items' element={<AllSimulations />} />
              <Route path='/add-country' element={<AddCountry updateCountries={updateCountries} navigator={navigator} />} />
              <Route path='/edit-country' element={<AddCountry updateCountries={updateCountries} navigator={navigator} isEdit={true} />} />
      </Layout>
    </>
  )
}

export default App
