import { useState, useEffect } from 'react';
import { edit } from './fields';
import AllRankableClubs from './AllRankableClubs';
import RankableCollection from './RankableCollection';
import ValidationSummary from './ValidationSummary'


function RankClubs({ navigator, clubs, updateSimulations, isResimulation = false }) {
    const [rankableClubs, setRankableClubs] = useState(clubs);
    const [simName, setSimName] = useState("");
    const [validationSummary, setValidationSummary] = useState([]);
    let selectedClubs = rankableClubs.filter(c => c.rank > 0);
    const scenario = edit.scenario;
    const pots = scenario.numberOfPot;
    const clubsPerPot = scenario.numberOfTeamsPerPot;
    const targetSelection = pots * clubsPerPot;

    var potIndices = [];
    for (var i = 1; i <= pots; i++) {
        potIndices[i - 1] = i;
    }
    var clubsIndices = [];
    for (var i = 1; i <= clubsPerPot; i++) {
        clubsIndices[i - 1] = i;
    }

    if (isResimulation) {
        let clubsRanking = edit.simulationClubs_Ranking;
        clubsRanking.map((cr)=> clubs.filter(c=>c.id == cr.id)[0].rank = cr.rank);

    }

    useEffect(() => {
        isModelValid();
    }, [simName, rankableClubs])

    function updateRankableCLubs(id, ranking, ev) {
        const targetElm = ev.target;
        if (targetElm.childNodes.length === 0) {
            const transformedCollection = rankableClubs.map((club) => (club.id === id ? { ...club, rank: ranking } : { ...club }));
            updateClubs(transformedCollection);
        }
        else if (targetElm.childNodes.length === 1) {
            let otherRanking = rankableClubs.filter((a) => a.id == id)[0].rank;
            const transformedCollection = rankableClubs.map((club) => (club.id === id || club.rank == ranking ? (club.id == id ? { ...club, rank: ranking } : { ...club, rank: otherRanking }) : { ...club }));
            updateClubs(transformedCollection);
        }
    }

    function updateClubs(transformedCollection) {
        var orderTransformedCollection = transformedCollection.sort((a, b) => b.priority - a.priority || a.name.localeCompare(b.name));
        setRankableClubs(orderTransformedCollection)
    }
    function updatePriorityClubs(id) {
        const transformedCollection = rankableClubs.map((club) => (club.id === id ? { ...club, priority: !club.priority } : { ...club }));
        updateClubs(transformedCollection);
    }

    function unSelectClub(id) {
        const transformedCollection = rankableClubs.map((club) => (club.id === id ? { ...club, rank: 0 } : { ...club }));
        updateClubs(transformedCollection);
    }

    async function intitiateSimulation() {
        if (isModelValid()) {
            var data = { name: simName, scenarioId: scenario.id, clubs: selectedClubs };
            const endpoint = 'api/simulation/create';
            try {
                const response = await fetch(endpoint, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(data)
                });
                if (response.ok) {
                    updateSimulations();
                    navigator('/Simulations');
                }
                else {
                    var resp = await response.json().then(ar => ar.error);
                    setValidationSummary([resp]);
                }
            } catch (e) {
                console.log('Error sending data:', e)
            }
        }
        //console.log(obj);

    }

    function isModelValid() {
        var modelErrors = [];
        if (simName.length < 4)
            modelErrors.push('Enter a Name of atleast 4 characters long');
        if (selectedClubs.length < targetSelection)
            modelErrors.push(`Selection is incomplete. ${selectedClubs.length} of ${targetSelection} selected`);
        
        setValidationSummary(modelErrors);
        return modelErrors.length == 0;
    }

    return (
        <div>
            <ValidationSummary Summary={validationSummary} />
            <div className="row">
                <h3 className="col-sm-6">{scenario.name}</h3>
                <label className="col-sm-2 col-form-label">Simulation:</label>
                <div className="col-sm-2">
                    <input name="numberOfGamesPerPot" placeholder="Name for Simulation" className="form-control" required={true} value={simName} onChange={(e) => setSimName(e.target.value)} />
                </div>
            </div>
          <div className="rankingDiv">
                {potIndices.map(pi => <RankableCollection key={pi} name={`Pot ${pi}`} rank={pots - pi} subCount={clubsPerPot} updateRankableCLubs={updateRankableCLubs} rankableClubs={rankableClubs } />)}
          </div>
            <div className="unrankedCountriesDiv" onDragOver={e => e.preventDefault()} onDrop={e => unSelectClub(e.dataTransfer.getData('entityId'))}>
              <h4>All clubs</h4>
                <AllRankableClubs clubs={rankableClubs.filter((rc) => rc.rank === 0)} updatePriorityClubs={ updatePriorityClubs} />
            </div>
            <div>
                <button type="submit" className="btn btn-primary" onClick={intitiateSimulation }>Save Progress</button>
            </div>
      </div>
  );
}

export default RankClubs;