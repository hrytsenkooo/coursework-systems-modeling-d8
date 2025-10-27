namespace coursework.Generators
{
    public class ConstantDelayGenerator : IDelayGenerator
    {
        private double _averageDelay;

        public ConstantDelayGenerator(double averageDelay)
        {
            _averageDelay = averageDelay;
        }

        public double GetDelay()
        {
            return _averageDelay;
        }
    }
}
