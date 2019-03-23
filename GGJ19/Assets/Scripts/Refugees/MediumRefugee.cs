using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Conversation;
using Assets.Scripts.Extensions;
using Assets.Scripts.Objects.PortableObjects;
using Assets.Scripts.Refugees.Events;
using Assets.Scripts.StorageSystem;

namespace Assets.Scripts.Refugees
{
    public class MediumRefugee : RefugeeWithBasicNeeds
    {
        private const int MinLine = 10;
        private const int MaxLine = 18;

        private static PortableObjectType[] PossibleGifts = new[]
        {
            PortableObjectType.Ball,
            PortableObjectType.Book,
            PortableObjectType.Rose,
            PortableObjectType.Guitar,
            PortableObjectType.Bottle,
            PortableObjectType.Toy
        };

        public MediumDialogLine DialogLine { get; set; }
        public bool NostalgiaResolved { get; set; }
        public List<PortableObjectType> ValidObjectTypes { get; private set; }

        public override void Talk()
        {
            if (!NostalgiaResolved)
            {
                _dialogManager.WriteMediumDialogLine(DialogLine, Name);
            }
            else
            {
                base.Talk();
            }
        }

        public override void GiveObject(PortableObjectType objectType)
        {
            int lineId;
            MediumDialogLine line;
            
            if (!NostalgiaResolved && ValidObjectTypes.Contains(objectType))
            {
                lineId = MediumDialogLine.ThanksLines.GetRandomElement();
                line = _dialogManager.MediumDialogLines.SingleOrDefault(l => l.LineId == lineId);
                _dialogManager.WriteMediumDialogLine(line, Name);
                UpdateKarma(_refugeesSettings.NostalgiaResolvedPoints);
                NostalgiaResolved = true;
                _gameEventsManager.AddEvent(new RightHomeObjectEvent());
            }
            else
            {
                base.GiveObject(objectType);
                return;
            }
        }

        public override void LeaveCamp()
        {
            if (!NostalgiaResolved)
            {
                UpdateKarma(- _refugeesSettings.NostalgiaResolvedPoints);
                var randomNumber = _random.Next(0, 100);
                if (randomNumber < 50)
                {
                    FindObjectOfType<Storage>().AddGift(PossibleGifts.GetRandomElement());
                }
            }
            else
            {
                FindObjectOfType<Storage>().AddGift(PossibleGifts.GetRandomElement());
            }

            base.LeaveCamp();
        }

        public void SetRandomLine()
        {
            var mediumLines = GetPossibleLines();

            DialogLine = mediumLines.GetRandomElement();

            SelectValidObjectTypes();
        }

        public void SetLine(int lineId)
        {
            DialogLine = _dialogManager.MediumDialogLines.SingleOrDefault(line => line.LineId == lineId);

            SelectValidObjectTypes();
        }

        private List<MediumDialogLine> GetPossibleLines()
        {
            var otherRefugeesLineIds = GetOtherRefugeesLineIds();

            var mediumLines = new List<MediumDialogLine>(_dialogManager.MediumDialogLines.Skip(MinLine));

            if (IsChild)
            {
                mediumLines.RemoveAll(l => l.AdultsOnly);
            }

            var uniqueLines = mediumLines.Where(l => !otherRefugeesLineIds.Contains(l.LineId)).ToList();

            return uniqueLines.Any() ? uniqueLines : mediumLines;
        }

        private List<int> GetOtherRefugeesLineIds()
        {
            var otherRefugees = FindObjectsOfType<MediumRefugee>().Where(refugee => refugee != this).ToList();
            var otherLines = new List<int>();
            if (otherRefugees != null || otherRefugees.Any())
            {
                otherLines = otherRefugees.Select(
                    refugee => refugee.DialogLine.LineId).ToList();
            }
            return otherLines;
        }

        private void SelectValidObjectTypes()
        {
            ValidObjectTypes = new List<PortableObjectType>();
            foreach (var dialogLineRelatedObject in DialogLine.RelatedObjects)
            {
                if (Enum.TryParse(dialogLineRelatedObject, out PortableObjectType validObjectType))
                {
                    ValidObjectTypes.Add(validObjectType);
                }
            }
        }
    }
}
