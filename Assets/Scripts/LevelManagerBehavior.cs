using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class LevelManagerBehavior : MonoBehaviour
{
    public int m_LevelIndex = 0;

    [SerializeField]
    private float m_LevelTime = 30f;
    [SerializeField]
    private bool m_TutorialLevel = false;
    [SerializeField]
    private GameObject m_SpaceshipPrefab;
    [SerializeField]
    private GameObject m_CoinPrefab;
    [SerializeField]
    private AudioMixer m_GeneralMixer;

    private Vector3 m_SpaceshipPosition;
    private Quaternion m_SpaceshipRotation;
    private CanvasBehavior m_Canvas;
    private EndScreenBehavior m_EndScreen;
    private PauseScreenBehavior m_PauseScreen;
    private GameObject m_FailScreen;
    private SpaceshipBehavior m_Spaceship;
    private float m_LevelTimeAcc = 0f;
    private bool m_LevelStarted = false;
    private bool m_OnPause = false;
    private int m_CoinsPicked = 0;
    private bool m_InputEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        if (m_SpaceshipPrefab == null)
        {
            Debug.LogError("No m_SpaceshipPrefab connected on " + gameObject.name);
        }
        if (m_CoinPrefab == null)
        {
            Debug.LogError("No Coin prefab connected on " + gameObject.name);
        }
        Transform SpaceshipTransform = GameObject.FindGameObjectWithTag("Player").transform;
        m_SpaceshipPosition = SpaceshipTransform.position;
        m_SpaceshipRotation = SpaceshipTransform.rotation;
        m_Canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasBehavior>();
        m_Canvas.m_LevelManager = this;
        m_EndScreen = m_Canvas.transform.GetChild(1).GetComponent<EndScreenBehavior>();
        m_EndScreen.m_LevelManager = this;
        m_PauseScreen = m_Canvas.transform.GetChild(3).GetComponent<PauseScreenBehavior>();
        m_PauseScreen.m_LevelManager = this;
        m_FailScreen = m_Canvas.transform.GetChild(2).gameObject;
        m_Spaceship = GameObject.FindGameObjectWithTag("Player").GetComponent<SpaceshipBehavior>();
        ResetLevel();
        if (m_TutorialLevel)
        {
            PauseGame();
            m_PauseScreen.ShowTutorial();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_LevelStarted)
        {
            float TimeLeft = m_LevelTimeAcc - Time.deltaTime;
            if (TimeLeft <= 0f)
            {
                LevelTimeOut();
            }
            else
            {
                m_LevelTimeAcc = TimeLeft;
                m_Canvas.UpdateTimer(m_LevelTimeAcc);
            }
        }

        /* 
         * Debug purposes
         * */
#if UNITY_EDITOR
        if (Input.GetKeyUp("space"))
        {
            LevelCompleted();
        }
#endif
    }

    public void ResetLevel()
    {
        GameObject[] Coins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (GameObject coin in Coins)
        {
            Destroy(coin);
        }
        GameObject[] CoinSpawns = GameObject.FindGameObjectsWithTag("CoinSpawn");
        foreach (GameObject coinSpawn in CoinSpawns)
        {
            GameObject CoinGO = Instantiate(m_CoinPrefab, coinSpawn.transform.position, coinSpawn.transform.rotation);
            CoinGO.GetComponent<CoinBehavior>().m_LevelManager = this;
        }
        Time.timeScale = 1f;
        m_LevelStarted = false;
        m_LevelTimeAcc = m_LevelTime;
        m_CoinsPicked = 0;
        ResetCanvas();
    }

    public void StartLevel()
    {
        m_LevelStarted = true;
    }

    public bool HasLevelStarted()
    {
        return m_LevelStarted;
    }

    public bool LevelOnPause()
    {
        return m_OnPause;
    }

    public void CoinPicked()
    {
        m_CoinsPicked++;
        m_Canvas.AddCoinFeedback();
        m_Spaceship.AddAstronaut(m_CoinsPicked);
    }

    public void LevelCompleted()
    {
        m_OnPause = true;
        Time.timeScale = 0f;
        bool CoinsStatus = false;
        bool TimeStatus = false;
        if (m_CoinsPicked == 3)
        {
            CoinsStatus = true;
        }
        if (m_LevelTimeAcc > 0f)
        {
            TimeStatus = true;
        }

        m_EndScreen.Show(CoinsStatus, TimeStatus);
    }

    public void LevelFailed()
    {
        m_OnPause = true;
        m_LevelStarted = false;
        Time.timeScale = 0f;
        m_FailScreen.SetActive(true);
    }

    public void PauseGame()
    {
        m_OnPause = true;
        Time.timeScale = 0f;
        m_GeneralMixer.SetFloat("HighpassCutoff", 1500f);
        m_PauseScreen.Show();
    }

    public void ResumeGame()
    {
        m_OnPause = false;
        Time.timeScale = 1f;
        m_GeneralMixer.SetFloat("HighpassCutoff", 10f);
    }

    public void Replay()
    {
        LoadAudio();
        m_GeneralMixer.SetFloat("HighpassCutoff", 10f);
        ResetLevel();
        GameObject SpaceshipInstance = GameObject.FindGameObjectWithTag("Player");
        Destroy(SpaceshipInstance);
        GameObject SpaceshipGO = Instantiate(m_SpaceshipPrefab, m_SpaceshipPosition, m_SpaceshipRotation);
        m_Spaceship = SpaceshipGO.GetComponent<SpaceshipBehavior>();
        m_OnPause = false;
    }

    public void BackToMenu()
    {
        m_GeneralMixer.SetFloat("HighpassCutoff", 10f);
        SceneManager.LoadScene(0);
    }

    public int ModifyMusicVolume()
    {
        int MusicVolume = PlayerPrefs.GetInt("MusicVolume", 1);
        PlayerPrefs.SetInt("MusicVolume", (MusicVolume == 1) ? 0 : 1);
        LoadAudio();
        return (MusicVolume == 1) ? 0 : 1;
    }

    public int ModifyEffectsVolume()
    {
        int EffectsVolume = PlayerPrefs.GetInt("EffectsVolume", 1);
        PlayerPrefs.SetInt("EffectsVolume", (EffectsVolume == 1) ? 0 : 1);
        LoadAudio();
        return (EffectsVolume == 1) ? 0 : 1;
    }

    public void InputEnabled(bool enabled)
    {
        m_InputEnabled = enabled;
    }

    public bool IsInputEnabled()
    {
        return m_InputEnabled;
    }

    private void LevelTimeOut()
    {
        m_LevelTimeAcc = 0f;
    }

    private void ResetCanvas()
    {
        m_Canvas.ClearCoins();
        m_Canvas.UpdateTimer(m_LevelTimeAcc);
    }

    private void LoadAudio()
    {
        if (PlayerPrefs.GetInt("MusicVolume", 1) == 1)
        {
            m_GeneralMixer.SetFloat("MusicVolume", 0f);
        }
        else
        {
            m_GeneralMixer.SetFloat("MusicVolume", -80f);
        }
        if (PlayerPrefs.GetInt("EffectsVolume", 1) == 1)
        {
            m_GeneralMixer.SetFloat("EffectsVolume", 0f);
        }
        else
        {
            m_GeneralMixer.SetFloat("EffectsVolume", -80f);
        }
    }

}
