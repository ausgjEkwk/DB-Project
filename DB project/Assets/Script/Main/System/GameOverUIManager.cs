using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUIManager : MonoBehaviour
{
    public static GameOverUIManager Instance { get; private set; } // 싱글톤 인스턴스

    [Header("References")]
    public GameObject gameOverPanel;      // 게임오버 UI 패널
    public RectTransform selector;        // 메뉴 선택 화살표 이미지

    [Header("Selector Fixed Positions")]
    public Vector2 mainMenuPos = new Vector2(300f, -250f); // Main Menu 화살표 위치
    public Vector2 retryPos = new Vector2(300f, -316f);    // Retry 화살표 위치
    public bool IsShown => isShown;  // 외부에서 UI 표시 여부를 읽기 전용으로 확인

    private int currentIndex = 0; // 현재 선택 인덱스 (0 = MainMenu, 1 = Retry)
    private bool isShown = false; // 게임오버 UI 표시 여부

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // 중복 방지
    }

    private void Start()
    {
        // 시작 시 UI 숨기기
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isShown) return; // UI가 안 보이면 입력 무시

        // 위/아래 키 입력으로 선택 이동
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            currentIndex = (currentIndex - 1 + 2) % 2; // 인덱스 순환
            UpdateSelectorPosition(); // 화살표 위치 갱신
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            currentIndex = (currentIndex + 1) % 2; // 인덱스 순환
            UpdateSelectorPosition(); // 화살표 위치 갱신
        }

        // 선택 확정 (Enter 또는 Z)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z))
        {
            ActivateCurrent();
        }
    }

    // 게임오버 UI 표시
    public void ShowGameOverUI()
    {
        isShown = true;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        currentIndex = 0; // 기본 선택 MainMenu
        UpdateSelectorPosition(); // 화살표 위치 갱신

        // 플레이어 사망 BGM 재생
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayerDied();
    }

    // 선택 화살표 위치 갱신
    private void UpdateSelectorPosition()
    {
        if (selector == null) return;

        Vector2 pos = currentIndex == 0 ? mainMenuPos : retryPos;
        selector.anchoredPosition = pos;
    }

    // 현재 선택한 메뉴 활성화
    private void ActivateCurrent()
    {
        Time.timeScale = 1f; // 씬 이동 전에 시간 복구

        switch (currentIndex)
        {
            case 0: // Main Menu 선택 시
                if (AudioManager.Instance != null)
                    AudioManager.Instance.StopBGMWithFade(); // BGM 페이드 아웃

                Destroy(this.gameObject); // GameOverUIManager 오브젝트 삭제
                SceneManager.LoadScene("Menu"); // 메뉴 씬 로드
                break;

            case 1: // Retry 선택 시
                if (TimeStop.Instance != null)
                    Destroy(TimeStop.Instance.gameObject); // 시간 정지 인스턴스 제거

                if (HealthUIManager.Instance != null)
                    HealthUIManager.Instance.SetPreventAutoInitialize(true); // UI 초기화 방지

                if (AudioManager.Instance != null)
                    AudioManager.Instance.RetryReset(); // BGM 재설정

                SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬 재시작
                break;
        }
    }
}
