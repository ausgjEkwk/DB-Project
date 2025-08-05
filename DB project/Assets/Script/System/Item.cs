using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float fallSpeed = 2f;
    public float attractSpeed = 6f;     // 플레이어에게 끌려가는 속도
    public float attractRange = 4f;     // 끌려가기 시작하는 거리

    private Transform playerTransform;
    private bool isAttracted = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance < attractRange)
            isAttracted = true;

        if (isAttracted)
        {
            Vector3 dir = (playerTransform.position - transform.position).normalized;
            transform.position += dir * attractSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        }

        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //  PlayerController로 수정 (Player 라는 스크립트가 아니라면 꼭 이렇게!)
            other.GetComponent<PlayerController>()?.AddItem();
            Destroy(gameObject);
        }
    }
}
