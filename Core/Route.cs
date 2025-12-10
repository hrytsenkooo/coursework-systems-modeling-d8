using coursework.Core;
using coursework.Elements;
using System.Linq; 

namespace coursework.Core.Routing
{
    public delegate bool BlockPredicate(Model model);
    public delegate int BatchSizeCalculator(Model model);

    public class Route
    {
        public Element Target { get; }
        public BlockPredicate BlockCondition { get; }
        public BatchSizeCalculator GetBatchSize { get; }

        public Route(Element target, BlockPredicate block = null, BatchSizeCalculator batchCalc = null)
        {
            Target = target;
            BlockCondition = block ?? (_ => false);
            GetBatchSize = batchCalc ?? (_ => 1);
        }
    }

    public interface IRouter
    {
        Route SelectRoute(Element current, Model model);
    }

    public class PriorityRouter : IRouter
    {
        private List<Route> _routes = new();
        public void AddRoute(Route r) => _routes.Add(r);

        public Route SelectRoute(Element current, Model model)
        {
            return _routes.FirstOrDefault(r => !r.BlockCondition(model));
        }
    }
}