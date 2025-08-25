using UnityEngine;

public class GamePauseUIManager : MonoBehaviour
{
    [Header("References")]
    public GameObject pausePanel;
    public RectTransform selector;

    [Header("Selector Fixed Positions")]
    public Vector2 continuePos = new Vector2(300f, -250f);
    public Vector2 retryPos = new Vector2(300f, -316f);
    public Vector2 mainMenuPos = new Vector2(300f, -382f);

    public bool IsShown => isShown;

    private int currentIndex = 0;
    private bool isShown = false;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isShown) HidePauseUI();
            else ShowPauseUI();
        }

        if (!isShown) return;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            currentIndex = (currentIndex - 1 + 3) % 3;
            UpdateSelectorPosition();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            currentIndex = (currentIndex + 1) % 3;
            UpdateSelectorPosition();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            ActivateCurrent();
        }
    }

    public void ShowPauseUI()
    {
        isShown = true;
        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f; // 게임 일시정지 (패턴2와 충돌 X)

        currentIndex = 0;
        UpdateSelectorPosition();
    }

    public void HidePauseUI()
    {
        isShown = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f; // 게임 재개
    }

    private void UpdateSelectorPosition()
    {
        if (selector == null) return;

        Vector2 pos = currentIndex switch
        {
            0 => continuePos,
            1 => retryPos,
            2 => mainMenuPos,
            _ => continuePos
        };
        selector.anchoredPosition = pos;
    }

    private void ActivateCurrent()
    {
        Time.timeScale = 1f; // 씬 이동 전 시간 복구

        switch (currentIndex)
        {
            case 0:
                HidePauseUI();
                break;
            case 1:
                if (TimeStop.Instance != null)
                    Destroy(TimeStop.Instance.gameObject);

                if (HealthUIManager.Instance != null)
                    HealthUIManager.Instance.SetPreventAutoInitialize(true);

                UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneReloadedForRetry;
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                break;
            case 2:
                if (AudioManager.Instance != null)
                    Destroy(AudioManager.Instance.gameObject);

                UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
                break;
        }
    }

    private void OnSceneReloadedForRetry(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneReloadedForRetry;
        if (AudioManager.Instance != null)
            AudioManager.Instance.RetryReset();
    }
}
