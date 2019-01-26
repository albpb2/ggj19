using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Karma : MonoBehaviour
    {
        private const int MinKarma = 0;
        private const int MaxKarma = 100;
        private const int HalfKarma = 50;

        [SerializeField]
        private SpriteRenderer _greenBar;
        [SerializeField]
        private SpriteRenderer _redBar;
        [SerializeField]
        private int _initialKarma;

        private int _halfKarma;

        public int Amount { get; set; }

        public void Start()
        {
            Amount = Math.Max(Math.Min(_initialKarma, MaxKarma), MinKarma);
        }

        public void Increment(int quantity)
        {
            Amount = Math.Min(Amount + quantity, MaxKarma);
            UpdateBars();
        }

        public void Reduce(int quantity)
        {
            Amount = Math.Max(Amount - quantity, MinKarma);
            UpdateBars();
        }

        private void UpdateBars()
        {
            _greenBar.transform.localScale = new Vector3(
                Math.Max(Amount - HalfKarma, 0),
                _greenBar.transform.localScale.y,
                _greenBar.transform.localScale.z);
            _greenBar.transform.localPosition = new Vector3(
                0.02f * _greenBar.transform.localScale.x,
                _greenBar.transform.localPosition.y,
                _greenBar.transform.localPosition.z);

            _redBar.transform.localScale = new Vector3(
                Math.Max(0 - (Amount - HalfKarma), MinKarma),
                _redBar.transform.localScale.y,
                _redBar.transform.localScale.z);
            _redBar.transform.localPosition = new Vector3(
                0 - 0.03f * _redBar.transform.localScale.x,
                _redBar.transform.localPosition.y,
                _redBar.transform.localPosition.z);
        }
    }
}
