using System.Collections;
using UnityEngine;

public class PBossPatternManager : MonoBehaviour
{
    [Header("���� ���ӽð� (��)")]
    public float pattern1Duration = 10f;
    public float pattern2Duration = 5f;

    [Header("���� ��ũ��Ʈ ����")]
    public PBossPattern1 pattern1;
    public PBossPattern2 pattern2;
    public PBossPattern3 pattern3;

    private Coroutine patternCoroutine;

    public void StartPatternSequence()
    {
        if (patternCoroutine != null)
            StopCoroutine(patternCoroutine);

        patternCoroutine = StartCoroutine(PatternSequenceCoroutine());
    }

    private IEnumerator PatternSequenceCoroutine()
    {
        // PBossBGM ���
        BAudioManager.Instance?.PlayBossBGM();

        // BGM ���� �� 2.5�� ���
        yield return new WaitForSecondsRealtime(2.5f);

        while (true) // ��ü ����: ����1 �� ����2 �� ����3 3ȸ �ݺ�
        {
            // ===== ����1 =====
            if (pattern1 != null) pattern1.SetActivePattern(true);
            yield return new WaitForSecondsRealtime(pattern1Duration);
            if (pattern1 != null) pattern1.SetActivePattern(false);

            DestroyAllPBullets();
            yield return new WaitForSecondsRealtime(1f);

            // ===== ����2 =====
            if (pattern2 != null) pattern2.SetActivePattern(true);
            yield return new WaitForSecondsRealtime(pattern2Duration);
            if (pattern2 != null) pattern2.SetActivePattern(false);

            DestroyAllPBullets();
            yield return new WaitForSecondsRealtime(1f);

            // ===== ����3 3ȸ �ݺ� =====
            for (int i = 0; i < 3; i++)
            {
                if (pattern3 != null) pattern3.SetActivePattern(true);

                // ����3 �Ϸ� ��� (źȯ ��ġ + ���� �̵�)
                if (pattern3 != null)
                {
                    while (!pattern3.IsPatternFinished)
                        yield return null;
                }

                // ����3 ����
                if (pattern3 != null) pattern3.SetActivePattern(false);

                // ����3 źȯ ����
                DestroyAllPBullets();

                // ���� �ݺ� �� ���
                yield return new WaitForSecondsRealtime(1f);
            }

            yield return new WaitForSecondsRealtime(1f); // ���� ���� �� �߰� ���
        }
    }

    private void DestroyAllPBullets()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("PBullet");
        foreach (GameObject b in bullets)
        {
            if (b != null)
                Destroy(b);
        }
    }
}
