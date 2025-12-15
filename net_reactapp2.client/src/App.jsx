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
import AddEditScenario from './components/AddEditScenario';
import RankClubs from './components/RankClubs';
import Simulation from './components/Simulation';


function App() {
    const navigator = useNavigate();
    const [countries, setCountries] = useState([]);
    const [clubs, setClubs] = useState([]);
    const [scenarios, setScenarios] = useState([]);
    const [simulations, setSimulations] = useState([]);
    useEffect(() => updateCountries, []);
    useEffect(() => updateClubs, []);
    useEffect(() => updateScenarios, []);
    useEffect(() => updateSimulations, []);
    
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
    async function updateScenarios() {
        const response = await fetch(`api/scenario/getall`);
        const data = await response.json();
        setScenarios(data);
    }
    async function updateSimulations() {
        const response = await fetch(`api/simulation/getall`);
        const data = await response.json();
        setSimulations(data);
    }
    const rankableCLubs = clubs.map((c) => ({ ...c, rank: 0, priority: false }));
  return (
    <>    
      <Layout>
              <Route exact path='/' element={<AllCountries countries={countries} navigator={navigator} />} />
              <Route path='/Clubs' element={<AllClubs clubs={clubs} navigator={navigator} />} />
              <Route path='/Scenarios' element={<AllScenarios scenarios={scenarios} navigator={navigator} />} />
              <Route path='/Simulations' element={<AllSimulations simulations={simulations} navigator={navigator} />} />
              <Route path='/add-country' element={<AddCountry updateCountries={updateCountries} navigator={navigator} />} />
              <Route path='/edit-country' element={<AddCountry updateCountries={updateCountries} navigator={navigator} isEdit={true} />} />
              <Route path='/add-club' element={<AddEditClub updateClubs={updateClubs} navigator={navigator} countries={countries} />} />
              <Route path='/edit-club' element={<AddEditClub updateClubs={updateClubs} navigator={navigator} countries={countries} isEdit={true} />} />
              <Route path='/add-scenario' element={<AddEditScenario updateScenarios={updateScenarios} navigator={navigator} />} />
              <Route path='/edit-scenario' element={<AddEditScenario updateScenarios={updateScenarios} navigator={navigator} isEdit={true} />} />
              <Route path='/rank-clubs' element={<RankClubs navigator={navigator} clubs={rankableCLubs} updateSimulations={updateSimulations} />} />
              <Route path='/resimulate' element={<RankClubs navigator={navigator} clubs={rankableCLubs} updateSimulations={updateSimulations} isResimulation={true } />} />
              <Route path='/view-simulation' element={<Simulation navigator={navigator}/>} />
      </Layout>
    </>
  )
}

export default App
