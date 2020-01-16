using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasBehavior : MonoBehaviour
{
    [HideInInspector]
    public LevelManagerBehavior m_LevelManager;

    [SerializeField]
    private Image m_VelocityArrowUp;
    [SerializeField]
    private Image m_VelocityArrowDown;
    [SerializeField]
    private Color m_MaxColor;
    [SerializeField]
    private float m_MaxVelocityFactor = 2f;
    [SerializeField]
    private Image m_FuelFillImage;
    [SerializeField]
    private Text m_EmptyFuelText;
    [SerializeField]
    private Text m_TimerText;
    [SerializeField]
    private Image[] m_Coins;

    private Color m_VelocityArrowInitColor;

    // Start is called before the first frame update
    void Start()
    {
        if (m_FuelFillImage == null)
        {
            Debug.LogError("Missing Image on " + gameObject.name);
        }
        if (m_EmptyFuelText == null)
        {
            Debug.LogError("Missing Text on " + gameObject.name);
        }
        if (m_TimerText == null)
        {
            Debug.LogError("Missing Text on " + gameObject.name);
        }
        ClearCoins();
        m_VelocityArrowInitColor = m_VelocityArrowUp.GetComponent<Image>().color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateVelocityFeedback(float velocityValue)
    {
        Image VelocityUp = m_VelocityArrowUp.GetComponent<Image>();
        Image VelocityDown = m_VelocityArrowDown.GetComponent<Image>();
        if (velocityValue < 0f)
        {
            if (VelocityUp.enabled)
                VelocityUp.enabled = false;
            VelocityDown.enabled = true;
            VelocityDown.color = Color.Lerp(m_VelocityArrowInitColor, m_MaxColor, (Mathf.Abs(velocityValue) / m_MaxVelocityFactor));
        }
        else
        {
            if (VelocityDown.enabled)
                VelocityDown.enabled = false;
            VelocityUp.enabled = true;
            VelocityUp.color = Color.Lerp(m_VelocityArrowInitColor, m_MaxColor, (velocityValue / m_MaxVelocityFactor));
        }
    }

    public void UpdateFuelFeedback(float percentage)
    {
        if (percentage <= 0f)
        {
            m_EmptyFuelText.enabled = true;
        }
        else
        {
            m_EmptyFuelText.enabled = false;
        }
        m_FuelFillImage.fillAmount = percentage;
    }

    public void UpdateTimer(float timeLeft)
    {
        m_TimerText.text = Mathf.RoundToInt(timeLeft).ToString();
    }

    public void AddCoinFeedback()
    {
        int i = 0;
        bool AuxFlag = false;
        while (i < m_Coins.Length && !AuxFlag)
        {
            if (m_Coins[i].color.a < 1f)
            {
                Color CoinColor = m_Coins[i].color;
                CoinColor.a = 1f;
                m_Coins[i].color = CoinColor;
                AuxFlag = true;
            }
            else
            {
                i++;
            }
        }
    }

    public void ClearCoins()
    {
        foreach(Image coin in m_Coins)
        {
            if (coin.color.a == 1f)
            {
                Color CoinColor = coin.color;
                CoinColor.a *= 0.3f;
                coin.color = CoinColor;
            }
        }
    }

    public void Pause()
    {
        if (m_LevelManager.IsInputEnabled())
            m_LevelManager.PauseGame();
    }

}
