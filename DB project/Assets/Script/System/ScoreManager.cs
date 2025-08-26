using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;  // UI Text 컴포넌트 연결 (Inspector에서 할당)
    private int score = 0;  // 현재 점수

    private void Start()
    {
        UpdateScoreText(); // 시작 시 점수 UI 초기화
    }

    // 점수 추가 함수 (외부에서 호출 가능)
    public void AddScore(int amount)
    {
        score += amount;    // 점수 증가
        UpdateScoreText();  // UI 갱신
    }

    // 점수 텍스트 갱신
    void UpdateScoreText()
    {
        // "Score : 00000001" 형식으로 표시, 총 8자리
        scoreText.text = "Score : " + score.ToString("D8");
    }
}
