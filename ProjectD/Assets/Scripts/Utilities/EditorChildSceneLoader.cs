using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

public class EditorChildSceneLoader : MonoBehaviour {
#if UNITY_EDITOR
    [SerializeField]
    public List<SceneAsset> ChildScenesToLoadConfig;

    void Update() {
        // DO NOT DELETE keep this so we can enable/disable this script... (used in ChildSceneLoader)
    }

    public void SaveSceneSetup() {
        ChildScenesToLoadConfig ??= new List<SceneAsset>();
        ChildScenesToLoadConfig.Clear();
        foreach (var sceneSetup in EditorSceneManager.GetSceneManagerSetup()) {
            ChildScenesToLoadConfig.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>(sceneSetup.path));
        }
        EditorUtility.SetDirty(this);
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
#endif
}


#if UNITY_EDITOR
[CustomEditor(typeof(EditorChildSceneLoader))]
public class ChildSceneLoaderInspectorGUI : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var currentInspectorObject = (EditorChildSceneLoader)target;

        if (GUILayout.Button("Save scene setup to config"))
            currentInspectorObject.SaveSceneSetup();

        if (GUILayout.Button("Reset scene setup from config"))
            currentInspectorObject.ResetSceneSetupToConfig();
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

            Debug.Log("Setup done for root scene and child scenes");
        };
    }
}
#endif
