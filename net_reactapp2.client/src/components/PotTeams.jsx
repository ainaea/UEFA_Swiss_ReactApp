import PotFixture from "./PotFixtures";
import PotFixtures from "./PotFixtures";

function PotTeams({ clubsFixtures }) {
    console.log(clubsFixtures);
    return ( 
        <>
            {
                clubsFixtures.map((cfx, i) =>
                    <div key={i }>
                        <PotFixtures potFixtures={ cfx} />
                    </div>
                )
            }
            
        </>
        
  );
}

export default PotTeams;