using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenBehavior : MonoBehaviour
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
    private Button m_NextLevelButton;

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
        if (m_NextLevelButton == null)
        {
            Debug.LogError("No m_NextLevelButton connected on " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show(bool CoinsCompleted, bool TimeCompleted)
    {
        gameObject.SetActive(true);
        LevelContainer LevelCollectionLoad = LevelContainer.Load(Path.Combine(Application.persistentDataPath, "levels.xml"));
        m_CompletedStatus.sprite = m_CompletedStatusSprite;
        LevelCollectionLoad.Levels[m_LevelManager.m_LevelIndex - 1].CompletedStatus = true;
        if (CoinsCompleted || LevelCollectionLoad.Levels[m_LevelManager.m_LevelIndex - 1].CoinStatus)
        {
            m_CoinStatus.sprite = m_CoinStatusSprite;
            LevelCollectionLoad.Levels[m_LevelManager.m_LevelIndex - 1].CoinStatus = true;
        }
        if (TimeCompleted || LevelCollectionLoad.Levels[m_LevelManager.m_LevelIndex - 1].TimeStatus)
        {
            m_TimeStatus.sprite = m_TimeStatusSprite;
            LevelCollectionLoad.Levels[m_LevelManager.m_LevelIndex - 1].TimeStatus = true;
        }
        LevelCollectionLoad.Save(Path.Combine(Application.persistentDataPath, "levels.xml"));
        if (m_LevelManager.m_LevelIndex >= LevelCollectionLoad.Levels.Count || LevelCollectionLoad.Levels[m_LevelManager.m_LevelIndex].StarsToUnlock > LevelCollectionLoad.GetTotalStars())
            m_NextLevelButton.interactable = false;
    }

    public void ReplayLevel()
    {
        m_LevelManager.Replay();
        gameObject.SetActive(false);
    }

    public void GoToLevelSelection()
    {
        SceneManager.LoadScene(0);
    }

    public void GoToNextLevel()
    {
        int NextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (NextLevelIndex <= GameInfo.NumTotalLevels)
        {
            SceneManager.LoadScene(NextLevelIndex);
        }
    }
}
