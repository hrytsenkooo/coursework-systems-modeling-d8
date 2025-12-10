using coursework.Core;

namespace coursework.Elements
{
    public class Create : Element
    {
        private double _meanInterval;

        public Create(string name, double meanInterval) : base(name, null)
        {
            _meanInterval = meanInterval;
            Tnext = 0.0;
        }

        public override void OutAct()
        {
            base.OutAct();

            if (Router != null)
            {
                var route = Router.SelectRoute(this, _model);

                if (route != null)
                {
                    route.Target.InAct(new Item(_model.Tcurr));
                }
            }

            Tnext = _model.Tcurr + _meanInterval;
        }
    }
}