using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private AnimationClip animationFinal;
    private string sceneToLoad;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void LoadScene(string sceneName)
    {
        sceneToLoad = sceneName;
        StartCoroutine(changeScreen());
    }

    IEnumerator changeScreen()
    {
        animator.SetTrigger("Iniciar");
        yield return new WaitForSeconds(animationFinal.length);
    }

    // Esta función será llamada por el Animation Event
    public void OnAnimationComplete()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
