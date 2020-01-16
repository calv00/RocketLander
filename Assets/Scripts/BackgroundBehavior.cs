using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundBehavior : MonoBehaviour
{
    [SerializeField]
    private Sprite[] m_AnimationFrames;
    [SerializeField]
    private float m_FrameInterval = 0.1f;

    private Image m_Image;
    private float m_FrameIntervalAcc = 0f;
    private int m_FrameIndex = 0;
    private int m_FrameIndexAcc = 0;
    private int m_FrameCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_Image = gameObject.GetComponent<Image>();
        m_FrameCount = m_AnimationFrames.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_FrameIntervalAcc > m_FrameInterval)
        {
            m_FrameIndexAcc = (m_FrameIndexAcc + 1) % m_FrameCount;
            m_Image.sprite = m_AnimationFrames[m_FrameIndexAcc];
            m_FrameIntervalAcc = 0f;
        }
        else
        {
            m_FrameIntervalAcc = m_FrameIntervalAcc + Time.deltaTime;
        }
    }
}
