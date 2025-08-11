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
        UIManager.Instance.OpenUI<SettingUI>();
    }

    private void OnClickReplayBtn()
    {
        LoadingManager.instance.LoadScene("Puzzle");
    }

    private void OnClickContinueBtn()
    {
        GameManager.Instance.State = GameState.Playing;
        UIManager.Instance.CloseUI<PauseUI>();
    }
}
