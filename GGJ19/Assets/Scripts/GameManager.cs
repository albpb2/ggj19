using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private Image _pauseImage;

        public bool Pause { get; set; }
        private bool _a;

        public bool GameFreezed
        {
            get => _a;
            set
            {
                _a = value;
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!IsOnAnyPauseState())
                {
                    PauseGame();
                }
                else if (IsOnPauseScreen())
                {
                    ContinueGame();
                }
            }
        }

        public void PauseGame()
        {
            GameFreezed = true;
            _pauseImage.gameObject.SetActive(true);
            Pause = true;
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void ContinueGame()
        {
            GameFreezed = false;
            _pauseImage.gameObject.SetActive(false);
            Pause = false;
        }

        private bool IsOnAnyPauseState()
        {
            return GameFreezed || Pause;
        }

        private bool IsOnPauseScreen() => Pause;
    }
}
