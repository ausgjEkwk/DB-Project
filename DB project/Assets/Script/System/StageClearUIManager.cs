using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClearUIManager : MonoBehaviour
{
    public static StageClearUIManager Instance { get; private set; } // 싱글톤 인스턴스

    [Header("References")]
    public GameObject clearPanel;   // Stage Clear UI 패널
    public RectTransform selector;  // 선택 화살표 이미지

    [Header("Selector Fixed Positions")]
    public Vector2 mainMenuPos = new Vector2(-75f, -30f); // Main Menu 위치
    public Vector2 retryPos = new Vector2(-45f, -100f);   // Retry 위치

    public bool IsShown => isShown; // 외부에서 UI 표시 여부 확인 가능

    private int currentIndex = 0; // 0 = Main Menu, 1 = Retry
    private bool isShown = false; // UI 표시 상태

    private void Awake()
    {
        // 싱글톤 유지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // 시작 시 UI 비활성화
        if (clearPanel != null)
            clearPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isShown) return; // UI 미표시 시 입력 무시

        // ↑ 또는 W 키로 선택 위로 이동
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            currentIndex = (currentIndex - 1 + 2) % 2;
            UpdateSelector();
        }

        // ↓ 또는 S 키로 선택 아래로 이동
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            currentIndex = (currentIndex + 1) % 2;
            UpdateSelector();
        }

        // Z 또는 Enter 키로 선택 확정
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z))
        {
            ActivateOption();
        }
    }

    // Stage Clear UI 표시
    public void ShowClearUI()
    {
        isShown = true;
        if (clearPanel != null)
            clearPanel.SetActive(true);

        Time.timeScale = 0f; // 게임 정지 (클리어 연출 유지)
        currentIndex = 0;
        UpdateSelector();
    }

    // 화살표 위치 갱신
    private void UpdateSelector()
    {
        if (selector == null) return;

        Vector2 pos = currentIndex == 0 ? mainMenuPos : retryPos;
        selector.anchoredPosition = pos;
    }

    // 선택 옵션 실행
    private void ActivateOption()
    {
        Time.timeScale = 1f; // 씬 이동 전 시간 복구

        switch (currentIndex)
        {
            case 0: // Main Menu 선택
                Destroy(this.gameObject); // UI 제거
                SceneManager.LoadScene("Menu"); // 메뉴 씬 로드
                break;

            case 1: // Retry 선택
                // 시간정지 인스턴스 제거
                if (TimeStop.Instance != null)
                    Destroy(TimeStop.Instance.gameObject);

                // HealthUI 자동 초기화 방지
                if (HealthUIManager.Instance != null)
                    HealthUIManager.Instance.SetPreventAutoInitialize(true);

                // BGM 초기화
                if (AudioManager.Instance != null)
                    AudioManager.Instance.RetryReset();

                // 현재 씬 다시 로드
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }
    }
}
