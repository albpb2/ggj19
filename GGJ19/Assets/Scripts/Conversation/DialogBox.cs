using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Assets.Scripts.Conversation
{
    public class DialogBox : MonoBehaviour
    {
        [SerializeField]
        private Image _textBox;
        [SerializeField]
        private Text _textBoxName;
        [SerializeField]
        private Text _textBoxText;
        [SerializeField]
        private int _speed = 1;

        private List<string> _names;
        private List<string> _textsToRead;
        private int _index;
        private GameManager _gameManager;
        private Character _character;

        public void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
            _character = FindObjectOfType<Character>();

            _names = new List<string>();
            _textsToRead = new List<string>();
        }

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
                    _textBoxText.text += "Click to continue.";
                }
            }

            if (Input.GetMouseButton(0) && _index >= textToRead.Length)
            {
                _textBoxText.text = string.Empty;
                _textBoxName.text = string.Empty;
                _textsToRead.RemoveAt(0);
                _names.RemoveAt(0);
                _index = 0;
                _speed = 1;
                if (!_textsToRead.Any())
                {
                    _textBox.gameObject.SetActive(false);
                    _gameManager.GameFreezed = false;
                    _character.EndInteraction();
                }
            }
        }

        public void ShowText(string name, string text)
        {
            _gameManager.GameFreezed = true;

            _textBox.gameObject.SetActive(true);

            if (!_textsToRead.Any())
            {
                _textBoxText.text = string.Empty;
                _textBoxName.text = string.Empty;
                _index = 0;
                _speed = 1;
            }

            _names.Add(name);
            _textsToRead.Add(text);
        }
    }
}
