namespace coursework.Generators
{
    public class ExponentialDelayGenerator : IDelayGenerator
    {
        private double _averageDelay;

        private Random _random;

        public ExponentialDelayGenerator(double averageDelay, Random randomGenerator)
        {
            _averageDelay = averageDelay;
            _random = randomGenerator; 
        }

        public double GetDelay()
        {
            float randNumber = (float)_random.NextDouble();

            if (randNumber == 0)
            {
                randNumber = float.Epsilon;
            }

            return -_averageDelay * MathF.Log(randNumber);
        }
    }
}
