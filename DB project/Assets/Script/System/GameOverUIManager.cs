using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUIManager : MonoBehaviour
{
    [Header("References")]
    public GameObject gameOverPanel;      // GameOverPanel
    public RectTransform selector;        // ȭ��ǥ �̹��� RectTransform
    public RectTransform[] menuOptions;   // MainMenuText, RetryText (�� ������ �巡��)

    [Header("Selector Offset (px)")]
    public float selectorXOffset = -80f;  // �ؽ�Ʈ ���ʿ� ȭ��ǥ �ΰ� ������ ����

    private int currentIndex = 0;
    private bool isShown = false;

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isShown) return;

        // ��/��/W/S �Է�
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            currentIndex = (currentIndex - 1 + menuOptions.Length) % menuOptions.Length;
            UpdateSelectorPosition();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            currentIndex = (currentIndex + 1) % menuOptions.Length;
            UpdateSelectorPosition();
        }

        // ����/�����̽� ����
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
        if (selector == null || menuOptions == null || menuOptions.Length == 0) return;

        // selector�� menuOptions�� ���� �θ�(MenuItems) �Ʒ��� �ִٰ� ����
        RectTransform target = menuOptions[currentIndex];

        // ����� Y�� ���߰� X�� ������ ����
        Vector2 newAnchored = new Vector2(selectorXOffset, target.anchoredPosition.y);
        selector.anchoredPosition = newAnchored;
    }

    private void ActivateCurrent()
    {
        // Time.timeScale = 0 �����̹Ƿ�, �� ���� ���� �ݵ�� 1�� ����
        Time.timeScale = 1f;

        switch (currentIndex)
        {
            case 0:
                // Main Menu
                SceneManager.LoadScene("MainMenuScene"); // �� ���� ���θ޴� �� �̸����� ����
                break;

            case 1:
                // Retry (���� �� �����)
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }
    }
}
