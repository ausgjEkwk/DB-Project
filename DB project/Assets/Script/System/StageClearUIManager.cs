using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClearUIManager : MonoBehaviour
{
    [Header("References")]
    public GameObject clearPanel;   // Ŭ���� �г�
    public RectTransform selector;  // ȭ��ǥ �̹���

    [Header("Selector Positions")]
    public Vector2 option1Pos = new Vector2(-75f, -30f);   // ��: ���� ��������
    public Vector2 option2Pos = new Vector2(-45f, -100f);  // ��: ���� �޴�

    private int currentIndex = 0;

    private void Start()
    {
        clearPanel.SetActive(false);
    }

    private void Update()
    {
        if (!clearPanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = 0;
            UpdateSelector();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex = 1;
            UpdateSelector();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            ActivateOption();
        }
    }

    public void ShowClearUI()
    {
        clearPanel.SetActive(true);
        currentIndex = 0;
        UpdateSelector();
    }

    private void UpdateSelector()
    {
        if (currentIndex == 0)
            selector.anchoredPosition = option1Pos;
        else
            selector.anchoredPosition = option2Pos;
    }

    private void ActivateOption()
    {
        if (currentIndex == 0)
        {
            // ���� �������� �ε�
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            // ���� �޴� �̵�
            SceneManager.LoadScene("Menu");
        }
    }
}
