async function fetchDataAsync() {
    try{
        let response = await fetch("https://jsonplaceholder.typicode.com/users")
        let jsonData = await response.json();
        console.log(jsonData);
        console.log(jsonData[0].name); // Accessing the name of the first users
        let container = document.getElementById('data-container');
        container.innerHTML = ""; // Clear previous data
        container.innerHTML = jsonData.map(user => `<p>${user.name}</p>`).join(''); // Display new data
    }
    catch (error) {
        console.error(error);
    }
}

//fetchDataAsync();

let fetchButton = document.getElementById('fetch-data');
fetchButton.addEventListener('click', fetchDataAsync);