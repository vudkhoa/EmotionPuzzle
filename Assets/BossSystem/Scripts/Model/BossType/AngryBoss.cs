using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AngryBoss : Boss
{
    [Header(" Tile ")]
    [SerializeField] private TileBase FlagAttack;

    private Vector2Int dangerZonePos;

    public override void Setup(List<float> healths, float cooldownTimeSkill, int totalItems, 
                                Vector2Int startPos, Vector2Int endPos)
    {
        base.Setup(healths, cooldownTimeSkill, totalItems, startPos, endPos);
        this.BossType = BossType.AngryBoss;

        this.dangerZonePos = new Vector2Int(-1, -1);
        this.CurPhase = 1;
        this.CurHealth = this.Healths[CurPhase - 1];
    }

    public override void ActiveSkill()
    {
        Vector2Int posPlayer = SlideController.Instance.GetPlayerPos();
        Vector3Int worldPos = new Vector3Int(posPlayer.x, posPlayer.y, 0);
        this.dangerZonePos = posPlayer;
        SlideController.Instance.bossTilemap.SetTile(worldPos, this.FlagAttack);
        Invoke(nameof(ActingDangerZone), 2f);
    }

    private void ActingDangerZone()
    {
        if (CheckPlayerInDangerZone())
        {
            Debug.Log("Lose Game");
        }
        else
        {
            Vector3Int worldPos = new Vector3Int(this.dangerZonePos.x, this.dangerZonePos.y, 0);
            this.dangerZonePos = new Vector2Int(-1, -1);
            SlideController.Instance.bossTilemap.SetTile(worldPos, null);
            GroundTileController.Instance.RemoveGroundTile(new Vector2Int(worldPos.x, worldPos.y));
            //SlideController.Instance.groundTilemap.SetTile(worldPos, null);
            //SlideController.Instance.bgSmallTilemap.SetTile(worldPos, null);
        }
    }

    private bool CheckPlayerInDangerZone()
    {
        return SlideController.Instance.GetPlayerPos() == this.dangerZonePos;
    }

    public override void CheckDie()
    {
        if (CurPhase < this.Healths.Count)
        {
            this.NextPhase();
        }
        else
        {
            this.BossState = BossState.Dead;
            SlideController.Instance.BossId = 0;
            Destroy(this.gameObject);
        }
    }

    public override void NextPhase()
    {
        this.CurPhase++;
        base.NextPhase();
    }
}
