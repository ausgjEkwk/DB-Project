using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pbullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 savedVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        savedVelocity = rb.velocity;
    }

    void Update()
    {
        if (TimeStop.Instance != null && TimeStop.Instance.IsTimeStopped)
        {
            if (rb.velocity != Vector2.zero)
            {
                savedVelocity = rb.velocity;
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
            }
        }
        else
        {
            if (rb.velocity == Vector2.zero)
            {
                rb.isKinematic = false;
                rb.velocity = savedVelocity;
            }
        }
    }
}
