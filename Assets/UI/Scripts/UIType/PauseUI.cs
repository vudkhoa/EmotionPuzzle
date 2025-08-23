using SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : UICanvas
{
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button replayBtn;
    [SerializeField] private Button settingBtn;

    private void OnEnable()
    {
        continueBtn.onClick.AddListener(OnClickContinueBtn);
        replayBtn.onClick.AddListener(OnClickReplayBtn);
        settingBtn.onClick.AddListener(OnClickSettingBtn);
    }

    private void OnDisable()
    {
        continueBtn.onClick.RemoveListener(OnClickContinueBtn);
        replayBtn.onClick.RemoveListener(OnClickReplayBtn);
        settingBtn.onClick.RemoveListener(OnClickSettingBtn);
    }

    private void OnClickSettingBtn()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        UIManager.Instance.OpenUI<SettingUI>();
    }

    private void OnClickReplayBtn()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        if (SavePointController.Instance.IsSave())
        {
            SlideController.Instance.Reload();
        }
        else
        {
            LoadingManager.instance.LoadScene("Puzzle");
        }
        GameManager.Instance.State = GameState.Playing;
        UIManager.Instance.CloseUI<PauseUI>();
    }

    private void OnClickContinueBtn()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        GameManager.Instance.State = GameState.Playing;
        UIManager.Instance.CloseUI<PauseUI>();
    }
}
