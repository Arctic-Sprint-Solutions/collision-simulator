using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistEarth : MonoBehaviour
{
    private static PersistEarth instance;
    private string debrisSceneName = "SpaceDebrisScene";
    private string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == debrisSceneName)
        {
            DontDestroyOnLoad(gameObject);
        }
        else if (scene.name == mainMenuSceneName)
        {
            
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}