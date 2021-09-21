'use strict';

document.addEventListener('DOMContentLoaded', () => {
    loadHomePage();
    setInterval(updateHomePage, 3000);
});

async function loadBalance() {
    const balanceContainer = document.getElementById('balance');

    if (balanceContainer) {
        fetch('api/Data/balance').then(response => response.json())
            .then(body => {
                if (+balanceContainer.innerText !== +body) {
                    balanceContainer.innerText = `${body} $`;
                }
            });
    }
}

async function loadAccounts() {
    const container = document.getElementById('accounts');
    const table = document.createElement('table');

    if (container) {
        fetch('/template/accountHeader.html').then(response => response.text())
            .then(template => {
                const headerRow = table.insertRow(0);
                headerRow.innerHTML = template;
            });
        fetch('/template/account.html').then(response => response.text())
            .then(template => {
                fetch('api/Data/accounts').then(response => response.json())
                    .then(body => {
                        for (let account of body) {
                            account.coinName = account.coin.name;
                            const row = document.createElement('tr');
                            row.innerHTML = applyTemplate(template, account);
                            table.appendChild(row);
                        }
                        if (container.firstChild) {
                            container.removeChild(container.firstChild);
                        }
                        container.appendChild(table);
                    });
            });
    }
}

async function updateAccounts() {
    const container = document.getElementById('accounts');

    if (container) {
        const table = container.querySelector('table');

        if (table) {
            fetch('/template/account.html').then(response => response.text())
                .then(template => {
                    fetch('api/Data/accounts').then(response => response.json())
                        .then(body => {
                            if (body.length !== table.rows.length - 1) {
                                loadAccounts();
                            } else {
                                for (let i = 0; i < body.length; i++) {
                                    body[i].coinName = body[i].coin.name;
                                    const row = document.createElement('tr');
                                    row.innerHTML = applyTemplate(template, body[i]);
                                    if (table.rows[i + 1].innerHTML !== row.innerHtml) {
                                        table.rows[i + 1].innerHTML = row.innerHTML;
                                    }
                                }
                            }
                        });
                });
        } else {
            loadAccounts();
        }
    }
}

