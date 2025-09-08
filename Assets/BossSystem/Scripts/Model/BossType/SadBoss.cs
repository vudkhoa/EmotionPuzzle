using DG.Tweening;
using SoundManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SadBoss : Boss
{
    [Header(" SkilSprite ")]
    [SerializeField] private GameObject SkillPrefab;
    private List<GameObject> cooldownList;
    [SerializeField] private GameObject CooldownTimePrefab;
    [SerializeField] private Transform BarParent;

    public override void Setup(List<float> healths, float cooldownTimeSkill, int totalItems, 
                                Vector2Int startPos, Vector2Int endPos)
    {
        base.Setup(healths, cooldownTimeSkill, totalItems, startPos, endPos);
        this.BossType = BossType.SadBoss;
        this.CurPhase = 1;
        this.CurHealth = this.Healths[CurPhase - 1];
        UIManager.Instance.GetUI<GameplayUI>().SetupBoss("Sad Boss", this.TotalItems);
        this.UpdateHealthUI(this.CurHealth, this.CurHealth);
    }

    public override void ActiveSkill()
    {
        if (this.BossState == BossState.Dead)
        {
            return;
        }

        this.IsActingSkill = true;
        //if (CurPhase == 1)
        //{
        //    Invoke(nameof(ActiveSkillPhase1), 0.25f);
        //}
        //else
        //{
        //    Invoke(nameof(ActiveSkillPhase2), 0.25f);
        //}
        Invoke(nameof(SetupSkill), 0.25f);
        // Cho Player bị khựng lại 1 nhịp cảm giác tốt hơn
        Invoke(nameof(ResetIsActingSkill), 0.35f);
    }

    //public void ActiveSkillPhase1()
    //{
    //    this.ItemList = ItemTileController.Instance.FindItemCluster();
    //    this.RemoveItems();
    //}

    public void SetupSkill()
    {
        List<Vector2Int> tmpList = new List<Vector2Int>();
        tmpList = ItemTileController.Instance.FindItemAbsMin();
        this.ItemList = new List<Vector2Int>();
        this.ItemList.Add(tmpList[0]);
        tmpList.RemoveAt(0);
        this.cooldownList = new List<GameObject>();
        SetupAllCooldownSkill();
        StartCoroutine(ActiveSkillPhase1(this.ItemList, this.cooldownList));
    }

    public IEnumerator ActiveSkillPhase1(List<Vector2Int> itemList, List<GameObject> goList)
    {
        yield return new WaitForSeconds(0.5f);
        this.RemoveItems(itemList, goList);
    }

    private void SetupAllCooldownSkill()
    {
        foreach (Vector2Int posItem in this.ItemList)
        {
            TileBase tile = SlideController.Instance.itemTilemap.GetTile(new Vector3Int(posItem.x, posItem.y, 0));
            ItemTileController.Instance.RemoveItem(posItem);
            SlideController.Instance.obstacleTilemap.SetTile(new Vector3Int(posItem.x, posItem.y, 0), tile);
            Vector3Int worldPos = new Vector3Int(posItem.x, posItem.y, 0);
            this.SetupCooldownPrefab(worldPos);
        }
    }

    private void SetupCooldownPrefab(Vector3Int worldPos)
    {
        Vector3 posForCooldownTime = SlideController.Instance.groundTilemap.CellToWorld(worldPos) + SlideController.Instance.groundTilemap.cellSize / 2;
        posForCooldownTime.y -= 0.35f;
        GameObject sl = Instantiate(CooldownTimePrefab, posForCooldownTime, Quaternion.identity, this.BarParent);
        sl.GetComponent<SliderCooldown>().Setup(0.5f, 1f, true);
        this.cooldownList.Add(sl);
    }

    public void RemoveItems(List<Vector2Int> itemList, List<GameObject> goList)
    {
        int index = -1;
        foreach (Vector2Int posItem in itemList)
        {
            index++;
            SlideController.Instance.obstacleTilemap.SetTile(new Vector3Int(posItem.x, posItem.y, 0), null);
            Destroy(goList[index].gameObject);

            Sprite sp = this.SkillPrefab.GetComponent<SpriteRenderer>().sprite;
            Vector3Int pos = new Vector3Int(posItem.x, posItem.y, 0);
            TileFake ob = Instantiate(SlideController.Instance.itemTileFakePrefab,
                            SlideController.Instance.itemTilemap.GetCellCenterWorld(pos), Quaternion.identity);
            ob.SetSprite(sp);
            ob.transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InBack).OnComplete(() =>
            {
                Destroy(ob.gameObject);
            });
            
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
            this.Die();
            SlideController.Instance.LoadNextLevelAfterBoss();
        }
    }

    public override void NextPhase()
    {
        this.CurPhase++;
        base.NextPhase();
    }
}
