using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderCooldown : MonoBehaviour
{
    private float m_time;
    private float m_offset;
    private float m_curTime;
    //private int index;

    List<Vector2Int> itemList = new List<Vector2Int>();
    List<GameObject> goList = new List<GameObject>();
    BossType bossType = BossType.None;

    public void SetupActiveSkill(List<Vector2Int> itemList, List<GameObject> gameobjectList, BossType type)
    {
        this.itemList  = itemList;
        this.goList    = gameobjectList;
        this.bossType  = type;
    }


    public void Setup(float time, float startValue, bool isNagative)
    {
        m_time = time;
        this.m_curTime = this.m_time;
        GetComponent<Slider>().value = startValue;
        if (isNagative)
        {
            m_offset = -1f;
        }
        else
        {
            m_offset = 1f;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

        if (this.m_offset < 0)
        {
            this.m_curTime -= Time.deltaTime;
        }
        else
        {
            this.m_curTime += Time.deltaTime;
        }

        GetComponent<Slider>().value = this.m_curTime / this.m_time;
        if ((bossType == BossType.HappyBoss || bossType == BossType.SadBoss) && this.m_curTime <= 0)
        {
            BossController.Instance.Boss.ActiveSkillAfterCooldown(this.itemList, this.goList);
            Destroy(gameObject);
        }
        else if (bossType == BossType.AngryBoss && this.m_curTime <= 0)
        {
            BossController.Instance.Boss.ActiveSkillAfterCooldownTime();
            Destroy(gameObject);
        }
    }
}
