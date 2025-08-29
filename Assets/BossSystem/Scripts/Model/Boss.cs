using DG.Tweening;
using SoundManager;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum BossState
{
    None = 0,
    Dead = 1,
    Active = 2,
}
public abstract class Boss : MonoBehaviour
{
    [Header(" Info ")]
    public List<float> Healths;
    public float CurHealth;

    public float CooldownTimeSkill;
    public int TotalItems;
    public int MaxItem;

    public BossType BossType;
    public BossState BossState;
    public Vector2Int StartPos;
    public Vector2Int EndPos;

    public int CurPhase;
    public bool IsActingSkill;
    private float curTimeSkill;

    public List<Vector2Int> ItemList;

    [Header(" UI ")]
    [SerializeField] private Slider EnergyBar;
    [SerializeField] private Slider HealthBar;
    [SerializeField] private TextMeshProUGUI HealthText;
    [SerializeField] private GameObject Body;

    public virtual void Setup(  List<float> healths, float cooldownTimeSkill, int totalItems, 
                                Vector2Int startPos, Vector2Int endPos)
    {
        this.Healths = healths;
        this.CooldownTimeSkill = cooldownTimeSkill;

        this.TotalItems = totalItems;
        this.MaxItem = totalItems;

        this.StartPos = startPos;
        this.EndPos = endPos;

        this.IsActingSkill = false;
        this.BossState = BossState.Active;

        this.ItemList = new List<Vector2Int>();
        this.EnergyBar.value = 0f;
        this.HealthBar.value = 1f;

        this.AddCanvasGroup();
        this.BlockBars();
    }

    private void AddCanvasGroup()
    {
        this.HealthBar.AddComponent<CanvasGroup>();
        this.EnergyBar.AddComponent<CanvasGroup>();
    }

    private void BlockBars()
    {
        this.HealthBar.GetComponent<CanvasGroup>().blocksRaycasts = false;
        this.EnergyBar.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    private void Update()
    {
        if (SlideController.Instance.BossId <= 0)
        {
            return;
        }

        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

        this.CaculateCooldownTimeSkill();
        this.CooldownTimeSkillUI();
    }

    public virtual void CooldownTimeSkillUI() { }

    public void UpdateHealthUI(float curHealth, float maxHealth)
    {
        this.HealthBar.value = curHealth / maxHealth;
        this.HealthText.text = curHealth.ToString() + "/" + maxHealth.ToString();
    }

    public abstract void ActiveSkill();

    public void TakeDamage(int damage)
    {
        this.CurHealth -= damage;
        this.UpdateHealthUI(this.CurHealth, this.Healths[this.CurPhase - 1]);
        BossController.Instance.SpawnItems();
        SoundsManager.Instance.PlaySFX(SoundType.ExBomb);
        this.transform.DOShakePosition(
            duration: 0.2f,
            strength: new Vector3(0.2f, 0.2f, 0),
            vibrato: 100,
            randomness: 90,
            snapping: false,
            fadeOut: true
        );




        if (this.CurHealth <= 0)
        {
            if (this.CurPhase == this.Healths.Count) { this.DecreaseItems(1); }
            this.CheckDie();
        }
    }

    public virtual void AttackingPlayer(float time = 0.28f) {}

    public void CaculateCooldownTimeSkill()
    {
        this.curTimeSkill += Time.deltaTime;
        this.EnergyBar.value = this.curTimeSkill / this.CooldownTimeSkill;
        if (this.curTimeSkill >= this.CooldownTimeSkill)
        {
            this.curTimeSkill = 0f;
            this.ActiveSkill();
        }
    }

    public void ResetIsActingSkill()
    {
        this.IsActingSkill = false;
    }

    public abstract void CheckDie();

    public virtual void NextPhase()
    {
        this.CurHealth = this.Healths[CurPhase - 1];
        this.UpdateHealthUI(this.CurHealth, this.Healths[this.CurPhase - 1]);
        this.CooldownTimeSkill /= 2f;
    }

    public void DecreaseItems(int count)
    {
        if (this.BossState == BossState.Dead)
        {
            return;
        }

        this.TotalItems -= count;
        UIManager.Instance.GetUI<GameplayUI>().UpdatePlayerHealth(this.TotalItems, this.MaxItem);

        if (this.TotalItems < this.CurHealth)
        {
            SlideController.Instance.PlayerDie();
            SoundsManager.Instance.PlaySFX(SoundType.LoseBoss);
            GameManager.Instance.State = GameState.GameOver;
            this.BossState = BossState.Dead;
            Invoke(nameof(LoseGameBoss), 0.3f);
        }
    }

    public void LoseGameBoss()
    {
        UIManager.Instance.OpenUI<LoseUI>();
    }

    public void Die()
    {
        DG.Tweening.Sequence deathSq = DOTween.Sequence();

        deathSq.Append(this.Body.transform.DOShakePosition(0.2f, 0.5f, 100, 90, false, true));
        deathSq.Join(this.Body.transform.DORotate(new Vector3(0, 0, 720f), 0.2f, RotateMode.FastBeyond360));
        deathSq.OnComplete(() => 
        { 
            Destroy(this.Body.gameObject);
        });
    }
}