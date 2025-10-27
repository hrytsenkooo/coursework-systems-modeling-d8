using coursework.Core; 

namespace coursework.Experiments
{
    public static class SensitivityAnalysis
    {
        private static readonly SimulationConfig _baseCfg = new(
            MeanInterarrival: 60,
            S: 20,
            s: 3, 
            Policy: ReplenishmentPolicyType.sS, 
            Qfixed: 3,
            AdminDelay: 60,
            KittingUniform: (40, 80),
            DeliveryUniform: (55, 65),
            WarmupTime: 2000,
            RunTime: 20000,
            Replications: 30,
            Seed: 12345
        );

        public static void RunAll()
        {
            Console.WriteLine($"\n--- SENSITIVITY ANALYSIS ({_baseCfg.Replications} reps, {_baseCfg.RunTime} time, {_baseCfg.WarmupTime} warmup) ---");

            Console.WriteLine("\n--- Baseline Scenario ---");
            ExperimentRunner.RunSeries($"Baseline (s,S)=({_baseCfg.s},{_baseCfg.S})", _baseCfg);

            Console.WriteLine("\n--- Searching for optimal 's' (Policy: sS, S=20) ---");
            int s_min = 1;  
            int s_max = 12; 

            for (int current_s = s_min; current_s <= s_max; current_s++)
            {
                if (current_s == _baseCfg.s && _baseCfg.Policy == ReplenishmentPolicyType.sS)
                {
                    Console.WriteLine($"\n--- Skipping s={current_s} (already run as Baseline) ---");
                    continue;
                }

                var cfg_s = _baseCfg with { s = current_s };

                ExperimentRunner.RunSeries($"Policy: (s,S)=({current_s},{_baseCfg.S})", cfg_s);
            }
            Console.WriteLine("\n--- Search for optimal 's' finished ---");

            Console.WriteLine("\n--- Policy Comparison ---");

            var cfg_sQ3_3 = _baseCfg with { Policy = ReplenishmentPolicyType.sQ, Qfixed = 3, s = 3 }; 
            ExperimentRunner.RunSeries("Policy (s,Q)=(3,3)", cfg_sQ3_3);

            int optimal_s = 9; 
            var cfg_sQopt_3 = _baseCfg with { Policy = ReplenishmentPolicyType.sQ, Qfixed = 3, s = optimal_s };

            var cfg_sQ5_20 = _baseCfg with { Policy = ReplenishmentPolicyType.sQ, Qfixed = 20, s = 5 };
            ExperimentRunner.RunSeries("Policy (s,Q)=(5,20)", cfg_sQ5_20); 


            Console.WriteLine("\n--- Investigating High 's' values with Extended Runtime ---");
            double extendedRunTime = 100000; 
            int extendedReps = _baseCfg.Replications; 
            int[] high_s_values = { 9, 10, 11, 12, 13, 14 }; 

            Console.WriteLine($"--- (Runtime={extendedRunTime} min, Warmup={_baseCfg.WarmupTime} min, Reps={extendedReps}) ---");

            foreach (int current_s in high_s_values)
            {
                var cfg_extended = _baseCfg with
                {
                    s = current_s,
                    RunTime = extendedRunTime,
                    Replications = extendedReps
                };

                ExperimentRunner.RunSeries($"Extended Run: Policy (s,S)=({current_s},{_baseCfg.S})", cfg_extended);
            }
            Console.WriteLine("\n--- Investigation for High 's' finished ---");

            /*
            Console.WriteLine("\n--- Optional: Other Sensitivity Checks ---");

            ExperimentRunner.RunSeries("Load: Low Demand (-20%)", _baseCfg with { MeanInterarrival = 60 * 1.2 });
            ExperimentRunner.RunSeries("Load: High Demand (+20%)", _baseCfg with { MeanInterarrival = 60 * 0.8 });

            ExperimentRunner.RunSeries("Bottleneck: Admin Delay +50%", _baseCfg with { AdminDelay = 60 * 1.5 });
            ExperimentRunner.RunSeries("Bottleneck: Kitting Time +50%", _baseCfg with { KittingUniform = (60, 120) }); 
            ExperimentRunner.RunSeries("Bottleneck: Delivery Time +50%", _baseCfg with { DeliveryUniform = (55 * 1.5, 65 * 1.5) });
            ExperimentRunner.RunSeries("Bottleneck: All Delays +50%", _baseCfg with {
                AdminDelay = 60 * 1.5,
                KittingUniform = (60, 120), 
                DeliveryUniform = (55 * 1.5, 65 * 1.5)
            });

            ExperimentRunner.RunSeries("Policy: Low Capacity S=15", _baseCfg with { S = 15 });
            ExperimentRunner.RunSeries("Policy: High Capacity S=30", _baseCfg with { S = 30 });
            */
        }
    }
}