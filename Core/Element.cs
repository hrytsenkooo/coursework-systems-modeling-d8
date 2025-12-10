using coursework.Core.Routing;
using coursework.Generators; 

namespace coursework.Core
{
    public abstract class Element
    {
        public string Name { get; private set; }
        public double Tnext { get; set; }
        public double Tcurr { get; set; }
        public int Id { get; set; }

        public IRouter Router { get; set; }

        protected IDelayGenerator _delayGen;
        protected Model _model;

        public int QuantityProcessed { get; protected set; }

        private static int _nextId = 0;

        public Element(string name, IDelayGenerator delayGen = null)
        {
            Id = _nextId++;
            Name = name;
            _delayGen = delayGen;
            Tnext = double.MaxValue;
        }

        public double GetDelay() => _delayGen?.GetDelay() ?? 0.0;

        public virtual void InAct(Item item) { }

        public virtual void OutAct()
        {
            QuantityProcessed++;
        }

        public virtual void DoStatistics(double delta) { }

        public static void ResetId() => _nextId = 0;
        public void SetModel(Model m) => _model = m;
    }
}