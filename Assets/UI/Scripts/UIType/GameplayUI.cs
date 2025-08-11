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

    [Header(" ElementPower ")]
    [SerializeField] private GameObject ElementPower;

    [Header(" Boss ")]
    [SerializeField] private BossUI BossUI; 
    [SerializeField] private TextMeshProUGUI BossName;
    [SerializeField] private Slider BossHealthBar;
    [SerializeField] private TextMeshProUGUI BossHealthText;
    [SerializeField] private Slider PlayerHealthBar;
    [SerializeField] private TextMeshProUGUI PlayerHealthText;


    private void OnEnable()
    {
        pauseBtn.onClick.AddListener(OnClickPauseBtn);
        tutorialBtn.onClick.AddListener(OnClickTutorialBtn);

        fire.onClick.AddListener(OnClickFireButton);
        water.onClick.AddListener(OnClickWaterButton);
        wind.onClick.AddListener(OnClickWindButton);
        ice.onClick.AddListener(OnClickIceButton);

        HideElementGuideBtn();
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
        UIManager.Instance.OpenUI<GuideUI>().Init();
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
