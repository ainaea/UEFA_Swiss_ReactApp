import SimulationCard from './SimulationCard'

function AllSimulations({ simulations, navigator }) {
    return (
        simulations == null ? <h3>No Club to display at the moment</h3> :
            <div className="bg-secondary pt-2 pb-2 row">
                {simulations.map(s => <SimulationCard key={s.id} simulation={s} navigator={navigator} />)}                
            </div>
  );
}

export default AllSimulations;