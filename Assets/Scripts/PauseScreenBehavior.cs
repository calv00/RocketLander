using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreenBehavior : MonoBehaviour
{
    [HideInInspector]
    public LevelManagerBehavior m_LevelManager;

    [SerializeField]
    private Image m_CompletedStatus;
    [SerializeField]
    private Sprite m_CompletedStatusSprite;
    [SerializeField]
    private Image m_CoinStatus;
    [SerializeField]
    private Sprite m_CoinStatusSprite;
    [SerializeField]
    private Image m_TimeStatus;
    [SerializeField]
    private Sprite m_TimeStatusSprite;
    [SerializeField]
    private Button m_MusicButton;
    [SerializeField]
    private Button m_EffectsButton;
    [SerializeField]
    private Sprite m_MusicOnImage;
    [SerializeField]
    private Sprite m_MusicOffImage;
    [SerializeField]
    private Sprite m_EffectsOnImage;
    [SerializeField]
    private Sprite m_EffectsOffImage;
    [SerializeField]
    private TutorialScreenBehavior m_TutorialScreen;

    // Start is called before the first frame update
    void Start()
    {
        if (m_CompletedStatus == null)
        {
            Debug.LogError("No m_CompletedStatus connected on " + gameObject.name);
        }
        if (m_CoinStatus == null)
        {
            Debug.LogError("No m_CoinStatus connected on " + gameObject.name);
        }
        if (m_TimeStatus == null)
        {
            Debug.LogError("No m_TimeStatus connected on " + gameObject.name);
        }
        if (m_MusicButton == null)
        {
            Debug.LogError("Missing m_MusicButton on " + gameObject.name);
        }
        if (m_EffectsButton == null)
        {
            Debug.LogError("Missing m_EffectsButton on " + gameObject.name);
        }
        if (m_TutorialScreen == null)
        {
            Debug.LogError("Missing m_TutorialScreen on " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        gameObject.SetActive(true);
        UpdateAudioButtons();
        LevelContainer LevelCollectionLoad = LevelContainer.Load(Path.Combine(Application.persistentDataPath, "levels.xml"));
        if (LevelCollectionLoad.Levels[m_LevelManager.m_LevelIndex - 1].CompletedStatus)
            m_CompletedStatus.sprite = m_CompletedStatusSprite;
        if (LevelCollectionLoad.Levels[m_LevelManager.m_LevelIndex - 1].CoinStatus)
            m_CoinStatus.sprite = m_CoinStatusSprite;
        if (LevelCollectionLoad.Levels[m_LevelManager.m_LevelIndex - 1].TimeStatus)
            m_TimeStatus.sprite = m_TimeStatusSprite;
    }

    public void ReplayLevel()
    {
        m_LevelManager.Replay();
        gameObject.SetActive(false);
    }

    public void GoToLevelSelection()
    {
        m_LevelManager.BackToMenu();
    }

    public void ShowTutorial()
    {
        m_TutorialScreen.Show();
    }

    public void ResumeGame()
    {
        m_LevelManager.ResumeGame();
        gameObject.SetActive(false);
    }

    public void MusicVolumePressed()
    {
        if (m_LevelManager.ModifyMusicVolume() == 1)
        {
            m_MusicButton.transform.GetChild(0).GetComponent<Image>().sprite = m_MusicOnImage;
        }
        else
        {
            m_MusicButton.transform.GetChild(0).GetComponent<Image>().sprite = m_MusicOffImage;
        }
    }

    public void EffectsVolumePressed()
    {
        if (m_LevelManager.ModifyEffectsVolume() == 1)
        {
            m_EffectsButton.transform.GetChild(0).GetComponent<Image>().sprite = m_EffectsOnImage;
        }
        else
        {
            m_EffectsButton.transform.GetChild(0).GetComponent<Image>().sprite = m_EffectsOffImage;
        }
    }

    private void UpdateAudioButtons()
    {
        if (PlayerPrefs.GetInt("MusicVolume", 1) == 1)
        {
            m_MusicButton.transform.GetChild(0).GetComponent<Image>().sprite = m_MusicOnImage;
        }
        else
        {
            m_MusicButton.transform.GetChild(0).GetComponent<Image>().sprite = m_MusicOffImage;
        }
        if (PlayerPrefs.GetInt("EffectsVolume", 1) == 1)
        {
            m_EffectsButton.transform.GetChild(0).GetComponent<Image>().sprite = m_EffectsOnImage;
        }
        else
        {
            m_EffectsButton.transform.GetChild(0).GetComponent<Image>().sprite = m_EffectsOffImage;
        }
    }
}
