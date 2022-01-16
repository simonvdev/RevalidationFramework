using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RevalidationFramework
{
    public class SceneChooser : MonoBehaviour
    {
        public OVRManager OVRManager;
        public Text LoadText;

        bool _loading = false;

        private PhysicsHand[] physicsHands;

        private void Start()
        {
            OVRManager = FindObjectOfType<OVRManager>();
            physicsHands = FindObjectsOfType<PhysicsHand>();
        }
        public void LoadScene(int sceneIndex)
        {
            if (!_loading)
                StartCoroutine(LoadSceneAsync(sceneIndex));
        }

        public void ToggleDebugMode()
        {
            for(int i = 0; i < physicsHands.Length; i++)
            {
                physicsHands[i].ToggleDebug();
            }
        }

        public void RecenterHMD()
        {
            OVRManager.display.RecenterPose();
        }

        IEnumerator LoadSceneAsync(int sceneIndex)
        {
            _loading = true;
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

            while (!asyncLoad.isDone)
            {
                if (LoadText) LoadText.text = "LOAD PROGRESS: " + (asyncLoad.progress * 100) + "%";

                yield return null;
            }
            _loading = false;
        }
    }
}

