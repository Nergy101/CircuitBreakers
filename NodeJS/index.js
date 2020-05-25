const CircuitBreaker = require("./CircuitBreaker.js");
const Status = require('./StatusEnum.js');

cb = new CircuitBreaker('https://jsonplaceholder.typicode.com/todos/1', 3000, 3)

cb.Start().then(response => { // then(response) => if we got a response
    console.log('response =', response);
    console.log("\ntotal retries:", cb.Retries);
    console.log("final status:", Status.getKeyByValue(cb.Status));
    
}).catch(circuit_breaker_exception => console.log('error happened:', circuit_breaker_exception));
// catch(err => for when it eventually throws an error)

