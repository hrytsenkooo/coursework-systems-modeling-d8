namespace coursework
{
    public enum ReplenishmentPolicyType
    {
        sS,
        sQ
    }

    public record SimulationConfig(
        int S,                         // кількість "пристроїв" D1–D20, одночасно N комплектів
        int s,                         // пороговий рівень для поповнення
        ReplenishmentPolicyType Policy,
        int Qfixed,                    // Q для (s,Q)
        double StockMeanTime,          // t_B: середній час exp(1200) на складі
        double AdminDelay,             // Dадмін
        (double Min, double Max) KittingUniform,   // Dкомплект
        (double Min, double Max) DeliveryUniform,  // Dдоставка
        double WarmupTime,
        double RunTime,
        int Replications,
        int? Seed = null
    );
}