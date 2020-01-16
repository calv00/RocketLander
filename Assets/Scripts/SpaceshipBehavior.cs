using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SpaceshipBehavior : MonoBehaviour
{

    [SerializeField]
    private float m_RotationSpeed = 80f;
    [SerializeField]
    private float m_RotationLimit = 90f;
    [SerializeField]
    private float m_RocketForce = 80f;
    [SerializeField]
    private float m_SpaceShipDestructionVelocity = -0.75f;
    [SerializeField]
    private float m_FuelQuantity = 10f;
    [SerializeField]
    private Sprite[] m_SpaceshipSprites;
    [SerializeField]
    private float m_RectFactor = 0.3f;
    [SerializeField]
    private float m_RectFactorVertical = 0.85f;
    [SerializeField]
    private AudioMixer m_RocketAudioMixer;
    [SerializeField]
    private float m_ExplosionRadius = 20f;
    [SerializeField]
    private float m_ExplosionAnimationTime = 1.5f;
    [SerializeField]
    private AudioSource m_ExplosionAudioSource;

    private LevelManagerBehavior m_LevelManager;
    private CanvasBehavior m_CanvasBehavior;
    private Rigidbody2D m_Rigidbody2D;
    private ConstantForce2D m_ConstantForce;
    private float m_FuelQuantityAcc = 0f;
    private ParticleSystem m_FireParticle;
    private ParticleSystem m_GasParticleLeft;
    private ParticleSystem m_GasParticleRight;
    private ParticleSystem m_Explosion;
    private bool m_Propelling = false;
    private bool m_PropellingLeft = false;
    private AudioSource m_AudioGasLeft;
    private bool m_PropellingRight = false;
    private AudioSource m_AudioGasRight;
    private IEnumerator m_SoundFadeEnum;
    private Vector2 m_FirstTouchPosition;
    private Vector2 m_SecondTouchPosition;
    private bool m_SecondTouch = false;

    private float m_LeftConstraint = 0.0f;
    private float m_RightConstraint = 0.0f;
    private float m_Buffer = .1f;
    private float m_ResetBuffer = 1f;
    private float m_TopConstraint = 0.0f;
    private float m_BottomConstraint = 0.0f;

    private Rect m_LeftRect;
    private Rect m_RightRect;
    private Rect m_BottomRect;

    private Vector3 m_RocketPartPos_Top;
    private Vector3 m_RocketPartPos_Middle;
    private Vector3 m_RocketPartPos_BotLeft;
    private Vector3 m_RocketPartPos_BotMiddle;
    private Vector3 m_RocketPartPos_BotRight;


    void OnGUI()
    {
        // Debug purposes
        /*
        GUI.Box(m_LeftRect, "");
        GUI.Box(m_RightRect, "");
        GUI.Box(m_BottomRect, "");
        */
    }

    // Start is called before the first frame update
    void Start()
    {
        m_LeftConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, 0 - Camera.main.transform.position.z)).x;
        m_RightConstraint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, 0 - Camera.main.transform.position.z)).x;
        m_TopConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, 0 - Camera.main.transform.position.z)).y;
        m_BottomConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, 0 - Camera.main.transform.position.z)).y;
        m_LeftRect = new Rect(0, (Screen.height * (1 - m_RectFactorVertical)), (Screen.width * m_RectFactor), (Screen.height * m_RectFactorVertical));
        m_RightRect = new Rect((Screen.width * (1 - m_RectFactor)), (Screen.height * (1 - m_RectFactorVertical)), (Screen.width * m_RectFactor), (Screen.height * m_RectFactorVertical));
        m_BottomRect = new Rect((Screen.width * m_RectFactor), (Screen.height * (1 - m_RectFactorVertical)), (Screen.width - (m_LeftRect.width * 2)), (Screen.height * m_RectFactorVertical));


        if (m_RocketAudioMixer == null)
            Debug.LogError("Missing m_RocketAudioMixer on " + gameObject.name);
        if (m_ExplosionAudioSource == null)
            Debug.LogError("Missing m_ExplosionAudioSource on " + gameObject.name);

        m_CanvasBehavior = GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasBehavior>();
        m_LevelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManagerBehavior>();
        m_Rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        m_ConstantForce = gameObject.GetComponent<ConstantForce2D>();
        m_FuelQuantityAcc = m_FuelQuantity;
        m_FireParticle = transform.GetChild(0).GetComponent<ParticleSystem>();
        ClearEmission(m_FireParticle);
        m_GasParticleLeft = transform.GetChild(1).GetComponent<ParticleSystem>();
        ClearEmission(m_GasParticleLeft);
        m_GasParticleRight = transform.GetChild(2).GetComponent<ParticleSystem>();
        ClearEmission(m_GasParticleRight);
        m_AudioGasLeft = transform.GetChild(1).GetComponent<AudioSource>();
        m_AudioGasRight = transform.GetChild(2).GetComponent<AudioSource>();
        m_RocketPartPos_Top = transform.GetChild(3).transform.position;
        m_RocketPartPos_Middle = transform.GetChild(4).transform.position;
        m_RocketPartPos_BotLeft = transform.GetChild(5).transform.position;
        m_RocketPartPos_BotMiddle = transform.GetChild(6).transform.position;
        m_RocketPartPos_BotRight = transform.GetChild(7).transform.position;
        m_Explosion = transform.GetChild(8).GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        float RotationValue = m_RotationSpeed * Time.deltaTime;
       
        /* 
         * Mobile device controls
         * */
        if (Input.touchCount > 0 && m_LevelManager.IsInputEnabled() && !m_LevelManager.LevelOnPause())
        {
            Touch FirstTouch = Input.GetTouch(0);
            m_FirstTouchPosition = new Vector2(FirstTouch.position.x, Screen.height - FirstTouch.position.y);
            TouchMovement(RotationValue, FirstTouch);
            if (Input.touchCount > 1)
            {
                Touch SecondTouch = Input.GetTouch(1);
                m_SecondTouch = true;
                m_SecondTouchPosition = new Vector2(SecondTouch.position.x, Screen.height - SecondTouch.position.y);
                TouchMovement(RotationValue, SecondTouch);
            }
            else
            {
                m_SecondTouch = false;
            }
            ClearAudios();
        }

        /* 
         * Keyboard controls
         * */
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftArrow) && m_LevelManager.IsInputEnabled())
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + RotationValue);
            StartEmission(m_GasParticleRight, 50f);
            if (m_AudioGasLeft.isPlaying)
                m_AudioGasLeft.Stop();
            if (!m_AudioGasRight.isPlaying)
                m_AudioGasRight.Play();
        }
        else if (Input.GetKey(KeyCode.RightArrow) && m_LevelManager.IsInputEnabled())
        {
            float RightAngle = (transform.eulerAngles.z > 180) ? transform.eulerAngles.z - 360 : transform.eulerAngles.z;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, RightAngle - RotationValue);
            StartEmission(m_GasParticleLeft, 50f);
            if (m_AudioGasRight.isPlaying)
                m_AudioGasRight.Stop();
            if (!m_AudioGasLeft.isPlaying)
                m_AudioGasLeft.Play();
        }
        if (m_FuelQuantityAcc <= 0f)
        {
            // NO FUEL
            m_FuelQuantityAcc = 0f;
            if (m_Propelling)
            {
                StopCoroutine(m_SoundFadeEnum);
                StartCoroutine(StartFade(m_RocketAudioMixer, "RocketFireVolume", .2f, 0f));
                m_Propelling = false;
            }
        }
        else if (Input.GetKey(KeyCode.UpArrow) && m_LevelManager.IsInputEnabled())
        {
            if (!m_LevelManager.HasLevelStarted()) m_LevelManager.StartLevel();
            m_Rigidbody2D.AddForce(transform.up * Time.deltaTime * m_RocketForce);
            m_FuelQuantityAcc -= Time.deltaTime;
            FuelFeedback();
            StartEmission(m_FireParticle, 40f);

            if (!m_Propelling){
                m_SoundFadeEnum = StartFade(m_RocketAudioMixer, "RocketFireVolume", .5f, 2f);
                StartCoroutine(m_SoundFadeEnum);
                m_Propelling = true;
            }
        }
        if (Input.GetKeyUp(KeyCode.UpArrow) && m_LevelManager.IsInputEnabled())
        {
            StopCoroutine(m_SoundFadeEnum);
            StartCoroutine(StartFade(m_RocketAudioMixer, "RocketFireVolume", .2f, 0f));
            m_Propelling = false;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow) && m_LevelManager.IsInputEnabled())
        {
            m_AudioGasRight.Stop();
        }
        if (Input.GetKeyUp(KeyCode.RightArrow) && m_LevelManager.IsInputEnabled())
        {
            m_AudioGasLeft.Stop();
        }
