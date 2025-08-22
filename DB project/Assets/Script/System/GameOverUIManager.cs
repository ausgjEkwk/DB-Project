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
    public bool IsShown => isShown;  // 외부에서 읽기만 가능

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

        // 🔹 여기서 사망 BGM 실행
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayerDied();
    }

    private void UpdateSelectorPosition()
    {
        if (selector == null) return;

        Vector2 pos = currentIndex == 0 ? mainMenuPos : retryPos;
        selector.anchoredPosition = pos;
    }

    private void ActivateCurrent()
    {
        Time.timeScale = 1f; // 씬 이동 전에 시간 복구

        switch (currentIndex)
        {
            case 0: // Main Menu
                    // 기존 BGM 페이드 아웃
                if (AudioManager.Instance != null)
                    AudioManager.Instance.StopBGMWithFade();

                // MainMenu 씬 로드 전에 GameOver 오브젝트 파괴
                Destroy(this.gameObject); // GameOverUIManager 포함 오브젝트 삭제
                SceneManager.LoadScene("Menu");
                break;

            case 1: // Retry
                    // 시간 정지 인스턴스 제거
                if (TimeStop.Instance != null)
                    Destroy(TimeStop.Instance.gameObject);

                // HealthUIManager 자동 초기화 방지
                if (HealthUIManager.Instance != null)
                    HealthUIManager.Instance.SetPreventAutoInitialize(true);

                // Retry 시 BGM 리셋
                if (AudioManager.Instance != null)
                    AudioManager.Instance.RetryReset();

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }
    }

}
