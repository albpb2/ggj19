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
        private const int MaxLine = 20;

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

            var validObjectTypes = new List<PortableObjectType>();

            foreach (var dialogLineRelatedObject in DialogLine.RelatedObjects)
            {
                if (Enum.TryParse(dialogLineRelatedObject, out PortableObjectType validObjectType))
                {
                    validObjectTypes.Add(validObjectType);
                }
            }
            
            if (!NostalgiaResolved && validObjectTypes.Contains(objectType))
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
            if (!RefugeeCountsForKarma())
            {
                return;
            }

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

        public void ChooseLine()
        {
            var otherRefugees = FindObjectsOfType<MediumRefugee>().Where(refugee => refugee != this).ToList();
            var otherLines = new List<int>();
            if (otherRefugees != null || otherRefugees.Any())
            {
                otherLines = otherRefugees.Select(
                    refugee => refugee.DialogLine.LineId).ToList();
            }

            var random = new Random();
            var dialogLineId = 0;
            while (dialogLineId == 0)
            {
                dialogLineId = random.Next(MinLine, MaxLine + 1);
                if (otherLines.Contains(dialogLineId))
                {
                    dialogLineId = 0;
                }
            }

            DialogLine = _dialogManager.MediumDialogLines.SingleOrDefault(line => line.LineId == dialogLineId);
        }
    }
}
