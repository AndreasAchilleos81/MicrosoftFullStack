async function fetchJsonData() {
    try {
        let response = await fetch('https://jsonplaceholder.typicode.com/users')
        if (!response.ok) {
            throw new Error('Network response was not ok' + response.statusText);
        }
        let jsonData = await response.json();
        console.log(jsonData);
        let userContainer = document.getElementById('users-container');
        jsonData.forEach(user => {
            let userDiv = document.createElement('div');
            //userDiv.className = 'user';
            userDiv.innerHTML = `<h3>${user.name}</h3><p>${user.email}</p>`;
            userContainer.appendChild(userDiv);
        });
    }
    catch (error) {
        console.error('Error fetching JSON data:', error);
    }
}

fetchJsonData();

let jsonString = 
`[ { 
    "name": "Alice", 
     "age": 25 
    }, 
    { 
    "name": "Bob", 
    "age": 30
    }]`;

console.log(JSON.parse(jsonString));

let jsonObjects = JSON.parse(jsonString);
console.log(jsonObjects[0].name);    // Output: Alice
console.log(jsonObjects[1].age);     // Output: 30

let jsonStringify = JSON.stringify(jsonObjects);
console.log(jsonStringify); // Output: [{"name":"Alice","age":25},{"name":"Bob","age":30}]

let settings ={
    theme: "dark",
    Language: "en",
};

localStorage.setItem("settings", JSON.stringify(settings));

let retrievedSettings = JSON.parse(localStorage.getItem("settings"));
console.log(retrievedSettings); 

