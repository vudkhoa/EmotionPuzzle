using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class SavePointController : SingletonMono<SavePointController>
{
    //[Header(" Data ")]
    public SavePointSO SavePointData;
    public GameObject CheckPointPrefab;
    public int id;
    public Vector2Int curSavePoint = new Vector2Int(-1000, -1000);
    public Vector2Int endSavePoint;
    public Vector2Int startSavePoint;

    private List<bool> isMoved;
    private GameObject checkPointOb;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void Setup(int id)
    {
        if (id <= 0)
        {
            return;
        }
        this.id = id;
        curSavePoint = new Vector2Int(0, 0);
        isMoved = new List<bool>();
        foreach (PointDetail pointDetail in SavePointData.SavePointDetails[id - 1].Points)
        {
            Vector3Int spPos = new Vector3Int(pointDetail.SavePoint.x, pointDetail.SavePoint.y, 0);
            //SlideController.Instance.savePointTilemap.SetTile(spPos, SavePointData.SavePointDetails[id - 1].TileSavePoint);
            isMoved.Add(false);
        }
    }

    public void SetCheckPoint(Vector2Int savePoint)
    {
        if (id == 0)
        {
            return;
        }

        foreach (PointDetail pointDetail in SavePointData.SavePointDetails[id - 1].Points)
        {
            if (pointDetail.SavePoint == savePoint)
            {
                curSavePoint = pointDetail.SavePoint;
                startSavePoint = pointDetail.StartPoint;
                endSavePoint = pointDetail.EndPoint;

                int index = SavePointData.SavePointDetails[id - 1].Points.IndexOf(pointDetail);
                if (!isMoved[index])
                {
                    Vector3Int spPos = new Vector3Int(pointDetail.SavePoint.x, pointDetail.SavePoint.y, 0);
                    //SlideController.Instance.savePointTilemap.SetTile(spPos, SavePointData.SavePointDetails[id - 1].TileSavePoint);
                    if (checkPointOb != null)
                    {
                        Destroy(checkPointOb);
                    }
                    checkPointOb = Instantiate(CheckPointPrefab, SlideController.Instance.savePointTilemap.GetCellCenterWorld(spPos), Quaternion.identity);
                    checkPointOb.transform.localScale = Vector3.zero;
                    checkPointOb.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
                    isMoved[index] = true;
                }

                //Reset init data
                ElementController.Instance.ResetInitData();
                ItemTileController.Instance.ResetInitData();
                ObstacleTileController.Instance.ResetInitData();
                RotateObjectController.Instance.ResetInitData();
                BlockTileController.Instance.ResetInitData();

                return;
            }
        }
    }

    public bool IsSave()
    {
        return curSavePoint != new Vector2Int(-1000, -1000);
    }
}
