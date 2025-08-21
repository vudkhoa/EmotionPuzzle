using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireGuideUI : UICanvas
{
    public Button closeBtn;

    private void OnEnable()
    {
        closeBtn.onClick.AddListener(OnClickCloseBtn);
        GameInput.Instance.OnCloseUI += GameInput_OnCloseUI;
    }


    private void OnDisable()
    {
        closeBtn.onClick.RemoveListener(OnClickCloseBtn);
        GameInput.Instance.OnCloseUI -= GameInput_OnCloseUI;
    }
    private void GameInput_OnCloseUI(object sender, EventArgs e)
    {
        if (GameManager.Instance.State == GameState.Pause)
        {
            OnClickCloseBtn();
        }
    }

    public void OnClickCloseBtn()
    {
        UIManager.Instance.CloseUI<FireGuideUI>();
        GameManager.Instance.State = GameState.Playing;
    }
}
