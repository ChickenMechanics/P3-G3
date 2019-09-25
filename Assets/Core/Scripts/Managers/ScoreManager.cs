using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager GetInstance { get; private set; }

    #region design vars
    public float m_ComboTimeInSecMax = 1.0f;
    public float m_ComboBaseMultiplier = 1.5f;
    public float m_ComboScaler = 1.15f;
    #endregion

    // TODO: Changed all that is named combo to chain
    #region get / set
    [HideInInspector]
    public float PassedComboTime { get; private set; }
    [HideInInspector]
    public float PlayerScore { get; private set; }
    [HideInInspector]
    public int CurrentComboChain { get; private set; }
    [HideInInspector]
    public float CurrentComboMultiplier { get; private set; }
    [HideInInspector]
    public int TotalCombos { get; private set; }
    [HideInInspector]
    public int LongestCombo { get; private set; }
    #endregion

    private bool m_ComboAlive;

    public enum EText
    {
        SCORE = 0,
        TOTAL_CHAINS,
        LONGEST_CHAIN,
        CHAIN_TIME_LEFT,
        CURRENT_CHAIN,
        CURRENT_MULTI,
        SIZE
    }


    public float GetComboTimeMax()
    {
        return m_ComboTimeInSecMax;
    }


    public void AddComboPoints(float value)    //  Call this for everything included in the combo points system
    {
        ++CurrentComboChain;
        if (CurrentComboChain == 1)
        {
            m_ComboAlive = true;
        }

        ComboEvaluator(value);
    }


    public void AddVanillaPoints(float value)   //  Call this for vanilla points
    {
        PlayerScore += value;
    }


    private void UpdatePoints()
    {
        if (m_ComboAlive == true)
        {
            PassedComboTime += Time.deltaTime;

            ComboUpdater();
        }
    }


    private void ComboEvaluator(float value)
    {
        if (CurrentComboChain > 1)  // Each kill that is chained in a combo is worth more then the previous
        {
            CurrentComboMultiplier *= m_ComboScaler;
        }

        if (CurrentComboChain < 2)     // Normalizes the multiplier if it's the first enemy killed
        {
            CurrentComboMultiplier /= CurrentComboMultiplier;
        }

        PlayerScore += value * CurrentComboMultiplier;  // TODO: If time bonus or whatever exists, implement here

        // Combo alive
        if (PassedComboTime <= m_ComboTimeInSecMax)    
        {
            PassedComboTime = 0.0f;
        }
    }


    private void ComboUpdater()
    {
        // Combo dead
        if (PassedComboTime > m_ComboTimeInSecMax)
        {
            if (CurrentComboChain > LongestCombo)
            {
                LongestCombo = CurrentComboChain;
            }

            PassedComboTime = 0.0f;
            CurrentComboMultiplier = 1.0f;
            CurrentComboChain = 0;
            ++TotalCombos;
            m_ComboAlive = false;
        }
    }


    public void ResetPlayer()
    {
        PassedComboTime = 0.0f;  
        PlayerScore = 0.0f;
        CurrentComboMultiplier = 1.0f;
        CurrentComboChain = 0;
        TotalCombos = 0;
        LongestCombo = 0;
        m_ComboAlive = false;
    }


    private void Init()
    {
        if (GetInstance != null && GetInstance != this)
        {
            Destroy(gameObject);
        }
        GetInstance = this;

        DontDestroyOnLoad(gameObject);

        if (m_ComboScaler < 0.0f)
        {
            m_ComboScaler = 0.0f;
        }

        ResetPlayer();
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
