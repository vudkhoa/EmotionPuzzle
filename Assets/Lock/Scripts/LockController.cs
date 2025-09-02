using CustomUtils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LockController : SingletonMono<LockController>
{
    [Header(" Infor ")]
    private TileBase lockTile;
    private Sprite lockSprite;
    public TileFake lockTileFakePrefab;
    public GameObject LockPrefab;

    public enum LockType
    {
        None = 0,
        Tile = 1,
        Sprite = 2,
    }

    [SerializeField] private LockType CurType;
    private LockType oldType;

    public List<Vector3Int> LockPosList { get; set; }
    public List<GameObject> LockObjectList;

    public void Setup()
    {
        int id = SlideController.Instance.curLevelId;
        this.lockTile = DataManager.Instance.LevelData.LevelDetails[id - 1].LockTile;
        this.lockSprite = DataManager.Instance.LevelData.LevelDetails[id - 1].LockSprite;
        this.LockPosList = new List<Vector3Int>();
        this.LockObjectList = new List<GameObject>();
        this.CurType = LockType.Tile;
        this.oldType = CurType;
    }

    public void ResetTonewType(LockType type)
    {
        //Debug.Log("Lock Type changed to: " + type.ToString());
        int id = -1;
        if (this.LockObjectList == null || this.LockPosList == null) return;

        foreach (Vector3Int pos in LockPosList)
        {
            id++;
            if (type == LockType.Tile)
            {
                SlideController.Instance.lockTilemap.SetTile(pos, lockTile);
                this.LockObjectList[id].SetActive(false);
            }
            else
            {
                SlideController.Instance.lockTilemap.SetTile(pos, null);
                this.LockObjectList[id].SetActive(true);
            }
        }
    }

    public void SetLock(Vector3Int gridPos)
    {
        if (this.lockTile == null || this.lockSprite == null)
        {
            this.Setup();
        }

        this.LockPosList.Add(gridPos);
        Vector3 worldPos = SlideController.Instance.groundTilemap.CellToWorld(gridPos) + SlideController.Instance.groundTilemap.cellSize / 2;
        GameObject lockObj = Instantiate(LockPrefab, worldPos, Quaternion.identity);
        lockObj.SetActive(false);

        if (CurType == LockType.Tile)
        {
            SlideController.Instance.lockTilemap.SetTile(gridPos, lockTile);
        }
        else
        {
            lockObj.gameObject.SetActive(true);
        }

        this.LockObjectList.Add(lockObj);
    }

    public void RemoveLock(Vector3Int gridPos)
    {
        int id = -1;
        id = this.LockPosList.IndexOf(gridPos);

        if (id == -1)
        {
            return;
        }

        if (CurType == LockType.Tile)
        {
            
            SlideController.Instance.lockTilemap.SetTile(gridPos, null);
        }
        else
        {
            this.LockObjectList[id].SetActive(false);
        }

        this.LockPosList.RemoveAt(id);
        Destroy(this.LockObjectList[id]);
        this.LockObjectList.RemoveAt(id);
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            if (this.oldType != this.CurType)
            {
                this.ResetTonewType(this.CurType);
                this.oldType = this.CurType;
            }
        }
    }
}
