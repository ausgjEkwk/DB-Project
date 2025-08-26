using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    public GameObject itemPrefab; // 폭발 시 변환될 아이템 프리팹
    public float duration = 1f;   // 폭발 및 회전 지속 시간

    private HashSet<GameObject> processedObjects = new HashSet<GameObject>(); // 이미 처리된 오브젝트 추적

    private void Start()
    {
        StartCoroutine(RotateAndExplodeOverTime()); // 시작과 동시에 회전 + 폭발 코루틴 실행
    }

    IEnumerator RotateAndExplodeOverTime()
    {
        float timer = 0f;

        while (timer < duration)                       // duration 동안 반복
        {
            transform.Rotate(0, 0, 180 * Time.deltaTime); // 반시계 방향으로 회전

            ExplodeEffect();                           // 회전 중 몬스터 & 총알 처리

            timer += Time.deltaTime;
            yield return null;                         // 다음 프레임까지 대기
        }

        Destroy(gameObject);                           // duration 끝나면 폭발 오브젝트 삭제
    }

    void ExplodeEffect()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster"); // 모든 몬스터 검색
        foreach (GameObject monster in monsters)
        {
            if (!processedObjects.Contains(monster))   // 아직 처리하지 않은 몬스터만
            {
                EnemyMoveToTarget moveScript = monster.GetComponent<EnemyMoveToTarget>();
                if (moveScript != null)
                {
                    moveScript.DestroyByBoom();       // EnemyMoveToTarget 통해 폭발 처리
                }
                else
                {
                    Instantiate(itemPrefab, monster.transform.position, Quaternion.identity); // 아이템 생성
                    Destroy(monster);                // 몬스터 직접 삭제
                }

                processedObjects.Add(monster);       // 처리 완료 표시
            }
        }

        RemoveBulletsByTag("Rbullet");               // Rbullet 제거
        RemoveBulletsByTag("Bbullet");               // Bbullet 제거
        RemoveBulletsByTag("Ybullet");               // Ybullet 제거
        RemoveBulletsByTag("BossBullet");            // 보스 총알 제거
    }

    void RemoveBulletsByTag(string tag)
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag(tag); // 지정 태그 총알 검색
        foreach (GameObject bullet in bullets)
        {
            if (!processedObjects.Contains(bullet))  // 처리 안 된 총알만
            {
                Instantiate(itemPrefab, bullet.transform.position, Quaternion.identity); // 아이템 변환
                Destroy(bullet);                      // 총알 삭제
                processedObjects.Add(bullet);        // 처리 완료 표시
            }
        }
    }
}
