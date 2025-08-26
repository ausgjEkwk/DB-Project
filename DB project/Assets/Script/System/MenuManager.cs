using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI pressAnyKeyText;
    public GameObject menuPanel;
    public TextMeshProUGUI[] menuOptions; // Start, Options
    public RectTransform selector;        // 화살표 이미지
    public GameObject optionsPanel;       // 옵션 UI 패널

    private int selectedIndex = 0;
    private bool isMenuActive = false;
    private bool isProcessingSelection = false; // 선택 중 중복 방지

    // Start / Options 좌표값
    private Vector2[] selectorPositions = new Vector2[]
    {
        new Vector2(-220f, -144f), // Start
        new Vector2(63f, -144f)    // Options
    };

    void Start()
    {
        menuPanel.SetActive(false);
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    void Update()
    {
        if (!isMenuActive)
        {
            if (Input.anyKeyDown)
            {
                pressAnyKeyText.gameObject.SetActive(false);
                menuPanel.SetActive(true);
                isMenuActive = true;
                UpdateMenuUI();
            }
        }
        else if (!isProcessingSelection)
        {
            HandleMenuNavigation();
        }
    }

    void HandleMenuNavigation()
    {
        // 좌우 이동 (효과음 없음)
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedIndex = (selectedIndex - 1 + menuOptions.Length) % menuOptions.Length;
            UpdateMenuUI();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedIndex = (selectedIndex + 1) % menuOptions.Length;
            UpdateMenuUI();
        }
        // Z/Enter 선택 → 여기서만 효과음 재생
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z))
        {
            if (MenuBGMManager.Instance != null)
                MenuBGMManager.Instance.PlaySelectSFX();

            isProcessingSelection = true;

            if (selectedIndex == 0) // Start
                StartCoroutine(LoadSceneAfterDelay("Main", 1f)); // 1초 딜레이 씬 전환
            else if (selectedIndex == 1) // Options
                StartCoroutine(OpenOptionsAfterDelay(0.3f)); // 옵션 UI 열기
        }
    }

    IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator OpenOptionsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (optionsPanel != null)
            optionsPanel.SetActive(true);
        isProcessingSelection = false;
    }

    void UpdateMenuUI()
    {
        // 메뉴 색상 흰색 유지
        for (int i = 0; i < menuOptions.Length; i++)
        {
            menuOptions[i].color = Color.white;
        }

        // 선택 화살표 이동
        selector.anchoredPosition = selectorPositions[selectedIndex];
    }
}
