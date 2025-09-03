using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;          // 총알 이동 속도
    public float lifeTime = 5f;       // 총알 존재 시간(초)

    private Vector2 direction = Vector2.zero; // 총알 이동 방향

    void Start()
    {
        Destroy(gameObject, lifeTime); // lifeTime 이후 자동 파괴
    }

    void Update()
    {
        if (direction != Vector2.zero)          // 이동 방향이 설정되어 있을 때만 이동
        {
            transform.Translate(direction * speed * Time.deltaTime); // 방향에 맞춰 이동
        }
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;            // 전달받은 방향을 단위 벡터로 설정
    }
}
