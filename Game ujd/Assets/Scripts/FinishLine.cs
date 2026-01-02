using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [Header("UI")]
    public GameObject winPanel;

    [Header("Player")]
    public string playerTag = "Player";

    [Header("Audio")]
    public AudioManager audioManager;

    private bool finished;

    private void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (finished) return;

        if (!other.transform.root.CompareTag(playerTag))
            return;

        finished = true;

        winPanel.SetActive(true);

        if (audioManager != null)
            audioManager.PlayWinMusic();

        Time.timeScale = 0f;
    }
}



