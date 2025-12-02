function RankableClub({ club, updatePriorityClubs }) {
  return (
      <div className="bg-light col-md-2 bg-light border p-3 rounded-pill d-flex align-items-center">
          <div className="col-md-10" draggable={true} onDragStart={(e) => e.dataTransfer.setData('entityId', club.id) }>
              <h6>{club.name}</h6>
          </div>
          <div className="col-md-2">
              <input type='checkbox' name="homeAndAwayPerOpponent" className="form-check-input" checked={club.priority} onChange={(e) => {updatePriorityClubs(club.id) }} />
          </div>
      </div>
  );
}

export default RankableClub;