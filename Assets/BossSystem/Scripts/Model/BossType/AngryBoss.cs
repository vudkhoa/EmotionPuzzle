using UnityEngine;
using UnityEngine.Tilemaps;

public class AngryBoss : Boss
{
    [Header(" Tile ")]
    [SerializeField] private TileBase FlagAttack;

    private Vector2Int dangerZonePos;

    public override void Setup(float health, float cooldownTimeSkill, int totalItems, 
                                Vector2Int startPos, Vector2Int endPos, int totalPhases)
    {
        base.Setup(health, cooldownTimeSkill, totalItems, startPos, endPos, totalPhases);
        this.BossType = BossType.AngryBoss;

        this.dangerZonePos = new Vector2Int(-1, -1);
        this.Phase = 1;
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
            SlideController.Instance.groundTilemap.SetTile(worldPos, null);
            SlideController.Instance.bgSmallTilemap.SetTile(worldPos, null);
        }
    }

    private bool CheckPlayerInDangerZone()
    {
        return SlideController.Instance.GetPlayerPos() == this.dangerZonePos;
    }

    public override void CheckDie()
    {
        this.BossState = BossState.Dead;
        SlideController.Instance.BossId = 0;
        Destroy(this.gameObject);
    }
}
