function AllSimulations({ simulations }) {
    console.log(simulations);
  return (
      simulations.map(si => <p key={si.id}>{si.scenarioName }Scenario {si.name} simulation!</p>)
  );
}

export default AllSimulations;