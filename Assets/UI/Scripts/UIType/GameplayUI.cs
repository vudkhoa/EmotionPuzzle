using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : UICanvas
{
    [SerializeField] private float hideTime = 5f;

    [SerializeField] private Button pauseBtn;
    [SerializeField] private Button tutorialBtn;
    [SerializeField] private Transform messageListTf;
    [SerializeField] private GuideMessage messagePrefab;

    [Header(" Element Guide ")]
    [SerializeField] private Button fire;
    [SerializeField] private Button water;
    [SerializeField] private Button ice;
    [SerializeField] private Button wind;

    private void OnEnable()
    {
        pauseBtn.onClick.AddListener(OnClickPauseBtn);
        tutorialBtn.onClick.AddListener(OnClickTutorialBtn);

        fire.onClick.AddListener(OnClickFireButton);
        water.onClick.AddListener(OnClickWaterButton);
        wind.onClick.AddListener(OnClickWindButton);
        ice.onClick.AddListener(OnClickIceButton);
    }

    private void OnDisable()
    {
        pauseBtn.onClick.RemoveListener(OnClickPauseBtn);
        tutorialBtn.onClick.RemoveListener(OnClickTutorialBtn);

        fire.onClick.RemoveListener(OnClickFireButton);
        water.onClick.RemoveListener(OnClickWaterButton);
        wind.onClick.RemoveListener(OnClickWindButton);
        ice.onClick.RemoveListener(OnClickIceButton);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            OnClickPauseBtn();
        }
        else if (Input.GetKeyUp(KeyCode.T))
        {
            OnClickTutorialBtn();
        }
    }

    private void OnClickPauseBtn()
    {
        GameManager.Instance.State = GameState.Pause;
        UIManager.Instance.OpenUI<PauseUI>();
    }

    private void OnClickTutorialBtn()
    {
        UIManager.Instance.OpenUI<GuideUI>();
    }

    private void OnClickFireButton()
    {
        UIManager.Instance.OpenUI<FireGuideUI>();
    }
    private void OnClickWaterButton()
    {
        UIManager.Instance.OpenUI<WaterGuideUI>();
    }

    private void OnClickWindButton()
    {
        UIManager.Instance.OpenUI<WindGuideUI>();
    }

    private void OnClickIceButton()
    {
        UIManager.Instance.OpenUI<IceGuideUI>();
    }

    public void ShowTutorialText(string text)
    {
        GuideMessage gmOb = Instantiate(messagePrefab, messageListTf);
        gmOb.ShowTutorialText(text);
    }
}
