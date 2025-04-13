function fetchDataWithCallBack(callback) {
    let xhr = new XMLHttpRequest();
    xhr.open("GET", "https://jsonplaceholder.typicode.com/users", true);
    xhr.onload = () => {
        if (xhr.status === 200) {
            callback("Request complete:", xhr.responseTexts);
        }
        else {
            callback('Error fetching data', null);
        }
    };
    xhr.send();
    console.log("end of fetch data function request could still running asynchronously")
}

fetchDataWithCallBack((error, data) => {
    if (data) {
        console.log(data);
    }
    else {
        console.error(error);
    }
});
