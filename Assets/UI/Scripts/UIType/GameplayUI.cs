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
    [SerializeField] private Image tutorialFill;
    [SerializeField] private RectTransform panelTextTf;
    [SerializeField] private TextMeshProUGUI tutorialText;

    [Header(" Element Guide ")]
    [SerializeField] private Button fire;
    [SerializeField] private Button water;
    [SerializeField] private Button ice;
    [SerializeField] private Button wind;

    private bool isShowing = false;
    private float currentShowTime = 0f;

    private void Awake()
    {
        isShowing = false;
        currentShowTime = 0f;
    }

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
        if (isShowing)
        {
            currentShowTime += Time.deltaTime;
            tutorialFill.fillAmount = 1 - currentShowTime / hideTime;
            if (currentShowTime >= hideTime)
            {
                HideTutorialText();
            }
        }

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
        //panelTextTf.gameObject.SetActive(true);
        SetTutorialText(text);

        isShowing = true;
        currentShowTime = 0f;
        tutorialFill.fillAmount = 1f;
    }

    public void HideTutorialText()
    {
        //panelTextTf.gameObject.SetActive(false);
        isShowing = false;
        panelTextTf.DOScale(0f, 0.3f).SetEase(Ease.InBack);
    }

    private void SetTutorialText(string text)
    {
        tutorialText.text = text;
        ResizeImageToFitText();

        panelTextTf.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

    }

    private void ResizeImageToFitText()
    {
        // Force text to update its layout
        tutorialText.ForceMeshUpdate();

        // Get preferred width and height of the text
        Vector2 textSize = new Vector2(
            tutorialText.preferredWidth,
            tutorialText.preferredHeight
        );

        // Apply padding and set size
        panelTextTf.sizeDelta = textSize + new Vector2(20f, 10f);
    }
}
