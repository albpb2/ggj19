using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class Karma : MonoBehaviour
    {
        private const int MinKarma = -100;
        private const int MaxKarma = 100;
        
        [SerializeField]
        private int _initialKarma;
        [SerializeField]
        private int _centralBarX;
        [SerializeField]
        private int _minBarX;
        [SerializeField]
        private int _maxBarX;
        [SerializeField]
        private Image _goodBar;
        [SerializeField]
        private Image _badBar;

        private float _xPerKarmaUnit;

        public int Amount { get; set; }

        public void Start()
        {
            Amount = Math.Max(Math.Min(_initialKarma, MaxKarma), MinKarma);

            _xPerKarmaUnit = (float)(_maxBarX - _centralBarX) / 100;
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
            _goodBar.gameObject.SetActive(false);
            _badBar.gameObject.SetActive(false);

            if (Amount > 0)
            {
                _goodBar.gameObject.SetActive(true);
                var newX = _centralBarX + _xPerKarmaUnit * Amount;
                _goodBar.transform.localPosition = new Vector3(
                    newX,
                    _goodBar.transform.localPosition.y,
                    _goodBar.transform.localPosition.z);
            }
            else if (Amount < 0)
            {
                _badBar.gameObject.SetActive(true);
                var newX = _centralBarX + _xPerKarmaUnit * Amount;
                _badBar.transform.localPosition = new Vector3(
                    newX,
                    _badBar.transform.localPosition.y,
                    _badBar.transform.localPosition.z);
            }
        }
    }
}
