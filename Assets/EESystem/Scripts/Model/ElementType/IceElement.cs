using SoundManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IceElement : Element
{
    public TileBase IceTile;
    private List<Vector3Int> _iceActiveTileList;

    public override void Setup(EmotionType emotionType, Vector2Int currentPos)
    {
        base.Setup(emotionType, currentPos);
        InitOffsetList();
        this._iceActiveTileList = new List<Vector3Int>();
        this.ElementType = ElementType.Ice;
        if (this.EmotionType == EmotionType.Angry)
        {
            this.SetActivatePower();
        }
    }

    public override void Reload()
    {
        ReLoadIceActive();
        base.Reload();
    }

    public void ReLoadIceActive()
    {
        if (this._iceActiveTileList == null || this._iceActiveTileList.Count <= 0)
        {
            return;
        }

        List<Vector3Int> tempList = new List<Vector3Int>();

        while (this._iceActiveTileList.Count > 0)
        {
            if (SlideController.Instance.IsInSave(new Vector2Int(this._iceActiveTileList[0].x, this._iceActiveTileList[0].y)))
            {
                Vector3Int newFillPos = this._iceActiveTileList[0];
                this._iceActiveTileList.RemoveAt(0);
                SlideController.Instance.powerTilemap.SetTile(newFillPos, null);

            }
            else
            {
                tempList.Add(this._iceActiveTileList[0]);
                this._iceActiveTileList.RemoveAt(0);
            }
        }

        this._iceActiveTileList = tempList;
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

        bool isPower = false;

        for (int i = 0; i < scopePosList.Count; ++i)
        {
            count++;
            scopePosList[i] += this.CurrentPos;
            nearPos3 = new Vector3Int(scopePosList[i].x, scopePosList[i].y, 0);
            if (ItemTileController.Instance.ItemPosList.Contains(scopePosList[i]) &&
                SlideController.Instance.itemTilemap.HasTile(nearPos3))
            {
                isPower = true;
                if (!this._iceActiveTileList.Contains(nearPos3))
                {
                    this._iceActiveTileList.Add(nearPos3);
                }
                TileBase itemTileBase = SlideController.Instance.itemTilemap.GetTile(nearPos3);
                SlideController.Instance.obstacleTilemap.SetTile(nearPos3, itemTileBase);
                SlideController.Instance.itemTilemap.SetTile(nearPos3, null);
                SlideController.Instance.powerTilemap.SetTile(nearPos3, IceTile);
            }
        }

        if (isPower && isIceMove)
        {
            SoundsManager.Instance.PlaySFX(SoundType.IcePower);
            isIceMove = false;
        }
    }
    public override void ReloadElement(){ }
}
