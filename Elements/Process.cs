using coursework.Core;
using coursework.Generators;

namespace coursework.Elements
{
    public class Process : Element
    {
        private int _queueCapacity;
        private int _channelsCount;

        public int State { get; private set; }
        public int Queue { get; private set; }

        private double[] _channelsTnext;

        private double _meanQueueArea;
        private double _meanLoadArea;
        private double _zeroStateTime;
        private int _failures;

        public Process(string name, int channels, int queueCapacity, IDelayGenerator delayGen)
            : base(name, delayGen)
        {
            _channelsCount = channels;
            _queueCapacity = queueCapacity;
            _channelsTnext = new double[_channelsCount];
            for (int i = 0; i < _channelsCount; i++) _channelsTnext[i] = double.MaxValue;
            Tnext = double.MaxValue;
        }

        public override void InAct(Item item)
        {
            base.InAct(item);
            int freeIndex = GetFreeChannelIndex();
            if (freeIndex != -1)
            {
                State++;
                _channelsTnext[freeIndex] = _model.Tcurr + GetDelay();
                UpdateTnext();
            }
            else
            {
                if (_queueCapacity == -1 || Queue < _queueCapacity)
                {
                    Queue++;
                }
                else
                {
                    _failures++;
                }
            }
        }

        public override void OutAct()
        {
            int channelIndex = GetMinTnextIndex();

            base.OutAct();

            _channelsTnext[channelIndex] = double.MaxValue;
            State--;

            if (Queue > 0)
            {
                Queue--;
                State++;
                _channelsTnext[channelIndex] = _model.Tcurr + GetDelay();
            }

            if (Router != null)
            {
                var route = Router.SelectRoute(this, _model);
                if (route != null)
                {
                    int batchSize = route.GetBatchSize(_model);
                    for (int k = 0; k < batchSize; k++)
                    {
                        route.Target.InAct(new Item(_model.Tcurr));
                    }
                }
            }

            UpdateTnext();
        }

        private int GetFreeChannelIndex() => Array.IndexOf(_channelsTnext, double.MaxValue);

        private int GetMinTnextIndex()
        {
            double min = double.MaxValue;
            int index = -1;
            for (int i = 0; i < _channelsCount; i++)
            {
                if (_channelsTnext[i] < min) { min = _channelsTnext[i]; index = i; }
            }
            return index;
        }

        private void UpdateTnext()
        {
            double min = double.MaxValue;
            for (int i = 0; i < _channelsCount; i++) if (_channelsTnext[i] < min) min = _channelsTnext[i];
            Tnext = min;
        }

        public override void DoStatistics(double delta)
        {
            _meanQueueArea += Queue * delta;
            _meanLoadArea += State * delta;

            if (State == 0) 
            {
                _zeroStateTime += delta;
            }
        }

        public (double AvgLoad, double DowntimeShare, double StockoutShare) GetStats(double time)
        {
            if (time == 0) return (0, 0, 0);
            double downtimeShare = _zeroStateTime / time;
            return (_meanLoadArea / time, downtimeShare, downtimeShare);
        }
    }
}