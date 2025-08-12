using SoundManager;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : UICanvas
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button closeBtn;

    private void Awake()
    {
        SetSlider(PlayerPrefs.GetFloat("Music Volume", 0.5f), PlayerPrefs.GetFloat("VFX Volume", 1));
    }

    private void OnEnable()
    {
        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSFXVolume);
        closeBtn.onClick.AddListener(OnCloseBtnClick);
    }

    private void OnDisable()
    {
        musicSlider.onValueChanged.RemoveListener(ChangeMusicVolume);
        sfxSlider.onValueChanged.RemoveListener(ChangeSFXVolume);
        closeBtn.onClick.RemoveListener(OnCloseBtnClick);
    }

    public void OnCloseBtnClick()
    {
        UIManager.Instance.CloseUI<SettingUI>();
    }

    public void SetSlider(float musicValue, float sfxValue)
    {
        musicSlider.value = musicValue;
        sfxSlider.value = sfxValue;
    }

    public void ChangeMusicVolume(float value)
    {
        SoundsManager.Instance.SetMusicVolume(value);
    }

    public void ChangeSFXVolume(float value)
    {
        SoundsManager.Instance.SetSFXVolume(value);
    }
}
