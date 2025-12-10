namespace coursework.Core
{
    public class Item
    {
        public double CreationTime { get; }
        public int Quantity { get; set; } = 1;
        public bool IsReplenishment { get; set; } = false;

        public Item(double time)
        {
            CreationTime = time;
        }
        public Item()
        {
            CreationTime = 0.0;
        }
    }
}