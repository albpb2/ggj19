﻿using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Extensions;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.Conversation
{
    public class DialogManager : MonoBehaviour
    {
        [SerializeField]
        private TextAsset _basicDialogLines;
        [SerializeField]
        private TextAsset _mediumDialogLines;
        [SerializeField]
        protected DialogBox _dialogBox;

        public List<BasicDialogLine> BasicDialogLines { get; set; }
        public List<MediumDialogLine> MediumDialogLines { get; set; }

        public void Start()
        {
            BasicDialogLines = ParseBasicDialogNames();
            MediumDialogLines = ParseMediumDialogNames();
        }

        private List<BasicDialogLine> ParseBasicDialogNames()
        {
            return JsonConvert.DeserializeObject<List<BasicDialogLine>>(_basicDialogLines.text);
        }

        private List<MediumDialogLine> ParseMediumDialogNames()
        {
            return JsonConvert.DeserializeObject<List<MediumDialogLine>>(_mediumDialogLines.text);
        }

        public void WriteBasicDialogLine(DialogLine dialogLine, string refugeeName, bool isObjectRequest)
        {
            WriteDialogLine(dialogLine, refugeeName, BasicDialogLines.Select(l => l as DialogLine).ToList(), isObjectRequest);
        }

        public void WriteMediumDialogLine(DialogLine dialogLine, string refugeeName, bool isObjectRequest)
        {
            WriteDialogLine(dialogLine, refugeeName, MediumDialogLines.Select(l => l as DialogLine).ToList(), isObjectRequest);
        }

        private void WriteDialogLine(DialogLine dialogLine, string refugeeName, List<DialogLine> dialogLines, bool isObjectRequest)
        {
            const string OwnLineHeader = "You";
            var name = dialogLine.OwnLine ? OwnLineHeader : refugeeName;

            _dialogBox.ShowText(name, dialogLine.Text, isObjectRequest);

            if (dialogLine.PossibleResponses.Any(possibleResponse => possibleResponse.Any()))
            {
                var responseSet = dialogLine.PossibleResponses.Where(possibleResponse => possibleResponse.Any())
                    .ToList().GetRandomElement();

                var lineId = responseSet.GetRandomElement();
                dialogLine = dialogLines.SingleOrDefault(l => l.LineId == lineId);

                WriteBasicDialogLine(dialogLine, refugeeName, isObjectRequest);
            }
        }
    }
}
