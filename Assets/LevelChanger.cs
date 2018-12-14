using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour {

    public Animator animator;

    private int levelToLoad;

    void Start()
    {
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("LoadScreen"))
        {
            StartCoroutine(PlayFadeOut());
        }
        
    }

    IEnumerator PlayFadeOut()
    {
        yield return new WaitForSeconds(5);
        FadeToLevel(1);
    }

    public void FadeToLevel(int levelIndex)
    {
        levelToLoad = levelIndex;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
