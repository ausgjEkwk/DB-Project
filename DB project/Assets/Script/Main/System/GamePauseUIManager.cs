using UnityEngine;

public class GamePauseUIManager : MonoBehaviour
{
    // === 싱글톤 인스턴스 ===
    public static GamePauseUIManager Instance { get; private set; } // 외부에서 접근 가능

    [Header("References")]
    public GameObject pausePanel;       // 일시정지 UI 패널
    public RectTransform selector;      // 메뉴 선택 화살표
    public GameObject optionPanel;      // 옵션 UI 패널 추가

    [Header("Selector Fixed Positions")]
    public Vector2 continuePos = new Vector2(300f, -250f); // 계속하기 위치
    public Vector2 retryPos = new Vector2(300f, -316f);    // 다시시작 위치
    public Vector2 mainMenuPos = new Vector2(300f, -382f); // 메인메뉴 위치
    public Vector2 optionPos = new Vector2(300f, -448f);   // Option 위치

    public bool IsShown => isShown; // 외부에서 UI 표시 여부 확인 가능

    private int currentIndex = 0;   // 현재 선택 메뉴 인덱스 (0=Continue,1=Retry,2=MainMenu)
    private bool isShown = false;   // UI 표시 여부
    private int menuCount = 4;      // 메뉴 개수

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false); // 시작 시 UI 숨기기

        if (optionPanel != null)
            optionPanel.SetActive(false); // 옵션 패널도 숨기기
    }

    private void Update()
    {
        // GameOverUI가 켜져있으면 일시정지 입력 무시
        if (GameOverUIManager.Instance != null && GameOverUIManager.Instance.IsShown)
            return;

        // ESC 키 입력 시 일시정지 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 만약 옵션 패널이 열려있으면 → 옵션 패널 닫고 PausePanel 복귀
            if (optionPanel != null && optionPanel.activeSelf)
            {
                optionPanel.SetActive(false);
                pausePanel.SetActive(true);
                return;
            }

            if (isShown) HidePauseUI();
            else ShowPauseUI();
        }

        if (!isShown) return; // UI가 보이지 않으면 나머지 입력 무시

        // 위/아래 키로 메뉴 선택 이동
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            currentIndex = (currentIndex - 1 + menuCount) % menuCount; // 인덱스 순환
            UpdateSelectorPosition(); // 화살표 위치 갱신
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            currentIndex = (currentIndex + 1) % menuCount; // 인덱스 순환
            UpdateSelectorPosition(); // 화살표 위치 갱신
        }

        // Enter 또는 Z 입력 시 선택 활성화
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z))
        {
            ActivateCurrent();
        }
    }


    // 일시정지 UI 표시
    public void ShowPauseUI()
    {
        isShown = true;
        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f; // 게임 시간 정지

        // 모든 오디오 일시정지
        if (AudioManager.Instance != null)
            AudioManager.Instance.PauseAllAudio();

        currentIndex = 0; // 기본 선택 메뉴
        UpdateSelectorPosition();
    }

    // 일시정지 UI 숨김
    public void HidePauseUI()
    {
        isShown = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (optionPanel != null)
            optionPanel.SetActive(false);

        Time.timeScale = 1f; // 시간 복구

        // 오디오 재개
        if (AudioManager.Instance != null)
            AudioManager.Instance.ResumeAllAudio();
    }

    /// 화살표 위치 갱신
    private void UpdateSelectorPosition()
    {
        if (selector == null) return;

        Vector2 pos = currentIndex switch
        {
            0 => continuePos,
            1 => retryPos,
            2 => mainMenuPos,
            3 => optionPos,
            _ => continuePos
        };
        selector.anchoredPosition = pos;
    }


    // 현재 선택 메뉴 활성화
    private void ActivateCurrent()
    {
        switch (currentIndex)
        {
            case 0: // Continue 선택
                HidePauseUI(); // UI 숨김 + 시간 복구
                break;
            case 1: // Retry 선택
                Time.timeScale = 1f; // 씬 재시작 시 시간 복구
                if (TimeStop.Instance != null)
                    Destroy(TimeStop.Instance.gameObject);

                if (HealthUIManager.Instance != null)
                    HealthUIManager.Instance.SetPreventAutoInitialize(true);

                UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneReloadedForRetry;
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                break;
            case 2: // Main Menu 선택
                Time.timeScale = 1f; // 시간 복구
                if (AudioManager.Instance != null)
                    Destroy(AudioManager.Instance.gameObject);

                UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
                break;
            case 3: // Option 선택
                if (optionPanel != null)
                {
                    // PausePanel 숨기기
                    if (pausePanel != null)
                        pausePanel.SetActive(false);

                    // OptionPanel 켜기
                    optionPanel.SetActive(true);

                    // 시간 유지
                    Time.timeScale = 0f;

                    // UI 상태 유지
                    isShown = true;

                    // Selector Option 위치로 이동
                    currentIndex = 3;
                    UpdateSelectorPosition();
                }
                break;

        }
    }


    // 씬 재시작 후 호출되는 콜백
    private void OnSceneReloadedForRetry(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneReloadedForRetry; // 콜백 제거
        if (AudioManager.Instance != null)
            AudioManager.Instance.RetryReset(); // BGM 재설정
    }
}
