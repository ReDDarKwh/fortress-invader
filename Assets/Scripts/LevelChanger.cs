using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{

    public Animator animator;

    public int levelToLoad;

    public List<string> menuScenes = new List<string> { "Menu" };
    private Scene previousScene;


    public event EventHandler MenuExited;

    protected virtual void OnMenuExited(EventArgs e)
    {
        EventHandler handler = MenuExited;
        handler?.Invoke(this, e);
    }

    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    public void ExitMenu()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.SetActiveScene(previousScene);
        SetActiveObjectsInScene(true, previousScene);
        OnMenuExited(null);
    }

    public void SetActiveObjectsInScene(bool active, Scene scene)
    {
        // overlay menu was loaded
        var rootObjects = scene.GetRootGameObjects();
        foreach (var obj in rootObjects)
        {
            obj.SetActive(active);
        }
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        FadeIn();

        if (menuScenes.Contains(scene.name))
        {

            var currentScene = SceneManager.GetActiveScene();
            SetActiveObjectsInScene(false, currentScene);
            previousScene = currentScene;
            SceneManager.SetActiveScene(scene);
        }
    }

    public void LoadMenu(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex, LoadSceneMode.Additive);
    }

    public void FadeToLevel(int levelIndex)
    {
        levelToLoad = levelIndex;
        animator.SetBool("black", true);
    }

    public void FadeIn()
    {
        animator.SetBool("black", false);
    }


    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
