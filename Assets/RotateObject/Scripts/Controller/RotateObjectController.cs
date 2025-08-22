using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;
using SoundManager;
using UnityEngine.Tilemaps;

public class RotateObjectController : SingletonMono<RotateObjectController>
{
    public List<RotateObject> RotateObjects;

    [Header(" Prefab ")]
    [SerializeField] private RotateObject I_Prefab;
    [SerializeField] private RotateObject L_Prefab;

    public void Init()
    {
        RotateObjects = new List<RotateObject>();

        for (int i = -100; i <= 100; i++)
        {
            for (int j = -100; j <= 100; j++)
            {
                if (SlideController.Instance.rotateTilemap.HasTile(new Vector3Int(i, j, 0)))
                {
                    string nameTile = SlideController.Instance.rotateTilemap.GetTile(new Vector3Int(i, j, 0)).name;
                    Vector3 rotation = SlideController.Instance.rotateTilemap.GetTransformMatrix(new Vector3Int(i, j, 0)).rotation.eulerAngles;
                    
                    Vector2Int rotatePos = new Vector2Int(i, j);
                    List<Vector2Int> containPosList = new List<Vector2Int>();

                    if (nameTile.StartsWith("I"))
                    {
                        RotateObject obj = Instantiate(I_Prefab, SlideController.Instance.groundTilemap.GetCellCenterWorld(new Vector3Int(i, j, 0)), Quaternion.identity);
                        obj.transform.rotation = Quaternion.Euler(0f, 0f, rotation.z);
                        if (rotation.z == 0)
                        {
                            Vector2Int temp = rotatePos + new Vector2Int(1, 0);
                            containPosList.Add(temp);
                            temp = rotatePos + new Vector2Int(-1, 0);
                            containPosList.Add(temp);
                        }
                        else
                        {
                            Vector2Int temp = rotatePos + new Vector2Int(0, 1);
                            containPosList.Add(temp);
                            temp = rotatePos + new Vector2Int(0, -1);
                            containPosList.Add(temp);
                        }
                        obj.Setup(rotatePos, containPosList);
                        this.RotateObjects.Add(obj);
                    }
                    else
                    {
                        RotateObject obj = Instantiate(L_Prefab, SlideController.Instance.groundTilemap.GetCellCenterWorld(new Vector3Int(i, j, 0)), Quaternion.identity);
                        obj.transform.rotation = Quaternion.Euler(0f, 0f, rotation.z);
                        if (rotation.z == 0)
                        {
                            Vector2Int temp = rotatePos + new Vector2Int(-1, 0);
                            containPosList.Add(temp);
                            temp = rotatePos + new Vector2Int(0, 1);
                            containPosList.Add(temp);
                        }
                        else if (rotation.z == 90)
                        {
                            Vector2Int temp = rotatePos + new Vector2Int(-1, 0);
                            containPosList.Add(temp);
                            temp = rotatePos + new Vector2Int(0, -1);
                            containPosList.Add(temp);
                        }
                        else if (rotation.z == 180)
                        {
                            Vector2Int temp = rotatePos + new Vector2Int(1, 0);
                            containPosList.Add(temp);
                            temp = rotatePos + new Vector2Int(0, -1);
                            containPosList.Add(temp);
                        }
                        else
                        {
                            Vector2Int temp = rotatePos + new Vector2Int(1, 0);
                            containPosList.Add(temp);
                            temp = rotatePos + new Vector2Int(0, 1);
                            containPosList.Add(temp);
                        }
                        obj.Setup(rotatePos, containPosList);
                        this.RotateObjects.Add(obj);
                    }

                    SlideController.Instance.rotateTilemap.SetTile(new Vector3Int(i, j, 0), null);
                }
            }
        }
    }

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
                //Sounds
                SoundsManager.Instance.PlaySFX(SoundType.Rotate);

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

            return true;
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
