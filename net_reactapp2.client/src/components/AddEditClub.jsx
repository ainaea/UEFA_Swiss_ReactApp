import { useState } from 'react';
import { edit } from './fields';

function AddEditClub({ updateClubs, navigator, countries, isEdit = false, model = null }) {
    if (isEdit) {
        model = edit.club;
    }
    const [name, setName] = useState(isEdit ? model.name : "");
    const [countryId, setcountryId] = useState(isEdit ? model.countryId : null);
    const submitText = isEdit ? "Update" : "Register";
    const endpoint = isEdit ? 'api/club/edit' : 'api/club/create';

    async function handleUpdate(e) {
        e.preventDefault();

        const data = isEdit ? { name: name, countryId: countryId, id: model.id, logo: model.logo } : { name, countryId };
        try {
            const response = await fetch(endpoint, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });

            if (response.ok) {
                updateClubs();
                navigator('/Clubs');
            }
            else {
                console.log(Object.values(data).flat())
            }
        } catch (e) {
            console.log('Error sending data:', e)
        }
    }

  return (
      <form onSubmit={handleUpdate} method="post">
          <div className="mb-3 row">
              <label className="col-sm-2 col-form-label">Name</label>
              <div className="col-sm-6">
                  <input name="Name" className="form-control" value={name} onChange={(e) => setName(e.target.value)} />
              </div>
          </div>
          <div className="mb-3 row">
              <label className="col-sm-2 col-form-label">Country</label>
              <div className="col-sm-4">
                  <select className="form-control form-select" value={countryId} onChange={(e) => setcountryId(e.target.value)}>
                      {countries.map(c => <option key={c.id } value={c.id}>{c.name} ({c.abbrevation})</option>) }
                  </select>                  
              </div>
          </div>

          <div>
              <button type="submit" className="btn btn-primary">{submitText}</button>
          </div>
      </form>
  );
}

export default AddEditClub;