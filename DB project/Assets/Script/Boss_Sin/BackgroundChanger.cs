using UnityEngine;

public class BackgroundChanger : MonoBehaviour
{
    [Header("BackGround MeshRenderer")]
    public MeshRenderer bgRenderer; // BackGround ������Ʈ�� MeshRenderer

    [Header("���� ���� ���")]
    public Texture bossTexture;     // �ٲ� �ؽ�ó

    // ����� ������ �̹����� �����ϴ� �Լ�
    public void ChangeToBossBackground()
    {
        if (bgRenderer != null && bossTexture != null)
        {
            bgRenderer.material.mainTexture = bossTexture;
            Debug.Log("����� ���������� ����Ǿ����ϴ�!");
        }
        else
        {
            Debug.LogWarning("BackGround �Ǵ� ���� �ؽ�ó�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }
}
