using System;
using System.Collections.Generic;

namespace Assets.Scripts.Conversation
{
    [Serializable]
    public class BasicDialogLine
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

        public int LineId;

        public bool OwnLine;

        public string Text;

        public List<List<int>> PossibleResponses;
    }
}
