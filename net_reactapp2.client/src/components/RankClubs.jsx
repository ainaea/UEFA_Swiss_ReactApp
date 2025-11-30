import { useState } from 'react';
import { edit } from './fields';
import AllRankableClubs from './AllRankableClubs';
import RankableCollection from './RankableCollection';

function RankClubs({ navigator, clubs }) {
    const [rankableClubs, setRankableClubs] = useState(clubs);
    console.log('above', rankableClubs.filter((rc) => rc.rank > 0).length)
    console.log('equal', rankableClubs.filter((rc) => rc.rank === 0).length)
    console.log('all', rankableClubs.length)

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

    function updateRankableCLubs(id, ranking, ev) {
        const targetElm = ev.target;
        if (targetElm.childNodes.length === 0) {
            const transformedCollection = rankableClubs.map((club) => (club.id === id ? { ...club, rank: ranking } : { ...club }));
            //console.log(transformedCollection.filter((a)=> a.rank == ranking));
            setRankableClubs(transformedCollection);
        }        
    }


    return (
        <div>
          <div className="rankingDiv">
                {potIndices.map(pi => <RankableCollection key={pi} name={`Pot ${pi}`} rank={pots - pi} subCount={clubsPerPot} updateRankableCLubs={updateRankableCLubs} rankableClubs={rankableClubs } />)}
          </div>
          <div className="unrankedCountriesDiv">
              <h4>All clubs</h4>
                <AllRankableClubs clubs={rankableClubs.filter((rc)=>rc.rank === 0)} />
          </div>
      </div>
  );
}

export default RankClubs;