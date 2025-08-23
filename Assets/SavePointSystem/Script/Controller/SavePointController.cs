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

    private List<Tilemap> mapLayers = new List<Tilemap>();
    private Dictionary<Tilemap, Dictionary<Vector3Int, TileBase>> savedTiles
        = new Dictionary<Tilemap, Dictionary<Vector3Int, TileBase>>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void Setup(int savePointId)
    {
        id = savePointId;
        curSavePoint = new Vector2Int(0, 0);

        mapLayers = new List<Tilemap>()
        {
            SlideController.Instance.groundTilemap,
            SlideController.Instance.itemTilemap,
            SlideController.Instance.obstacleTilemap,
            SlideController.Instance.blockTilemap,
            SlideController.Instance.elementTilemap,
            SlideController.Instance.rotateTilemap,
            SlideController.Instance.bgWaterTilemap,
            SlideController.Instance.waterTilemap,
            SlideController.Instance.bgSmallTilemap,
            SlideController.Instance.bossTilemap,
            SlideController.Instance.iceStarTilemap,
            SlideController.Instance.powerTilemap
        };
    }

    public void SaveProgress(Vector2Int savePoint)
    {
        savedTiles.Clear();

        curSavePoint = savePoint;

        foreach (var layer in mapLayers)
        {
            var layerTiles = new Dictionary<Vector3Int, TileBase>();

            foreach (Vector3Int pos in layer.cellBounds.allPositionsWithin)
            {
                if (pos.x < savePoint.x && pos.y < savePoint.y) // before checkpoint X
                {
                    TileBase tile = layer.GetTile(pos);
                    if (tile != null)
                        layerTiles[pos] = tile;
                }
            }

            savedTiles[layer] = layerTiles;
        }
    }
}
