using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;
using UnityEngine.Tilemaps;

public enum BossType
{
    None = 0,
    AngryBoss = 1,
    SadBoss = 2,
    HappyBoss = 3,
}

public class BossController : SingletonMono<BossController>
{
    [Header(" Tile ")]
    [SerializeField] private TileBase itemTile;

    public Boss Boss;
    public List<Vector2Int> PosGround;
    

    public void SpawnBoss(  List<float> healths, float cooldownTimeSkill, int totalItems,
                            Vector2Int startPos, Vector2Int endPos, Boss bossPrefab)
    {
        int bossId = SlideController.Instance.BossId;
        Vector3Int pos = new Vector3Int(startPos.x, startPos.y, 0);
        this.Boss = Instantiate(bossPrefab, pos, Quaternion.identity);
        this.Boss.Setup(
            healths,
            cooldownTimeSkill,
            totalItems,
            startPos,
            endPos
            );
        this.GetGroundPosList();
        this.SpawnItems();
    }

    private void GetGroundPosList()
    {
        this.PosGround = new List<Vector2Int>();
        for (int i = 0; i <= 100; ++i )
        {
            for (int j = 0; j <= 100; ++j)
            {
                if (SlideController.Instance.groundTilemap.HasTile(new Vector3Int(i, j, 0))
                    && !this.CheckExistsBoss(new Vector2Int(i, j))
                    && (i != 0 && j != 0)
                    )
                {
                    this.PosGround.Add(new Vector2Int(i, j));
                }
            }
        }
    }

    public bool CheckExistsBoss(Vector2Int pos)
    {
        if ((pos.x >= this.Boss.StartPos.x && pos.y >= this.Boss.StartPos.y) && 
            (pos.x <= this.Boss.EndPos.x && pos.y <= this.Boss.EndPos.y))
        {
            return true;
        }
        return false;
    }

    public void SpawnItems()
    {
        if (ItemTileController.Instance.ItemPosList.Count > 0 || this.Boss.TotalItems <= 0)
        {
            return;
        }

        if (this.Boss.BossState == BossState.Dead)
        {
            return;
        }

        if (GameManager.Instance.State == GameState.GameOver)
        {
            return;
        }

        int count = 0;
        for (int i = 0; i < this.Boss.TotalItems; ++i)
        {
            int index = Random.Range(0, this.PosGround.Count);
            Vector2Int pos = this.PosGround[index];
            if (!SlideController.Instance.itemTilemap.HasTile(new Vector3Int(pos.x, pos.y, 0)) &&
                !SlideController.Instance.bossTilemap.HasTile(new Vector3Int(pos.x, pos.y, 0)) &&
                SlideController.Instance.bgSmallTilemap.HasTile(new Vector3Int(pos.x, pos.y, 0)) &&
                SlideController.Instance.GetPlayerPos() != pos &&
                !SlideController.Instance.obstacleTilemap.HasTile(new Vector3Int(pos.x, pos.y, 0))
                )
            {
                SlideController.Instance.itemTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), itemTile);
                ItemTileController.Instance.ItemPosList.Add(pos);
                ItemTileController.Instance.ItemTypeList.Add(ItemType.None);
                count++;
            }

            if (count == 6 || count == this.Boss.TotalItems - 1)
            {
                return;
            }
        }

        this.SpawnItems();
    }

    public void TakeDamage()
    {
        foreach (Vector2Int itemPos in ItemTileController.Instance.ItemPosList)
        {
            if (CheckExistsBoss(itemPos))
            {
                ItemTileController.Instance.RemoveItem(itemPos);
                this.Boss.TakeDamage(1);
                this.Boss.DecreaseItems(1);
            }
        }
    }
}
