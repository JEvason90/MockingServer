# Mocking Server and Circuit Breaker

This project is meant for learning and demo purposes of a circuit breaker pattern.

## Rust Server

This server currently echos the body of a request and will "randomly" return a 503 on some requests to simulate a dependency outage

## Azure Function

This function is written in C# and has a rough and crude circuit breaker applied (please note that this is PoC and doesn't cover concepts like open, closed and half open, it's simply to demonstrate the theory)