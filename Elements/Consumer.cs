using coursework.Core;

namespace coursework.Elements
{
    public class Consumer : Element
    {
        public int ItemsConsumed { get; private set; }

        public Consumer(string name) : base(name, null)
        {
        }

        public override void InAct(Item item)
        {
            ItemsConsumed++;
            base.InAct(item);
        }
    }
}