using SoundManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IceElement : Element
{
    public TileBase IceTile;
    public List<Vector3Int> IceActiveTileList;

    public override void Setup(EmotionType emotionType, Vector2Int currentPos)
    {
        base.Setup(emotionType, currentPos);
        InitOffsetList();
        this.IceActiveTileList = new List<Vector3Int>();
        this.ElementType = ElementType.Ice;
        if (this.EmotionType == EmotionType.Angry)
        {
            this.SetActivatePower();
        }
    }

    public override void Reload(ElementData elementData)
    {
        Debug.Log("Reload Ice");
        foreach (Vector3Int pos in this.IceActiveTileList)
        {
            Debug.Log(pos);
        }
        ReLoadIceActive();
        base.Reload(elementData);
    }

    public void ReLoadIceActive()
    {
        if (this.IceActiveTileList == null || this.IceActiveTileList.Count <= 0)
        {
            return;
        }

        List<Vector3Int> tempList = new List<Vector3Int>();

        while (this.IceActiveTileList.Count > 0)
        {
            if (SlideController.Instance.IsInSave(new Vector2Int(this.IceActiveTileList[0].x, this.IceActiveTileList[0].y)))
            {
                Debug.Log(this.IceActiveTileList[0]);
                Vector3Int newFillPos = this.IceActiveTileList[0];
                this.IceActiveTileList.RemoveAt(0);
                SlideController.Instance.powerTilemap.SetTile(newFillPos, null);

            }
            else
            {
                tempList.Add(this.IceActiveTileList[0]);
                this.IceActiveTileList.RemoveAt(0);
            }
        }

        this.IceActiveTileList = tempList;
    }

    public override void InitOffsetList()
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
                Debug.Log("Ice Power at: " + nearPos3);
                isPower = true;
                if (this.IceActiveTileList == null)
                {      
                    Debug.Log("Init IceActiveTileList");
                    this.IceActiveTileList = new List<Vector3Int>();
                }
                if (!this.IceActiveTileList.Contains(nearPos3) || this.IceActiveTileList.Count <= 0)
                {
                    Debug.Log("Add IceActiveTileList: " + nearPos3);
                    this.IceActiveTileList.Add(nearPos3);
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
