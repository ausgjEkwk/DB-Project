using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    public float lifeTime = 5f;

    private Vector2 direction = Vector2.zero;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (direction != Vector2.zero)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }
}

