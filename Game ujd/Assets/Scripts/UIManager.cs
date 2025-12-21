using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This class is responsible for managing the UI elements of the game.
/// </summary>
public class UIManager : MonoBehaviour
{
    public Image fuelImageProgressBar;
    public Gradient fuelGradient;
    public Image lowFuelWarning;
    public Image pauseButtonBg;

    public Text coinText;
    public Text distanceCounter;
    public Text boostCounter;
    public Text rpmCounter;
    public Text fuelCounter;

    public CollectiblesManager collectiblesManager;
    private Vector3 _startPosition;

    public Button sfxButton;
    public Sprite sfxButtonOn;
    public Sprite sfxButtonOff;
    public AudioManager audioManager;
    public Image coinSprite;

    private bool _a = true;

    private void Start()
    {
        _startPosition = collectiblesManager.carBody.transform.position;
    }

    /// <summary>
    /// This method is responsible for updating all the UI elements.
    /// </summary>
    private void Update()
    {
        if (collectiblesManager.fuel <= 0)
        {
            RestartLevel();
        }

        CheckLowFuelWarning();
        UpdateFuelGUI();
        UpdateCoinGUI();
        UpdateRpmCounter();
        UpdateBoostCounter();
    }

    private void UpdateBoostCounter()
    {
        boostCounter.text = Mathf.Abs(
            collectiblesManager.carBody.linearVelocity.magnitude * 0.5f
        ).ToString("F0");
    }

    private void UpdateRpmCounter()
    {
        var distance = collectiblesManager.carBody.transform.position - _startPosition;
        distanceCounter.text = distance.magnitude.ToString("F0") + "m";

        rpmCounter.text = Mathf.Abs(
            collectiblesManager.carBody.linearVelocity.magnitude * 3
        ).ToString("F0");
    }

    private void CheckLowFuelWarning()
    {
        if (collectiblesManager.fuel < 20)
        {
            lowFuelWarning.enabled = true;
            lowFuelWarning.color = new Color(
                lowFuelWarning.color.r,
                lowFuelWarning.color.g,
                lowFuelWarning.color.b,
                Mathf.PingPong(Time.time, 0.5f)
            );
        }
        else
        {
            lowFuelWarning.enabled = false;
        }
    }

    public static void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        if (_a)
        {
            Time.timeScale = 0;
            pauseButtonBg.color = Color.red;
            _a = false;
        }
        else
        {
            Time.timeScale = 1;
            pauseButtonBg.color = Color.white;
            _a = true;
        }
    }

    private void UpdateFuelGUI()
    {
        float fuel01 = collectiblesManager.fuel / 100f;

        fuelImageProgressBar.fillAmount = fuel01;
        fuelImageProgressBar.color = fuelGradient.Evaluate(fuel01);

        fuelCounter.text = collectiblesManager.fuel.ToString("F0") + "%";
        fuelCounter.color = fuelGradient.Evaluate(fuel01);
    }

    private void UpdateCoinGUI()
    {
        coinText.text = collectiblesManager.coins.ToString();
    }

    public void PulseCoinSprite()
    {
        coinSprite.transform.localScale =
            Vector3.one * (0.3f + Mathf.PingPong(Time.time, 0.2f));
    }

    public void ToggleSfx()
    {
        sfxButton.image.sprite =
            sfxButton.image.sprite == sfxButtonOn
                ? sfxButtonOff
                : sfxButtonOn;

        audioManager.ToggleAllSfx();
    }
}

