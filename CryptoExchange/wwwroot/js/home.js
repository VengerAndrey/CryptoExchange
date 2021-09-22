'use strict';

document.addEventListener('DOMContentLoaded', () => {
    updateHomePage();
    setInterval(updateHomePage, 2000);
});

async function updateBalance() {
    const balanceContainer = document.getElementById('balance');

    if (balanceContainer) {
        fetch('api/Data/balance').then(response => response.json())
            .then(body => {
                if (+balanceContainer.innerText !== +body) {
                    balanceContainer.innerText = `${body}$`;
                }
            });
    }
}

async function updateAccounts() {
    const container = document.getElementById('accounts');

    if (container) {
        let table = container.querySelector('table');

        if (table) {
            fetch('/template/account.html').then(response => response.text())
                .then(template => {
                    fetch('api/Data/accounts').then(response => response.json())
                        .then(body => {
                            const toDelete = [];

                            for (let i = 1; i < table.rows.length; i++) {
                                if (body.some(x => x.coinId == table.rows[i].cells[0].innerText)) {
                                    continue;
                                } else {
                                    toDelete.push(table.rows[i]);
                                }
                            }

                            for (const row of toDelete) {
                                table.removeChild(row);
                            }

                            for (let i = 0; i < body.length; i++) {
                                body[i].coinName = body[i].coin.name;
                                const row = document.createElement('tr');
                                row.innerHTML = applyTemplate(template, body[i]);
                                const existingRow = Array.from(table.rows).find(x => x.cells[0].innerText == body[i].coinId);
                                if (existingRow) {
                                    if (existingRow.innerHTML !== row.innerHTML) {
                                        existingRow.innerHTML = row.innerHTML;
                                    }
                                } else {
                                    table.appendChild(row);
                                }
                            }
                        });
                });
        } else {
            table = document.createElement('table');
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
}

async function updateCoins() {
    const container = document.getElementById('coins');

    if (container) {
        let table = container.querySelector('table');

        if (table) {
            fetch('/template/coin.html').then(response => response.text())
                .then(template => {
                    fetch('api/Data/coins').then(response => response.json())
                        .then(body => {
                            const toDelete = [];

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
                                    if (existingRow.innerHTML !== row.innerHTML) {
                                        existingRow.innerHTML = row.innerHTML;
                                    }
                                } else {
                                    table.appendChild(row);
                                }
                            }
                        });
                });
        } else {
            table = document.createElement('table');
            fetch('/template/coinHeader.html').then(response => response.text())
                .then(template => {
                    const headerRow = table.insertRow(0);
                    headerRow.innerHTML = template;
                });
            fetch('/template/coin.html').then(response => response.text())
                .then(template => {
                    fetch('api/Data/coins').then(response => response.json())
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
        }
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
                const loadedTradeSection = window['loaded-trade-' + section];
                window['loaded-trade-' + section] = true;

                button.disabled = true;

                if (loadedTradeSection) {
                    const toDelete = [];

                    for (const option of Array.from(selectInput.children)) {
                        if (body.some(x => x.id == option.value)) {
                            continue;
                        } else {
                            toDelete.push(option);
                        }
                    }

                    for (const option of toDelete) {
                        selectInput.removeChild(option);
                    }
                } 

                for (let i = 0; i < body.length; i++) {
                    const option = document.createElement('option');
                    option.value = body[i].id;
                    option.innerHTML = body[i].name;
                    const existingOption = Array.from(selectInput.children).find(x => x.value == option.value);
                    if (existingOption) {
                        if (existingOption.innerHTML !== option.innerHtml) {
                            existingOption.innerHTML = option.innerHTML;
                        }
                        continue;
                    }
                    selectInput.appendChild(option);
                }

                selectInput.onchange = () => {
                    if (selectInput.value) {
                        const coin = body.find(x => x.id == selectInput.value);
                        amountInput.max = coin.amount;
                        if (amountInput.edited) {
                            if (+amountInput.value > +coin.amount) {
                                amountInput.value = coin.amount;
                            }
                        } else {
                            amountInput.value = 0;
                        }
                        rateContainer.innerText = coin[section + 'Rate'];
                    } else {
                        amountInput.value = 0;
                        amountInput.max = 0;
                        rateContainer.innerText = 0;
                    }
                }

                button.onclick = () => {
                    submitListener(selectInput.value, amountInput.value);
                }

                amountInput.oninput = amountOnInput;

                selectInput.dispatchEvent(new Event('change'));

                button.disabled = false;
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
    if (amount <= 0) {
        return 'Empty transaction.';
    }
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
    if (amount <= 0) {
        return 'Empty transaction.';
    }
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

function updateHomePage() {
    updateBalance();
    updateAccounts();
    updateCoins();
    updateTradeSection('buy', buy);
    updateTradeSection('sell', sell);
}