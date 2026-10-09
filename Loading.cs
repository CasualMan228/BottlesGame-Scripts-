using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public Animator animator;
    private void Start()
    {
        StartCoroutine(SetLoading());
    }
    IEnumerator SetLoading()
    {
        yield return new WaitForSeconds(2f);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("SampleScene");
        asyncOperation.allowSceneActivation = false;
        while (asyncOperation.progress < 0.9f)
        {
            yield return null;
        }
        animator.SetBool("isLoaded", true);
        yield return new WaitForSeconds(2f);
        asyncOperation.allowSceneActivation = true;
    }
}
