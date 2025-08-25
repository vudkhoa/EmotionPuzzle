using SoundManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterElement : Element
{
    private List<Vector3Int> _fillWaterList;
    private List<Vector3Int> _saveFillWaterList;
    [SerializeField] TileBase WaterTile;

    public override void Setup(EmotionType emotionType, Vector2Int currentPos)
    {
        base.Setup(emotionType, currentPos);
        InitOffsetList();
        this.ElementType = ElementType.Water;
        if (this.EmotionType == EmotionType.Angry)
        {
            this.SetActivatePower();
        }
        this._saveFillWaterList = new List<Vector3Int>();
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
                if ((i == 0 && j == 0) || (i != 0 && j != 0))
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

        Vector3Int nearPos3 = new Vector3Int(0, 0, 0);
        Vector2Int nearPos2 = new Vector2Int(0, 0);
        int count = -1;

        bool isPower = false;

        foreach (Vector2Int offset in this.OffsetList)
        {
            count++;
            nearPos2 = offset + this.CurrentPos;
            nearPos3 = new Vector3Int(nearPos2.x, nearPos2.y, 0);

            if (SlideController.Instance.bgWaterTilemap.HasTile(nearPos3) &&
            !SlideController.Instance.waterTilemap.HasTile(nearPos3) &&
            this.ActivePowerList[count] == true)
            {
                isPower = true;

                FillWater(nearPos3);
                SlideController.Instance.SpawnRaft(nearPos2);
                this.ActivePowerList[count] = false;
            }
        }

        if (isPower)
        {
            SoundsManager.Instance.PlaySFX(SoundType.WaterPower);
        }
    }

    private void FillWater(Vector3Int nearPos3)
    {
        this._fillWaterList = new List<Vector3Int>();
        this._fillWaterList.Add(nearPos3);
        while (this._fillWaterList.Count > 0)
        {
            Vector3Int newFillPos = this._fillWaterList[0];
            this._saveFillWaterList.Add(newFillPos);
            this._fillWaterList.RemoveAt(0);
            foreach (Vector2Int offset in this.OffsetList)
            {
                if (SlideController.Instance.bgWaterTilemap.HasTile(newFillPos + new Vector3Int(offset.x, offset.y, 0)) &&
                    !SlideController.Instance.waterTilemap.HasTile(newFillPos + new Vector3Int(offset.x, offset.y, 0)))
                {
                    this._fillWaterList.Add(newFillPos + new Vector3Int(offset.x, offset.y, 0));
                    SlideController.Instance.waterTilemap.SetTile(newFillPos + new Vector3Int(offset.x, offset.y, 0), this.WaterTile);
                }
            }
        }
    }

    public bool IsInSave(Vector2Int pos)
    {
        if (SavePointController.Instance.startSavePoint.x <= pos.x
            && SavePointController.Instance.startSavePoint.y <= pos.y
            && SavePointController.Instance.endSavePoint.x >= pos.x
            && SavePointController.Instance.endSavePoint.y >= pos.y)
        {
            return true;
        }

        return false;
    }

    public override void ReloadElement()
    {
        ReFillWater();
    }

    public override void Reload()
    {
        ReFillWater();
        base.Reload();
    }

    public void ReFillWater()
    {
        if (this._saveFillWaterList == null || this._saveFillWaterList.Count <= 0)
        {
            return;
        }

        List<Vector3Int> tempList = new List<Vector3Int>();

        while (this._saveFillWaterList.Count > 0)
        {
            if (IsInSave(new Vector2Int(this._saveFillWaterList[0].x, this._saveFillWaterList[0].y))) 
            {
                Vector3Int newFillPos = this._saveFillWaterList[0];
                this._saveFillWaterList.RemoveAt(0);
                foreach (Vector2Int offset in this.OffsetList)
                {
                    if (SlideController.Instance.bgWaterTilemap.HasTile(newFillPos + new Vector3Int(offset.x, offset.y, 0)) &&
                        SlideController.Instance.waterTilemap.HasTile(newFillPos + new Vector3Int(offset.x, offset.y, 0)))
                    {
                        SlideController.Instance.waterTilemap.SetTile(newFillPos + new Vector3Int(offset.x, offset.y, 0), null);
                    }
                }
            }
            else
            {
                tempList.Add(this._saveFillWaterList[0]);
                this._saveFillWaterList.RemoveAt(0);
            }
        }

        this._saveFillWaterList = tempList;

        List<Vector2Int> tempRaftList = new List<Vector2Int>();
        foreach (Raft raft in SlideController.Instance.RaftList)
        {
            if (IsInSave(raft.GetCurrentPos()))
            {
                tempRaftList.Add(raft.GetCurrentPos());
            }
        }

        foreach (Vector2Int raftPos in tempRaftList)
        {
            SlideController.Instance.RemoveRaft(raftPos);
        }
    }
}