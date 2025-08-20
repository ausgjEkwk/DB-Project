using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI pressAnyKeyText;
    public GameObject menuPanel;
    public TextMeshProUGUI[] menuOptions; // Start, Options
    public RectTransform selector;        // 화살표 (Image)

    private int selectedIndex = 0;
    private bool isMenuActive = false;

    // Start / Options 좌표값 미리 지정
    private Vector2[] selectorPositions = new Vector2[]
    {
        new Vector2(-220f, -144f), // Start
        new Vector2(63f, -144f)    // Options
    };

    void Start()
    {
        menuPanel.SetActive(false); // 처음엔 비활성화
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
        else
        {
            HandleMenuNavigation();
        }
    }

    void HandleMenuNavigation()
    {
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
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (selectedIndex == 0) // Start
            {
                SceneManager.LoadScene("Main");
            }
            else if (selectedIndex == 1) // Options
            {
                Debug.Log("Options 선택됨");
            }
        }
    }

    void UpdateMenuUI()
    {
        // 항상 흰색 유지
        for (int i = 0; i < menuOptions.Length; i++)
        {
            menuOptions[i].color = Color.white;
        }

        // 선택된 위치로 selector 이동 (고정 좌표 사용)
        selector.anchoredPosition = selectorPositions[selectedIndex];
    }
}
