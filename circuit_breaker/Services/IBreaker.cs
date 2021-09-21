namespace CircuitBreaker.Services
{
    public interface IBreaker
    {
        CircuitState GetCircuitState();
        void UpdateCircuitState(bool state, string message);
        void AddErrorCount();
        int GetErrorCount();
        void ResetErrorCount();
    }
}