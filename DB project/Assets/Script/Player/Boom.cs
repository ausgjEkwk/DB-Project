using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    public GameObject itemPrefab; // 변환할 아이템 프리팹
    public float duration = 1f;   // 회전 시간

    private HashSet<GameObject> processedObjects = new HashSet<GameObject>();

    private void Start()
    {
        StartCoroutine(RotateAndExplodeOverTime());
    }

    IEnumerator RotateAndExplodeOverTime()
    {
        float timer = 0f;

        while (timer < duration)
        {
            transform.Rotate(0, 0, 180 * Time.deltaTime); // 반시계 방향 회전

            ExplodeEffect();

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    void ExplodeEffect()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsters)
        {
            if (!processedObjects.Contains(monster))
            {
                EnemyMoveToTarget moveScript = monster.GetComponent<EnemyMoveToTarget>();
                if (moveScript != null)
                {
                    moveScript.DestroyByBoom();
                }
                else
                {
                    // 혹시 스크립트 없으면 직접 삭제
                    Instantiate(itemPrefab, monster.transform.position, Quaternion.identity);
                    Destroy(monster);
                }

                processedObjects.Add(monster);
            }
        }

        RemoveBulletsByTag("Rbullet");
        RemoveBulletsByTag("Bbullet");
        RemoveBulletsByTag("Ybullet");
        RemoveBulletsByTag("BossBullet");
    }

    void RemoveBulletsByTag(string tag)
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject bullet in bullets)
        {
            if (!processedObjects.Contains(bullet))
            {
                Instantiate(itemPrefab, bullet.transform.position, Quaternion.identity);
                Destroy(bullet);
                processedObjects.Add(bullet);
            }
        }
    }
}
