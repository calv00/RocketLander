using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    [HideInInspector]
    public LevelManagerBehavior m_LevelManager;

    [SerializeField]
    private float m_RotationSpeed = 30f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 CoinAngles = transform.eulerAngles;
        CoinAngles.z += m_RotationSpeed * Time.deltaTime;
        transform.eulerAngles = CoinAngles;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            m_LevelManager.CoinPicked();
            Destroy(gameObject);
        }
    }
}
