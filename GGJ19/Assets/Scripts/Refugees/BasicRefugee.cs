namespace Assets.Scripts.Refugees
{
    public class BasicRefugee : Refugee
    {
        public BasicNeeds BasicNeeds { get; set; }

        public override void WakeUp()
        {
            BasicNeeds.Reset();
        }
    }
}
