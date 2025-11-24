using coursework.Experiments;

namespace coursework
{
    class Program
    {
        static void Main()
        {
            WarmupExperiment.Run();
            Verification.RunAll();
            SensitivityAnalysis.RunAll();

            var baseCfg = new SimulationConfig(
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

            Console.WriteLine("\n--- MAIN EXPERIMENTS ---");

            ExperimentRunner.RunSeries("(s,S) = (3,20)", baseCfg with { Policy = ReplenishmentPolicyType.sS });
            ExperimentRunner.RunSeries("(s,Q) = (3,3)", baseCfg with { Policy = ReplenishmentPolicyType.sQ });

            Console.WriteLine("\n--- All experiments finished. Press any key to exit. ---");
            Console.ReadKey();
        }
    }
}