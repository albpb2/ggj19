using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class SceneManager : MonoBehaviour
    {
        public void OpenScene(int sceneIndex)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
        }
    }
}
