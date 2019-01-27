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
        private Bag _bag;

        public Refugee InteractingWith { get; set; }

        public void Start()
        {
            _karma = GetComponent<Karma>();
            _bag = GetComponent<Bag>();
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
            _bag.Items.Remove(_bag.Items.First(item => item.Type == objectType));
        }
    }
}
