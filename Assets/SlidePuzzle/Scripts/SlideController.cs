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
    [SerializeField] private TileFake groudTileFakePrefab;
    [SerializeField] private TileFake itemTileFakePrefab;
    [SerializeField] private TileFake enemyNotMoveTileFakePrefab;
    [SerializeField] private TileFake bossLongTileFakePrefab;

    [Header(" Player ")]
    [SerializeField] private Player playerPrefab;

    [Header(" TileMap ")]
    public Tilemap groundTilemap;
    public Tilemap obstacleTilemap;

    private Player _player;
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
        ResetCanSlide();
    }

    private void ResetCanSlide()
    {
        float time = 0.26f;
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
        if (cellMoveList.Count <= 1 || obstacleTilemap.HasTile(cellPlayer))
        {
            return false;
        }
        return true;
    }

    public void MoveGroundTile(List<Vector2Int> cellsToSlides, Direction direction, bool inPuzzleSort = false)
    {
        List<Vector2Int> cellsToSlide = new List<Vector2Int>(cellsToSlides);

        //Spawn các tile động theo thứ tự cells
        List<TileFake> clones = new List<TileFake>();
        List<TileBase> tileOrder = new List<TileBase>();

        foreach (Vector2Int cellPos in cellsToSlide)
        {
            Vector3Int cell = new Vector3Int(cellPos.x, cellPos.y, 0);
            TileBase tile = groundTilemap.GetTile(cell);
            if (tile == null) continue;

            // Lưu thứ tự tile để xử lý logic wrap-around
            tileOrder.Add(tile);

            // Tạo GameObject clone để tween
            TileFake obj = Instantiate(groudTileFakePrefab, groundTilemap.GetCellCenterWorld(cell), Quaternion.identity);
            Sprite sprite = GetSpriteFromTile(tile);
            if (sprite != null)
                obj.SetSprite(sprite);

            obj.gridPos = cellPos;
            clones.Add(obj);

            groundTilemap.SetTile(cell, null);
        }

        //Di chuyển các tile
        int count = cellsToSlide.Count;
        for (int i = 1; i < count; i++)
        {
            Vector2Int toGrid = cellsToSlide[i - 1];
            Vector3 worldPos = groundTilemap.GetCellCenterWorld(new Vector3Int(toGrid.x, toGrid.y, 0));

            clones[i].MoveTo(toGrid, worldPos);
        }

        // Tile cuối
        TileFake wrapTile = clones[0];
        Vector2Int wrapToGrid = cellsToSlide[count - 1];
        Vector3 wrapToPos = groundTilemap.GetCellCenterWorld((Vector3Int)cellsToSlide[count - 1]);

        // 1. Scale nhỏ dần để ẩn tile
        wrapTile.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() =>
        {
            // 2. Di chuyển đến đầu hàng
            wrapTile.transform.position = wrapToPos;
            wrapTile.gridPos = wrapToGrid;

            // 3. Scale lớn lên để hiện tile lại
            wrapTile.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
        });

        //Bước 3: Sau khi tween xong → cập nhật lại Tilemap và xóa clone
        DOVirtual.DelayedCall(0.25f, () =>
        {
            for (int i = 0; i < count; i++)
            {
                int fromIndex = (i + 1) % count;
                Vector2Int toCell = cellsToSlide[i];
                groundTilemap.SetTile(new Vector3Int(toCell.x, toCell.y, 0), tileOrder[fromIndex]);
            }

            foreach (var obj in clones)
                Destroy(obj.gameObject);
        });
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
        CreateGridPrefab();
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
                case "Obstacle":
                    this.obstacleTilemap = c.GetComponent<Tilemap>();
                    break;
            }
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
