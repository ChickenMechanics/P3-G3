using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    #region design vars
    public float m_ComboTimeInSecMax = 1.0f;
    public float m_ComboBaseMultiplier = 1.5f;
    public float m_ComboScaler = 1.15f;
    #endregion

    #region get / set
    [HideInInspector]
    public float m_PassedComboTime { get; private set; }
    [HideInInspector]
    public float m_PlayerScore { get; private set; }
    [HideInInspector]
    public int m_CurrentComboChain { get; private set; }
    [HideInInspector]
    public int m_TotalCombos { get; private set; }
    [HideInInspector]
    public int m_LongestCombo { get; private set; }
    #endregion

    private float m_CurrentComboMultiplier;
    private bool m_ComboAlive;


    public float GetComboTimeMax()
    {
        return m_ComboTimeInSecMax;
    }


    public float GetCurrentComboMultiplier()
    {
        return m_CurrentComboMultiplier;
    }


    public void AddComboPoints(float value)    //  Call this for everything included in the combo points system
    {
        ++m_CurrentComboChain;
        if (m_CurrentComboChain == 1)
        {
            m_ComboAlive = true;
        }

        ComboEvaluator(value);
    }


    public void AddVanillaPoints(float value)   //  Call this for vanilla points
    {
        m_PlayerScore += value;
    }


    private void UpdatePoints()
    {
        if (m_ComboAlive == true)
        {
            m_PassedComboTime += Time.deltaTime;
        }
    }


    private void ComboEvaluator(float value)
    {
        if(m_CurrentComboChain > 1)  // Each kill that is chained in a combo is worth more then the one before
        {
            m_CurrentComboMultiplier *= m_ComboScaler;
        }

        if (m_CurrentComboChain < 2)     // Normalize multiplier if it's the first enemy killed
        {
            m_CurrentComboMultiplier /= m_CurrentComboMultiplier;
        }

        m_PlayerScore += value * m_ComboBaseMultiplier;  // TODO: If time bonus or whatever, implement here

        // Combo alive
        if (m_PassedComboTime <= m_ComboTimeInSecMax)
        {
            m_PassedComboTime = 0.0f;
            return;
        }

        // Combo dead
        if(m_CurrentComboChain > m_LongestCombo)
        {
            m_LongestCombo = m_CurrentComboChain;
        }

        m_PassedComboTime = 0.0f;
        m_CurrentComboMultiplier = m_ComboBaseMultiplier;
        m_CurrentComboChain = 0;
        ++m_TotalCombos;
        m_ComboAlive = false;
    }


    public void ResetPlayer()
    {
        m_PassedComboTime = 0.0f;  
        m_PlayerScore = 0.0f;
        m_CurrentComboMultiplier = m_ComboBaseMultiplier;
        m_CurrentComboChain = 0;
        m_TotalCombos = 0;
        m_LongestCombo = 0;
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
