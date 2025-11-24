using coursework.Generators; 
using coursework.Statistics; 

namespace coursework.Core
{
    public class Model
    {
        public double Now { get; private set; }
        public int I { get; private set; }            
        public bool ReorderPending { get; private set; } 
        public int Qcurrent { get; private set; }     

        private double _lastTime;
        private double _areaInventory;
        private double _downtimeTime;
        private int _demandCount;
        private int _stockoutDemandCount;
        private readonly double _warmupTime;
        private bool IsStockout => I == 0;
        private bool InWarmup => Now < _warmupTime;
        private readonly bool _debugTrace;

        private readonly SimulationConfig _cfg;
        private readonly Random _rng;
        private readonly IDelayGenerator _exp;
        private readonly IDelayGenerator _admin;
        private readonly IDelayGenerator _kit;
        private readonly IDelayGenerator _del;
        private readonly PriorityQueue<SimEvent, double> _fel = new();

        public Model(SimulationConfig cfg, int replicationIndex = 0, bool debugTrace = false)
        {
            _cfg = cfg;
            _warmupTime = cfg.WarmupTime;
            _debugTrace = debugTrace;

            I = cfg.S; 

            int seed = cfg.Seed ?? Environment.TickCount;
            _rng = new Random(unchecked(seed + replicationIndex * 7919));

            _exp = new ExponentialDelayGenerator(cfg.MeanInterarrival, _rng);
            _admin = new ConstantDelayGenerator(cfg.AdminDelay);
            _kit = new UniformDelayGenerator(cfg.KittingUniform.Min, cfg.KittingUniform.Max, _rng);
            _del = new UniformDelayGenerator(cfg.DeliveryUniform.Min, cfg.DeliveryUniform.Max, _rng);
        }

        private void Schedule(EventType type, double time, ServiceNode? node = null) => _fel.Enqueue(new SimEvent(time, type, node), time);

        private void ScheduleServiceCompletion(ServiceNode node, double delay) => Schedule(EventType.ServiceCompletion, Now + delay, node);

        private double NextExp() => _exp.GetDelay();
        private double NextAdmin() => _admin.GetDelay();
        private double NextKit() => _kit.GetDelay();
        private double NextDel() => _del.GetDelay();

        public ReplicationResult Run()
        {
            _fel.Clear();
            Schedule(EventType.DemandArrival, Now + NextExp());

            double stopAt = _cfg.WarmupTime + _cfg.RunTime;

            while (_fel.Count > 0)
            {
                var ev = _fel.Dequeue();

                if (ev.Time > stopAt)
                {
                    Now = stopAt;
                    AccumulateContinuousStats(); 
                    break;
                }

                Now = ev.Time;
                AccumulateContinuousStats();

                switch (ev.Type)
                {
                    case EventType.DemandArrival:
                        HandleDemandArrival();
                        Schedule(EventType.DemandArrival, Now + NextExp());
                        break;

                    case EventType.ServiceCompletion:
                        HandleServiceCompletion(ev.Node!.Value);
                        break;
                }
            }

            double horizon = _cfg.RunTime;
            double avgInv = _areaInventory / horizon;
            double downtimeShare = _downtimeTime / horizon;
            double stockoutShare = (_demandCount > 0) ? (double)_stockoutDemandCount / _demandCount : 0.0;

            return new ReplicationResult(avgInv, downtimeShare, stockoutShare);
        }

        private void HandleDemandArrival()
        {
            int Iprev = I;
            if (!InWarmup) _demandCount++;

            if (I > 0) 
            {
                I -= 1; 
                if (_debugTrace) Console.WriteLine($"t={Now:F2}: demand, I: {Iprev} -> {I}");
            }
            else
            {
                if (!InWarmup) _stockoutDemandCount++; 
                if (_debugTrace) Console.WriteLine($"t={Now:F2}: demand, STOCKOUT persists (I=0)");
            }

            if (!ReorderPending && Iprev > _cfg.s && I <= _cfg.s)
            {
                ReorderPending = true;

                Qcurrent = _cfg.Policy == ReplenishmentPolicyType.sS
                    ? Math.Max(0, _cfg.S - I) 
                    : _cfg.Qfixed;             

                if (_debugTrace) Console.WriteLine($"t={Now:F2}: TRIGGER reorder, Q={Qcurrent}, I={I}");

                ScheduleServiceCompletion(ServiceNode.Admin, NextAdmin());
            }
        }

        private void HandleDelivery()
        {
            int before = I;
            I = Math.Min(_cfg.S, I + Qcurrent);
            ReorderPending = false; 
            Qcurrent = 0;

            if (_debugTrace)
                Console.WriteLine($"t={Now:F2}: Delivery arrived (Q={Qcurrent}), I: {before} -> {I}");
        }


        private void AccumulateContinuousStats()
        {
            double from = Math.Max(_lastTime, _warmupTime);
            double to = Now;

            if (to > from)
            {
                double dt = to - from; 

                _areaInventory += I * dt;

                if (IsStockout)
                {
                    _downtimeTime += dt;
                }
            }
            _lastTime = Now; 
        }

        private void HandleServiceCompletion(ServiceNode node)
        {
            switch (node)
            {
                case ServiceNode.Admin:
                    if (_debugTrace) Console.WriteLine($"t={Now:F2}: admin done → start kitting");
                    ScheduleServiceCompletion(ServiceNode.Kitting, NextKit());
                    break;

                case ServiceNode.Kitting:
                    if (_debugTrace) Console.WriteLine($"t={Now:F2}: kitting done → start delivery");
                    ScheduleServiceCompletion(ServiceNode.Delivery, NextDel());
                    break;

                case ServiceNode.Delivery:
                    if (_debugTrace) Console.WriteLine($"t={Now:F2}: delivery arrived");
                    HandleDelivery();
                    break;
            }
        }
    }

}