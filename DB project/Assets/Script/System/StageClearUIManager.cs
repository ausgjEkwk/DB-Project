using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClearUIManager : MonoBehaviour
{
    public static StageClearUIManager Instance { get; private set; } // ← 추가

    [Header("References")]
    public GameObject clearPanel;   // Stage Clear Panel
    public RectTransform selector;  // 화살표 이미지

    [Header("Selector Fixed Positions")]
    public Vector2 mainMenuPos = new Vector2(-75f, -30f); // Main Menu
    public Vector2 retryPos = new Vector2(-45f, -100f);   // Retry

    public bool IsShown => isShown;

    private int currentIndex = 0; // 0 = Main Menu, 1 = Retry
    private bool isShown = false;

    private void Awake() // ← 추가
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (clearPanel != null)
            clearPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isShown) return;

        // 위/아래 이동
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            currentIndex = (currentIndex - 1 + 2) % 2;
            UpdateSelector();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            currentIndex = (currentIndex + 1) % 2;
            UpdateSelector();
        }

        // 선택
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z))
        {
            ActivateOption();
        }
    }

    public void ShowClearUI()
    {
        isShown = true;
        if (clearPanel != null)
            clearPanel.SetActive(true);

        Time.timeScale = 0f; // 게임 정지 (클리어 연출 후 입력 대기)
        currentIndex = 0;
        UpdateSelector();
    }

    private void UpdateSelector()
    {
        if (selector == null) return;

        Vector2 pos = currentIndex == 0 ? mainMenuPos : retryPos;
        selector.anchoredPosition = pos;
    }

    private void ActivateOption()
    {
        Time.timeScale = 1f; // 씬 이동 전 시간 복구

        switch (currentIndex)
        {
            case 0: // Main Menu
                Destroy(this.gameObject);
                SceneManager.LoadScene("Menu");
                break;

            case 1: // Retry (현재 씬 다시 로드)
                if (TimeStop.Instance != null)
                    Destroy(TimeStop.Instance.gameObject);

                if (HealthUIManager.Instance != null)
                    HealthUIManager.Instance.SetPreventAutoInitialize(true);

                if (AudioManager.Instance != null)
                    AudioManager.Instance.RetryReset();

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }
    }
}
