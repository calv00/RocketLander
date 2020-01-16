using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonBehavior : MonoBehaviour
{
    [HideInInspector]
    public MainMenuController m_MenuController;

    public int m_Level = 1;
    public int m_StarsToUnlock = 0;

    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).GetComponent<Text>().text = m_Level.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayLevel()
    {
        m_MenuController.PlayLevel(m_Level);
    }
}
