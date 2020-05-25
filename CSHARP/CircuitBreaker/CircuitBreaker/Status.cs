namespace CircuitBreaker
{
    public enum Status
    {
        OPEN = 0,
        HALFOPEN = 1,
        CLOSED = 2,
        SUCCESS = 3,
    }
}
