using SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinUI : UICanvas
{
    [SerializeField] private Button homeBtn;
    [SerializeField] private Button replayBtn;
    [SerializeField] private Button continueBtn;

    private void OnEnable()
    {
        continueBtn.onClick.AddListener(OnClickContinueBtn);
        replayBtn.onClick.AddListener(OnClickReplayBtn);
        homeBtn.onClick.AddListener(OnClickHomeBtn);
    }

    private void OnDisable()
    {
        continueBtn.onClick.RemoveListener(OnClickContinueBtn);
        replayBtn.onClick.RemoveListener(OnClickReplayBtn);
        homeBtn.onClick.RemoveListener(OnClickHomeBtn);
    }

    private void OnClickHomeBtn()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        LoadingManager.instance.LoadScene("Menu");
    }

    private void OnClickReplayBtn()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        LoadingManager.instance.LoadScene("Board");
    }

    private void OnClickContinueBtn()
    {
        
    }

}
