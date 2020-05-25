import requests  # you need to add this to your interpreter through pip
import time
import random


# using a Class to keep state on the CircuitBreaker

#
#          The main thing of this excersize is that the CircuitBreaker can be created in multiple ways.
#          I've used a Class to keep the state of the *state*-machine
#          I've used recursion to call back into the function we just started, with the main difference being:
#             1: Some retry-count was incremented
#             2: The State was changed, so a different codeblock will be executed
#
#          I chose to not use an Enum for Python, just because I got lazy :)

class CircuitBreaker(object):
    def __init__(self, url, time_out, retry_count):
        self.url = url
        self.time_out = time_out
        self.retry_count = retry_count
        self.status = "OPEN"
        self.retries = 0

    """tries to get the actual response from the URL through the requests library"""

    def call(self):
        # try to do HTTP request
        try:
            response = requests.get(self.url)
            rand = random.randrange(0, 2)  # success-rate of 50%
            if rand:  # if rand == 1
                self.status = "SUCCESS"
                return response.json()
            else:
                raise Exception("fake bad response")
        except Exception as e:
            print(e)
            raise e

    """this is the recursive, state-machine function"""

    def start(self):  # state-machine
        print(self.status)

        if self.status == "OPEN":
            try:
                return self.call()
            except Exception as bad_response:
                # do something with bad_response...

                self.status = "HALF-OPEN"
                self.retries += 1
                return self.start()  # lets try again with our status set to HALF-OPEN

        if self.status == "HALF-OPEN":
            time.sleep(self.time_out)
            try:
                return self.call()
            except:
                if self.retries < self.retry_count:  # retry until the retries is exceeded
                    self.retries += 1
                    return self.start()
                else:
                    self.status = "CLOSED"  # set status to CLOSED
                    return self.start()

        if self.status == "CLOSED":  # raise our final error
            raise Exception(f"CircuitBreaker failed for {self.url} : {self.retries} times : timeout {self.time_out}")


# we create a new CB
cb = CircuitBreaker('https://jsonplaceholder.typicode.com/todos', 1, 3)

try:
    response = cb.start()
    print('response =', response)
except Exception as circuit_breaker_exception:
    print(circuit_breaker_exception)

print()
print("total retries:", cb.retries)
print("final status:", cb.status)
