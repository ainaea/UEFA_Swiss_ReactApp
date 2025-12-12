import { useState, useEffect } from 'react';
import { edit } from './fields';
import ValidationSummary from './ValidationSummary'
function AddEditScenario({ updateScenarios, navigator, isEdit = false, model = null }) {
    if (isEdit) {
        model = edit.scenario;
    }

    const [name, setName] = useState(isEdit ? model.name : "");
    const [numberOfPot, setNumberOfPot] = useState(isEdit ? model.numberOfPot : 2);
    const [numberOfTeamsPerPot, setNumberOfTeamsPerPot] = useState(isEdit ? model.numberOfTeamsPerPot : 2);
    const [numberOfGamesPerPot, setNumberOfGamesPerPot] = useState(isEdit ? model.numberOfGamesPerPot : 1);
    const [homeAndAwayPerOpponent, setHomeAndAwayPerOpponent] = useState(isEdit ? model.homeAndAwayPerOpponent : false);
    const [validationSummary, setValidationSummary] = useState([]);
    //console.log(homeAndAwayPerOpponent);
    const submitText = isEdit ? "Update" : "Register";
    const endpoint = isEdit ? 'api/scenario/edit' : 'api/scenario/create';

    useEffect(() => {
        isModelValid();
    }, [name, numberOfPot, numberOfTeamsPerPot, numberOfGamesPerPot])

    function isModelValid() {
        var modelErrors = [];
        if (name.length < 4)
            modelErrors.push('Enter a Name of atleast 4 characters long');
        if (numberOfGamesPerPot > numberOfTeamsPerPot)
            modelErrors.push(`It is impossible to get ${numberOfGamesPerPot} opponents from ${numberOfTeamsPerPot} teams`);
        if (numberOfGamesPerPot == numberOfTeamsPerPot)
            modelErrors.push(`It is impossible to get a fully matched pot against itself`);
        if ((numberOfGamesPerPot * numberOfTeamsPerPot) % 2 != 0)
            modelErrors.push(`Can not create ${((numberOfGamesPerPot * numberOfTeamsPerPot)+1) / 2} fixtures with ${numberOfGamesPerPot * numberOfTeamsPerPot} team slots`);

        setValidationSummary(modelErrors);
        return modelErrors.length == 0;
    }

    async function handleCreation(e) {
        e.preventDefault();
        if (!isModelValid())
            return;

        const data = isEdit ? { name, numberOfPot, numberOfTeamsPerPot, numberOfGamesPerPot, homeAndAwayPerOpponent, id: model.id } : { name, numberOfPot, numberOfTeamsPerPot, numberOfGamesPerPot, homeAndAwayPerOpponent };
        try {
            const response = await fetch(endpoint, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });
            if (response.ok) {
                updateScenarios();
                navigator('/Scenarios');
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
            <ValidationSummary Summary={validationSummary } />
            <div className="mb-3 row">
                <div className="col-sm-4 row">
                    <label className="col-sm-2 col-form-label">Name:</label>
                    <div className="col-sm-10">
                        <input name="Name" className="form-control" value={name} onChange={(e) => setName(e.target.value)} />
                    </div>
                </div>
                <div className="col-sm-2 row">
                    <label className="col-sm-3 col-form-label">Pots:</label>
                    <div className="col-sm-6">
                        <input type='number' min={2} name="numberOfPot" className="form-control" value={numberOfPot} onChange={(e) => setNumberOfPot(e.target.value)} />
                    </div>
                </div>
                <div className="col-sm-3 row">
                    <label className="col-sm-5 col-form-label">Teams per Pot:</label>
                    <div className="col-sm-4">
                        <input type='number' min={1} name="numberOfTeamsPerPot" className="form-control" value={numberOfTeamsPerPot} onChange={(e) => setNumberOfTeamsPerPot(e.target.value) } />
                    </div>
                </div>
            </div>
            <div className="mb-3 row">
                <div className="col-sm-3 row">
                    <label className="col-sm-7 col-form-label">Opponents per Pot:</label>
                    <div className="col-sm-4">
                        <input type='number' min={1} name="numberOfGamesPerPot" className="form-control" value={numberOfGamesPerPot} onChange={(e) => setNumberOfGamesPerPot(e.target.value)} />
                    </div>
                </div>
                <div className="col-sm-4 row">
                    <label className="col-sm-9 col-form-label">Home and Away Per Opponent:</label>
                    <div className="col-sm-3">
                        <input type='checkbox' name="homeAndAwayPerOpponent" className="form-check-input" checked={homeAndAwayPerOpponent} onChange={(e) => setHomeAndAwayPerOpponent(e.target.checked) } />
                    </div>
                </div>
            </div>

            <div>
                <button type="submit" className="btn btn-primary">{submitText}</button>
            </div>
        </form>
    );
}

export default AddEditScenario;