#endif
        VelocityFeedback();
        if (transform.position.x < m_LeftConstraint - m_Buffer)
        {
            transform.position = new Vector3(m_RightConstraint + m_Buffer, transform.position.y);
        }
        else if (transform.position.x > m_RightConstraint + m_Buffer)
        {
            transform.position = new Vector3(m_LeftConstraint - m_Buffer, transform.position.y);
        }
        if (transform.position.y < m_TopConstraint - m_ResetBuffer && m_LevelManager.IsInputEnabled() && m_LevelManager.HasLevelStarted())
        {
            Fail(transform.position, 10f);
        }
        else if (transform.position.y > m_BottomConstraint + m_ResetBuffer && m_LevelManager.IsInputEnabled() && m_LevelManager.HasLevelStarted())
        {
            Fail(transform.position, 10f);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "End" && m_LevelManager.IsInputEnabled())
        {
            if (collision.relativeVelocity.magnitude > 0f && collision.relativeVelocity.magnitude >= 1f)
            {
                Fail(collision.contacts[0].point, collision.relativeVelocity.magnitude);
            }
            else if (collision.gameObject.tag == "End")
            {
                LevelComplete();
            }
        }
    }

    public void AddAstronaut(int spriteIndex)
    {
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = m_SpaceshipSprites[spriteIndex];
    }

    public void Fail(Vector3 failPosition, float collisionMagnitude)
    {
        m_AudioGasLeft.Stop();
        m_AudioGasRight.Stop();
        StopAllCoroutines();
        m_RocketAudioMixer.ClearFloat("RocketFireVolume");
        m_RocketAudioMixer.SetFloat("MusicVolume", -80f);
        m_ExplosionAudioSource.Play();
        m_Explosion.Play();
        BoxCollider2D[] BoxComponents = GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D boxComponent in BoxComponents)
        {
            boxComponent.enabled = false;
        }
        Explode(failPosition, collisionMagnitude);
    }

    private void Explode(Vector3 explosionPosition, float collisionMagnitude)
    {
        m_LevelManager.InputEnabled(false);
        StartCoroutine(StartExploding());

        ExplosionForce(explosionPosition, collisionMagnitude, transform.GetChild(3).GetComponent<Rigidbody2D>());
        ExplosionForce(explosionPosition, collisionMagnitude, transform.GetChild(4).GetComponent<Rigidbody2D>());
        ExplosionForce(explosionPosition, collisionMagnitude, transform.GetChild(5).GetComponent<Rigidbody2D>());
        ExplosionForce(explosionPosition, collisionMagnitude, transform.GetChild(6).GetComponent<Rigidbody2D>());
        ExplosionForce(explosionPosition, collisionMagnitude, transform.GetChild(7).GetComponent<Rigidbody2D>());
    }

    private void ExplosionForce(Vector3 explosionPoint, float collisionMagnitude, Rigidbody2D bodyPart)
    {
        // https://forum.unity.com/threads/need-rigidbody2d-addexplosionforce.212173/#post-1426983
        Vector3 ForceDirection = (bodyPart.transform.position - explosionPoint);
        float WearOff = 1 - (ForceDirection.magnitude / m_ExplosionRadius);
        bodyPart.bodyType = RigidbodyType2D.Dynamic;
        bodyPart.AddForce(ForceDirection.normalized * 25f * WearOff * collisionMagnitude);
    }

    private void TouchMovement(float rotationValue, Touch touch)
    {
        Vector2 TouchPosition = new Vector2(touch.position.x, Screen.height - touch.position.y);
        if (m_LeftRect.Contains(TouchPosition))
        {
            if (touch.phase == TouchPhase.Ended)
            {
                m_AudioGasRight.Stop();
            }
            else
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + rotationValue);
                StartEmission(m_GasParticleRight, 50f);
                if (!m_AudioGasRight.isPlaying)
                    m_AudioGasRight.Play();
            }
        }
        else if (m_RightRect.Contains(TouchPosition))
        {
            if (touch.phase == TouchPhase.Ended)
            {
                m_AudioGasLeft.Stop();
            }
            else
            {
                float RightAngle = (transform.eulerAngles.z > 180) ? transform.eulerAngles.z - 360 : transform.eulerAngles.z;
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, RightAngle - rotationValue);
                StartEmission(m_GasParticleLeft, 50f);
                if (!m_AudioGasLeft.isPlaying)
                    m_AudioGasLeft.Play();
            }
        }
        if (m_FuelQuantityAcc <= 0f)
        {
            m_FuelQuantityAcc = 0f;
            StopCoroutine(m_SoundFadeEnum);
            StartCoroutine(StartFade(m_RocketAudioMixer, "RocketFireVolume", .2f, 0f));
            m_Propelling = false;
        }
        else if (m_BottomRect.Contains(TouchPosition))
        {
            if (touch.phase == TouchPhase.Ended)
            {
                StopCoroutine(m_SoundFadeEnum);
                StartCoroutine(StartFade(m_RocketAudioMixer, "RocketFireVolume", .2f, 0f));
                m_Propelling = false;
            }
            else
            {
                if (!m_LevelManager.HasLevelStarted()) m_LevelManager.StartLevel();
                m_Rigidbody2D.AddForce(transform.up * Time.deltaTime * m_RocketForce);
                m_FuelQuantityAcc -= Time.deltaTime;
                FuelFeedback();
                StartEmission(m_FireParticle, 40f);
                if (!m_Propelling)
                {
                    m_SoundFadeEnum = StartFade(m_RocketAudioMixer, "RocketFireVolume", .5f, 2f);
                    StartCoroutine(m_SoundFadeEnum);
                    m_Propelling = true;
                }
            }
        }
    }

    private void ClearAudios()
    {
        float RocketFireVol;
        if (!m_SecondTouch)
        {
            if (!m_LeftRect.Contains(m_FirstTouchPosition))
            {
                if (m_AudioGasRight.isPlaying)
                    m_AudioGasRight.Stop();
            }
            if (!m_RightRect.Contains(m_FirstTouchPosition))
            {
                if (m_AudioGasLeft.isPlaying)
                    m_AudioGasLeft.Stop();
            }
            if (!m_BottomRect.Contains(m_FirstTouchPosition))
            {
                if (m_SoundFadeEnum != null)
                {
                    m_RocketAudioMixer.GetFloat("RocketFireVolume", out RocketFireVol);
                    if (RocketFireVol > -80f)
                    {
                        StopCoroutine(m_SoundFadeEnum);
                        m_RocketAudioMixer.SetFloat("RocketFireVolume", -80f);
                        m_Propelling = false;
                    }
                }
            }
        }
        else
        {
            if (!m_LeftRect.Contains(m_FirstTouchPosition) && !m_LeftRect.Contains(m_SecondTouchPosition))
            {
                if (m_AudioGasRight.isPlaying)
                    m_AudioGasRight.Stop();
            }
            if (!m_RightRect.Contains(m_FirstTouchPosition) && !m_RightRect.Contains(m_SecondTouchPosition))
            {
                if (m_AudioGasLeft.isPlaying)
                    m_AudioGasLeft.Stop();
            }
            if (!m_BottomRect.Contains(m_FirstTouchPosition) && !m_BottomRect.Contains(m_SecondTouchPosition))
            {
                if (m_SoundFadeEnum != null)
                {
                    m_RocketAudioMixer.GetFloat("RocketFireVolume", out RocketFireVol);
                    if (RocketFireVol > -80f)
                    {
                        StopCoroutine(m_SoundFadeEnum);
                        m_RocketAudioMixer.SetFloat("RocketFireVolume", -80f);
                        m_Propelling = false;
                    }
                }
            }
        }
    }

    private void VelocityFeedback()
    {
        m_CanvasBehavior.UpdateVelocityFeedback(m_Rigidbody2D.velocity.y);
    }

    private void FuelFeedback()
    {
        m_CanvasBehavior.UpdateFuelFeedback((m_FuelQuantityAcc / m_FuelQuantity));
    }

    private void LevelComplete()
    {
        m_AudioGasLeft.Stop();
        m_AudioGasRight.Stop();
        StopAllCoroutines();
        m_RocketAudioMixer.ClearFloat("RocketFireVolume");
        m_LevelManager.LevelCompleted();
    }

    private void ClearEmission(ParticleSystem particle)
    {
        ParticleSystem.EmissionModule emissionModule = particle.emission;
        emissionModule.enabled = false;
    }
    
    private void StartEmission(ParticleSystem particle, float emissionRate)
    {
        particle.Emit((int)(250 * Time.deltaTime));
    }

    private IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
    {
        float currentTime = 0;
        float currentVol;

        audioMixer.GetFloat(exposedParam, out currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);
        while(currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
            yield return null;
        }
        yield break;
    }

    private IEnumerator StartExploding()
    {
        yield return new WaitForSeconds(m_ExplosionAnimationTime);
        m_LevelManager.InputEnabled(true);
        m_LevelManager.LevelFailed();
    }

}
