namespace coursework.Generators
{
    class UniformDelayGenerator : IDelayGenerator
    {
        private double _min;
        private double _max;
        private Random _random;

        public UniformDelayGenerator(double min, double max, Random randomGenerator)
        {
            if (max < min) throw new ArgumentException("min > max");
            _min = min;
            _max = max;
            _random = randomGenerator;
        }

        public double GetDelay()
        {
            return _min + _random.NextDouble() * (_max - _min);
        }
    }
}