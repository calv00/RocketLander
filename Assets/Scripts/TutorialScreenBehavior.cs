using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScreenBehavior : MonoBehaviour
{
    [SerializeField]
    private Sprite[] m_TutorialSprites;

    private Image m_TutorialImage;
    private Button m_PrevScreenButton;
    private Button m_NextScreenButton;
    private int m_ScreenIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        InitTutorial();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitTutorial()
    {
        m_TutorialImage = gameObject.transform.GetChild(0).GetComponent<Image>();
        m_PrevScreenButton = gameObject.transform.GetChild(2).GetComponent<Button>();
        m_NextScreenButton = gameObject.transform.GetChild(3).GetComponent<Button>();
    }

    private void UpdateScreenButtons()
    {
        if (m_ScreenIndex <= 0)
        {
            m_PrevScreenButton.interactable = false;
        }
        else
        {
            m_PrevScreenButton.interactable = true;
        }
            
        if (m_ScreenIndex >= (m_TutorialSprites.Length - 1))
        {
            m_NextScreenButton.interactable = false;
        }
        else
        {
            m_NextScreenButton.interactable = true;
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        InitTutorial();
        m_TutorialImage.sprite = m_TutorialSprites[m_ScreenIndex];
        UpdateScreenButtons();
    }

    public void NextScreen()
    {
        m_ScreenIndex = (m_ScreenIndex + 1) % m_TutorialSprites.Length;
        m_TutorialImage.sprite = m_TutorialSprites[m_ScreenIndex];
        UpdateScreenButtons();
    }

    public void PrevScreen()
    {
        m_ScreenIndex = (m_ScreenIndex - 1) % m_TutorialSprites.Length;
        m_TutorialImage.sprite = m_TutorialSprites[m_ScreenIndex];
        UpdateScreenButtons();
    }

    public void ExitTutorial()
    {
        gameObject.SetActive(false);
    }
}
