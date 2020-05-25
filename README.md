# Circuit Breaker

## Origins
The circuit breaker is a software-pattern used in distributed systems where:
* one can not know if there will be an answer from external services
* other servers/services within the same architecture could be (temporarily) down

## What it does
The CircuitBreaker aims to add retry-logic to *any* type of request. In this example HTTP requests are used.

In this way, when a single request fails the following things will happen:
* the request will be retried X amount of times with a delay of Y inbetween calls
  * this gives the other server/service time to fix itself
* if the request is not succesful after X amount of times, we can "fail gracefully" by returning some useful error or other response

# More research
You can read more here: https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-circuit-breaker-pattern

    As noted earlier, you should handle faults that might take a variable amount of time to recover from, as might happen when you try to connect to a remote service or resource. Handling this type of fault can improve the stability and resiliency of an application.


    In a distributed environment, calls to remote resources and services can fail due to transient faults, such as slow network connections and timeouts, or if resources are responding slowly or are temporarily unavailable. These faults typically correct themselves after a short time, and a robust cloud application should be prepared to handle them by using a strategy like the "Retry pattern".

    However, there can also be situations where faults are due to unanticipated events that might take much longer to fix. These faults can range in severity from a partial loss of connectivity to the complete failure of a service. In these situations, it might be pointless for an application to continually retry an operation that's unlikely to succeed.

    Instead, the application should be coded to accept that the operation has failed and handle the failure accordingly.
