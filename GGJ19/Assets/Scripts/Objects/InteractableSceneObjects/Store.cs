using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.StorageSystem;

namespace Assets.Scripts.Objects.InteractableSceneObjects
{
    public class Store : InteractableSceneObject
    {
        private Storage _storage;

        public override void Start()
        {
            base.Start();

            _storage = FindObjectOfType<Storage>();
        }

        public override void Interact()
        {
            _gameManager.GameFreezed = true;
            _inputManager.SetDefaultCursor();
            _storage.OpenStorage();
        }
    }
}
