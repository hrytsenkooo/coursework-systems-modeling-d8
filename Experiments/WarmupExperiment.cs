using coursework.Core;
using coursework.Statistics;

namespace coursework.Experiments
{
    public static class WarmupExperiment
    {
        public static void Run()
        {
            var baseCfg = new SimulationConfig(
                MeanInterarrival: 60,
                S: 20,
                s: 3,
                Policy: ReplenishmentPolicyType.sS,
                Qfixed: 3,
                AdminDelay: 60,
                KittingUniform: (40, 80),
                DeliveryUniform: (55, 65),
                WarmupTime: 0,      
                RunTime: 20000,      
                Replications: 30,
                Seed: 12345
            );

            double[] warmups = { 0, 500, 1000, 1500, 2000, 3000, 5000 };

            Console.WriteLine("\n--- WARMUP EXPERIMENT ---");
            Console.WriteLine("Warmup;MeanInv;CIInv;MeanDowntime;CIDowntime");

            foreach (var w in warmups)
            {
                var cfg = baseCfg with { WarmupTime = w };

                var results_AvgInv = new List<double>();
                var results_Downtime = new List<double>();

                for (int r = 0; r < cfg.Replications; r++)
                {
                    var model = new Model(cfg, replicationIndex: r);
                    var result = model.Run();

                    results_AvgInv.Add(result.AvgInventory);
                    results_Downtime.Add(result.DowntimeShare);
                }

                var (mInv, ciInv) = StatsHelper.MeanAndCI95(results_AvgInv);
                var (mDt, ciDt) = StatsHelper.MeanAndCI95(results_Downtime);

                Console.WriteLine($"{w};{mInv:F4};{ciInv:F4};{mDt:F4};{ciDt:F4}");
            }

            Console.WriteLine("--- WARMUP EXPERIMENT FINISHED ---");
        }
    }
}
