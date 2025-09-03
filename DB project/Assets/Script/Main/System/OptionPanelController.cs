using UnityEngine;
using UnityEngine.UI;

public class OptionPanelController : MonoBehaviour
{
    [Header("UI References")]
    public Slider bgmSlider;          // 일반 BGM
    public Slider bossBgmSlider;      // 보스 BGM
    public Slider attackSfxSlider;    // 공격 SFX
    public Slider damagedSfxSlider;   // 피격 SFX

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

        // 슬라이더 이벤트 등록
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
