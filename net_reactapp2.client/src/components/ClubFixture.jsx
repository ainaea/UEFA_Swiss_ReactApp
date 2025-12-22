import { Pencil, AirplaneFill, HouseFill } from 'react-bootstrap-icons'

function ClubFixture({ fixture }) {

    return (
      <div className="bg-light col-md-3 bg-light mb-1 rounded-pill d-flex align-items-center">
            <div className="col-md-10">
                <h6 className="pl-1">{fixture.opponent.name} ({fixture.opponent.country.abbrevation}) ({fixture.potName})</h6>
            </div>
            {fixture != null &&  fixture.home && <HouseFill className="btn col-md-2 fs-3" size={55} color="blue" />}
            {fixture != null && !fixture.home && <AirplaneFill className="btn col-md-2 fs-3" size={55} color="blue" />}
        </div>
      /*<p> Club:{fixture.opponent.name} Home;{fixture.home == true ? "True" : "False"} Pot:{fixture.potName}</p>*/
  );
}

export default ClubFixture;