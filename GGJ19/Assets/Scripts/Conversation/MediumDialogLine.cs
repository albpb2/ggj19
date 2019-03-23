using System;
using System.Collections.Generic;

namespace Assets.Scripts.Conversation
{
    [Serializable]
    public class MediumDialogLine : DialogLine
    {
        public bool AdultsOnly { get; set; }

        public static int[] GreetingLines = new[]
        {
            4
        };

        public static int[] ThanksLines = new[]
        {
            5, 6, 7
        };

        public static int[] WrongChoiceLines = new[]
        {
            8, 9
        };

        public List<string> RelatedObjects;
    }
}
