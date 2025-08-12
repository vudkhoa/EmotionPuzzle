using DG.Tweening;
using SoundManager;
using System.Collections.Generic;
using UnityEngine;

public class SadBoss : Boss
{
    [Header(" SkilSprite ")]
    [SerializeField] private GameObject SkillPrefab;

    public override void Setup(List<float> healths, float cooldownTimeSkill, int totalItems, 
                                Vector2Int startPos, Vector2Int endPos)
    {
        base.Setup(healths, cooldownTimeSkill, totalItems, startPos, endPos);
        this.BossType = BossType.SadBoss;
        this.CurPhase = 1;
        this.CurHealth = this.Healths[CurPhase - 1];
        UIManager.Instance.GetUI<GameplayUI>().SetupBoss("Sad Boss", this.CurHealth, this.TotalItems);
    }

    public override void ActiveSkill()
    {
        if (this.BossState == BossState.Dead)
        {
            return;
        }

        this.IsActingSkill = true;
        if (CurPhase == 1)
        {
            Invoke(nameof(ActiveSkillPhase1), 0.25f);
        }
        else
        {
            Invoke(nameof(ActiveSkillPhase2), 0.25f);
        }

        // Cho Player bị khựng lại 1 nhịp cảm giác tốt hơn
        Invoke(nameof(ResetIsActingSkill), 0.35f);
    }

    public void ActiveSkillPhase1()
    {
        this.ItemList = ItemTileController.Instance.FindItemCluster();
        this.RemoveItems();
    }

    public void ActiveSkillPhase2()
    {
        this.ItemList = ItemTileController.Instance.FindItemAbsMin();
        this.RemoveItems();
    }

    public void RemoveItems()
    {
        foreach (Vector2Int posItem in this.ItemList)
        {
            Sprite sp = this.SkillPrefab.GetComponent<SpriteRenderer>().sprite;
            Vector3Int pos = new Vector3Int(posItem.x, posItem.y, 0);
            TileFake ob = Instantiate(SlideController.Instance.itemTileFakePrefab,
                            SlideController.Instance.itemTilemap.GetCellCenterWorld(pos), Quaternion.identity);
            ob.SetSprite(sp);
            ob.transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InBack).OnComplete(() =>
            {
                Destroy(ob.gameObject);
            });

            ItemTileController.Instance.RemoveItem(posItem);
            
            this.DecreaseItems(1);
        }
        SoundsManager.Instance.PlaySFX(SoundType.BossDestroyItem);
        BossController.Instance.SpawnItems();
    }

    public override void CheckDie()
    {
        if (CurPhase < this.Healths.Count)
        {
            this.NextPhase();
        }
        else
        {
            this.BossState = BossState.Dead;
            SlideController.Instance.BossId = 0;
            Destroy(this.gameObject);
        }
    }

    public override void NextPhase()
    {
        this.CurPhase++;
        base.NextPhase();
    }
}
