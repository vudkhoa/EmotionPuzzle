using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;

public class ObstacleTileController : SingletonMono<ObstacleTileController>
{
    [SerializeField] private ParticleSystem burnDownEffect;

    public void RemoveObstacleTile(Vector2Int pos)
    {
        Instantiate(burnDownEffect, SlideController.Instance.obstacleTilemap.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0)), Quaternion.identity);
        SlideController.Instance.obstacleTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), null);
    }
}
