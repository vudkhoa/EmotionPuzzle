using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public enum BossState
{
    None = 0,
    Dead = 1,
    Active = 2,
}
public abstract class Boss : MonoBehaviour
{
    [Header(" Info ")]
    public float Health;
    public float CooldownTimeSkill;
    public int TotalItems;
    public BossType BossType;
    public BossState BossState;
    public Vector2Int StartPos;
    public Vector2Int EndPos;
    public int TotalPhases;

    public int Phase;
    public bool IsActingSkill;
    private float curTimeSkill;

    public List<Vector2Int> ItemList;

    public virtual void Setup(  float health, float cooldownTimeSkill, int totalItems, 
                                Vector2Int startPos, Vector2Int endPos, int totalPhases)
    {
        this.Health = health;
        this.CooldownTimeSkill = cooldownTimeSkill;
        this.TotalItems = totalItems;
        this.StartPos = startPos;
        this.EndPos = endPos;

        this.IsActingSkill = false;
        this.BossState = BossState.Active;

        this.ItemList = new List<Vector2Int>();
        this.TotalPhases = totalPhases;
    }

    private void Update()
    {
        if (SlideController.Instance.BossId <= 0)
        {
            return;
        }
        this.CaculateCooldownTimeSkill();
    }

    public abstract void ActiveSkill();

    public void TakeDamage(int damage)
    {
        this.Health -= damage;
        BossController.Instance.SpawnItems();
        this.transform.DOShakePosition(
            duration: 0.2f,
            strength: new Vector3(0.2f, 0.2f, 0),
            vibrato: 100,
            randomness: 90,
            snapping: false,
            fadeOut: true
        );

        if (this.Health <= 0)
        {
            this.CheckDie();
        }
    }

    public virtual void AttackingPlayer(float time = 0.28f) {}

    public void CaculateCooldownTimeSkill()
    {
        this.curTimeSkill += Time.deltaTime;
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
        this.Health = DataManager.Instance.SadBossData.BossList[SlideController.Instance.BossId - 1].Health;
        this.CooldownTimeSkill /= 2f;
    }

    public void DecreaseItems(int count)
    {
        this.TotalItems -= count;
    }
}
