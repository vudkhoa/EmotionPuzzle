using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
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
    }

    public abstract void ActiveSkill();

    public void TakeDamage(int damage)
    {
        this.CurHealth -= damage;
        UIManager.Instance.GetUI<GameplayUI>().UpdateBossHealth(this.CurHealth, this.Healths[this.CurPhase - 1]);
        BossController.Instance.SpawnItems();
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
        UIManager.Instance.GetUI<GameplayUI>().UpdateBossHealth(this.CurHealth, this.Healths[this.CurPhase - 1]);
        this.CooldownTimeSkill /= 2f;
    }

    public void DecreaseItems(int count)
    {
        this.TotalItems -= count;
        UIManager.Instance.GetUI<GameplayUI>().UpdatePlayerHealth(this.TotalItems, this.MaxItem);
    }
}