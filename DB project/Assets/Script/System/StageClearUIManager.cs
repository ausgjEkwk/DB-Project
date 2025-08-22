using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClearUIManager : MonoBehaviour
{
    [Header("References")]
    public GameObject clearPanel;   // 클리어 패널
    public RectTransform selector;  // 화살표 이미지

    [Header("Selector Positions")]
    public Vector2 option1Pos = new Vector2(-75f, -30f);   // 예: 다음 스테이지
    public Vector2 option2Pos = new Vector2(-45f, -100f);  // 예: 메인 메뉴

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
            // 다음 스테이지 로드
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            // 메인 메뉴 이동
            SceneManager.LoadScene("Menu");
        }
    }
}
