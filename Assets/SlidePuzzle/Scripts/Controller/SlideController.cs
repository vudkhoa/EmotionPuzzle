using CustomUtils;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SlideController : SingletonMono<SlideController> 
{
    [Header(" Tile Fake ")]
    public Tile groundNoneSprite;
    public TileFake groudTileFakePrefab;
    public TileFake obstacleTileFakePrefab;
    public TileFake itemTileFakePrefab;
    public Raft RaftPrefab;

    [Header(" Player ")]
    [SerializeField] private Player playerPrefab;

    [Header(" TileMap ")]
    public Tilemap groundTilemap;
    public Tilemap itemTilemap;
    public Tilemap obstacleTilemap;
    public Tilemap blockTilemap;
    public Tilemap elementTilemap;
    public Tilemap bgWaterTilemap;
    public Tilemap waterTilemap;
    public Tilemap bgSmallTilemap;
    public Tilemap bossTilemap;
    public Tilemap iceStarTilemap;
    public Tilemap powerTilemap;

    [Header(" Id Tile ")]
    public int itemId;
    public int blockId;
    public int elementId;
    public int BossId;
    public int IceStarId;
    public int rotateObId;
    public int elementGuideId;

    private Player _player;
    public List<Raft> RaftList;
    public bool canSlide;
    public bool canF;
    public bool isWaitMore;
    public int curLevelId;
    private int tutorialId;
    public bool isTutorial;
    public bool isElementGuideUI;

    private void Start()
    {
        canSlide = true;
        canF = true;
        isTutorial = false;
        isElementGuideUI = false;
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
        else if (Input.GetKeyDown(KeyCode.F))
        {
            FunctionedObject();
        }
    }

    private void FunctionedObject()
    {
        if (!canF)
        {
            return;
        }

        canF = false;

        if (rotateObId != 0)
        {
            if (RotateObjectController.Instance.RotateFunction(_player.GetCurrentPos()))
            {
                ResetCanF();
            }
            else
            {
                canF = true;
            }
        }
        else 
        {  
            canF = true; 
        }
    }

    private void ResetCanF()
    {
        float time = 0.22f;

        Invoke(nameof(SetCanF), time);
    }

    private void SetCanF()
    {
        canF = true;
    }

    private void Slide(Direction direction)
    {
        if (!canSlide)
        {
            return;
        }

        if (isTutorial)
        {
            return;
        }

        if (isElementGuideUI)
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

        //Move water
        newPlayerPos = _player.GetCurrentPos() + offset;
        int raftIndex = RaftList.FindIndex(r => r.GetCurrentPos() == newPlayerPos);
        if (raftIndex >= 0)
        {
            Vector3Int gp = new Vector3Int(newPlayerPos.x, newPlayerPos.y, 0);
            Vector3 p = groundTilemap.GetCellCenterWorld(gp);
        }
        else if (raftIndex < 0)
        {
            raftIndex = RaftList.FindIndex(r => r.GetCurrentPos() == _player.GetCurrentPos());
            
            if (raftIndex >= 0)
            {
                if (this.groundTilemap.HasTile(new Vector3Int(newPlayerPos.x, newPlayerPos.y, 0)))
                {
                    Vector3Int gp = new Vector3Int(newPlayerPos.x, newPlayerPos.y, 0);
                    Vector3 p = groundTilemap.GetCellCenterWorld(gp);
                }

                if (this.waterTilemap.HasTile(new Vector3Int(newPlayerPos.x, newPlayerPos.y, 0)))
                {
                    Vector3Int gp = new Vector3Int(newPlayerPos.x, newPlayerPos.y, 0);
                    Vector3 p = groundTilemap.GetCellCenterWorld(gp);
                    _player.MoveTo(newPlayerPos, p);

                    Vector2Int oldRaftPos = RaftList[raftIndex].GetCurrentPos();
                    Vector3Int oldRaftWorldPos = new Vector3Int(oldRaftPos.x, oldRaftPos.y, 0);
                    this.groundTilemap.SetTile(oldRaftWorldPos, null);
                    this.RaftList[raftIndex].MoveTo(newPlayerPos, p);
                    GroundTileController.Instance.SetGroundTileForRaft(this.RaftList[raftIndex].GetCurrentPos());
                    ResetCanSlide();
                    return;
                }

                //_player.Shake();
            }
        }

        //newPlayerPos = new Vector2Int(0, 0);

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

        if (IceStarId > 0)
        {
            IceStarController.Instance.pos2Player = newPlayerPos;
        }

        if (isTeleport)
        {
            _player.Teleport(newPlayerPos, pos);
        }
        else
        {
            _player.MoveTo(newPlayerPos, pos);
        }

        if (BossId > 0)
        {
            BossController.Instance.Boss.AttackingPlayer();
        }
        
        MoveGroundTile(cellMovePosList, direction);
        MoveItemTile(cellMovePosList, direction);
        MoveElement(cellMovePosList, direction);
        Invoke(nameof(ShowTutorial), 0.25f);
        Invoke(nameof(ShowElementGuide), 0.25f);

        Invoke(nameof(UnBlockTile), 0.28f);
        if (IceStarId > 0)
        {
            Invoke(nameof(SetActiveIceStars), 0.28f);
        }
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

            if (ElementController.Instance.CheckExistsElement(cellPlayer))
            {
                return false;
            }
        }

        if (BossId > 0)
        {
            if (BossController.Instance.CheckExistsBoss(new Vector2Int(cellPlayer.x, cellPlayer.y)))
            {
                return false;
            }

            if (BossController.Instance.Boss.IsActingSkill)
            {
                return false;
            }

            if (bossTilemap.HasTile(cellPlayer))
            {
                return false;
            }
        }

        if (IceStarId > 0 && !IceStarController.Instance.CheckPlayerCanMove(cellPlayer))
        {
            return false;
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

                if (ElementController.Instance.IsBlockItem(cell))
                {
                    return true;
                }

                if (this.BossId > 0)
                {
                    if (this.bossTilemap.HasTile(cell))
                    {
                        return true;
                    }
                }

                if (IceStarId > 0)
                {
                    foreach (Vector2Int pos2Laze in IceStarController.Instance.IceStarPostList)
                    {
                        if (pos2Laze == new Vector2Int(cell.x, cell.y))
                        {
                            return true;
                        }
                    }
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

    public void ShowTutorial()
    {
        TutorialManager.Instance.ShowTutorial(GetPlayerPos());
    }

    public void ShowElementGuide()
    {
        ElementGuideManager.Instance.ShowElementGuide(GetPlayerPos());
    }

    public void UnBlockTile()
    {
        if (blockId != 0)
        {
            BlockTileController.Instance.UnBlockTile();
        }
    }

    public Sprite GetSpriteFromTile(TileBase tile)
    {
        if (tile is Tile t)
            return t.sprite;

        // Handle RuleTile or custom tile types here if needed
        return null;
    }

    public void LoadNextLevel()
    {
        if (_player.GetCurrentPos() == DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].NextLevelPos &&
            DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].NextLevelPos != new Vector2Int(0, 0))
        {
            Debug.Log("Load Next Level");
            PlayerPrefs.SetInt(Constant.LEVELID, curLevelId+1);
            PlayerPrefs.Save();
            LoadingManager.instance.LoadScene("Puzzle");
        }
    }

    public void SpawnLevel()
    {
        curLevelId = PlayerPrefs.GetInt(Constant.LEVELID, 1);
        //curLevelId = 2;
        SetTutorial();
        this.SetElementGuide();

        //Set map
        CreateGridPrefab();
        SetItemTile();
        SetBlockTile();
        SetElement();
        SpawnPlayer();
        this.SetAngryBoss();
        this.SetSadBoss();
        this.SetHappyBoss();

        // Mini-game Mechanics
        this.SetIceStar();
        this.SetRotateObject();
    }

    private void SetElementGuide()
    {
        this.elementGuideId = DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].ElementGuideId;
        if (elementGuideId > 0)
        {
            ElementGuideManager.Instance.SetUp(elementGuideId);
        }
    }

    private void SetTutorial()
    {
        tutorialId = DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].TutorialId;
        if (tutorialId != 0)
        {

            TutorialManager.Instance.SetTutorialDetail(DataManager.Instance.TutorialData.TutorialLevelDetails[tutorialId - 1].TutorialDetails);
            TutorialManager.Instance.SetPopupDetail(DataManager.Instance.TutorialData.TutorialLevelDetails[tutorialId - 1].PopupDetails);
        }
    }

    private void CreateGridPrefab()
    {
        GameObject gridGO = new GameObject();
        bool isBoss = DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].IsBoss;
        if (!isBoss)
        {
            gridGO = Instantiate(Resources.Load<GameObject>("Level " + curLevelId.ToString()));
        }
        else
        {
            gridGO = Instantiate(Resources.Load<GameObject>("Boss_Level " + curLevelId.ToString()));
        }

        for (int i = 0; i < gridGO.transform.childCount; i++)
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
                case "Block":
                    this.blockTilemap = c.GetComponent<Tilemap>();
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
                case "BgSmall":
                    this.bgSmallTilemap = c.GetComponent<Tilemap>();
                    break;
                case "Boss":
                    this.bossTilemap = c.GetComponent<Tilemap>();
                    break;
                case "IceStar":
                    this.iceStarTilemap = c.GetComponent<Tilemap>();
                    break;
                case "Power":
                    this.powerTilemap = c.GetComponent<Tilemap>();
                    break;
            }
        }
    }

    private void SetRotateObject()
    {
        rotateObId = DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].RotateObId;
        if (rotateObId != 0)
        {
            RotateObjectController.Instance.Setup(DataManager.Instance.RotateObjectData.RotateObjectLevelDetails[rotateObId - 1].RotateObjectDetails);
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

    private void SetBlockTile()
    {
        blockId = DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].BlockId;
        if (blockId != 0)
        {
            BlockTileController.Instance.Setup(DataManager.Instance.BlockData.BlockDetails[blockId - 1].Blocks);
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
        Vector3Int initPlayerPos = new Vector3Int(pp.x, pp.y, 0);
        _player = Instantiate(playerPrefab, groundTilemap.CellToWorld(initPlayerPos) + groundTilemap.cellSize / 2, Quaternion.identity);
        _player.SetCurrentPos(pp);
        CameraFollower.Instance.target = _player.transform;
    }

    public void SpawnRaft(Vector2Int pos)
    {
        if (this.RaftList == null)
        {
            this.RaftList = new List<Raft>();
        }
        Vector3Int initRaftPos = new Vector3Int(pos.x, pos.y, 0);
        Raft RaftGO = Instantiate(RaftPrefab, groundTilemap.CellToWorld(initRaftPos) + groundTilemap.cellSize / 2, Quaternion.identity);
        RaftGO.SetCurrentPos(pos);
        RaftList.Add(RaftGO);
        this.groundTilemap.SetTile(initRaftPos, groundNoneSprite);
    }

    public void SetAngryBoss()
    {
        if (this.BossId > 0) { return; }
        this.BossId = DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].AngryBossId;
        if (BossId > 0)
        {
            AngryBossDetail AngryBossDetail = DataManager.Instance.AngryBossData.BossList[BossId - 1];
            BossController.Instance.SpawnBoss(AngryBossDetail.Health, AngryBossDetail.CooldownTimeSkill,
                AngryBossDetail.TotalItems, AngryBossDetail.StartPos, AngryBossDetail.EndPos, AngryBossDetail.BossPrefab,
                AngryBossDetail.TotalPhases);
        }
    }

    public void SetSadBoss()
    {
        if (this.BossId > 0) { return; }
        this.BossId = DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].SadBossId;
        if (BossId > 0)
        {
            SadBossDetail sadBossDetail = DataManager.Instance.SadBossData.BossList[BossId - 1];
            BossController.Instance.SpawnBoss(sadBossDetail.Health, sadBossDetail.CooldownTimeSkill, 
                sadBossDetail.TotalItems, sadBossDetail.StartPos, sadBossDetail.EndPos, sadBossDetail.BossPrefab, 
                sadBossDetail.TotalPhases);
        }
    }

    public void SetHappyBoss()
    {
        if (this.BossId > 0) { return; }
        this.BossId = DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].HappyBossId;
        if (BossId > 0)
        {
            HappyBossDetail happyBossDetail = DataManager.Instance.HappyBossData.BossList[BossId - 1];
            BossController.Instance.SpawnBoss(happyBossDetail.Health, happyBossDetail.CooldownTimeSkill, 
                happyBossDetail.TotalItems, happyBossDetail.StartPos, happyBossDetail.EndPos, 
                happyBossDetail.BossPrefab, happyBossDetail.TotalPhases);
        }
    }

    // Mini-game Mechanics
    public void SetIceStar()
    {
        IceStarId = DataManager.Instance.LevelData.LevelDetails[curLevelId - 1].IceStarId;
        if (IceStarId > 0)
        {
            IceStarController.Instance.pos2Player = _player.GetCurrentPos();
            IceStarController.Instance.SetInitIceStar(IceStarId);
        }
    }

    private void SetActiveIceStars()
    {
        IceStarController.Instance.SetIceStars();
    }

    // Bonus
    public Vector2Int GetPlayerPos()
    {
        return _player.GetCurrentPos();
    }

    public void PlayerTakeDamage()
    {
        _player.TakeDamage();
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
