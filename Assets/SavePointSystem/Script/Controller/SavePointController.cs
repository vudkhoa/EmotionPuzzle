using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;
using UnityEngine.Tilemaps;

public class SavePointController : SingletonMono<SavePointController>
{
    //[Header(" Data ")]
    public SavePointSO SavePointData;
    public int id;
    public Vector2Int curSavePoint = new Vector2Int(0, 0);
    public Vector2Int endSavePoint;
    public Vector2Int startSavePoint;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void Setup(int id)
    {
        this.id = id;
        curSavePoint = new Vector2Int(0, 0);
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

                return;
            }
        }
    }

    public bool IsSave()
    {
        return curSavePoint != Vector2Int.zero;
    }
}
