using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class SummaryWriter : MonoBehaviour
    {
        [SerializeField]
        private Text _summaryText;

        public void Start()
        {
            var gameState = FindObjectOfType<GameState>();

            if (gameState == null)
            {
                _summaryText.gameObject.SetActive(false);
                return;
            }

            _summaryText.gameObject.SetActive(true);
            _summaryText.text = $"Congratulations! You helped for {gameState.Days} days.";
            Destroy(gameState.gameObject);
        }
    }
}
