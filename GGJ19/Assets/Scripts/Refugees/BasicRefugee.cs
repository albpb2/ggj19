using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Conversation;
using Assets.Scripts.Extensions;

namespace Assets.Scripts.Refugees
{
    public class BasicRefugee : RefugeeWithBasicNeeds
    {
        public override void Talk()
        {
            var possibleLines = new List<int>();

            if (!HungerResolved)
            {
                possibleLines.AddRange(BasicDialogLine.HungerLines);
            }

            if (!ThirstResolved)
            {
                possibleLines.AddRange(BasicDialogLine.ThirstLines);
            }

            if (!HungerResolved && !ThirstResolved)
            {
                possibleLines.AddRange(BasicDialogLine.HungerAndThirstLines);
            }

            if (Ill & !IllnessResolved)
            {
                possibleLines.AddRange(BasicDialogLine.IllnessLines);
            }

            if (HungerResolved && ThirstResolved && (!Ill || IllnessResolved))
            {
                possibleLines.AddRange(BasicDialogLine.GreetingLines);
            }

            var lineId = possibleLines.GetRandomElement();
            var line = _dialogManager.BasicDialogLines.SingleOrDefault(l => l.LineId == lineId);

            _dialogManager.WriteLine(line, Name);
        }
    }
}
