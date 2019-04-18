using System;
using System.IO;
using System.Linq;
using Assets.Scripts.Objects.PortableObjects;
using Assets.Scripts.Refugees;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class Character : MonoBehaviour
    {
        [SerializeField]
        private Image _actionsBox;

        private Karma _karma;
        private BagHandler _bag;

        public Refugee InteractingWith { get; set; }

        public void Start()
        {
            _karma = GetComponent<Karma>();
            _bag = GetComponent<BagHandler>();
        }

        public void BeginInteraction(Refugee refugee)
        {
            InteractingWith = refugee;
        }

        public void EndInteraction()
        {
            InteractingWith = null;
        }

        public void TalkToRefugee()
        {
            InteractingWith.Talk();
        }

        public void OpenActionsBox()
        {
            _actionsBox.gameObject.SetActive(true);
        }

        public void GiveObjectToRefugee(PortableObjectType objectType)
        {
            InteractingWith.GiveObject(objectType);

            var itemToRemove = _bag.Items.FirstOrDefault(item => item.Type == objectType);
            if (itemToRemove != null)
            {
                _bag.Items.Remove(itemToRemove);
            }
        }

        public void OpenBag()
        {
            _bag.OpenDialogBag();
        }
    }
}
