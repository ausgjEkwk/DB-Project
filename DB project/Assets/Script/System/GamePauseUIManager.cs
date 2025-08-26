using UnityEngine;

public class GamePauseUIManager : MonoBehaviour
{
    // === �̱��� �ν��Ͻ� ===
    public static GamePauseUIManager Instance { get; private set; } // �ܺο��� ���� ����

    [Header("References")]
    public GameObject pausePanel;       // �Ͻ����� UI �г�
    public RectTransform selector;      // �޴� ���� ȭ��ǥ

    [Header("Selector Fixed Positions")]
    public Vector2 continuePos = new Vector2(300f, -250f); // ����ϱ� ��ġ
    public Vector2 retryPos = new Vector2(300f, -316f);    // �ٽý��� ��ġ
    public Vector2 mainMenuPos = new Vector2(300f, -382f); // ���θ޴� ��ġ

    public bool IsShown => isShown; // �ܺο��� UI ǥ�� ���� Ȯ�� ����

    private int currentIndex = 0;   // ���� ���� �޴� �ε��� (0=Continue,1=Retry,2=MainMenu)
    private bool isShown = false;   // UI ǥ�� ����

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false); // ���� �� UI �����
    }

    private void Update()
    {
        // ESC Ű �Է� �� �Ͻ����� ���
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isShown) HidePauseUI();
            else ShowPauseUI();
        }

        if (!isShown) return; // UI�� ������ ������ ������ �Է� ����

        // ��/�Ʒ� Ű�� �޴� ���� �̵�
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            currentIndex = (currentIndex - 1 + 3) % 3; // �ε��� ��ȯ
            UpdateSelectorPosition(); // ȭ��ǥ ��ġ ����
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            currentIndex = (currentIndex + 1) % 3; // �ε��� ��ȯ
            UpdateSelectorPosition(); // ȭ��ǥ ��ġ ����
        }

        // Enter �Ǵ� Z �Է� �� ���� Ȱ��ȭ
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z))
        {
            ActivateCurrent();
        }
    }

    // �Ͻ����� UI ǥ��
    public void ShowPauseUI()
    {
        isShown = true;
        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f; // ���� �ð� ����

        // ��� ����� �Ͻ�����
        if (AudioManager.Instance != null)
            AudioManager.Instance.PauseAllAudio();

        currentIndex = 0; // �⺻ ���� �޴�
        UpdateSelectorPosition();
    }

    // �Ͻ����� UI ����
    public void HidePauseUI()
    {
        isShown = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f; // �ð� ����

        // ����� �簳
        if (AudioManager.Instance != null)
            AudioManager.Instance.ResumeAllAudio();
    }

    // ȭ��ǥ ��ġ ����
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

    // ���� ���� �޴� Ȱ��ȭ
    private void ActivateCurrent()
    {
        Time.timeScale = 1f; // �� �̵� �� �ð� ����

        switch (currentIndex)
        {
            case 0: // Continue ����
                HidePauseUI(); // UI ����
                break;
            case 1: // Retry ����
                // �ð� ���� �ν��Ͻ� ����
                if (TimeStop.Instance != null)
                    Destroy(TimeStop.Instance.gameObject);

                // HealthUIManager �ڵ� �ʱ�ȭ ����
                if (HealthUIManager.Instance != null)
                    HealthUIManager.Instance.SetPreventAutoInitialize(true);

                // �� ����� �� BGM ����
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneReloadedForRetry;
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                break;
            case 2: // Main Menu ����
                // ���� AudioManager ����
                if (AudioManager.Instance != null)
                    Destroy(AudioManager.Instance.gameObject);

                UnityEngine.SceneManagement.SceneManager.LoadScene("Menu"); // �޴� �� �̵�
                break;
        }
    }

    // �� ����� �� ȣ��Ǵ� �ݹ�
    private void OnSceneReloadedForRetry(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneReloadedForRetry; // �ݹ� ����
        if (AudioManager.Instance != null)
            AudioManager.Instance.RetryReset(); // BGM �缳��
    }
}
