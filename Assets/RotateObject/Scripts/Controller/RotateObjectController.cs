using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;

public class RotateObjectController : SingletonMono<RotateObjectController>
{
    public List<RotateObject> RotateObjects;

    public void Setup(List<RotateObjectDetail> RotateObjects)
    {
        foreach (RotateObjectDetail objDetail in RotateObjects)
        {
            Vector3Int gridPos = new Vector3Int(objDetail.rotatePos.x, objDetail.rotatePos.y, 0);
            RotateObject obj = Instantiate(objDetail.RotateObjectPrefab, SlideController.Instance.groundTilemap.GetCellCenterWorld(gridPos), Quaternion.identity);
            obj.Setup(objDetail.rotatePos, objDetail.containPosList);
            obj.transform.rotation = Quaternion.Euler(0f, 0f, objDetail.angle);

            this.RotateObjects.Add(obj);
        }
    }

    public bool RotateFunction(Vector2Int playerPos)
    {
        bool isRotate = false;
        foreach (RotateObject obj in RotateObjects)
        {
            if (obj.rotatePos == playerPos)
            {
                ItemTileController.Instance.RotateItemTile(obj.rotatePos, obj.containPosList);
                ObstacleTileController.Instance.RotateObstacleTile(obj.rotatePos, obj.containPosList);
                ElementController.Instance.RotateElement(obj.rotatePos, obj.containPosList);
                
                obj.Rotate();

                isRotate = true;
                break;
            }
        }

        if (isRotate)
        {
            foreach (RotateObject obj in RotateObjects)
            {
                GroundTileController.Instance.SetGroundTileForRotateObj(obj.containPosList);
            }
        }

        return false;
    }

    public bool IsShowTutorial(Vector2Int playerPos)
    {        
        foreach (RotateObject obj in RotateObjects)
        {
            if (obj.rotatePos == playerPos)
            {
                return true;
            }
        }

        return false;
    }

    public Vector2Int RotateAroundPivot(Vector2Int point, Vector2Int pivot, float angleDegrees)
    {
        float radians = angleDegrees * Mathf.Deg2Rad;

        // Dịch điểm về gốc tọa độ
        Vector2 dir = point - pivot;

        // Xoay điểm quanh gốc
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        float rotatedX = dir.x * cos - dir.y * sin;
        float rotatedY = dir.x * sin + dir.y * cos;

        // Dịch ngược lại về vị trí ban đầu và làm tròn
        Vector2 rotatedPoint = new Vector2(rotatedX, rotatedY) + pivot;

        return new Vector2Int(Mathf.RoundToInt(rotatedPoint.x), Mathf.RoundToInt(rotatedPoint.y));
    }
}
