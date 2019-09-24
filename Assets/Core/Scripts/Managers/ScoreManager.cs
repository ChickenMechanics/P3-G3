using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    #region design vars
    public float m_ComboTimeMax = 1.0f;
    public float m_ComboBaseMultiplier = 1.5f;
    public float m_ComboScaler = 1.15f;
    #endregion

    #region get set
    [HideInInspector]
    public float m_PassedComboTime { get; private set; }
    [HideInInspector]
    public float m_PlayerScore { get; private set; }
    [HideInInspector]
    public int m_ComboCounter { get; private set; }
    #endregion

    private List<float> m_PointsCollector;
    private float m_CurrentComboMultiplier;
    private bool m_ComboAlive;


    public void AddComboPoints(float value)    //  Call this to add any points that is part of combos
    {
        ++m_ComboCounter;
        if (m_ComboCounter == 1)
        {
            m_ComboAlive = true;
        }

        m_PointsCollector.Add(value);
        ComboEvaluator();
    }


    public void AddBonusPoints(float value)   //  Call this for vanilla points
    {
        m_PointsCollector.Add(value);
    }


    private void UpdatePoints()
    {
        if (m_ComboAlive == true)
        {
            m_PassedComboTime += Time.deltaTime;
        }
    }


    private void ComboEvaluator()
    {
        if(m_ComboCounter > 1)  // Each kill that is chained in a combo is worth more then the one before
        {
            m_CurrentComboMultiplier *= m_ComboScaler;
        }

        if (m_ComboCounter < 2)     // Normalize multiplier if it's the first enemy killed
        {
            m_CurrentComboMultiplier /= m_CurrentComboMultiplier;
        }

        for (int i = 0; i < m_PointsCollector.Count; ++i)
        {
            m_PlayerScore += m_PointsCollector[i] * m_ComboBaseMultiplier;  // TODO: If time bonus or whatever, implement here
        }
        m_PointsCollector.Clear();

        // Combo alive
        if (m_PassedComboTime <= m_ComboTimeMax)
        {
            m_PassedComboTime = 0.0f;
            return;
        }

        // Combo dead
        m_PassedComboTime = 0.0f;
        m_CurrentComboMultiplier = m_ComboBaseMultiplier;
        m_ComboCounter = 0;
        m_ComboAlive = false;
    }


    // TODO: Implement bonus points from waves or similar here if/when the time comes
    public void BonusEvaluator()
    {
        for(int i = 0; i < m_PointsCollector.Count; ++i)
        {
            m_PlayerScore += m_PointsCollector[i];
        }
        m_PointsCollector.Clear();
    }


    public void ResetPlayer()
    {
        if(m_PointsCollector != null)
        {
            m_PointsCollector.Clear();
        }
        m_PointsCollector = new List<float>();

        m_PassedComboTime = 0.0f;  
        m_PlayerScore = 0.0f;
        m_CurrentComboMultiplier = m_ComboBaseMultiplier;
        m_ComboCounter = 0;
        m_ComboAlive = false;
    }


    private void Init()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;

        if (m_ComboScaler < 0.0f)
        {
            m_ComboScaler = 0.0f;
        }

        ResetPlayer();

        DontDestroyOnLoad(gameObject);
    }


    private void Awake()
    {
        Init();
    }


    private void Update()
    {
        UpdatePoints();
    }
}
