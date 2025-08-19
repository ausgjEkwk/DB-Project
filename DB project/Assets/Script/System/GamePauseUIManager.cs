using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePauseUIManager : MonoBehaviour
{
    [Header("References")]
    public GameObject pausePanel;      // 일시정지 패널
    public RectTransform selector;     // 화살표 이미지

    [Header("Selector Fixed Positions")]
    public Vector2 continuePos = new Vector2(300f, -250f);  // Continue 위치
    public Vector2 retryPos = new Vector2(300f, -316f);     // Retry 위치
    public Vector2 mainMenuPos = new Vector2(300f, -382f);  // MainMenu 위치

    public bool IsShown => isShown;

    private int currentIndex = 0; // 0 = Continue, 1 = Retry, 2 = MainMenu
    private bool isShown = false;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void Update()
    {
        // ESC 키로 일시정지 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isShown)
                HidePauseUI();
            else
                ShowPauseUI();
        }

        if (!isShown) return;

        // 위/아래 키 입력
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

        // 선택
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

        Time.timeScale = 0f; // 게임 일시정지
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
        Time.timeScale = 1f; // 씬 이동 전에 시간 복구

        switch (currentIndex)
        {
            case 0: // Continue
                HidePauseUI(); // 일시정지 해제
                break;
            case 1: // Retry
                if (TimeStop.Instance != null)
                    Destroy(TimeStop.Instance.gameObject); // ★ TimeStop 제거

                if (HealthUIManager.Instance != null)
                    HealthUIManager.Instance.SetPreventAutoInitialize(true);

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;

            case 2: // MainMenu
                SceneManager.LoadScene("Menu");
                break;
        }
    }
}
