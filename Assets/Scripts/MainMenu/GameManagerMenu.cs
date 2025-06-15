using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManagerMenu : MonoBehaviour
{
    FadeScene fade;

    public void Start()
    {
        fade = FindObjectOfType<FadeScene>();
    }

    public IEnumerator ChangeScene(string sceneName)
    {
        fade.FadeIn();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(ChangeScene(sceneName));
    }

    public void LoadScene2(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    } 
    // Fungsi untuk keluar dari game
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
