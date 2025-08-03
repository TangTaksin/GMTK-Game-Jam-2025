using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName;


    public void CallTransition()
    {
        Transition.CalledFadeIn?.Invoke();
        Transition.FadeInOver += LoadScene;
    }

    void LoadScene()
    {
        Transition.FadeInOver -= LoadScene;
        SceneManager.LoadScene(sceneName);
    }
}
