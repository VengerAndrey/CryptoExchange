'use strict';

document.addEventListener('DOMContentLoaded', () => {
    const buttonStockSettings = document.getElementById('button-stock-settings');
    if (buttonStockSettings) {
        buttonStockSettings.onclick = updateStockSettings;
    }

    const buttonAddCoin = document.getElementById('button-add-coin');
    if (buttonAddCoin) {
        buttonAddCoin.onclick = addCoin;
    }

    const buttonDeleteCoin = document.getElementById('button-delete-coin');
    if (buttonDeleteCoin) {
        buttonDeleteCoin.onclick = deleteCoin;
    }

    loadStockSettings()
        .then(loadCoinsSection);
});

async function loadStockSettings() {
    return loadRateMargin()
        .then(loadRandomMargin)
        .then(loadInitialPurchase)
        .then(loadAdditionalPurchase)
        .then(loadInitialGrant);
}

async function loadRateMargin() {
    const rateMarginInput = document.getElementById('rate-margin');

    if (rateMarginInput) {
        return fetch('Admin/RateMargin')
            .then(response => response.text())
            .then(rateMargin => {
                rateMarginInput.value = rateMargin;
            });
    }

    return emptyPromise;
}

async function loadRandomMargin() {
    const randomMarginInput = document.getElementById('random-margin');

    if (randomMarginInput) {
        return fetch('Admin/RandomMargin')
            .then(response => response.text())
            .then(randomMargin => {
                randomMarginInput.value = randomMargin;
            });
    }

    return emptyPromise;
}

async function loadInitialPurchase() {
    const initialPurchaseInput = document.getElementById('initial-purchase');

    if (initialPurchaseInput) {
        return fetch('Admin/InitialPurchase')
            .then(response => response.text())
            .then(initialPurchase => {
                initialPurchaseInput.value = initialPurchase;
            });
    }

    return emptyPromise;
}

async function loadAdditionalPurchase() {
    const additionalPurchaseInput = document.getElementById('additional-purchase');

    if (additionalPurchaseInput) {
        return fetch('Admin/AdditionalPurchase')
            .then(response => response.text())
            .then(additionalPurchase => {
                additionalPurchaseInput.value = additionalPurchase;
            });
    }

    return emptyPromise;
}

async function loadInitialGrant() {
    const initialGrantInput = document.getElementById('initial-grant');

    if (initialGrantInput) {
        return fetch('Admin/InitialGrant')
            .then(response => response.text())
            .then(initialGrant => {
                initialGrantInput.value = initialGrant;
            });
    }

    return emptyPromise;
}

async function updateStockSettings() {
    updateRateMargin()
        .then(updateRandomMargin)
        .then(updateInitialPurchase)
        .then(updateAdditionalPurchase)
        .then(updateInitialGrant)
        .then(loadStockSettings)
        .catch(loadStockSettings);
}

async function updateRateMargin() {
    const rateMarginInput = document.getElementById('rate-margin');

    if (rateMarginInput) {
        return fetch('Admin/RateMargin',
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(+rateMarginInput.value)
            });
    }

    return emptyPromise;
}

async function updateRandomMargin() {
    const randomMarginInput = document.getElementById('random-margin');

    if (randomMarginInput) {
        return fetch('Admin/RandomMargin',
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(+randomMarginInput.value)
            });
    }

    return emptyPromise;
}

async function updateInitialPurchase() {
    const initialPurchaseInput = document.getElementById('initial-purchase');

    if (initialPurchaseInput) {
        return fetch('Admin/InitialPurchase',
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(+initialPurchaseInput.value)
            });
    }

    return emptyPromise;
}

async function updateAdditionalPurchase() {
    const additionalPurchaseInput = document.getElementById('additional-purchase');

    if (additionalPurchaseInput) {
        return fetch('Admin/AdditionalPurchase',
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(+additionalPurchaseInput.value)
            });
    }

    return emptyPromise;
}

async function updateInitialGrant() {
    const initialGrantInput = document.getElementById('initial-grant');

    if (initialGrantInput) {
        return fetch('Admin/InitialGrant',
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(+initialGrantInput.value)
            });
    }

    return emptyPromise;
}

async function loadCoinTable() {
    const container = document.getElementById('coins');

    if (container) {
        const table = document.createElement('table');

        if (table) {
            return fetch('/template/adminCoinHeader.html').then(response => response.text())
                .then(template => {
                    const headerRow = table.insertRow(0);
                    headerRow.innerHTML = template;
                })
                .then(r => {
                    return fetch('/template/adminCoin.html').then(response => response.text())
                        .then(template => {
                            return fetch('Admin/Coins').then(response => response.json())
                                .then(body => {
                                    for (let coin of body) {
                                        const row = document.createElement('tr');
                                        row.innerHTML = applyTemplate(template, coin);
                                        table.appendChild(row);
                                    }
                                    if (container.firstChild) {
                                        container.removeChild(container.firstChild);
                                    }
                                    container.appendChild(table);
                                });
                        });
                });
            
        }
    }

    return emptyPromise;
}

async function loadAvailableCoins() {
    const select = document.getElementById('select-coin-add');

    if (select) {
        return fetch('Admin/AvailableCoins')
            .then(response => response.json())
            .then(body => {
                while (select.firstChild) {
                    select.removeChild(select.lastChild);
                }

                for (const coin of body) {
                    const option = document.createElement('option');
                    option.value = coin.id;
                    option.innerHTML = `${coin.id} - ${coin.name}`;
                    select.appendChild(option);
                }
            });
    }

    return emptyPromise;
}

function addCoin() {
    const select = document.getElementById('select-coin-add');

    if (select) {
        const coinId = select.value;

        if (coinId.length) {
            return fetch('Admin/AddCoin',
                    {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify(coinId)
                    })
                .then(loadCoinsSection);
        }
    }

    return emptyPromise;
}

async function loadCurrentCoins() {
    const select = document.getElementById('select-coin-delete');

    if (select) {
        return fetch('Admin/Coins')
            .then(response => response.json())
            .then(body => {
                while (select.firstChild) {
                    select.removeChild(select.lastChild);
                }

                for (const coin of body) {
                    const option = document.createElement('option');
                    option.value = coin.id;
                    option.innerHTML = `${coin.id} - ${coin.name}`;
                    select.appendChild(option);
                }
            });
    }

    return emptyPromise;
}

function deleteCoin() {
    const select = document.getElementById('select-coin-delete');

    if (select) {
        const coinId = select.value;

        if (coinId.length) {
            return fetch('Admin/DeleteCoin',
                    {
                        method: 'DELETE',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify(coinId)
                    })
                .then(loadCoinsSection);
        }
    }

    return emptyPromise;
}

async function loadCoinsSection() {
    return loadCoinTable()
        .then(loadAvailableCoins)
        .then(loadCurrentCoins);
}
