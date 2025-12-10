using coursework.Elements;
using coursework.Scenarios;
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
                var model = InventoryModelBuilder.Build(cfg, replicationIndex: r);

                model.Run(cfg.WarmupTime + cfg.RunTime, cfg.WarmupTime);

                var warehouse = (Process)model.GetElementByName("Warehouse");
                var stats = warehouse.GetStats(cfg.RunTime);

                results_AvgInv.Add(stats.AvgLoad);
                results_Downtime.Add(stats.DowntimeShare);
                results_Stockout.Add(stats.StockoutShare);
            }

            var (mInv, ciInv) = StatsHelper.MeanAndCI95(results_AvgInv);
            var (mDt, ciDt) = StatsHelper.MeanAndCI95(results_Downtime);
            var (mSo, ciSo) = StatsHelper.MeanAndCI95(results_Stockout);

            Console.WriteLine("--- Results (95% C.I.) ---");
            Console.WriteLine($"Avg inventory: {mInv:F3} ± {ciInv:F3} units");
            Console.WriteLine($"Downtime share: {mDt:P2} ± {ciDt:P2}");
            Console.WriteLine($"Stockout share: {mSo:P2} ± {ciSo:P2}");
        }
    }
}