using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MenuCameraReset : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();

        if (!cam.orthographic)
        {
            Debug.LogError("이 스크립트는 Orthographic 카메라에서만 작동합니다.");
            return;
        }

        // Menu 씬에서도 화면 가득 사용
        cam.aspect = (float)Screen.width / Screen.height;
        cam.rect = new Rect(0f, 0f, 1f, 1f);
    }
}
