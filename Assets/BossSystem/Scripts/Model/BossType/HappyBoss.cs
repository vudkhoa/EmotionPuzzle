using CustomUtils;
using SoundManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HappyBoss : Boss
{
    [Header(" Info ")]
    public List<Vector2Int> DarkItems;
    //public List<Vector2Int> CrimsonItems;

    [Header(" Tile ")]
    [SerializeField] private TileBase DarkItem;
    //[SerializeField] private TileBase CrimsonItem;

    public override void Setup(List<float> healths, float cooldownTimeSkill, int totalItems, 
                                Vector2Int startPos, Vector2Int endPos)
    {
        base.Setup(healths, cooldownTimeSkill, totalItems, startPos, endPos);
        this.BossType = BossType.HappyBoss;
        this.CurPhase = 1;
        this.CurHealth = this.Healths[CurPhase - 1];

        this.DarkItems = new List<Vector2Int>();
        //this.CrimsonItems = new List<Vector2Int>();
        UIManager.Instance.GetUI<GameplayUI>().SetupBoss("Happy Boss", this.TotalItems);
        this.UpdateHealthUI(this.CurHealth, this.CurHealth);
    }

    public override void ActiveSkill()
    {
        if (this.BossState == BossState.Dead)
        {
            return;
        }

        this.IsActingSkill = true;
        Invoke(nameof(ActiveSkillPhase1), 0.25f);
        //if (CurPhase < this.Healths.Count)
        //{
        //    Invoke(nameof(ActiveSkillPhase1), 0.25f);
        //}
        //else
        //{
        //    Invoke(nameof(ActiveSkillPhase2), 0.25f);
        //}
        Invoke(nameof(ResetIsActingSkill), 0.35f);
    }

    public void ActiveSkillPhase1()
    {
        //Debug.Log("Happy Phase 1");
        this.ItemList = ItemTileController.Instance.FindItemCluster();
        this.CombineAndTransformItems(DarkItem);
    }

    //public void ActiveSkillPhase2()
    //{
    //    List<Vector2Int> tmpList = new List<Vector2Int>();
    //    tmpList = ItemTileController.Instance.FindItemAbsMin();
    //    this.ItemList = new List<Vector2Int>();
    //    this.ItemList.Add(tmpList[0]);
    //    tmpList.RemoveAt(0);
    //    this.CombineAndTransformItems(CrimsonItem);
    //}

    public void CombineAndTransformItems(TileBase newItem)
    {
        foreach (Vector2Int posItem in this.ItemList)
        {
            ItemTileController.Instance.RemoveItem(posItem);
            this.DarkItems.Add(posItem);
            //if (this.CurPhase < this.Healths.Count)
            //{
            //    this.DarkItems.Add(posItem);
            //}
            //else
            //{
            //    this.CrimsonItems.Add(posItem);
            //    this.InteractWithItemOther();
            //}
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
            //Invoke(nameof(InteractWithItemOther), (time + 0.01f));
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

                Vector3 worldPlayerPos = SlideController.Instance.obstacleTilemap.GetCellCenterWorld(new Vector3Int(playerPos.x, playerPos.y, 0));
                Vector3 worldTargetPos = SlideController.Instance.obstacleTilemap.GetCellCenterWorld(new Vector3Int(targetPos.x, targetPos.y, 0));

                Vector3 spawnPos = new Vector3();
                spawnPos.x = Mathf.Abs(worldPlayerPos.x + worldTargetPos.x) / 2;
                spawnPos.y = Mathf.Abs(worldPlayerPos.y + worldTargetPos.y) / 2;

                GroundTileController.Instance.ActiveBurnDownEffect(spawnPos);
            }
            //else if (this.CrimsonItems.Contains(targetPos))
            //{
            //    Vector3 worldPlayerPos = SlideController.Instance.obstacleTilemap.GetCellCenterWorld(new Vector3Int(playerPos.x, playerPos.y, 0));
            //    Vector3 worldTargetPos = SlideController.Instance.obstacleTilemap.GetCellCenterWorld(new Vector3Int(targetPos.x, targetPos.y, 0));

            //    Vector3 spawnPos = new Vector3();
            //    spawnPos.x = Mathf.Abs(worldPlayerPos.x + worldTargetPos.x) / 2;
            //    spawnPos.y = Mathf.Abs(worldPlayerPos.y + worldTargetPos.y) / 2;

            //    GroundTileController.Instance.ActiveBurnDownEffect(spawnPos);

            //    isAttacking = true;
            //    this.DecreaseItems(1);
            //}
        }

        if (isAttacking)
        {
            SlideController.Instance.PlayerTakeDamage();
            SoundsManager.Instance.PlaySFX(SoundType.ExBomb);
        }
    }

    //public void InteractWithItemOther()
    //{
    //    if (this.CurPhase == 1)
    //    {
    //        return;
    //    }
    //    List<Vector2Int> offsets = new List<Vector2Int>();
    //    offsets = Library.Instance.LibOffsets8;

    //    foreach (Vector2Int pos in this.CrimsonItems)
    //    {
    //        foreach (Vector2Int offset in offsets)
    //        {
    //            Vector2Int targetPos = pos + offset;
    //            if (SlideController.Instance.itemTilemap.HasTile(new Vector3Int(targetPos.x, targetPos.y, 0)))
    //            {
    //                this.DarkItems.Add(targetPos);
    //                ItemTileController.Instance.RemoveItem(targetPos);
    //                SlideController.Instance.bossTilemap.SetTile(new Vector3Int(targetPos.x, targetPos.y, 0), this.DarkItem);
    //                this.RemoveBossTilemap();
    //                this.DecreaseItems(1);
    //            }
    //        }
    //    }
    //}

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
            this.Die();
        }
    }

    public override void NextPhase()
    {
        this.CurPhase++;
        base.NextPhase();
    }
}
