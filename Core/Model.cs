namespace coursework.Core
{
    public class Model
    {
        private List<Element> _elements = new();
        public double Tcurr { get; private set; }
        public int LastOrderedQuantity { get; set; }

        public Element GetElementByName(string name) => _elements.FirstOrDefault(e => e.Name == name);
        public void AddElement(Element e) { _elements.Add(e); e.SetModel(this); }

        public void Run(double duration, double warmupTime)
        {
            Tcurr = 0.0;
            while (Tcurr < duration)
            {
                double tnext = double.MaxValue;
                foreach (var e in _elements)
                    if (e.Tnext < tnext) tnext = e.Tnext;

                if (tnext == double.MaxValue) break; 

                double dt = tnext - Tcurr;
                Tcurr = tnext;

                if (Tcurr > warmupTime)
                {
                    foreach (var e in _elements) e.DoStatistics(dt);
                }

                foreach (var e in _elements) e.Tcurr = Tcurr;

                var currentEventElement = _elements.FirstOrDefault(e => Math.Abs(e.Tnext - Tcurr) < 1e-9);
                if (currentEventElement != null)
                {
                    currentEventElement.OutAct();
                }
            }
        }
    }
}