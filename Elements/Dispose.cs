using coursework.Core;

namespace coursework.Elements
{
    public class Dispose : Element
    {
        public int Accepted { get; private set; }

        public Dispose(string name) : base(name, null)
        {
        }

        public override void InAct(Item item)
        {
            Accepted++;
            base.InAct(item);
        }
    }
}