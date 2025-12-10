using coursework.Core;
using coursework.Core.Routing;
using coursework.Elements;
using coursework.Generators;

namespace coursework.Scenarios
{
    public static class InventoryModelBuilder
    {
        public static Model Build(SimulationConfig cfg, int replicationIndex = 0)
        {
            var model = new Model();
            int seed = (cfg.Seed ?? Environment.TickCount) + replicationIndex * 7919;
            Random rng = new Random(seed);

            var warehouse = new Process("Warehouse", cfg.S, 0, new ExponentialDelayGenerator(cfg.StockMeanTime, rng));

            var orderGen = new Create("OrderGen", 0.1);

            var admin = new Process("Admin", 1, -1, new ConstantDelayGenerator(cfg.AdminDelay));
            var kitting = new Process("Kitting", 1, -1, new UniformDelayGenerator(cfg.KittingUniform.Min, cfg.KittingUniform.Max, rng));
            var delivery = new Process("Delivery", 1, -1, new UniformDelayGenerator(cfg.DeliveryUniform.Min, cfg.DeliveryUniform.Max, rng));

            var consumedSink = new Dispose("Consumed");

            model.AddElement(warehouse);
            model.AddElement(orderGen);
            model.AddElement(admin);
            model.AddElement(kitting);
            model.AddElement(delivery);
            model.AddElement(consumedSink);

            for (int i = 0; i < cfg.S; i++) warehouse.InAct(new Item(0.0));

            var warehouseRouter = new PriorityRouter();
            warehouseRouter.AddRoute(new Route(consumedSink));
            warehouse.Router = warehouseRouter;

            var genRouter = new PriorityRouter();

            BlockPredicate triggerCondition = (m) =>
            {
                var wh = (Process)m.GetElementByName("Warehouse");
                var adm = (Process)m.GetElementByName("Admin");
                var kit = (Process)m.GetElementByName("Kitting");
                var del = (Process)m.GetElementByName("Delivery");

                bool isPending = (adm.State > 0 || adm.Queue > 0 ||
                                  kit.State > 0 || kit.Queue > 0 ||
                                  del.State > 0 || del.Queue > 0);

                if (wh.State <= cfg.s && !isPending)
                {
                    if (cfg.Policy == ReplenishmentPolicyType.sS)
                        m.LastOrderedQuantity = cfg.S - wh.State; 
                    else
                        m.LastOrderedQuantity = cfg.Qfixed;
                    return false; 
                }
                return true;
            };

            genRouter.AddRoute(new Route(admin, block: triggerCondition));
            orderGen.Router = genRouter;

            var adminRouter = new PriorityRouter();
            adminRouter.AddRoute(new Route(kitting));
            admin.Router = adminRouter;

            var kittingRouter = new PriorityRouter();
            kittingRouter.AddRoute(new Route(delivery));
            kitting.Router = kittingRouter;

            var deliveryRouter = new PriorityRouter();

            BatchSizeCalculator batchCalc = (m) =>
            {
                return m.LastOrderedQuantity; 
            };

            deliveryRouter.AddRoute(new Route(warehouse, batchCalc: batchCalc));
            delivery.Router = deliveryRouter;

            return model;
        }
    }
}