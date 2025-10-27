using coursework.Core;
using coursework.Statistics;

namespace coursework.Experiments
{
    public static class ExperimentRunner
    {
        public static void RunSeries(string title, SimulationConfig cfg)
        {
            var results_AvgInv = new List<double>();
            var results_Downtime = new List<double>();
            var results_Stockout = new List<double>();

            Console.WriteLine($"\n--- Running Series: {title} ({cfg.Replications} replications) ---");

            for (int r = 0; r < cfg.Replications; r++)
            {
                var model = new Model(cfg, replicationIndex: r);
                var result = model.Run();

                results_AvgInv.Add(result.AvgInventory);
                results_Downtime.Add(result.DowntimeShare);
                results_Stockout.Add(result.StockoutShare);
            }

            var (mInv, ciInv) = StatsHelper.MeanAndCI95(results_AvgInv);
            var (mDt, ciDt) = StatsHelper.MeanAndCI95(results_Downtime);
            var (mSo, ciSo) = StatsHelper.MeanAndCI95(results_Stockout);

            Console.WriteLine("--- Results (95% C.I.) ---");
            Console.WriteLine($"Avg inventory (avg load): {mInv:F3} ± {ciInv:F3} units");
            Console.WriteLine($"Downtime share (P(I=0)): {mDt:P2} ± {ciDt:P2}");
            Console.WriteLine($"Stockout share (P(demand|I=0)): {mSo:P2} ± {ciSo:P2}");
        }
    }
}