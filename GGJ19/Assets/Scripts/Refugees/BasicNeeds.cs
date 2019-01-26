namespace Assets.Scripts.Refugees
{
    public class BasicNeeds
    {
        public bool HungerResolved { get; set; }

        public bool ThirstResolved { get; set; }

        public void Reset()
        {
            HungerResolved = false;
            ThirstResolved = false;
        }
    }
}
