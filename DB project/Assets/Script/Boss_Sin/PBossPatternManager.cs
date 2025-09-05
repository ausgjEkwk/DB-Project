using System.Collections;
using UnityEngine;

public class PBossPatternManager : MonoBehaviour
{
    [Header("패턴 지속시간 (초)")]
    public float pattern1Duration = 10f;
    public float pattern2Duration = 5f;

    [Header("패턴 스크립트 참조")]
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
        // PBossBGM 재생
        BAudioManager.Instance?.PlayBossBGM();

        // BGM 시작 후 2.5초 대기
        yield return new WaitForSecondsRealtime(2.5f);

        while (true) // 전체 루프: 패턴1 → 패턴2 → 패턴3 3회 반복
        {
            // ===== 패턴1 =====
            if (pattern1 != null) pattern1.SetActivePattern(true);
            yield return new WaitForSecondsRealtime(pattern1Duration);
            if (pattern1 != null) pattern1.SetActivePattern(false);

            DestroyAllPBullets();
            yield return new WaitForSecondsRealtime(1f);

            // ===== 패턴2 =====
            if (pattern2 != null) pattern2.SetActivePattern(true);
            yield return new WaitForSecondsRealtime(pattern2Duration);
            if (pattern2 != null) pattern2.SetActivePattern(false);

            DestroyAllPBullets();
            yield return new WaitForSecondsRealtime(1f);

            // ===== 패턴3 3회 반복 =====
            for (int i = 0; i < 3; i++)
            {
                if (pattern3 != null) pattern3.SetActivePattern(true);

                // 패턴3 완료 대기 (탄환 배치 + 랜덤 이동)
                if (pattern3 != null)
                {
                    while (!pattern3.IsPatternFinished)
                        yield return null;
                }

                // 패턴3 중지
                if (pattern3 != null) pattern3.SetActivePattern(false);

                // 패턴3 탄환 삭제
                DestroyAllPBullets();

                // 다음 반복 전 대기
                yield return new WaitForSecondsRealtime(1f);
            }

            yield return new WaitForSecondsRealtime(1f); // 다음 루프 전 추가 대기
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
