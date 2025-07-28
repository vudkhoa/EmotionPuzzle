using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;
using UnityEngine.Tilemaps;
using System;
using DG.Tweening;

public class SlideController : SingletonMono<SlideController> 
{
    [Header(" Tile Fake ")]
    public TileFake groudTileFakePrefab;
    public TileFake itemTileFakePrefab;
    public Raft RaftPrefab;

    [Header(" Player ")]
    [SerializeField] private Player playerPrefab;

    [Header(" TileMap ")]
    public Tilemap groundTilemap;
    public Tilemap itemTilemap;
    public Tilemap obstacleTilemap;
    public Tilemap elementTilemap;
    public Tilemap bgWaterTilemap;
    public Tilemap waterTilemap;

    [Header(" Id Tile ")]
    public int itemId;
    public int elementId;

    private Player _player;
    private Raft _raft;
    public bool canSlide;
    public bool isWaitMore;
    private int curLevelId;

    private void Start()
    {
        canSlide = true;
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            Slide(Direction.Left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Slide(Direction.Right);
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Slide(Direction.Up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            Slide(Direction.Down);
        }
    }

    private void Slide(Direction direction)
    {
        if (!canSlide)
        {
            return;
        }

        canSlide = false;

        //Setup, get cell list
        Vector2Int dir1 = new Vector2Int(0, 0), dir2 = new Vector2Int(0, 0);
        Vector2Int curPlayerPos = _player.GetCurrentPos();

        if (direction == Direction.Left || direction == Direction.Right)
        {
            dir1 = new Vector2Int(-1, 0);
            dir2 = new Vector2Int(1, 0);
        }
        else if (direction == Direction.Up || direction == Direction.Down)
        {
            dir1 = new Vector2Int(0, -1);
            dir2 = new Vector2Int(0, 1);
        }

        List<Vector2Int> cellMovePosList = new List<Vector2Int>();

        for (int i = 1; i <= 100; i++)
        {
            Vector2Int newPos = curPlayerPos + i * dir1;
            if (groundTilemap.HasTile(new Vector3Int(newPos.x, newPos.y, 0)))
            {
                cellMovePosList.Add(newPos);
            }
            else
            {
                break;
            }
        }

        cellMovePosList.Reverse();

        cellMovePosList.Add(curPlayerPos);

        for (int i = 1; i <= 100; i++)
        {
            Vector2Int newPos = curPlayerPos + i * dir2;
            if (groundTilemap.HasTile(new Vector3Int(newPos.x, newPos.y, 0)))
            {
                cellMovePosList.Add(newPos);
            }
            else
            {
                break;
            }
        }

        if (direction == Direction.Right || direction == Direction.Up)
        {
            cellMovePosList.Reverse();
        }

        //Move Player
        Vector2Int offset = new Vector2Int(0, 0);
        switch (direction)
        {
            case Direction.Left:
                offset = new Vector2Int(-1, 0);
                break;
            case Direction.Right:
                offset = new Vector2Int(1, 0);
                break;
            case Direction.Up:
                offset = new Vector2Int(0, 1);
                break;
            case Direction.Down:
                offset = new Vector2Int(0, -1);
                break;
        }

        Vector2Int newPlayerPos = new Vector2Int(0, 0);
        bool isTeleport = false;
        if (cellMovePosList[0] == _player.GetCurrentPos())
        {
            newPlayerPos = cellMovePosList[cellMovePosList.Count-1];
            isTeleport = true;
        }
        else
        {
            newPlayerPos = _player.GetCurrentPos() + offset;
        }
        Vector3Int newPlayerGridPos = new Vector3Int(newPlayerPos.x, newPlayerPos.y, 0);
        if (!CheckPlayerCanMove(newPlayerGridPos, cellMovePosList, direction))
        {
            _player.Shake();
            canSlide = true;
            return;
        }

        isWaitMore = false;

        Vector3 pos = groundTilemap.GetCellCenterWorld(newPlayerGridPos);
        
        if (isTeleport)
        {
            _player.Teleport(newPlayerPos, pos);
        }
        else
        {
            _player.MoveTo(newPlayerPos, pos);
        }
        
        MoveGroundTile(cellMovePosList, direction);
        MoveItemTile(cellMovePosList, direction);
        MoveElement(cellMovePosList, direction);
        ResetCanSlide();
    }

    private void ResetCanSlide()
    {
        float time = 0.28f;
        if (isWaitMore)
        {
            time += 0.16f;
        }

        Invoke(nameof(SetCanSlide), time);
    }

    private void SetCanSlide()
    {
        canSlide = true;
    }

    private bool CheckPlayerCanMove(Vector3Int cellPlayer, List<Vector2Int> cellMoveList, Direction direction = Direction.None)
    {
        if (itemId != 0)
        {
            if (CheckItemCollideWithObstacle(cellMoveList))
            {
                return false;
            }
        }

        if (elementId > 0) 
        {
            if (!ElementController.Instance.CheckCanMoveElement(cellMoveList, direction, _player))
            {
                return false;
            }

            if (ElementController.Instance.CheckExitsElement(cellPlayer))
            {
                return false;
            }
        }

        if (cellMoveList.Count <= 1 || obstacleTilemap.HasTile(cellPlayer))
        {
            return false;
        }
        return true;
    }

    private bool CheckItemCollideWithObstacle(List<Vector2Int> cellMoveList)
    {
        for (int i = 0; i < cellMoveList.Count; i++)
        {
            Vector3Int cell = new Vector3Int(cellMoveList[i].x, cellMoveList[i].y, 0);

            if (itemTilemap.HasTile(cell))
            {
                int fromIndex = cellMoveList.Count - 1;
                if (i > 0)
                    fromIndex = i - 1;

                cell = new Vector3Int(cellMoveList[fromIndex].x, cellMoveList[fromIndex].y, 0);

                if (obstacleTilemap.HasTile(cell))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void MoveGroundTile(List<Vector2Int> cellsToSlides, Direction direction)
    {
        GroundTileController.Instance.MoveGroundTile(cellsToSlides, direction);
    }

    public void MoveItemTile(List<Vector2Int> cellsToSlides, Direction direction)
    {
        ItemTileController.Instance.MoveItemTile(cellsToSlides, direction);
    }

    public void MoveElement(List<Vector2Int> cellsToSlides, Direction direction)
    {
        ElementController.Instance.MoveElement(cellsToSlides, direction);
    }

    public Sprite GetSpriteFromTile(TileBase tile)
    {
        if (tile is Tile t)
            return t.sprite;

        // Handle RuleTile or custom tile types here if needed
        return null;
    }

    public void SpawnLevel()
    {
        curLevelId = PlayerPrefs.GetInt(Constant.LEVELID, 1);
        curLevelId = 1;
        CreateGridPrefab();
        SetItemTile();
        SetElement();
        SpawnPlayer();
    }

    private void CreateGridPrefab()
    {
        GameObject gridGO = Instantiate(Resources.Load<GameObject>("Level " + curLevelId.ToString()));
        for (int i =0; i < gridGO.transform.childCount; i++)
        {
            Transform c = gridGO.transform.GetChild(i);
            switch (c.gameObject.name)
            {
                case "Ground":
                    this.groundTilemap = c.GetComponent<Tilemap>();
                    break;
                case "Item":
                    this.itemTilemap = c.GetComponent<Tilemap>();
                    break;
                case "Obstacle":
                    this.obstacleTilemap = c.GetComponent<Tilemap>();
                    break;
                case "Element":
                    this.elementTilemap = c.GetComponent<Tilemap>();
                    break;
                case "BgWater":
                    this.bgWaterTilemap = c.GetComponent<Tilemap>();
                    break;
                case "Water":
                    this.waterTilemap = c.GetComponent<Tilemap>();
                    break;
            }
        }
    }

    private void SetItemTile()
    {
        itemId = DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].ItemId;
        if (itemId != 0)
        {
            ItemTileController.Instance.SetItemPosList(DataManager.Instance.ItemData.ItemDetails[itemId - 1].ItemPos);
            ItemTileController.Instance.SetItemTypeList(DataManager.Instance.ItemData.ItemDetails[itemId - 1].ItemTypes);
        }    
    }

    private void SetElement()
    {
        elementId = DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].ElementId;
        if (elementId != 0)
        {
            ElementController.Instance.SpawnElement(DataManager.Instance.ElementData.ElementLevelDetails[elementId - 1].ElementDetails);
        }
    }

    private void SpawnPlayer()
    {
        Vector2Int pp = DataManager.Instance.LevelData.LevelDetails[curLevelId-1].PlayerPosition;
        //Vector2Int pp = new Vector2Int(-10, 0);
        Vector3Int initPlayerPos = new Vector3Int(pp.x, pp.y, 0);
        _player = Instantiate(playerPrefab, groundTilemap.CellToWorld(initPlayerPos) + groundTilemap.cellSize / 2, Quaternion.identity);
        _player.SetCurrentPos(pp);
        CameraFollower.Instance.target = _player.transform;
    }

    public void SpawnRaft(Vector2Int pos)
    {
        Vector3Int initRaftPos = new Vector3Int(pos.x, pos.y, 0);
        _raft = Instantiate(RaftPrefab, groundTilemap.CellToWorld(initRaftPos) + groundTilemap.cellSize / 2, Quaternion.identity);
        _raft.SetCurrentPos(pos);
    }

}

[Serializable]
public enum Direction
{
    None = 0,
    Left = 1,
    Right = 2,
    Up = 3,
    Down = 4,
}
