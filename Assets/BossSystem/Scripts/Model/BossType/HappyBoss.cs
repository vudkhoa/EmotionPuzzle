using CustomUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HappyBoss : Boss
{
    [Header(" Info ")]
    public List<Vector2Int> DarkItems;
    public List<Vector2Int> CrimsonItems;

    [Header(" Tile ")]
    [SerializeField] private TileBase DarkItem;
    [SerializeField] private TileBase CrimsonItem;

    public override void Setup(float health, float cooldownTimeSkill, int totalItems, 
                                Vector2Int startPos, Vector2Int endPos, int totalPhases)
    {
        base.Setup(health, cooldownTimeSkill, totalItems, startPos, endPos, totalPhases);
        this.BossType = BossType.HappyBoss;
        this.Phase = 1;

        this.DarkItems = new List<Vector2Int>();
        this.CrimsonItems = new List<Vector2Int>();
    }

    public override void ActiveSkill()
    {
        if (this.BossState == BossState.Dead)
        {
            return;
        }

        this.IsActingSkill = true;
        if (Phase == 1)
        {
            Invoke(nameof(ActiveSkillPhase1), 0.25f);
        }
        else
        {
            Invoke(nameof(ActiveSkillPhase2), 0.25f);
        }
        Invoke(nameof(ResetIsActingSkill), 0.35f);
    }

    public void ActiveSkillPhase1()
    {
        this.ItemList = ItemTileController.Instance.FindItemCluster();
        this.CombineAndTransformItems(DarkItem);
    }

    public void ActiveSkillPhase2()
    {
        List<Vector2Int> tmpList = new List<Vector2Int>();
        tmpList = ItemTileController.Instance.FindItemAbsMin();
        this.ItemList = new List<Vector2Int>();
        this.ItemList.Add(tmpList[0]);
        tmpList.RemoveAt(0);
        this.CombineAndTransformItems(CrimsonItem);
    }

    public void CombineAndTransformItems(TileBase newItem)
    {
        foreach (Vector2Int posItem in this.ItemList)
        {
            ItemTileController.Instance.RemoveItem(posItem);
            if (this.Phase == 1)
            {
                this.DarkItems.Add(posItem);
            }
            else
            {
                this.CrimsonItems.Add(posItem);
                this.InteractWithItemOther();
            }
            SlideController.Instance.bossTilemap.SetTile(new Vector3Int(posItem.x, posItem.y, 0), newItem);
            this.DecreaseItems(1);
            this.AttackingPlayer(0f);
        }
        BossController.Instance.SpawnItems();
    }

    public override void AttackingPlayer(float time = 0.28f)
    {
        if (this.BossState == BossState.Active)
        {
            Invoke(nameof(RemoveBossTilemap), time);
            Invoke(nameof(InteractWithItemOther), (time + 0.01f));
        }
    }

    public void RemoveBossTilemap()
    {
        List<Vector2Int> offsets = new List<Vector2Int>();
        offsets = Library.Instance.LibOffsets8;
        Vector2Int playerPos = SlideController.Instance.GetPlayerPos();

        bool isAttacking = false;

        foreach (Vector2Int offset in offsets)
        {
            Vector2Int targetPos = playerPos + offset;
            if (this.DarkItems.Contains(targetPos))
            {
                isAttacking = true;
                this.DecreaseItems(1);
                this.DarkItems.Remove(targetPos);
                SlideController.Instance.bossTilemap.SetTile(new Vector3Int(targetPos.x, targetPos.y, 0), null);
            }
            else if (this.CrimsonItems.Contains(targetPos))
            {
                isAttacking = true;
                this.DecreaseItems(1);
            }
        }

        if (isAttacking)
        {
            SlideController.Instance.PlayerTakeDamage();
        }
    }

    public void InteractWithItemOther()
    {
        if (this.Phase == 1)
        {
            return;
        }
        List<Vector2Int> offsets = new List<Vector2Int>();
        offsets = Library.Instance.LibOffsets8;

        foreach (Vector2Int pos in this.CrimsonItems)
        {
            foreach (Vector2Int offset in offsets)
            {
                Vector2Int targetPos = pos + offset;
                if (SlideController.Instance.itemTilemap.HasTile(new Vector3Int(targetPos.x, targetPos.y, 0)))
                {
                    this.DarkItems.Add(targetPos);
                    ItemTileController.Instance.RemoveItem(targetPos);
                    SlideController.Instance.bossTilemap.SetTile(new Vector3Int(targetPos.x, targetPos.y, 0), this.DarkItem);
                    this.RemoveBossTilemap();
                    this.DecreaseItems(1);
                }
            }
        }
    }

    public override void CheckDie()
    {
        if (Phase == 1)
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
        this.Phase++;
        base.NextPhase();
    }
}
