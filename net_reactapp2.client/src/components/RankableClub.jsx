function RankableClub({club }) {
  return (
      <div className="bg-light col-md-2 bg-light border p-3 rounded-pill d-flex align-items-center">
          <div className="col-md-12" draggable={true} onDragStart={(e) => e.dataTransfer.setData('entityId', club.id) }>
              <h6>{club.name}</h6>
          </div>
      </div>
  );
}

export default RankableClub;