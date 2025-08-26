using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    public GameObject itemPrefab; // ���� �� ��ȯ�� ������ ������
    public float duration = 1f;   // ���� �� ȸ�� ���� �ð�

    private HashSet<GameObject> processedObjects = new HashSet<GameObject>(); // �̹� ó���� ������Ʈ ����

    private void Start()
    {
        StartCoroutine(RotateAndExplodeOverTime()); // ���۰� ���ÿ� ȸ�� + ���� �ڷ�ƾ ����
    }

    IEnumerator RotateAndExplodeOverTime()
    {
        float timer = 0f;

        while (timer < duration)                       // duration ���� �ݺ�
        {
            transform.Rotate(0, 0, 180 * Time.deltaTime); // �ݽð� �������� ȸ��

            ExplodeEffect();                           // ȸ�� �� ���� & �Ѿ� ó��

            timer += Time.deltaTime;
            yield return null;                         // ���� �����ӱ��� ���
        }

        Destroy(gameObject);                           // duration ������ ���� ������Ʈ ����
    }

    void ExplodeEffect()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster"); // ��� ���� �˻�
        foreach (GameObject monster in monsters)
        {
            if (!processedObjects.Contains(monster))   // ���� ó������ ���� ���͸�
            {
                EnemyMoveToTarget moveScript = monster.GetComponent<EnemyMoveToTarget>();
                if (moveScript != null)
                {
                    moveScript.DestroyByBoom();       // EnemyMoveToTarget ���� ���� ó��
                }
                else
                {
                    Instantiate(itemPrefab, monster.transform.position, Quaternion.identity); // ������ ����
                    Destroy(monster);                // ���� ���� ����
                }

                processedObjects.Add(monster);       // ó�� �Ϸ� ǥ��
            }
        }

        RemoveBulletsByTag("Rbullet");               // Rbullet ����
        RemoveBulletsByTag("Bbullet");               // Bbullet ����
        RemoveBulletsByTag("Ybullet");               // Ybullet ����
        RemoveBulletsByTag("BossBullet");            // ���� �Ѿ� ����
    }

    void RemoveBulletsByTag(string tag)
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag(tag); // ���� �±� �Ѿ� �˻�
        foreach (GameObject bullet in bullets)
        {
            if (!processedObjects.Contains(bullet))  // ó�� �� �� �Ѿ˸�
            {
                Instantiate(itemPrefab, bullet.transform.position, Quaternion.identity); // ������ ��ȯ
                Destroy(bullet);                      // �Ѿ� ����
                processedObjects.Add(bullet);        // ó�� �Ϸ� ǥ��
            }
        }
    }
}
