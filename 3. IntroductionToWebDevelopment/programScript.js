console.log("Start");

setTimeout(() => {
  console.log("This runs later");
}, 0);

console.log("End");



let arrayPointer = 0;
let headerOne = document.getElementById('quoteHeader');
let quoteDisplay = document.getElementById('quoteDisplay');
let button = document.getElementById('newQuoteButton');
let quoteArray = ['Quote One', 'Quote Two', 'Quote Three'];

button.addEventListener('click', function () { 
    let quote = getQuote()
    quoteDisplay.textContent = quote;
});

function getQuote() {
    let quote = quoteArray[arrayPointer];
    arrayPointer++;
    arrayPointer = arrayPointer % quoteArray.length;
    return quote;

}


function getQuoteRandomly() {
    let randomIndex = Math.floor(Math.random() * quoteArray.length);
    return quoteArray[randomIndex];
}