using System;
using System.Collections.Generic;

namespace Assets.Scripts.Conversation
{
    [Serializable]
    public abstract class DialogLine
    {
        public int LineId;

        public bool OwnLine;

        public string Text;

        public List<List<int>> PossibleResponses;
    }
}
