using CustomUtils;
using DG.Tweening;
using SoundManager;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class HappyBoss : Boss
{
    [Header(" Info ")]
    public List<Vector2Int> DarkItems;
    //public List<Vector2Int> CrimsonItems;

    [Header(" Tile ")]
    [SerializeField] private TileBase DarkItem;
    //[SerializeField] private TileBase CrimsonItem;

    [Header(" UI ")]
    [SerializeField] private GameObject CooldownTimePrefab;
    [SerializeField] private Transform BarParent;
    public Slider SliderGr;
    private List<GameObject> cooldownList;

    public GameObject PowerRingPrefab;
    private List<GameObject> PowerRingList;
    private List<List<GameObject>> IndexPowerRingList;

    public override void Setup(List<float> healths, float cooldownTimeSkill, int totalItems, 
                                Vector2Int startPos, Vector2Int endPos)
    {
        base.Setup(healths, cooldownTimeSkill, totalItems, startPos, endPos);
        this.BossType = BossType.HappyBoss;
        this.CurPhase = 1;
        this.CurHealth = this.Healths[CurPhase - 1];
        this.cooldownList = new List<GameObject>();
        this.DarkItems = new List<Vector2Int>();
        //this.CrimsonItems = new List<Vector2Int>();
        UIManager.Instance.GetUI<GameplayUI>().SetupBoss("Happy Boss", this.TotalItems);
        this.UpdateHealthUI(this.CurHealth, this.CurHealth);

        this.PowerRingList = new List<GameObject>();    
        this.IndexPowerRingList = new List<List<GameObject>>();
    }

    public override void ActiveSkill()
    {

        if (this.BossState == BossState.Dead)
        {
            return;
        }

        this.SetupSkillPhase1();
        //this.IsActingSkill = true;
        //Invoke(nameof(this.SetupSkillPhase1), 0.3f);
        //Invoke(nameof(ResetIsActingSkill), 0.35f);
    }

    public void SetupSkillPhase1()
    {
        List<Vector2Int> tmpList = new List<Vector2Int>();
        tmpList = ItemTileController.Instance.FindItemAbsMin();
        this.ItemList = new List<Vector2Int>();
        this.ItemList.Add(tmpList[0]);
        tmpList.RemoveAt(0);

        this.cooldownList = new List<GameObject>();
        SetupAllCooldownSkill(this.ItemList, this.cooldownList);
    }

    public void CombineAndTransformItems(List<Vector2Int> itemList, List<GameObject> goList)
    {
        int index = -1;
        foreach (Vector2Int posItem in itemList)
        {
            index++;
            this.DarkItems.Add(posItem);
            this.InitPowerRingForBomb(posItem);
            SlideController.Instance.bossTilemap.SetTile(new Vector3Int(posItem.x, posItem.y, 0), DarkItem);
            SlideController.Instance.obstacleTilemap.SetTile(new Vector3Int(posItem.x, posItem.y, 0), null);
            this.DecreaseItems(1);
            this.AttackingPlayer(0f);
        }
        BossController.Instance.SpawnItems();
    }

    private void SetupAllCooldownSkill(List<Vector2Int> itemList, List<GameObject> goList)
    {
        int i = 0;
        foreach (Vector2Int posItem in this.ItemList)
        {

            i++;
            TileBase tile = SlideController.Instance.itemTilemap.GetTile(new Vector3Int(posItem.x, posItem.y, 0));
            
            SlideController.Instance.obstacleTilemap.SetTile(new Vector3Int(posItem.x, posItem.y, 0), tile);
            ItemTileController.Instance.RemoveItem(posItem);
            Vector3Int worldPos = new Vector3Int(posItem.x, posItem.y, 0);
            this.SetupCooldownPrefab(worldPos, this.ItemList, this.cooldownList);
        }
    }

    private void SetupCooldownPrefab(Vector3Int worldPos, List<Vector2Int> itemList, List<GameObject> goList)
    {
        Vector3 posForCooldownTime = SlideController.Instance.groundTilemap.CellToWorld(worldPos) + SlideController.Instance.groundTilemap.cellSize / 2;
        posForCooldownTime.y -= 0.35f;
        GameObject sl = Instantiate(CooldownTimePrefab, posForCooldownTime, Quaternion.identity, this.BarParent);
        sl.GetComponent<SliderCooldown>().Setup(0.5f, 1f, true);
        sl.GetComponent<SliderCooldown>().SetupActiveSkill(itemList, goList, BossType.HappyBoss);
        this.cooldownList.Add(sl);
    }

    public override void ActiveSkillAfterCooldown(List<Vector2Int> itemList, List<GameObject> goList)
    {
        this.IsActingSkill = true;
        StartCoroutine(IE(itemList, goList));
        Invoke(nameof(ResetIsActingSkill), 0.35f);
    }

    private IEnumerator IE(List<Vector2Int> itemList, List<GameObject> goList)
    {
        yield return new WaitForSeconds(0.25f);
        this.CombineAndTransformItems(itemList, goList);
    }


    public override void AttackingPlayer(float time = 0.3f)
    {
        if (this.BossState == BossState.Active)
        {
            Invoke(nameof(RemoveBossTilemap), time);
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

            //if (!SlideController.Instance.bossTilemap.HasTile(new Vector3Int(playerPos.x, playerPos.y, 0)))
            //{
            //    continue;
            //}

            if (this.DarkItems.Contains(targetPos))
            {
                isAttacking = true;
                this.DecreaseItems(1);

                int id = this.DarkItems.IndexOf(targetPos);

                this.DarkItems.Remove(targetPos);
                this.RemovePowerRingForBomb(id);

                SlideController.Instance.obstacleTilemap.SetTile(new Vector3Int(targetPos.x, targetPos.y, 0), null);
                SlideController.Instance.bossTilemap.SetTile(new Vector3Int(targetPos.x, targetPos.y, 0), null);

                Vector3 worldPlayerPos = SlideController.Instance.obstacleTilemap.GetCellCenterWorld(new Vector3Int(playerPos.x, playerPos.y, 0));
                Vector3 worldTargetPos = SlideController.Instance.obstacleTilemap.GetCellCenterWorld(new Vector3Int(targetPos.x, targetPos.y, 0));

                Vector3 spawnPos = new Vector3();
                spawnPos.x = Mathf.Abs(worldPlayerPos.x + worldTargetPos.x) / 2;
                spawnPos.y = Mathf.Abs(worldPlayerPos.y + worldTargetPos.y) / 2;

                GroundTileController.Instance.ActiveBurnDownEffect(spawnPos);
            }
        }

        if (isAttacking)
        {
            SlideController.Instance.PlayerTakeDamage();
            SoundsManager.Instance.PlaySFX(SoundType.ExBomb);
        }
    }

    public override void NextPhase()
    {
        this.CurPhase++;
        base.NextPhase();
    }

    private void InitPowerRingForBomb(Vector2Int pos)
    {
        Debug.Log("Init Power Ring");
        List<GameObject> goList = new List<GameObject>();
        List<Vector2Int> offsets = new List<Vector2Int>();
        offsets = Library.Instance.LibOffsets8;

        for (int i = 0; i < offsets.Count; ++i)
        {
            Vector2Int targetPos = pos + offsets[i];
            Vector3Int gridPos = new Vector3Int(targetPos.x, targetPos.y, 0);

            if (targetPos == SlideController.Instance.GetPlayerPos())
            {
                SlideController.Instance.obstacleTilemap.SetTile(gridPos, null);
            }

            if (BossController.Instance.CheckExistsBoss(targetPos) ||
                SlideController.Instance.bgSmallTilemap.HasTile(gridPos) == false
                )
            {
                continue;
            }

            GameObject go = Instantiate(
                     this.PowerRingPrefab,
                     SlideController.Instance.bgSmallTilemap.GetCellCenterWorld(new Vector3Int(targetPos.x, targetPos.y, 0)),
                     Quaternion.identity,
                     this.transform
                     );
            this.PowerRingList.Add(go);
            goList.Add(go);
        }

        this.IndexPowerRingList.Add(goList);
    }

    private void RemovePowerRingForBomb(int index)
    {
        List<GameObject> idList = this.IndexPowerRingList[index];
        List<GameObject> go = new List<GameObject>();


        foreach (GameObject i in idList)
        {
            Destroy(i.gameObject);
        }

        this.IndexPowerRingList.RemoveAt(index);
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
            this.Die();
            SlideController.Instance.LoadNextLevelAfterBoss();
        }
    }

}
