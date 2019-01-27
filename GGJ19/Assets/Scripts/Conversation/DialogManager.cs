using System.Collections.Generic;
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
        protected DialogBox _dialogBox;

        public List<BasicDialogLine> BasicDialogLines { get; set; }

        public void Start()
        {
            BasicDialogLines = ParseBasicDialogNames();
            _dialogBox = FindObjectOfType<DialogBox>();
        }

        private List<BasicDialogLine> ParseBasicDialogNames()
        {
            return JsonConvert.DeserializeObject<List<BasicDialogLine>>(_basicDialogLines.text);
        }

        public void WriteLine(BasicDialogLine dialogLine, string refugeeName)
        {
            const string OwnLineHeader = "You";
            var header = dialogLine.OwnLine ? OwnLineHeader : refugeeName;
            header += " : ";

            _dialogBox.ShowText(header + dialogLine.Text);

            if (dialogLine.PossibleResponses.Any(possibleResponse => possibleResponse.Any()))
            {
                var responseSet = dialogLine.PossibleResponses.Where(possibleResponse => possibleResponse.Any())
                    .ToList().GetRandomElement();

                var lineId = responseSet.GetRandomElement();
                dialogLine = BasicDialogLines.SingleOrDefault(l => l.LineId == lineId);

                WriteLine(dialogLine, refugeeName);
            }
        }
    }
}
