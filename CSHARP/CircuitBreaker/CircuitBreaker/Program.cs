using System;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public static class Program
    {

        /*
         The main thing of this excersize is that the CircuitBreaker can be created in multiple ways.
         I've used a Class to keep the state of the *state*-machine
         I've used recursion to call back into the function we just started, with the main difference being:
            1: Some retry-count was incremented
            2: The State was changed so a different codeblock will be executed
         */
        private static async Task Main()
        {
            // create a CircuitBreaker instance
            var circuitBreaker = new CircuitBreaker()
            {
                Url = "https://jsonplaceholder.typicode.com/todos/1", // really fun free testing-API :)
                RetryCount = 3,
                TimeOut = 3000, // 3000 ms = 3 seconds
            };

            try // try and get a response from the CircuitBreaker
            {
                // do async call and await the result
                var todo = await circuitBreaker.Start().ConfigureAwait(false);
                Console.WriteLine($"title: {todo.Title}, id: {todo.Id}, completed: {todo.Completed}");
            }
            catch
            {
                Console.WriteLine("prolonged error happened, lets return a graceful error message");
            }

        }
    }
}
