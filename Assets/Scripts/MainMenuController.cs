using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    [SerializeField]
    private GameObject m_MainMenuPanel;
    [SerializeField]
    private GameObject m_LevelsPanel;
    [SerializeField]
    private Text m_CoinsText;
    [SerializeField]
    private AudioMixer m_GeneralMixer;
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

    private LevelButtonBehavior[] m_LevelButtons;

    // Start is called before the first frame update
    void Start()
    {
        if (m_MainMenuPanel == null)
        {
            Debug.LogError("Missing m_MainMenuPanel on " + gameObject.name);
        }
        if (m_LevelsPanel == null)
        {
            Debug.LogError("Missing m_LevelsPanel on " + gameObject.name);
        }
        if (m_CoinsText == null)
        {
            Debug.LogError("Missing m_CoinsText on " + gameObject.name);
        }
        if (m_GeneralMixer == null)
        {
            Debug.LogError("Missing m_GeneralMixer on " + gameObject.name);
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

        LoadGame();
    }

    // Update is called once per frame
    void Update()
    {
        /* 
         * Debug purposes
         * */
#if UNITY_EDITOR
        if (Input.GetKeyUp("space"))
        {
            ResetSavedData();
            LoadGame();
        }
#endif
    }

    private void ResetSavedData()
    {
        LevelContainer NewLevelCollection = new LevelContainer();
        for (int i = 0; i < GameInfo.NumTotalLevels; i++)
        {
            NewLevelCollection.Levels.Add(new Level((i + 1), m_LevelButtons[i].m_StarsToUnlock));
        }
        NewLevelCollection.Save(Path.Combine(Application.persistentDataPath, "levels.xml"));
    }

    private void LoadGame()
    {
        LoadButtons();
        LoadAudio();
        try
        {
            LevelContainer LevelCollectionLoad = LevelContainer.Load(Path.Combine(Application.persistentDataPath, "levels.xml"));
            UnlockLevels(LevelCollectionLoad);
        }
        catch (Exception e)
        {
            ResetSavedData();
            LoadGame();
        }
    }

    private void LoadButtons()
    {
        Transform LevelButtons = GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(2).transform.GetChild(1);
        GameInfo.NumTotalLevels = LevelButtons.childCount;
        m_LevelButtons = new LevelButtonBehavior[GameInfo.NumTotalLevels];
        for (int i = 0; i < GameInfo.NumTotalLevels; i++)
        {
            m_LevelButtons[i] = LevelButtons.GetChild(i).GetComponent<LevelButtonBehavior>();
            m_LevelButtons[i].m_MenuController = this;
        }
    }

    private void LoadAudio()
    {
        if (PlayerPrefs.GetInt("MusicVolume", 1) == 1)
        {
            m_GeneralMixer.SetFloat("MusicVolume", 0f);
            m_MusicButton.transform.GetChild(0).GetComponent<Image>().sprite = m_MusicOnImage;
        }
        else
        {
            m_GeneralMixer.SetFloat("MusicVolume", -80f);
            m_MusicButton.transform.GetChild(0).GetComponent<Image>().sprite = m_MusicOffImage;
        }
        if (PlayerPrefs.GetInt("EffectsVolume", 1) == 1)
        {
            m_GeneralMixer.SetFloat("EffectsVolume", 0f);
            m_EffectsButton.transform.GetChild(0).GetComponent<Image>().sprite = m_EffectsOnImage;
        }
        else
        {
            m_GeneralMixer.SetFloat("EffectsVolume", -80f);
            m_EffectsButton.transform.GetChild(0).GetComponent<Image>().sprite = m_EffectsOffImage;
        }
    }

    private void UnlockLevels(LevelContainer levelCollection)
    {
        int CollectedCoins = levelCollection.GetTotalStars();
        for (int i = 0; i < m_LevelButtons.Length; i++)
        {
            if (m_LevelButtons[i].m_StarsToUnlock > CollectedCoins)
            {
                m_LevelButtons[i].GetComponent<Button>().interactable = false;
            }
            m_LevelButtons[i].gameObject.transform.GetChild(1).GetComponent<Text>().text = m_LevelButtons[i].m_StarsToUnlock.ToString();
            m_LevelButtons[i].gameObject.transform.GetChild(2).GetComponent<Text>().text = levelCollection.GetLevelStars(i).ToString() + "/3";
        }

        m_CoinsText.text = CollectedCoins.ToString();
    }

    public void Play()
    {
        LevelContainer LevelCollectionLoad = LevelContainer.Load(Path.Combine(Application.persistentDataPath, "levels.xml"));
        int i = 0;
        while (i < LevelCollectionLoad.Levels.Count)
        {
            if (!LevelCollectionLoad.Levels[i].CompletedStatus)
            {
                break;
            }
            i++;
        }
        if (i == 0)
        {
            SceneManager.LoadScene(i+1);
        }
        else if (i == LevelCollectionLoad.Levels.Count)
        {
            SceneManager.LoadScene(i);
        }
        else if (LevelCollectionLoad.Levels[i].StarsToUnlock > LevelCollectionLoad.GetTotalStars())
        {
            SceneManager.LoadScene(i);
        }
        else
        {
            SceneManager.LoadScene((i + 1));
        }
    }

    public void Levels()
    {
        m_MainMenuPanel.SetActive(false);
        m_LevelsPanel.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void PlayLevel(int level = 1)
    {
        SceneManager.LoadScene(level);
    }

    public void MainMenu()
    {
        m_MainMenuPanel.SetActive(true);
        m_LevelsPanel.SetActive(false);
    }

    public void MusicVolumePressed()
    {
        int MusicVolume = PlayerPrefs.GetInt("MusicVolume", 1);
        PlayerPrefs.SetInt("MusicVolume", (MusicVolume == 1) ? 0 : 1);
        LoadAudio();
    }

    public void EffectsVolumePressed()
    {
        int MusicVolume = PlayerPrefs.GetInt("EffectsVolume", 1);
        PlayerPrefs.SetInt("EffectsVolume", (MusicVolume == 1) ? 0 : 1);
        LoadAudio();
    }

    public void ShowTutorial()
    {
        m_TutorialScreen.Show();
    }
}
