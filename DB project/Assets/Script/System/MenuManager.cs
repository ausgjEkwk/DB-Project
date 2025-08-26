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
    public RectTransform selector;        // ȭ��ǥ �̹���
    public GameObject optionsPanel;       // �ɼ� UI �г�

    private int selectedIndex = 0;
    private bool isMenuActive = false;
    private bool isProcessingSelection = false; // ���� �� �ߺ� ����

    // Start / Options ��ǥ��
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
        // �¿� �̵� (ȿ���� ����)
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
        // Z/Enter ���� �� ���⼭�� ȿ���� ���
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z))
        {
            if (MenuBGMManager.Instance != null)
                MenuBGMManager.Instance.PlaySelectSFX();

            isProcessingSelection = true;

            if (selectedIndex == 0) // Start
                StartCoroutine(LoadSceneAfterDelay("Main", 1f)); // 1�� ������ �� ��ȯ
            else if (selectedIndex == 1) // Options
                StartCoroutine(OpenOptionsAfterDelay(0.3f)); // �ɼ� UI ����
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
        // �޴� ���� ��� ����
        for (int i = 0; i < menuOptions.Length; i++)
        {
            menuOptions[i].color = Color.white;
        }

        // ���� ȭ��ǥ �̵�
        selector.anchoredPosition = selectorPositions[selectedIndex];
    }
}
