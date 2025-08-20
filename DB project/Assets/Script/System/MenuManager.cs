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
    public RectTransform selector;        // ȭ��ǥ (Image)

    private int selectedIndex = 0;
    private bool isMenuActive = false;

    // Start / Options ��ǥ�� �̸� ����
    private Vector2[] selectorPositions = new Vector2[]
    {
        new Vector2(-220f, -144f), // Start
        new Vector2(63f, -144f)    // Options
    };

    void Start()
    {
        menuPanel.SetActive(false); // ó���� ��Ȱ��ȭ
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
                Debug.Log("Options ���õ�");
            }
        }
    }

    void UpdateMenuUI()
    {
        // �׻� ��� ����
        for (int i = 0; i < menuOptions.Length; i++)
        {
            menuOptions[i].color = Color.white;
        }

        // ���õ� ��ġ�� selector �̵� (���� ��ǥ ���)
        selector.anchoredPosition = selectorPositions[selectedIndex];
    }
}
