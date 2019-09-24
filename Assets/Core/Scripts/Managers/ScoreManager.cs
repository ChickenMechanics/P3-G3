using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    #region design vars
    public float m_ComboTimeMax = 1.0f;
    public int m_ComboMultiplier = 2;
    #endregion

    #region get set
    [HideInInspector]
    public float m_PassedComboTime { get; private set; }
    [HideInInspector]
    public int m_PlayerScore { get; private set; }
    [HideInInspector]
    public int m_ComboCounter { get; private set; }
    #endregion

    private List<int> m_PointsCollector;
    private bool m_ComboAlive;


    public void AddPoints(int value)
    {
        ++m_ComboCounter;
        if (m_ComboCounter == 1)
        {
            m_ComboAlive = true;
        }

        m_PointsCollector.Add(value);
    }


    private void UpdatePoints()
    {
        if (m_ComboAlive == true)
        {
            ComboEvaluator();
        }
    }


    private void ComboEvaluator()
    {
        m_PassedComboTime += Time.deltaTime;

        // Combo alive
        if (m_PassedComboTime <= m_ComboTimeMax)
        {
            // Normalize multi if it's the first enemy killed
            if (m_ComboCounter < 2)
            {
                m_ComboMultiplier /= m_ComboMultiplier;
            }

            for (int i = 0; i < m_PointsCollector.Count; ++i)
            {
                m_PlayerScore += m_PointsCollector[i] * m_ComboMultiplier;
            }
            m_PointsCollector.Clear();

            m_PassedComboTime = 0.0f;
        }

        // Combo dead
        for (int i = 0; i < m_PointsCollector.Count; ++i)
        {
            m_PlayerScore += m_PointsCollector[i];
        }
        m_PointsCollector.Clear();

        m_PassedComboTime = 0.0f;
        m_ComboCounter = 0;
        m_ComboAlive = false;
    }


    public void ResetPlayer()
    {
        if(m_PointsCollector != null)
        {
            m_PointsCollector.Clear();
        }
        m_PointsCollector = new List<int>();

        m_PassedComboTime = 0.0f;  
        m_PlayerScore = 0;
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
