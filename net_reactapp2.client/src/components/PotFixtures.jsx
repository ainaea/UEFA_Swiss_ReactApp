import ClubFixture from "./ClubFixture";

function PotFixture({ potFixtures }) {
  return (
      <div className="bg-secondary pt-2 pb-2 row" >
          <h6 className="bg-light rounded-pill p-1">{potFixtures.mainClub.name}</h6>
          {potFixtures.fixtures.map((fx, j) =>
              <ClubFixture key={j} fixture={fx} />
          )}
      </div>
  );
}

export default PotFixture;
