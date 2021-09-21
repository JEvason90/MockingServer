namespace CircuitBreaker.Services
{
    public class Breaker : IBreaker
    {
        private CircuitState circuitState;

        private int _errorCount;

        public Breaker()
        {
            _errorCount = 0;
            circuitState = new CircuitState() {IsPaused = false, ErrorState = "NONE" };
        }

        public void Pause()
        {
            this.circuitState.IsPaused = true;
        }

        public void Play()
        {
            this.circuitState.IsPaused = false;
        }

        public CircuitState GetCircuitState()
        {
            return this.circuitState;
        }
        
        public void AddErrorCount()
        {
            _errorCount++;
        }

        public int GetErrorCount()
        {
            return _errorCount;
        }

        public void ResetErrorCount()
        {
            _errorCount = 0;
        }

        public void UpdateCircuitState(bool state, string message)
        {
            this.circuitState.IsPaused = state;
            this.circuitState.ErrorState = message;
        }
    }

    public class CircuitState
    {
        public bool IsPaused {get;set;}
        public string ErrorState {get;set;}
    }
}