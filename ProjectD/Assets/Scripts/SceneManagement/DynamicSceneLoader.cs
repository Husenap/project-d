using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PD {
    public class DynamicSceneLoader : MonoBehaviour {
        public string sceneName = "";
        public float delayBeforeUnload = 5.0f;
        enum SceneState {
            Loaded,
            Unloaded,
            Loading,
            Unloading,
            WaitingToUnload,
        }

        private int counter = 0;
        private SceneState sceneState = SceneState.Unloaded;
        private Coroutine unloadCoroutine;

        private void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        void OnSceneLoaded(Scene s, LoadSceneMode _) {
            if (s.name == sceneName) {
                sceneState = SceneState.Loaded;
            }
        }
        void OnSceneUnloaded(Scene s) {
            if (s.name == sceneName) {
                sceneState = SceneState.Unloaded;
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Player")) {
                ++counter;

                if (unloadCoroutine != null) {
                    StopCoroutine(unloadCoroutine);
                    if (sceneState == SceneState.WaitingToUnload)
                        sceneState = SceneState.Loaded;
                }
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.CompareTag("Player")) {
                --counter;
            }
        }

        private void FixedUpdate() {
            if (sceneState == SceneState.Unloaded && counter > 0) {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                sceneState = SceneState.Loading;
            } else if (sceneState == SceneState.Loaded && counter == 0) {
                unloadCoroutine = StartCoroutine(WaitToUnloadCoroutine());
                sceneState = SceneState.WaitingToUnload;
            }
        }

        IEnumerator WaitToUnloadCoroutine() {
            yield return new WaitForSeconds(delayBeforeUnload);

            var scene = SceneManager.GetSceneByName(sceneName);
            if (scene.isLoaded) {
                SceneManager.UnloadSceneAsync(scene);
                sceneState = SceneState.Unloading;
            }
        }
    }
}
