const CalculatorModule = (function () {
    let result = 0;  // Initialize the result to 0

    function add(value) {
        result += value;
        displayResult();
    }

    function subtract(value) {
        result -= value;
        displayResult();
    }

    function displayResult() {
        document.getElementById('output').textContent = `Result: ${result}`;  // Update the UI
    }

    return {
        add,  // Expose the add function
        subtract // Expose the subtract function
    };
})();

let addButton = document.getElementById('add');
let subtractButton = document.getElementById('subtract');
let inputValue = document.getElementById('number');
let outputValue = document.getElementById('output');

addButton.addEventListener('click', function () {
    let value = parseInt(inputValue.value, 10);  // Parse the input value
    CalculatorModule.add(value);
});

subtractButton.addEventListener('click', function () {
    let value = parseInt(inputValue.value, 10);  // Parse the input value
    CalculatorModule.subtract(value);
});


class Subject {
    constructor() {
        this.observers = [];  // Initialize the observers list
    }

    subscribe(observer) {
        this.observers.push(observer);  // Add an observer
    }

    unsubscribe(observer) {
        this.observers = this.observers.filter(obs => obs === observer);  // Remove an observer
    }

    notify() {
        this.observers.forEach(observer => observer.update());  // Notify all observers
    }
}

class Observer {
    constructor(name) {
        this.name = name;  // Store the observer's name
    }

    update() {
        updateStatus(`${this.name} received notification!`);
    }
}

function updateStatus(text) {
    statusP.innerHTML += `${text}<br>`;// Update the status paragraph with the provided text
}

const subject = new Subject();  // Create a new subject  

let addSubscribeButton = document.getElementById('addSubscriber');
let unsubscribeButton = document.getElementById('removeSubscriber');
let subscriberName = document.getElementById('subscriberName');
let notificationButton = document.getElementById('notify');
let statusP = document.getElementById('status');
let clearStatusButton = document.getElementById('clear');
clearStatusButton.addEventListener('click', function () {
    statusP.innerHTML = '';  // Clear the status paragraph
});

addSubscribeButton.addEventListener('click', function () {
    let observer = new Observer(subscriberName.value);  // Create a new observer
    subject.subscribe(observer);  // Subscribe the observer
    updateStatus(`${observer.name} has subscribed`);  // Update the status
});

unsubscribeButton.addEventListener('click', function () {
    if (subscriberName.value) {
        subject.unsubscribe(subscriberName.value);  // Unsubscribe the observer
        updateStatus(`${subscriberName.value} has unsubscribed`);  // Update the status
    }
});

notificationButton.addEventListener('click', function () {
    subject.notify();  // Notify all observers
});

// Singleton pattern implementation
class Settings {
    constructor() {
        if (Settings.instance) {
            return Settings.instance;  // Return the existing instance
        }

        this.configuration = {};  // Initialize the configuration object
        Settings.instance = this;  // Store the instance
    }

    set(key, value) {
        this.configuration[key] = value;  // Set a configuration value
    }

    get(key) {
        return this.configuration[key];  // Retrieve a configuration value
    }
}

let keyEntry = document.getElementById('key');
let valueEntry = document.getElementById('value');
let setButton = document.getElementById('setKey');
let showSettingsButton = document.getElementById('showSettings');
let settings = new Settings();  // Create a new settings instance

setButton.addEventListener('click', function () {
    if (keyEntry.value === '' || valueEntry.value === '') {
        return;
    }
    settings.set(keyEntry.value, valueEntry.value);  // Set the configuration value
});

showSettingsButton.addEventListener('click', function () {
    let output = '';  // Initialize the output string
    for (let key in settings.configuration) {
        if (settings.configuration.hasOwnProperty(key)) {
            output += `${key}: ${settings.configuration[key]}<br>`;  // Append each configuration to the output
        }
    }
    document.getElementById('settings').innerHTML = output;  // Update the settings output
});
