using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;

namespace CircuitBreaker
{
    public class CircuitBreaker
    {
        public string Url { get; set; }
        public int RetryCount { get; set; }
        public int TimeOut { get; set; }
        public int Retries { get; set; }
        public Status Status { get; set; }


        /*
         Call() actually calls the given URL and returns the result if succesful.
         Also sets the Status to Status.SUCCESS
         */
        public async Task<Todo> Call()
        {
            var rand = new Random().Next(1, 11); // get a random number between 1 and 10
            Console.WriteLine("random int: " + rand);
            if (rand > 5) // success-rate of 50%, if number > 5
            {
                Status = Status.SUCCESS;
                // Url is a string, with Flurl.Http we can just call GetJsonAsync and do a Get request on the URL.
                return await Url.GetJsonAsync<Todo>().ConfigureAwait(false);
            }
            // if rand <= 5 throw a fake bad-response error (e.g.)
            throw new Exception("fake bad response");
        }


        // Recursive, retry-logic, state-machine function
        public async Task<Todo> Start()
        {  
            // let's keep track of our function's status
            Console.WriteLine();
            Console.WriteLine("status: " + Status.ToString());
            Console.WriteLine("retries: " + Retries.ToString());

            // if Start() is called with the CircuitBreaker status being OPEN
            if (Status == Status.OPEN)
            {
                try
                {
                    // lets do our initial call
                    return await Call().ConfigureAwait(false);
                }
                catch (Exception badResponse)
                {
                    // do something with bad_response...
                    Console.WriteLine(badResponse.Message);
                    // set status to HALFOPEN because our initial request failed
                    Status = Status.HALFOPEN;
                    Retries++;
                    return await Start().ConfigureAwait(false); // retry with our Status being HALFOPEN
                }
            }

            // if Start() is called with the CircuitBreaker status being HALFOPEN
            if (Status == Status.HALFOPEN)
            {
                Console.WriteLine("TIMEOUT...");
                Thread.Sleep(TimeOut);
                try
                {
                    // try a call again
                    return await Call().ConfigureAwait(false);
                }
                catch
                {
                    if (Retries < RetryCount) // did we retry too many times already?
                    {
                        Retries++;
                        return await Start().ConfigureAwait(false); // Start this function again with the same status HALFOPEN
                    }

                    // looks like we retried too many times, time to set status to CLOSED
                    Status = Status.CLOSED;
                    return await Start().ConfigureAwait(false);
                }
            }

            // if Start() is called with the CircuitBreaker status being CLOSED
            if (Status == Status.CLOSED)
            {
                // we throw our final error message
                throw new Exception($"CircuitBreaker failed for { Url} : { Retries} " +
                    $"times: TimeOut { TimeOut}");
            }

            return await Start().ConfigureAwait(false); // this should never be reached, but acts as a safety-net
        }
    }
}
