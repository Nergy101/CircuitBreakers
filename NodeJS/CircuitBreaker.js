const axios = require('axios');
const Status = require('./StatusEnum.js');

module.exports = class CircuitBreaker {
    constructor(Url, TimeOut, RetryCount) {
        this.Url = Url;
        this.TimeOut = TimeOut;
        this.RetryCount = RetryCount;
        this.Status = Status.OPEN;
        this.Retries = 0;
    }


    Sleep(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    // this requests the actual URL by using axios
    async Call() {
        // try to do HTTP request
        try {
            const response = await axios.get(this.Url);
            const rand = Math.random() * 10; // get random number between 1-10
            if (rand > 5) {  // success-rate of 50%
                this.Status = Status.SUCCESS;
                return response.data;
            }
            throw Error("fake bad response");
        }
        catch (e) {
            console.log(e.message);
            throw e;
        }
    }

    // actual recursive state-machine function
    async Start() {
        console.log();
        console.log('status:',  Status.getKeyByValue(this.Status));
        console.log('retries:', this.Retries);

        // initial (first) Call
        if (this.Status == Status.OPEN) {
            try {
                return await this.Call();
            }
            catch (bad_response) {
                // do something with bad_response...
                // change status to HALFOPEN
                this.Status = Status.HALFOPEN; 
                this.Retries++;
                return await this.Start();
            }
        }

        if (this.Status == Status.HALFOPEN) {
            await this.Sleep(this.TimeOut);
            try {
                // retry call
                return await this.Call();
            }
            catch {
                if (this.Retries < this.RetryCount) { // did we exceed the retries yet?
                    this.Retries++;
                    return await this.Start();
                }
                this.Status = Status.CLOSED; // if we did, set status to CLOSED
                return await this.Start();
            }
        }

        // return our final error
        if (this.Status == Status.CLOSED) {
            throw Error(`CircuitBreaker failed for ${this.Url} : ${this.Retries} times : TimeOut ${this.TimeOut}`);
        }
    }
}
