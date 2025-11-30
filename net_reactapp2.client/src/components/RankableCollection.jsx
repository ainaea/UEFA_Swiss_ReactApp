function RankableCollection({ name, rank, subCount, updateRankableCLubs, rankableClubs }) {
    var subCountIndices = [];
    for (var i = 1; i <= subCount; i++) {
        subCountIndices[subCount -i] = i;
    }

    function updateEntityRanking(id, ranking, ev) {
        updateRankableCLubs(id, ranking, ev)
    }
  return (
      <div className="rankableCollectionDiv">
          <h4>{name}</h4>
          <div className="bg-secondary pt-2 pb-2 row">
              {subCountIndices.map(ci => 
                  <div key={`${name}_${ci}`} className="bg-light col-md-2 bg-light border p-3 rounded-pill d-flex align-items-center" onDragOver={e => e.preventDefault()} onDrop={e => { e.preventDefault(); updateEntityRanking(e.dataTransfer.getData('entityId'), rank * subCount + ci, e)}} >
                      {rankableClubs.filter((rc) => rc.rank == rank * subCount + ci).length > 0 ?
                          <div className="col-md-12" draggable={true} onDragStart={(e) => e.dataTransfer.setData('entityId', rankableClubs.filter((rc) => rc.rank == rank * subCount + ci)[0].id)}>
                              <h6>{rankableClubs.filter((rc) => rc.rank == rank * subCount + ci)[0].name}</h6>
                          </div> : false}
                  </div>)
              }
          </div>
      </div>
  );
}

export default RankableCollection;