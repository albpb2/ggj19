using Assets.Scripts.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Assets.Scripts.Conversation
{
    public class DialogBox : MonoBehaviour, IUIHideable
    {
        [SerializeField]
        private Image _textBox;
        [SerializeField]
        private Text _textBoxName;
        [SerializeField]
        private Text _textBoxText;
        [SerializeField]
        private int _speed = 1;
        [SerializeField]
        private Button _closeButton;
        [SerializeField]
        private Button _giveObjectButton;
        [SerializeField]
        private GameManager _gameManager;
        [SerializeField]
        private Character _character;

        private List<string> _names = new List<string>();
        private List<string> _textsToRead = new List<string>();
        private int _index;
        private bool _isObjectRequest;

        public bool IsOpen => _textBox?.gameObject.activeSelf ?? false;

        public void Update()
        {
            if (!_textsToRead.Any())
            {
                return;
            }

            var textToRead = _textsToRead.First();
            if (_index < textToRead.Length)
            {
                var charsToRead = Mathf.Min(_speed, textToRead.Length - _index);
                _textBoxText.text += textToRead.Substring(_index, charsToRead);
                _textBoxName.text = _names.First();

                _index += charsToRead;
                if (_index >= textToRead.Length)
                {
                    _textBoxText.text += Environment.NewLine;
                    _textBoxText.text += " ";
                }
            }

            if (Input.GetMouseButton(0) && _index >= textToRead.Length && _textsToRead.Any())
            {
                _textsToRead.RemoveAt(0);
                if (_textsToRead.Count == 1 && _isObjectRequest)
                {
                    _textBoxText.text = string.Empty;
                    _textBoxName.text = string.Empty;
                    _giveObjectButton.gameObject.SetActive(true);
                    _closeButton.gameObject.SetActive(true);
                }
                else if (_textsToRead.Any())
                {
                    _textBoxText.text = string.Empty;
                    _textBoxName.text = string.Empty;
                }

                _names.RemoveAt(0);
                _index = 0;
                if (!_textsToRead.Any() && !_isObjectRequest)
                {
                    Close();
                }
            }
        }

        public void ShowText(string name, string text, bool isObjectRequest)
        {
            _gameManager.GameFreezed = true;

            _textBox.gameObject.SetActive(true);

            if (_textsToRead == null || !_textsToRead.Any())
            {
                _textBoxText.text = string.Empty;
                _textBoxName.text = string.Empty;
                _index = 0;
            }

            _names.Add(name);
            _textsToRead.Add(text);

            _isObjectRequest = isObjectRequest;
        }

        public void HideUIElement()
        {
            if (IsOpen)
            {
                Close();
            }
        }

        public void Close()
        {
            Hide();

            _character.EndInteraction();
            _gameManager.GameFreezed = false;
        }

        public void OpenBag()
        {
            Hide();
            _character.OpenBag();
        }

        private void Hide()
        {
            _textBoxText.text = string.Empty;
            _textBoxName.text = string.Empty;

            _giveObjectButton.gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(false);
            _giveObjectButton.gameObject.SetActive(false);

            _textBox.gameObject.SetActive(false);
        }
    }
}
