using CustomUtils;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IceStarController : SingletonMono<IceStarController>
{
    [Header(" Information ")]
    private int _index = -1;
    public Vector2Int pos2Player;

    [Header(" Tile Prefab ")]
    private Tile _lightTile;
    private TileBase _groundTile;

    [Header(" Get Data ")]
    public List<Vector2Int> IceStarPostList;
    private IceStarSO _iceStarData;
    private List<Direction> _iceStarDirectionList;
    private List<Vector2Int> _iceStarLockPosition;

    [Header(" Temp Data ")]
    private List<Vector3Int> _iceStarLockPosTmp;
    private List<List<Vector3Int>> _listLight;
    private List<GameObject> _listCrossingLight;

    public void SetInitIceStar(int index)
    {
        if (index < 1)
        {
            this._index = -1;
            return;
        }
        this._index = index - 1;

        this._iceStarData = DataManager.Instance.IceStarData;
        this.IceStarPostList = new List<Vector2Int>();
        this._iceStarDirectionList = new List<Direction>();
        this._iceStarLockPosition = new List<Vector2Int>();

        _lightTile = ScriptableObject.CreateInstance<Tile>();
        _lightTile.sprite = this._iceStarData.LightSprite;
        _groundTile = this._iceStarData.GroundTile;
        
        foreach (var iceStar in this._iceStarData.IceStarLevelList[_index].IceStarList)
        {
            this.IceStarPostList.Add(iceStar.Position);
            this._iceStarDirectionList.Add(iceStar.Direction);
            this._iceStarLockPosition.Add(iceStar.LockPosition);
        }

        this._listLight = new List<List<Vector3Int>>();
        for (int i = 0; i < this.IceStarPostList.Count; ++i)
        {
            this._listLight.Add(new List<Vector3Int>());
        }
        this._listCrossingLight = new List<GameObject>();
        this.SetIceStars();
    }

    public void SetIceStars()
    {
        // Null Lights
        this.SetNullLights();

        // Set Lock
        this._iceStarLockPosTmp = new List<Vector3Int>();
        for (int i = 0; i < this._iceStarDirectionList.Count; ++i)
        {
            Vector2Int pos2 = this.IceStarPostList[i];
            Direction direction = this._iceStarDirectionList[i];
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
            this.SetIceStar(pos2, offset, i, false);
        }
        this.SetTileLock();

        // Set Ice Star
        for (int i = 0; i < this._iceStarDirectionList.Count; ++i)
        {
            Vector2Int pos2 = this.IceStarPostList[i];
            Direction direction = this._iceStarDirectionList[i];
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
            if (this._listLight.Count <= 0)
            {
                this._listLight.Add(new List<Vector3Int>());
            }
            else
            {
                this._listLight[i] = this.SetIceStar(pos2, offset, i, true, direction);
            }
        }
    }

    private List<Vector3Int> SetIceStar(Vector2Int pos2, Vector2Int offset, int index, bool isSetTile = true, Direction direction = Direction.None)
    {
        List<Vector3Int> listLight = new List<Vector3Int>();
        Vector3Int pos3 = new Vector3Int(pos2.x, pos2.y, 0);
        for (int i = 0; i <= 100; ++i)
        {
            pos2 += offset;
            pos3 = new Vector3Int(pos2.x, pos2.y, 0);
            if (!CheckIceStar(pos3, index) || this.IceStarPostList.Contains(pos2))
            {
                return listLight;
            }
            pos3 = new Vector3Int(pos2.x, pos2.y, 0);
            if (isSetTile)
            {
                listLight.Add(pos3);
                SlideController.Instance.iceStarTilemap.SetTile(pos3, this._lightTile);
                Vector3 scaleTile = new Vector3(2f, 1f, 1f);
                Quaternion rotationTile = Quaternion.Euler(0, 0, 0);
                Vector3 offsetTile = new Vector3(0, 0.05f, 0);
                if (direction == Direction.Right)
                {
                    rotationTile = Quaternion.Euler(0, 0, 270);
                }
                else if (direction == Direction.Left)
                {
                    rotationTile = Quaternion.Euler(0, 0, 90);
                }
                else if (direction == Direction.Down)
                {
                    rotationTile = Quaternion.Euler(0, 0, 180);
                }
                    Matrix4x4 transformMatrix = Matrix4x4.TRS(offsetTile, rotationTile, scaleTile);
                SlideController.Instance.iceStarTilemap.SetTransformMatrix(pos3, transformMatrix);
            }

        }
        return listLight;
    }

    public void SetNullLights()
    {
        // Set Null Light
        if (this._listLight.Count != 0)
        {
            for (int i = 0; i < this._iceStarDirectionList.Count; ++i)
            {
                if (this._listLight.Count != 0)
                {
                    this.SetNullLight(i);
                }
            }
        }

        if (this._listCrossingLight.Count != 0)
        {
            foreach (GameObject crossingLight in this._listCrossingLight)
            {
                Destroy(crossingLight);
            }
        }
    }

    private void SetNullLight(int index)
    {
        if (this._listLight[index] == null) {
            return;
        }

        for (int i = 0; i < this._listLight[index].Count; ++i)
        {
            SlideController.Instance.iceStarTilemap.SetTile(this._listLight[index][i], null);
        }
        this._listLight[index] = new List<Vector3Int>();
    }

    private bool CheckIceStar(Vector3Int pos3, int i)
    {
        Vector2Int pos2IceStar = new Vector2Int(pos3.x, pos3.y);
        Vector3Int pos3Lock = new Vector3Int(0, 0, 0);
        if (pos2Player == pos2IceStar)
        {
            pos3Lock = new Vector3Int(this._iceStarLockPosition[i].x, this._iceStarLockPosition[i].y, 0);
            this._iceStarLockPosTmp.Add(pos3Lock);
            return false;
        }

        if (!SlideController.Instance.bgSmallTilemap.HasTile(pos3))
        {
            return false;
        }

        if (SlideController.Instance.obstacleTilemap.HasTile(pos3))
        {
            return false;
        }

        if (SlideController.Instance.itemTilemap.HasTile(pos3) || ItemTileController.Instance.ItemPosList.Contains(new Vector2Int(pos3.x, pos3.y)))
        {
            return false;
        }

        if (SlideController.Instance.elementId > 0 && ElementController.Instance.CheckExistsAllElement(pos3))
        {
            return false;
        }

        if (SlideController.Instance.iceStarTilemap.HasTile(pos3))
        {
            Vector3Int cell = new Vector3Int(pos2IceStar.x, pos2IceStar.y, 0);
            GameObject crossingIceStar= Instantiate(this._iceStarData.CrossingLightPrefab, transform);
            crossingIceStar.transform.position = SlideController.Instance.iceStarTilemap.GetCellCenterWorld(cell);
            this._listCrossingLight.Add(crossingIceStar);
        }

        return true;
    }

    public bool CheckPlayerCanMove(Vector3Int posPlayer)
    {
        if (this._index < 0)
        {
            return true;
        }
        foreach (Vector2Int pos2 in this.IceStarPostList)
        {
            Vector3Int pos3 = new Vector3Int(pos2.x, pos2.y, 0);
            if (posPlayer == pos3)
            {
                return false;
            }
        }
        return true;
    }

    private void SetTileLock()
    {
        foreach (Vector2Int pos2 in this._iceStarLockPosition)
        {
            Vector3Int pos3 = new Vector3Int(pos2.x, pos2.y, 0);
            SlideController.Instance.groundTilemap.SetTile(pos3, this._groundTile);
        }

        foreach (Vector3Int pos3 in this._iceStarLockPosTmp)
        {
            SlideController.Instance.groundTilemap.SetTile(pos3, null);
        }
    }

    public bool CheckExistsBlock(Vector3Int pos)
    {
        foreach (Vector2Int posLock in this._iceStarLockPosition)
        {
            Vector3Int posLock3 = new Vector3Int(posLock.x, posLock.y, 0);
            if (pos == posLock3)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckExistsSource(Vector3Int pos)
    {
        foreach (Vector2Int posSource in this.IceStarPostList)
        {
            Vector3Int posSourceWorld = new Vector3Int(posSource.x, posSource.y, 0);
            if (pos == posSourceWorld)
            {
                return true;
            }
        }
        return false;
    }
}
