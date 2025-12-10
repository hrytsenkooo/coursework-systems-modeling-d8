using coursework.Experiments;

namespace coursework
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Starting Coursework Experiments ===\n");
            WarmupExperiment.Run();

            Verification.RunAll();

            SensitivityAnalysis.RunAll();

            Console.WriteLine("\n--- FINAL BEST CONFIGURATION DEMO ---");
            var bestCfg = new SimulationConfig(
                S: 20,
                s: 9, 
                Policy: ReplenishmentPolicyType.sS,
                Qfixed: 3,
                StockMeanTime: 1200,
                AdminDelay: 60,
                KittingUniform: (40, 80),
                DeliveryUniform: (55, 65),
                WarmupTime: 2000,
                RunTime: 100000, 
                Replications: 30,
                Seed: 999
            );
            ExperimentRunner.RunSeries("Best Found: (s,S) = (9,20)", bestCfg);

            Console.WriteLine("\n--- All experiments finished. Press any key to exit. ---");
            Console.ReadKey();
        }
    }
}