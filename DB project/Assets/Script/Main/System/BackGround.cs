using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public Renderer backgroundRenderer;  // MeshRenderer ������Ʈ �Ҵ�
    public float scrollSpeed = 0.5f;     // ��ũ�� �ӵ� ���� ����

    private void Update()
    {
        float offset = Time.time * scrollSpeed;
        backgroundRenderer.material.mainTextureOffset = new Vector2(0, offset);
    }
}