async function loadCoins() {
    const container = document.getElementById('coins');
    const table = document.createElement('table');

    if (container) {
        fetch('/template/coinHeader.html').then(response => response.text())
            .then(template => {
                const headerRow = table.insertRow(0);
                headerRow.innerHTML = template;
            });
        fetch('/template/coin.html').then(response => response.text())
            .then(template => {
                fetch('api/Data/coins').then(response => response.json())
                    .then(body => {
                        window['coins'] = body;
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
    }
}

async function updateCoins() {
    const container = document.getElementById('coins');

    if (container) {
        const table = container.querySelector('table');

        if (table) {
            fetch('/template/coin.html').then(response => response.text())
                .then(template => {
                    fetch('api/Data/coins').then(response => response.json())
                        .then(body => {
                            const oldCoins = window['coins'];
                            if (!oldCoins.length) {
                                loadCoins();
                            } else {
                                window['coins'] = body;

                                let toDelete = [];

                                for (let i = 1; i < table.rows.length; i++) {
                                    if (body.some(x => x.id == table.rows[i].cells[0].innerText)) {
                                        continue;
                                    } else {
                                        toDelete.push(table.rows[i]);
                                    }
                                }

                                for (const row of toDelete) {
                                    table.removeChild(row);
                                }

                                for (let i = 0; i < body.length; i++) {
                                    const row = document.createElement('tr');
                                    row.innerHTML = applyTemplate(template, body[i]);
                                    const existingRow = Array.from(table.rows).find(x => x.cells[0].innerText == body[i].id);
                                    if (existingRow) {
                                        if (existingRow.innerHTML != row.innerHTML) {
                                            existingRow.innerHTML = row.innerHTML;
                                        }
                                    } else {
                                        table.appendChild(row);
                                    }
                                }
                            }
                        });
                });
        } else {
            loadCoins();
        }
    }
}

async function loadTradeSection(section, submitListener) {
    const selectInput = document.getElementById('select-coin-' + section);
    const amountInput = document.getElementById('amount-' + section);
    const rateContainer = document.getElementById('rate-' + section);
    const button = document.getElementById('button-' + section);

    if (selectInput && amountInput && rateContainer && button) {
        button.disabled = true;
        fetch('api/Data/coins-' + section).then(response => response.json())
            .then(body => {
                window['coins-' + section] = body;
                while (selectInput.firstChild) {
                    selectInput.removeChild(selectInput.lastChild);
                }
                for (let i = 0; i < body.length; i++) {
                    const option = document.createElement('option');
                    option.value = body[i].id;
                    option.innerHTML = body[i].name;
                    selectInput.appendChild(option);
                }

                selectInput.onchange = () => {
                    const coin = body.find(x => x.id == selectInput.value);
                    amountInput.value = 10;
                    amountInput.max = coin.amount;
                    rateContainer.innerText = coin[section + 'Rate'];
                }

                button.onclick = () => {
                    submitListener(selectInput.value, amountInput.value);
                }

                amountInput.oninput = amountOnInput;

                if (!amountInput.edited) {
                    selectInput.dispatchEvent(new Event('change'));
                } else {
                    const coin = body.find(x => x.id == selectInput.value);
                    if (+amountInput.value > +coin.amount) {
                        amountInput.value = coin.amount;
                    }
                }

                button.disabled = false;
            });
    }
}

async function updateTradeSection(section, submitListener) {
    const selectInput = document.getElementById('select-coin-' + section);
    const amountInput = document.getElementById('amount-' + section);
    const rateContainer = document.getElementById('rate-' + section);
    const button = document.getElementById('button-' + section);

    if (selectInput && amountInput && rateContainer && button) {
        fetch('api/Data/coins-' + section).then(response => response.json())
            .then(body => {
                const oldBody = window['coins-' + section];

                if (!oldBody.length) {
                    loadTradeSection(section, submitListener);
                } else {
                    window['coins-' + section] = body;

                    let toDelete = [];

                    for (const option of Array.from(selectInput.children)) {
                        if (body.some(x => x.id == option.value)) {
                            continue;
                        } else {
                            toDelete.push(option);
                        }
                    }

                    button.disabled = true;

                    for (const option of toDelete) {
                        selectInput.removeChild(option);
                    }

                    for (let i = 0; i < body.length; i++) {
                        const option = document.createElement('option');
                        option.value = body[i].id;
                        option.innerHTML = body[i].name;
                        const existingOption = Array.from(selectInput.children).find(x => x.value == option.value);
                        if (existingOption) {
                            if (existingOption.innerHTML != option.innerHtml) {
                                existingOption.innerHTML = option.innerHTML;
                            }
                            continue;
                        }
                        selectInput.appendChild(option);
                    }

                    selectInput.onchange = () => {
                        const coin = body.find(x => x.id == selectInput.value);
                        amountInput.value = 10;
                        amountInput.max = coin.amount;
                        rateContainer.innerText = coin[section + 'Rate'];
                    }

                    button.onclick = () => {
                        submitListener(selectInput.value, amountInput.value);
                    }

                    amountInput.oninput = amountOnInput;

                    if (!amountInput.edited) {
                        selectInput.dispatchEvent(new Event('change'));
                    } else {
                        const coin = body.find(x => x.id == selectInput.value);
                        if (+amountInput.value > +coin.amount) {
                            amountInput.value = coin.amount;
                        }
                    }

                    button.disabled = false;
                }
            });
    }
}

function amountOnInput() {
    this.edited = true;
    if (+this.value > +this.max) {
        this.value = this.max;
    }
    else if (+this.value < 0) {
        this.value = 0;
    }
}

function buy(coinId, amount) {
    fetch('api/Data',
        {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                coinId,
                amount
            })
        }).then(updateHomePage);
}

function sell(coinId, amount) {
    fetch('api/Data',
        {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                coinId,
                amount: -amount
            })
        }).then(updateHomePage);
}

function loadHomePage() {
    loadBalance();
    loadAccounts();
    loadCoins();
    loadTradeSection('buy', buy);
    loadTradeSection('sell', sell);
}

function updateHomePage() {
    loadBalance();
    updateAccounts();
    updateCoins();
    updateTradeSection('buy', buy);
    updateTradeSection('sell', sell);
}