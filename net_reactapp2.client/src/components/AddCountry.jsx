import { useState } from 'react'; 
import { edit } from './fields';

function AddCountry({ updateCountries, navigator, isEdit = false, model = null }) {
    if (isEdit) {
        model = edit.country;
    }
    const [name, setName] = useState(isEdit ? model.name:"");
    const [abbrevation, setAbbrevation] = useState(isEdit ? model.abbrevation:"");
    const submitText = isEdit ? "Update" : "Register";
    const endpoint = isEdit ? 'api/country/edit' : 'api/country/create';
    async function handleCreation(e) {
        e.preventDefault();

        const data = isEdit ? {name : name, abbrevation : abbrevation, id : model.id, flag : model.flag} : { name, abbrevation };
        try {
            const response = await fetch(endpoint, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });

            if (response.ok) {
                updateCountries();
                navigator('/');
            }
            else {
                console.log(Object.values(data).flat())
            }
        } catch (e) {
            console.log('Error sending data:', e)
        }
    }

  return (
      <form onSubmit={handleCreation} method="post">
          <div className="mb-3 row">
              <label className="col-sm-2 col-form-label">Name</label>
              <div className="col-sm-10">
                  <input name="Name" className="form-control" value={name} onChange={(e)=> setName(e.target.value) } />
              {/*    <span asp-validation-for="Name" className="text-danger"></span>*/}
              </div>
          </div>
          <div className="mb-3 row">
              <label className="col-sm-2 col-form-label">Abbrevation</label>
              <div className="col-sm-10">
                  <input name="Abbrevation" className="form-control" value={abbrevation} onChange={(e) => setAbbrevation(e.target.value.substring(0,3).toUpperCase())} />
              {/*    <span asp-validation-for="Abbrevation" className="text-danger"></span>*/}
              </div>
          </div>

          <div>
              <button type="submit" className="btn btn-primary">{submitText}</button>
          </div>
      </form>
  );
}

export default AddCountry;