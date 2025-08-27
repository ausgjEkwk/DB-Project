using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float fallSpeed = 2f;        // 아이템 낙하 속도
    public float attractSpeed = 6f;     // 플레이어에게 끌려가는 속도
    public float attractRange = 4f;     // 플레이어에게 끌려가기 시작하는 거리

    private Transform playerTransform;  // 플레이어 위치 참조
    private bool isAttracted = false;   // 플레이어에게 끌려가는 상태 여부

    void Start()
    {
        // 플레이어 오브젝트 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
    }

    void Update()
    {
        // 시간정지 중이면 이동 안함
        if (TimeStop.IsStopped)
            return;

        if (playerTransform == null) return; // 플레이어 없으면 이동 안함

        // 플레이어와 아이템 간 거리 계산
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // 일정 거리 이내면 플레이어에게 끌려감
        if (distance < attractRange)
            isAttracted = true;

        if (isAttracted)
        {
            // 플레이어 방향으로 이동
            Vector3 dir = (playerTransform.position - transform.position).normalized;
            transform.position += dir * attractSpeed * Time.deltaTime;
        }
        else
        {
            // 일반 낙하
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        }

        // 화면 아래로 벗어나면 삭제
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    // 플레이어와 충돌 시 아이템 수집 처리
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // PlayerController 스크립트에서 아이템 획득 처리
            other.GetComponent<PlayerController>()?.AddItem();
            Destroy(gameObject); // 아이템 삭제
        }
    }
}
