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
        SetSlider(0.5f, 1f);
    }

    private void OnEnable()
    {
        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        musicSlider.onValueChanged.AddListener(ChangeSFXVolume);
        closeBtn.onClick.AddListener(OnCloseBtnClick);
    }

    private void OnDisable()
    {
        musicSlider.onValueChanged.RemoveListener(ChangeMusicVolume);
        musicSlider.onValueChanged.RemoveListener(ChangeSFXVolume);
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
