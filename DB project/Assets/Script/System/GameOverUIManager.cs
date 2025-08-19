// GameOverUIManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUIManager : MonoBehaviour
{
    [Header("References")]
    public GameObject gameOverPanel;      // GameOverPanel
    public RectTransform selector;        // ȭ��ǥ �̹���

    [Header("Selector Fixed Positions")]
    public Vector2 mainMenuPos = new Vector2(300f, -250f); // Main Menu ����
    public Vector2 retryPos = new Vector2(300f, -316f);    // Retry ����
    public bool IsShown => isShown;  // �ܺο��� �б⸸ ����

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

        // ��/�Ʒ� Ű �Է�
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

        // ����
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

        Vector2 pos = currentIndex == 0 ? mainMenuPos : retryPos;
        selector.anchoredPosition = pos;
    }

    private void ActivateCurrent()
    {
        Time.timeScale = 1f; // �� �̵� ���� �ð� ����

        switch (currentIndex)
        {
            case 0: // Main Menu
                SceneManager.LoadScene("Menu");
                break;
            case 1: // Retry
                if (TimeStop.Instance != null)
                    Destroy(TimeStop.Instance.gameObject); // �� TimeStop ����

                if (HealthUIManager.Instance != null)
                    HealthUIManager.Instance.SetPreventAutoInitialize(true);

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;

        }
    }
}
