using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IceGuideUI : UICanvas
{
    public Button closeBtn;

    private void OnEnable()
    {
        closeBtn.onClick.AddListener(OnClickCloseBtn);
    }

    private void OnDisable()
    {
        closeBtn.onClick.RemoveListener(OnClickCloseBtn);
    }

    public void OnClickCloseBtn()
    {
        UIManager.Instance.CloseUI<IceGuideUI>();
    }
}
