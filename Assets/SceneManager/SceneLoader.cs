using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _instance;
    private static SceneLoader Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject() {name = "SceneLoader" };
                var component = go.AddComponent<SceneLoader>();
                _instance = component;
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private static bool isLoading;

    public static void LoadScene(string sceneName)
    {
        if (isLoading) return;

        var currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == sceneName)
            throw new Exception("You are trying to load already loaded scene");

        Instance.StartCoroutine(LoadSceneRoutine(sceneName));
    }

    public static void LoadSceneInstantly(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void ReloadScene()
    {
        if (isLoading) return;

        Instance.StartCoroutine(LoadSceneRoutine(SceneManager.GetActiveScene().name));
    }

    private static IEnumerator LoadSceneRoutine(string sceneName)
    {
        isLoading = true;

        var waitFading = true;
        Fader.Instance.FadeIn(() => waitFading = false);
        while (waitFading) yield return null;

        var async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        while (async.progress < 0.9f) yield return null;

        async.allowSceneActivation = true;

        waitFading = true;
        Fader.Instance.FadeOut(() => waitFading = false);
        while (waitFading) yield return null;

        isLoading = false;
    }
}