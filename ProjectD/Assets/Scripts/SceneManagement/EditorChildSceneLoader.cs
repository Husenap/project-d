using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PD {

    public class EditorChildSceneLoader : MonoBehaviour {
#if UNITY_EDITOR
        private const string DYNAMIC_SCENE_LOADERS = "DynamicSceneLoaders";

        [SerializeField]
        private List<SceneAsset> ChildScenesToLoadConfig;

        [SerializeField]
        private float expansionAmount = 100.0f;

        public void SaveSceneSetup() {
            ChildScenesToLoadConfig ??= new List<SceneAsset>();
            ChildScenesToLoadConfig.Clear();
            foreach (var sceneSetup in EditorSceneManager.GetSceneManagerSetup()) {
                ChildScenesToLoadConfig.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneSetup.path));
            }
            EditorUtility.SetDirty(this);

            var dynamicSceneLoaders = GameObject.Find(DYNAMIC_SCENE_LOADERS);
            DestroyImmediate(dynamicSceneLoaders);
            dynamicSceneLoaders = new GameObject(DYNAMIC_SCENE_LOADERS);
            dynamicSceneLoaders.transform.parent = transform;

            for (int i = 1; i < SceneManager.sceneCount; i++) {
                var scene = SceneManager.GetSceneAt(i);

                var bounds = GetBoundsOfScene(scene);

                var sceneLoader = new GameObject(scene.name);
                sceneLoader.transform.parent = dynamicSceneLoaders.transform;

                var boxCollider = sceneLoader.AddComponent<BoxCollider>();
                boxCollider.isTrigger = true;
                boxCollider.providesContacts = true;
                boxCollider.center = bounds.Value.center;
                boxCollider.size = bounds.Value.size;

                var rigidBody = sceneLoader.AddComponent<Rigidbody>();
                rigidBody.isKinematic = true;

                var dynamicSceneLoader = sceneLoader.AddComponent<DynamicSceneLoader>();
                dynamicSceneLoader.sceneName = scene.name;
            }

            var navMesh = FindFirstObjectByType<NavMeshSurface>();
            if (navMesh)
                navMesh.BuildNavMesh();
        }

        public void ResetSceneSetupToConfig() {
            var sceneAssetsToLoad = ChildScenesToLoadConfig;

            var sceneSetupToLoad = new List<SceneSetup>();
            foreach (var sceneAsset in sceneAssetsToLoad) {
                sceneSetupToLoad.Add(new SceneSetup() {
                    path = AssetDatabase.GetAssetPath(sceneAsset),
                    isActive = false,
                    isLoaded = true
                });
            }

            sceneSetupToLoad[0].isActive = true;
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.RestoreSceneManagerSetup(sceneSetupToLoad.ToArray());
        }

        private Bounds? GetBoundsOfScene(Scene scene) {
            var bounds = new Bounds();
            foreach (var go in scene.GetRootGameObjects()) {
                foreach (var renderer in go.GetComponentsInChildren<Renderer>()) {
                    if (renderer) {
                        if (bounds.extents == Vector3.zero)
                            bounds = renderer.bounds;
                        else
                            bounds.Encapsulate(renderer.bounds);
                    }
                }
            }
            bounds.Expand(expansionAmount);
            return bounds;
        }
#endif
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(EditorChildSceneLoader))]
    public class ChildSceneLoaderInspectorGUI : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            var currentInspectorObject = (EditorChildSceneLoader)target;

            GUI.enabled = !EditorApplication.isPlaying;

            if (GUILayout.Button("Save scene setup to config"))
                currentInspectorObject.SaveSceneSetup();
            if (GUILayout.Button("Reset scene setup from config"))
                currentInspectorObject.ResetSceneSetupToConfig();

            GUI.enabled = true;
        }
    }

    [InitializeOnLoad]
    public class ChildSceneLoader {
        static ChildSceneLoader() {
            EditorSceneManager.sceneOpened += (_, mode) => {
                if (mode != OpenSceneMode.Single || BuildPipeline.isBuildingPlayer)
                    return;

                var scenesToLoadObjects = UnityEngine.Object.FindObjectsByType<EditorChildSceneLoader>(FindObjectsSortMode.None);
                if (scenesToLoadObjects.Length > 1)
                    throw new Exception("Should only have one root scene loaded at once!");

                if (scenesToLoadObjects.Length == 0 || !scenesToLoadObjects[0].enabled)
                    return;

                scenesToLoadObjects[0].ResetSceneSetupToConfig();
            };
        }
    }
#endif

}
