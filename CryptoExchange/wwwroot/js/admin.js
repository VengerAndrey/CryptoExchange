'use strict';

document.addEventListener('DOMContentLoaded', () => {
    loadStockSettings();

    const buttonStockSettings = document.getElementById('button-stock-settings');

    if (buttonStockSettings) {
        buttonStockSettings.onclick = updateStockSettings;
    }
});

async function loadStockSettings() {
    loadRateMargin();
    loadRandomMargin();
    loadInitialPurchase();
    loadAdditionalPurchase();
    loadInitialGrant();
}

async function loadRateMargin() {
    const rateMarginInput = document.getElementById('rate-margin');

    if (rateMarginInput) {
        fetch('Admin/RateMargin')
            .then(response => response.text())
            .then(rateMargin => {
                rateMarginInput.value = rateMargin;
            });
    }
}

async function loadRandomMargin() {
    const randomMarginInput = document.getElementById('random-margin');

    if (randomMarginInput) {
        fetch('Admin/RandomMargin')
            .then(response => response.text())
            .then(randomMargin => {
                randomMarginInput.value = randomMargin;
            });
    }
}

async function loadInitialPurchase() {
    const initialPurchaseInput = document.getElementById('initial-purchase');

    if (initialPurchaseInput) {
        fetch('Admin/InitialPurchase')
            .then(response => response.text())
            .then(initialPurchase => {
                initialPurchaseInput.value = initialPurchase;
            });
    }
}

async function loadAdditionalPurchase() {
    const additionalPurchaseInput = document.getElementById('additional-purchase');

    if (additionalPurchaseInput) {
        fetch('Admin/AdditionalPurchase')
            .then(response => response.text())
            .then(additionalPurchase => {
                additionalPurchaseInput.value = additionalPurchase;
            });
    }
}

async function loadInitialGrant() {
    const initialGrantInput = document.getElementById('initial-grant');

    if (initialGrantInput) {
        fetch('Admin/InitialGrant')
            .then(response => response.text())
            .then(initialGrant => {
                initialGrantInput.value = initialGrant;
            });
    }
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
}