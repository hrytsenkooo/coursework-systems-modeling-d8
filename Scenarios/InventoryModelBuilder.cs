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

            var admin = new Process("Admin", 1, -1, new ConstantDelayGenerator(cfg.AdminDelay));
            var kitting = new Process("Kitting", 1, -1, new UniformDelayGenerator(cfg.KittingUniform.Min, cfg.KittingUniform.Max, rng));
            var delivery = new Process("Delivery", 1, -1, new UniformDelayGenerator(cfg.DeliveryUniform.Min, cfg.DeliveryUniform.Max, rng));

            var workshop = new Consumer("Consumed");

            model.AddElement(warehouse);
            model.AddElement(admin);
            model.AddElement(kitting);
            model.AddElement(delivery);
            model.AddElement(workshop);

            for (int i = 0; i < cfg.S; i++) warehouse.InAct(new Item(0.0));

            var warehouseRouter = new PriorityRouter();

            BlockPredicate triggerCondition = (m) =>
            {
                bool isPending = (admin.State > 0 || admin.Queue > 0 || kitting.State > 0 || kitting.Queue > 0 || delivery.State > 0 || delivery.Queue > 0);

                if (warehouse.State <= cfg.s && !isPending)
                {
                    if (cfg.Policy == ReplenishmentPolicyType.sS)
                        m.LastOrderedQuantity = cfg.S - warehouse.State;
                    else
                        m.LastOrderedQuantity = cfg.Qfixed;
                    return false;
                }
                return true;
            };

            warehouseRouter.AddRoute(new Route(admin, block: triggerCondition));
            warehouseRouter.AddRoute(new Route(workshop));

            warehouse.Router = warehouseRouter;

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