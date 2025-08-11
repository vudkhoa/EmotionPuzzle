using SoundManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterElement : Element
{
    private List<Vector3Int> _fillWaterList;
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
}
