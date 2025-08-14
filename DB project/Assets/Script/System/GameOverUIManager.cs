using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUIManager : MonoBehaviour
{
    [Header("References")]
    public GameObject gameOverPanel;      // GameOverPanel
    public RectTransform selector;        // 화살표 이미지 RectTransform
    public RectTransform[] menuOptions;   // MainMenuText, RetryText (이 순서로 드래그)

    [Header("Selector Offset (px)")]
    public float selectorXOffset = -80f;  // 텍스트 왼쪽에 화살표 두고 싶으면 음수

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

        // ↑/↓/W/S 입력
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

        // 엔터/스페이스 선택
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

        // selector와 menuOptions가 같은 부모(MenuItems) 아래에 있다고 가정
        RectTransform target = menuOptions[currentIndex];

        // 대상의 Y에 맞추고 X는 오프셋 적용
        Vector2 newAnchored = new Vector2(selectorXOffset, target.anchoredPosition.y);
        selector.anchoredPosition = newAnchored;
    }

    private void ActivateCurrent()
    {
        // Time.timeScale = 0 상태이므로, 씬 변경 전에 반드시 1로 복구
        Time.timeScale = 1f;

        switch (currentIndex)
        {
            case 0:
                // Main Menu
                SceneManager.LoadScene("MainMenuScene"); // ← 실제 메인메뉴 씬 이름으로 변경
                break;

            case 1:
                // Retry (현재 씬 재시작)
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }
    }
}
