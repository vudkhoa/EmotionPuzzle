using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderCooldown : MonoBehaviour
{
    private float m_time;
    private float m_offset;

    public void Setup(float time, float startValue, bool isNagative)
    {
        m_time = time;
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
        GetComponent<Slider>().value += m_offset * (Time.deltaTime / m_time);
    }
}
