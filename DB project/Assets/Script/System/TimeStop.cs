using UnityEngine;

public class TimeStop : MonoBehaviour
{
    public static TimeStop Instance { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsTimeStopped { get; private set; } = false;

    public void StartTimeStop()
    {
        IsTimeStopped = true;

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null) player.enabled = false;

        BackgroundScroll bg = FindObjectOfType<BackgroundScroll>();
        if (bg != null) bg.enabled = false;
    }

    public void EndTimeStop()
    {
        IsTimeStopped = false;

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null) player.enabled = true;

        BackgroundScroll bg = FindObjectOfType<BackgroundScroll>();
        if (bg != null) bg.enabled = true;
    }
}
