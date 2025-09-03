using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;  // UI Text ������Ʈ ���� (Inspector���� �Ҵ�)
    private int score = 0;  // ���� ����

    private void Start()
    {
        UpdateScoreText(); // ���� �� ���� UI �ʱ�ȭ
    }

    // ���� �߰� �Լ� (�ܺο��� ȣ�� ����)
    public void AddScore(int amount)
    {
        score += amount;    // ���� ����
        UpdateScoreText();  // UI ����
    }

    // ���� �ؽ�Ʈ ����
    void UpdateScoreText()
    {
        // "Score : 00000001" �������� ǥ��, �� 8�ڸ�
        scoreText.text = "Score : " + score.ToString("D8");
    }
}
