using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Fade (scene change only)")]
    public Image fadeImage;
    public float fadeDuration = 1f;
    public int sceneIndex = 1;
    public GameObject playButton;
    public GameObject quitButton;

    private void Awake()
    {
        // Fade niewidoczny w menu
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");

        Application.Quit();

#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


    public void PlayGame()
    {
        playButton.SetActive(false);
        quitButton.SetActive(false);
        StartCoroutine(FadeAndLoad());
    }

    IEnumerator FadeAndLoad()
    {
        float t = 0f;

        // Fade OUT – tylko przy zmianie sceny
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }

        SceneManager.LoadScene(sceneIndex);
    }
}


