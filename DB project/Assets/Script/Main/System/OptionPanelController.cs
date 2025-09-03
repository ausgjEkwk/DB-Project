using UnityEngine;
using UnityEngine.UI;

public class OptionPanelController : MonoBehaviour
{
    [Header("UI References")]
    public Slider bgmSlider;          // �Ϲ� BGM
    public Slider bossBgmSlider;      // ���� BGM
    public Slider attackSfxSlider;    // ���� SFX
    public Slider damagedSfxSlider;   // �ǰ� SFX

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            if (bgmSlider != null)
                bgmSlider.value = AudioManager.Instance.NormalBGMVolume;

            if (bossBgmSlider != null)
                bossBgmSlider.value = AudioManager.Instance.BossBGMVolume;

            if (attackSfxSlider != null)
                attackSfxSlider.value = AudioManager.Instance.AttackSFXVolume;

            if (damagedSfxSlider != null)
                damagedSfxSlider.value = AudioManager.Instance.HitSFXVolume;
        }

        // �����̴� �̺�Ʈ ���
        if (bgmSlider != null)
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);

        if (bossBgmSlider != null)
            bossBgmSlider.onValueChanged.AddListener(OnBossBGMVolumeChanged);

        if (attackSfxSlider != null)
            attackSfxSlider.onValueChanged.AddListener(OnAttackSFXVolumeChanged);

        if (damagedSfxSlider != null)
            damagedSfxSlider.onValueChanged.AddListener(OnDamagedSFXVolumeChanged);
    }

    private void OnBGMVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.NormalBGMVolume = value;
    }

    private void OnBossBGMVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.BossBGMVolume = value;
    }

    private void OnAttackSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.AttackSFXVolume = value;
    }

    private void OnDamagedSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.HitSFXVolume = value;
    }
}
