using coursework.Core;
using coursework.Elements;
using coursework.Statistics;

namespace coursework.Experiments
{
    public static class Verification
    {
        private static readonly SimulationConfig _baseVerifyCfg = new(
            S: 20, s: 3, Policy: ReplenishmentPolicyType.sS, Qfixed: 3,
            StockMeanTime: 1200, AdminDelay: 60, KittingUniform: (40, 80),
            DeliveryUniform: (55, 65), WarmupTime: 0, RunTime: 10000,
            Replications: 10, Seed: 1
        );

        private static ReplicationResult RunSingleRep(SimulationConfig cfg, bool debugTrace = false)
        {
            var model = coursework.Scenarios.InventoryModelBuilder.Build(cfg, replicationIndex: 0);
            model.Run(cfg.WarmupTime + cfg.RunTime, cfg.WarmupTime);

            var warehouse = (Process)model.GetElementByName("Warehouse");
            var stats = warehouse.GetStats(cfg.RunTime);

            return new ReplicationResult(stats.AvgLoad, stats.DowntimeShare, stats.StockoutShare);
        }

        public static void RunAll()
        {
            Console.WriteLine("\n--- DETAILED MODEL VERIFICATION ---");
            Console.WriteLine($"--- (Runtime={_baseVerifyCfg.RunTime} min, Warmup={_baseVerifyCfg.WarmupTime} min, Seed={_baseVerifyCfg.Seed}) ---");

            Console.WriteLine("\n--- Scenario B: Baseline (s,S)=(3,20) ---");
            ExperimentRunner.RunSeries($"Baseline (Reps={_baseVerifyCfg.Replications})", _baseVerifyCfg);

            Console.WriteLine("\n--- Extreme Condition Checks (1 Replication) ---");

            var cfg2 = _baseVerifyCfg with { AdminDelay = 0, KittingUniform = (0, 0), DeliveryUniform = (0, 0), Replications = 1 };
            Console.Write("2. Instant Replenishment:".PadRight(35));
            var r2 = RunSingleRep(cfg2);
            bool ok2 = r2.DowntimeShare < 0.0001;
            Console.WriteLine($" -> AvgInv={r2.AvgInventory:F2}, Downtime={r2.DowntimeShare:P4}. Expected: ~0.0%. OK={ok2}");

            Console.WriteLine($"\n--- Sensitivity Checks ({_baseVerifyCfg.Replications} Replications Each) ---");

            var cfg5 = _baseVerifyCfg with { s = 5 };
            ExperimentRunner.RunSeries("5. Increase s (to 5)", cfg5);

            var cfg6 = _baseVerifyCfg with { AdminDelay = 90 };
            ExperimentRunner.RunSeries("6. Increase T_admin (+50%)", cfg6);

            var cfg7 = _baseVerifyCfg with { KittingUniform = (60, 120) };
            ExperimentRunner.RunSeries("7. Increase T_kit (+50% avg)", cfg7);

            var cfg8 = _baseVerifyCfg with { Policy = ReplenishmentPolicyType.sQ, Qfixed = 3 };
            ExperimentRunner.RunSeries("8. Change Policy to (s,Q=3)", cfg8);
        }
    }
}