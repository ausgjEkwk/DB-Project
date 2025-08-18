using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUIManager : MonoBehaviour
{
    [Header("References")]
    public GameObject gameOverPanel;      // GameOverPanel
    public RectTransform selector;        // 화살표 이미지

    [Header("Selector Fixed Positions")]
    public Vector2 mainMenuPos = new Vector2(300f, -250f); // Main Menu 왼쪽
    public Vector2 retryPos = new Vector2(300f, -316f);    // Retry 왼쪽

    private int currentIndex = 0; // 0 = MainMenu, 1 = Retry
    private bool isShown = false;

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isShown) return;

        // 위/아래 키 입력
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            currentIndex = (currentIndex - 1 + 2) % 2;
            UpdateSelectorPosition();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            currentIndex = (currentIndex + 1) % 2;
            UpdateSelectorPosition();
        }

        // 선택
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            ActivateCurrent();
        }
    }

    public void ShowGameOverUI()
    {
        isShown = true;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        currentIndex = 0;
        UpdateSelectorPosition();
    }

    private void UpdateSelectorPosition()
    {
        if (selector == null) return;

        // X는 고정, Y만 이동
        Vector2 pos = currentIndex == 0 ? mainMenuPos : retryPos;
        selector.anchoredPosition = pos;
    }

    private void ActivateCurrent()
    {
        Time.timeScale = 1f; // 씬 이동 전에 시간 복구

        switch (currentIndex)
        {
            case 0: // Main Menu
                SceneManager.LoadScene("MainMenuScene");
                break;
            case 1: // Retry
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }
    }
}
