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
import AddEditClub from './components/AddEditClub';


function App() {
    const navigator = useNavigate();
    const [countries, setCountries] = useState([]);
    const [clubs, setClubs] = useState([]);
    useEffect(() => updateCountries, []);
    useEffect(() => updateClubs, []);
    
    async function updateCountries() {
        const response = await fetch(`api/country/getall`);
        const data = await response.json();
        setCountries(data);
    }
    async function updateClubs() {
        const response = await fetch(`api/club/getall`);
        const data = await response.json();
        setClubs(data);
    }
  return (
    <>    
      <Layout>
              <Route exact path='/' element={<AllCountries countries={countries} navigator={navigator} />} />
              <Route path='/Clubs' element={<AllClubs clubs={clubs} navigator={navigator} />} />
              <Route path='/Scenarios' element={<AllScenarios />} />
              <Route path='/Simulations' element={<AllSimulations />} />
              <Route path='/rank-items' element={<AllSimulations />} />
              <Route path='/add-country' element={<AddCountry updateCountries={updateCountries} navigator={navigator} />} />
              <Route path='/edit-country' element={<AddCountry updateCountries={updateCountries} navigator={navigator} isEdit={true} />} />
              <Route path='/add-club' element={<AddEditClub updateClubs={updateClubs} navigator={navigator} countries={countries} />} />
              <Route path='/edit-club' element={<AddEditClub updateClubs={updateClubs} navigator={navigator} countries={countries} isEdit={true} />} />
      </Layout>
    </>
  )
}

export default App
