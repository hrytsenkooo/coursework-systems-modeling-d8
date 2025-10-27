namespace coursework
{
    public enum ReplenishmentPolicyType
    {
        sS,
        sQ
    }

    public record SimulationConfig(
        double MeanInterarrival,     
        int S,                      
        int s,                       
        ReplenishmentPolicyType Policy,
        int Qfixed,                  
        double AdminDelay,           
        (double Min, double Max) KittingUniform,  
        (double Min, double Max) DeliveryUniform, 
        double WarmupTime,           
        double RunTime,              
        int Replications,            
        int? Seed = null            
    );
}