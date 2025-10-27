namespace coursework.Statistics
{
    public static class StatsHelper
    {
        public static (double mean, double halfCI) MeanAndCI95(IReadOnlyList<double> xs)
        {
            if (xs.Count < 2) return (xs.Count == 1 ? xs[0] : 0, 0);

            double mean = 0;
            foreach (var x in xs) mean += x;
            mean /= xs.Count;

            double var = 0;
            foreach (var x in xs) var += (x - mean) * (x - mean);
            var /= (xs.Count - 1); 

            double sd = Math.Sqrt(var);

            double t = 1.96;

            double half = t * sd / Math.Sqrt(xs.Count);
            return (mean, half);
        }
    }
}
