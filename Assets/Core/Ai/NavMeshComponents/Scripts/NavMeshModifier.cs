using System.Collections.Generic;

namespace UnityEngine.AI
{
    [ExecuteInEditMode]
    [AddComponentMenu("Navigation/NavMeshModifier", 32)]
    [HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
    public class NavMeshModifier : MonoBehaviour
    {
        [field: SerializeField]
        public bool overrideArea { get; set; }

        [field: SerializeField]
        public int area { get; set; }

        [field: SerializeField]
        public bool ignoreFromBuild { get; set; }

        // List of agent types the modifier is applied for.
        // Special values: empty == None, m_AffectedAgents[0] =-1 == All.
        [SerializeField] private List<int> m_AffectedAgents =
            new List<int>(new int[] { -1 });    // Default value is All

        public static List<NavMeshModifier> activeModifiers { get; } = new List<NavMeshModifier>();

        private void OnEnable()
        {
            if (!activeModifiers.Contains(this))
                activeModifiers.Add(this);
        }

        private void OnDisable() { activeModifiers.Remove(this); }

        public bool AffectsAgentType(int agentTypeID)
        {
            if (m_AffectedAgents.Count == 0)
                return false;
            if (m_AffectedAgents[0] == -1)
                return true;
            return m_AffectedAgents.IndexOf(agentTypeID) != -1;
        }
    }
}
