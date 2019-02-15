namespace Assets.Scripts.Events
{
    public class DayEndedEvent : GameEvent
    {
        public int Day { get; set; }

        public int Karma { get; set; }
    }
}
