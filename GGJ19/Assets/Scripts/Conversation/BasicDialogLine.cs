using System;

namespace Assets.Scripts.Conversation
{
    [Serializable]
    public class BasicDialogLine : DialogLine
    {
        public static int[] HungerLines = new[]
        {
            1, 5, 6, 7
        };

        public static int[] ThirstLines = new[]
        {
            7
        };

        public static int[] HungerAndThirstLines = new[]
        {
            6
        };

        public static int[] GreetingLines = new[]
        {
            8, 9
        };

        public static int[] IllnessLines = new[]
        {
            10, 11
        };

        public static int[] ThanksLines = new[]
        {
            12, 13
        };

        public static int[] WrongChoiceLines = new[]
        {
            14, 15
        };

        public static int[] ColdLines = new[]
        {
            16, 17
        };
    }
}
