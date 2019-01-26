namespace Assets.Scripts.Refugees
{
    public class RefugeeWithBasicNeeds : Refugee
    {
        public bool HungerResolved { get; set; }

        public bool ThirstResolved { get; set; }

        public void ResetNeeds()
        {
            HungerResolved = false;
            ThirstResolved = false;
        }

        public override void WakeUp()
        {
            ResetNeeds();
        }
    }
}
