using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IceElement : Element
{
    public override void Setup(EmotionType emotionType, Vector2Int currentPos)
    {
        base.Setup(emotionType, currentPos);
        InitOffsetList();
        this.ElementType = ElementType.Ice;
        if (this.EmotionType == EmotionType.Angry)
        {
            this.SetActivatePower();
        }
    }

    private void InitOffsetList()
    {
        this.OffsetList = new List<Vector2Int>();
        this.ActivePowerList = new List<bool>();
        this.PowerRingList = new List<GameObject>();
        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                Vector2Int pos = new Vector2Int(i, j);
                this.OffsetList.Add(pos);
                this.ActivePowerList.Add(false);
                GameObject go = Instantiate(
                    this.PowerRingPrefab,
                    SlideController.Instance.bgSmallTilemap.GetCellCenterWorld(new Vector3Int(CurrentPos.x + pos.x, CurrentPos.y + pos.y, 0)),
                    Quaternion.identity,
                    this.transform
                    );
                go.gameObject.SetActive(false);
                this.PowerRingList.Add(go);
            }
        }
    }

    public override void Power()
    {
        if (this.EmotionType != EmotionType.Angry)
        {
            return;
        }

        if (SlideController.Instance.itemId <= 0)
        {
            return;
        }

        Vector3Int nearPos3 = new Vector3Int(0, 0, 0);
        List<Vector2Int> scopePosList = new List<Vector2Int>(this.OffsetList);
        int count = -1;
        for (int i = 0; i < scopePosList.Count; ++i)
        {
            count++;
            scopePosList[i] += this.CurrentPos;
            nearPos3 = new Vector3Int(scopePosList[i].x, scopePosList[i].y, 0);
            if (ItemTileController.Instance.ItemPosList.Contains(scopePosList[i]) &&
                SlideController.Instance.itemTilemap.HasTile(nearPos3) &&
                this.ActivePowerList[count] == true)
            {
                TileBase itemTileBase = SlideController.Instance.itemTilemap.GetTile(nearPos3);
                SlideController.Instance.obstacleTilemap.SetTile(nearPos3, itemTileBase);
                SlideController.Instance.itemTilemap.SetTile(nearPos3, null);
                this.ActivePowerList[count] = false;
            }
        }

        // Xóa các item tile không nằm trong phạm vi
        foreach (Vector2Int itemPos in ItemTileController.Instance.ItemPosList)
        {
            nearPos3 = new Vector3Int(itemPos.x, itemPos.y, 0);
            if (!scopePosList.Contains(itemPos) &&
                !SlideController.Instance.itemTilemap.HasTile(nearPos3))
            {
                TileBase obstacleTile = SlideController.Instance.obstacleTilemap.GetTile(nearPos3);
                SlideController.Instance.itemTilemap.SetTile(nearPos3, obstacleTile);
                SlideController.Instance.obstacleTilemap.SetTile(nearPos3, null);
            }
        }
    }
}
