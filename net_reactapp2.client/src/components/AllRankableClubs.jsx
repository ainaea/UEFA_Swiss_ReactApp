import RankableClub from './RankableClub';

function AllRankableClubs({ clubs }) {
    return (clubs == null ? <h3>No Club to display at the moment</h3> :
        <div className="bg-secondary pt-2 pb-2 row">
            {clubs.map(c => <RankableClub key={c.id} club={c} />)}
        </div>
  );
}

export default AllRankableClubs;