'use strict';

document.addEventListener('DOMContentLoaded', () => {
    const input = document.getElementById('refill-amount');
    if (input) {
        input.oninput = amountOnInput;
    }

    const buttonRefill = document.getElementById('button-refill');
    if (buttonRefill) {
        buttonRefill.onclick = refill;
    }
});

async function refill() {
    const input = document.getElementById('refill-amount');
    if (input && +input.value > 0) {
        return fetch('/Home/Refill',
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(+input.value)
                })
            .then(response => response.text())
            .then(message => {
                if (message) {
                    alert(message);
                }
            });
    }
}

function amountOnInput() {
    console.log(this.value);
    if (+this.value > +this.max) {
        this.value = this.max;
    }
    else if (+this.value < 0) {
        this.value = 0;
    }
}