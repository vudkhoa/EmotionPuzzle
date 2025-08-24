using DG.Tweening;
using SoundManager;
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

    [Header(" ElementPower ")]
    [SerializeField] private GameObject ElementPower;

    [Header(" Boss ")]
    [SerializeField] private BossUI BossUI; 
    [SerializeField] private TextMeshProUGUI BossName;
    [SerializeField] private Slider BossHealthBar;
    [SerializeField] private TextMeshProUGUI BossHealthText;
    [SerializeField] private Slider PlayerHealthBar;
    [SerializeField] private TextMeshProUGUI PlayerHealthText;

    [Header(" Button Number Count ")]
    public List<GameObject> buttonList;
    private int btnCount = 0;


    private void OnEnable()
    {
        pauseBtn.onClick.AddListener(OnClickPauseBtn);
        GameInput.Instance.OnPause += GameInput_OnPause;

        tutorialBtn.onClick.AddListener(OnClickTutorialBtn);
        GameInput.Instance.OnTutorial += GameInput_OnTutorial;

        fire.onClick.AddListener(OnClickFireButton);
        water.onClick.AddListener(OnClickWaterButton);
        wind.onClick.AddListener(OnClickWindButton);
        ice.onClick.AddListener(OnClickIceButton);

        GameInput.Instance.OnOneButton += GameInput_OnOneButton;
        GameInput.Instance.OnTWoButton += GameInput_OnTWoButton;
        GameInput.Instance.OnThreeButton += GameInput_OnThreeButton;
        GameInput.Instance.OnFourButton += GameInput_OnFourButton;

        HideElementGuideBtn();
    }

    private void OnDisable()
    {
        pauseBtn.onClick.RemoveListener(OnClickPauseBtn);
        GameInput.Instance.OnPause -= GameInput_OnPause;

        tutorialBtn.onClick.RemoveListener(OnClickTutorialBtn);
        GameInput.Instance.OnTutorial -= GameInput_OnTutorial;

        fire.onClick.RemoveListener(OnClickFireButton);
        water.onClick.RemoveListener(OnClickWaterButton);
        wind.onClick.RemoveListener(OnClickWindButton);
        ice.onClick.RemoveListener(OnClickIceButton);

        GameInput.Instance.OnOneButton -= GameInput_OnOneButton;
        GameInput.Instance.OnTWoButton -= GameInput_OnTWoButton;
        GameInput.Instance.OnThreeButton -= GameInput_OnThreeButton;
        GameInput.Instance.OnFourButton -= GameInput_OnFourButton;
    }

    private void GameInput_OnFourButton(object sender, System.EventArgs e)
    {
        OnClickFourButton();
    }

    private void GameInput_OnThreeButton(object sender, System.EventArgs e)
    {
        OnClickThreeButton();
    }

    private void GameInput_OnTWoButton(object sender, System.EventArgs e)
    {
        OnClickTwoButton();
    }

    private void GameInput_OnOneButton(object sender, System.EventArgs e)
    {
        OnClickOneButton();
    }

    private void GameInput_OnTutorial(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.State == GameState.Playing)
        {
            OnClickTutorialBtn();
        }
    }

    private void GameInput_OnPause(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.State == GameState.Playing)
        {
            OnClickPauseBtn();
        }
    }

    //private void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.Escape))
    //    {
    //        OnClickPauseBtn();
    //    }
    //    else if (Input.GetKeyUp(KeyCode.T))
    //    {
    //        OnClickTutorialBtn();
    //    }
    //}

    private void OnClickPauseBtn()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        GameManager.Instance.State = GameState.Pause;
        UIManager.Instance.OpenUI<PauseUI>();
    }

    private void OnClickTutorialBtn()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Book);
        GameManager.Instance.State = GameState.Pause;
        UIManager.Instance.OpenUI<GuideUI>().Init();
    }

    private void OnClickFireButton()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        GameManager.Instance.State = GameState.Pause;
        UIManager.Instance.OpenUI<FireGuideUI>();
    }
    private void OnClickWaterButton()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        GameManager.Instance.State = GameState.Pause;
        UIManager.Instance.OpenUI<WaterGuideUI>();
    }

    private void OnClickWindButton()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        GameManager.Instance.State = GameState.Pause;
        UIManager.Instance.OpenUI<WindGuideUI>();
    }

    private void OnClickIceButton()
    {
        SoundsManager.Instance.PlaySFX(SoundType.Click);
        GameManager.Instance.State = GameState.Pause;
        UIManager.Instance.OpenUI<IceGuideUI>();
    }

    public void ShowTutorialText(string text, float time)
    {
        GuideMessage gmOb = Instantiate(messagePrefab, messageListTf);
        gmOb.ShowTutorialText(text, time);
    }

    public void HideElementGuideBtn()
    {
        fire.gameObject.SetActive(false);
        water.gameObject.SetActive(false);
        wind.gameObject.SetActive(false);
        ice.gameObject.SetActive(false);
    }

    public void ShowElementGuideBtn(bool haveFire, bool haveWater, bool haveWind, bool haveIce)
    {
        if (haveFire)
        {
            fire.gameObject.SetActive(true);
        }

        if (haveWater)
        {
            water.gameObject.SetActive(true);
        }

        if (haveWind)
        {
            wind.gameObject.SetActive(true);
        }

        if (haveIce)
        {
            ice.gameObject.SetActive(true);
        }
    }

    public IEnumerator ShowElementGuideUI(bool haveFire, bool haveWater, bool haveWind, bool haveIce, float time)
    {

        yield return new WaitForSeconds(time);
        if (haveFire || haveWater || haveWind || haveIce)
        {
            GameManager.Instance.State = GameState.Pause;
        }


        if (haveFire)
        {
            UIManager.Instance.OpenUI<FireGuideUI>();
        }

        if (haveWater)
        {
            UIManager.Instance.OpenUI<WaterGuideUI>();
        }

        if (haveWind)
        {
            UIManager.Instance.OpenUI<WindGuideUI>();
        }

        if (haveIce)
        {
            UIManager.Instance.OpenUI<IceGuideUI>();
        }
    }

    public void UpdateElementButtonNumberGuide()
    {
        for (int i = 0; i < 4; i++)
        {
            this.buttonList[i].gameObject.SetActive(false);
        }

        btnCount = 0;
        for (int i = 0; i < 4; i++)
        {
            if (ElementGuideManager.Instance.isShowBtn[i])
            {
                btnCount++;
                this.buttonList[btnCount - 1].gameObject.SetActive(true);
            }
        }
    }

    public void OnClickOneButton()
    {
        if (btnCount >= 1)
        {
            if (ElementGuideManager.Instance.isShowBtn[0])
            {
                UIManager.Instance.OpenUI<FireGuideUI>();

                return;
            }

            if (ElementGuideManager.Instance.isShowBtn[1])
            {
                UIManager.Instance.OpenUI<WaterGuideUI>();

                return;
            }

            if (ElementGuideManager.Instance.isShowBtn[2])
            {
                UIManager.Instance.OpenUI<IceGuideUI>();

                return;
            }

            if (ElementGuideManager.Instance.isShowBtn[3])
            {
                UIManager.Instance.OpenUI<WindGuideUI>();

                return;
            }
        }
    }

    public void OnClickTwoButton()
    {
        if (btnCount >= 2)
        {
            if (ElementGuideManager.Instance.isShowBtn[0])
            {
                UIManager.Instance.OpenUI<FireGuideUI>();

                return;
            }

            if (ElementGuideManager.Instance.isShowBtn[1])
            {
                UIManager.Instance.OpenUI<WaterGuideUI>();

                return;
            }

            if (ElementGuideManager.Instance.isShowBtn[2])
            {
                UIManager.Instance.OpenUI<IceGuideUI>();

                return;
            }

            if (ElementGuideManager.Instance.isShowBtn[3])
            {
                UIManager.Instance.OpenUI<WindGuideUI>();

                return;
            }
        }
    }

    public void OnClickThreeButton()
    {
        if (btnCount >= 3)
        {
            if (ElementGuideManager.Instance.isShowBtn[0])
            {
                UIManager.Instance.OpenUI<FireGuideUI>();

                return;
            }

            if (ElementGuideManager.Instance.isShowBtn[1])
            {
                UIManager.Instance.OpenUI<WaterGuideUI>();

                return;
            }

            if (ElementGuideManager.Instance.isShowBtn[2])
            {
                UIManager.Instance.OpenUI<IceGuideUI>();

                return;
            }

            if (ElementGuideManager.Instance.isShowBtn[3])
            {
                UIManager.Instance.OpenUI<WindGuideUI>();

                return;
            }
        }
    }

    public void OnClickFourButton()
    {
        if (btnCount >= 4)
        {
            if (ElementGuideManager.Instance.isShowBtn[0])
            {
                UIManager.Instance.OpenUI<FireGuideUI>();

                return;
            }

            if (ElementGuideManager.Instance.isShowBtn[1])
            {
                UIManager.Instance.OpenUI<WaterGuideUI>();

                return;
            }

            if (ElementGuideManager.Instance.isShowBtn[2])
            {
                UIManager.Instance.OpenUI<IceGuideUI>();

                return;
            }

            if (ElementGuideManager.Instance.isShowBtn[3])
            {
                UIManager.Instance.OpenUI<WindGuideUI>();

                return;
            }
        }
    }

    public void SetPowerAndBossUI(bool haveElementPower)
    {
        this.ElementPower.SetActive(haveElementPower);
        this.BossUI.gameObject.SetActive(!haveElementPower);
    }

    public void SetupBoss(string bossName, float bossHealth, int playerHealth)
    {
        this.BossName.text = bossName;
        this.BossHealthBar.value = 1f;
        this.PlayerHealthBar.value = 1f;
        this.BossHealthText.text = bossHealth.ToString() + "/" + bossHealth.ToString();
        this.PlayerHealthText.text = playerHealth.ToString() + "/" + playerHealth.ToString();
    }

    public void UpdateBossHealth(float curHealth, float maxHealth)
    {
        this.BossHealthBar.value = curHealth / maxHealth;
        this.BossHealthText.text = curHealth.ToString() + "/" + maxHealth.ToString();
    }

    public void UpdatePlayerHealth(float curHealth, float maxHealth)
    {

        this.PlayerHealthBar.value = curHealth / maxHealth;
        this.PlayerHealthText.text = curHealth.ToString() + "/" + maxHealth.ToString();
    }
}
