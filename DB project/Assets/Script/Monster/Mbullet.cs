using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;          // �Ѿ� �̵� �ӵ�
    public float lifeTime = 5f;       // �Ѿ� ���� �ð�(��)

    private Vector2 direction = Vector2.zero; // �Ѿ� �̵� ����

    void Start()
    {
        Destroy(gameObject, lifeTime); // lifeTime ���� �ڵ� �ı�
    }

    void Update()
    {
        if (direction != Vector2.zero)          // �̵� ������ �����Ǿ� ���� ���� �̵�
        {
            transform.Translate(direction * speed * Time.deltaTime); // ���⿡ ���� �̵�
        }
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;            // ���޹��� ������ ���� ���ͷ� ����
    }
}
