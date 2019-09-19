using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PrefabUtility;

namespace Assets.Core.Ai.NavMeshComponents.Scripts
{
    public enum CollectObjects
    {
        All = 0,
        Volume = 1,
        Children = 2,
    }

    [ExecuteInEditMode]
    [DefaultExecutionOrder(-102)]
    [AddComponentMenu("Navigation/NavMeshSurface", 30)]
    [HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
    public class NavMeshSurface : MonoBehaviour
    {
        [SerializeField] private int m_AgentTypeID;
        public int agentTypeID { get => m_AgentTypeID; set => m_AgentTypeID = value; }

        [SerializeField] private CollectObjects m_CollectObjects = CollectObjects.All;
        public CollectObjects collectObjects
        {
            get => m_CollectObjects;
            set => m_CollectObjects = value;
        }

        [SerializeField] private Vector3 m_Size = new Vector3(10.0f, 10.0f, 10.0f);
        public Vector3 size { get => m_Size; set => m_Size = value; }

        [SerializeField] private Vector3 m_Center = new Vector3(0, 2.0f, 0);
        public Vector3 center { get => m_Center; set => m_Center = value; }

        [SerializeField] private LayerMask m_LayerMask = ~0;
        public LayerMask layerMask { get => m_LayerMask; set => m_LayerMask = value; }

        [SerializeField] private NavMeshCollectGeometry m_UseGeometry = NavMeshCollectGeometry.RenderMeshes;
        public NavMeshCollectGeometry useGeometry
        {
            get => m_UseGeometry;
            set => m_UseGeometry = value;

        }

        [SerializeField] private int m_DefaultArea;
        public int defaultArea
        {
            get => m_DefaultArea;
            set => m_DefaultArea = value;
        }

        [SerializeField] private bool m_IgnoreNavMeshAgent = true;
        public bool ignoreNavMeshAgent
        {
            get => m_IgnoreNavMeshAgent;
            set => m_IgnoreNavMeshAgent = value;
        }

        [SerializeField] private bool m_IgnoreNavMeshObstacle = true;
        public bool ignoreNavMeshObstacle
        {
            get => m_IgnoreNavMeshObstacle;
            set => m_IgnoreNavMeshObstacle = value;
        }

        [SerializeField] private bool m_OverrideTileSize;
        public bool overrideTileSize
        {
            get => m_OverrideTileSize;
            set => m_OverrideTileSize = value;
        }

        [SerializeField] private int m_TileSize = 256;
        public int tileSize
        {
            get => m_TileSize;
            set => m_TileSize = value;
        }

        [SerializeField] private bool m_OverrideVoxelSize;
        public bool overrideVoxelSize
        {
            get => m_OverrideVoxelSize;
            set => m_OverrideVoxelSize = value;
        }

        [SerializeField] private float m_VoxelSize;
        public float voxelSize
        {
            get => m_VoxelSize;
            set => m_VoxelSize = value;
        }

        // Currently not supported advanced options
        [SerializeField] private bool m_BuildHeightMesh;
        public bool buildHeightMesh
        {
            get => m_BuildHeightMesh;
            set => m_BuildHeightMesh = value;
        }

        // Reference to whole scene navmesh data asset.
        [UnityEngine.Serialization.FormerlySerializedAs("m_BakedNavMeshData")]
        [SerializeField]
        private NavMeshData m_NavMeshData;
        public NavMeshData navMeshData
        {
            get => m_NavMeshData;
            set => m_NavMeshData = value;
        }

        // Do not serialize - runtime only state.
        private NavMeshDataInstance m_NavMeshDataInstance;
        private Vector3 m_LastPosition = Vector3.zero;
        private Quaternion m_LastRotation = Quaternion.identity;

        private static readonly List<NavMeshSurface> s_NavMeshSurfaces = new List<NavMeshSurface>();

        public static List<NavMeshSurface> activeSurfaces => s_NavMeshSurfaces;

        private void OnEnable()
        {
            Register(this);
            AddData();
        }

        private void OnDisable()
        {
            RemoveData();
            Unregister(this);
        }

        public void AddData()
        {
            if (m_NavMeshDataInstance.valid)
                return;

            if (m_NavMeshData != null)
            {
                m_NavMeshDataInstance =
                    NavMesh.AddNavMeshData(
                        m_NavMeshData, transform.position, transform.rotation);
                m_NavMeshDataInstance.owner = this;
            }

            m_LastPosition = transform.position;
            m_LastRotation = transform.rotation;
        }

        public void RemoveData()
        {
            m_NavMeshDataInstance.Remove();
            m_NavMeshDataInstance = new NavMeshDataInstance();
        }

        public NavMeshBuildSettings GetBuildSettings()
        {
            var buildSettings = NavMesh.GetSettingsByID(m_AgentTypeID);
            if (buildSettings.agentTypeID == -1)
            {
                Debug.LogWarning("No build settings for agent type ID " + agentTypeID, this);
                buildSettings.agentTypeID = m_AgentTypeID;
            }

            if (overrideTileSize)
            {
                buildSettings.overrideTileSize = true;
                buildSettings.tileSize = tileSize;
            }
            if (overrideVoxelSize)
            {
                buildSettings.overrideVoxelSize = true;
                buildSettings.voxelSize = voxelSize;
            }
            return buildSettings;
        }

        public void BuildNavMesh()
        {
            var sources = CollectSources();

            // Use unscaled bounds -
            // this differs in behaviour from e.g. collider components.
            // But is similar to reflection probe -
            // and since navmesh data has no scaling support -
            // it is the right choice here.
            var sourcesBounds = new Bounds(m_Center, Abs(m_Size));
            if (m_CollectObjects == CollectObjects.All || m_CollectObjects == CollectObjects.Children)
            {
                sourcesBounds = CalculateWorldBounds(sources);
            }

            var data = NavMeshBuilder.BuildNavMeshData(GetBuildSettings(),
                    sources, sourcesBounds, transform.position, transform.rotation);

            if (data == null) return;

            data.name = gameObject.name;
            RemoveData();
            m_NavMeshData = data;
            if (isActiveAndEnabled)
                AddData();
        }

        public AsyncOperation UpdateNavMesh(NavMeshData data)
        {
            var sources = CollectSources();

            // Use unscaled bounds -
            // this differs in behaviour from e.g. collider components.
            // But is similar to reflection probe -
            // and since navmesh data has no scaling support -
            // it is the right choice here.
            var sourcesBounds = new Bounds(m_Center, Abs(m_Size));
            if (m_CollectObjects == CollectObjects.All || m_CollectObjects == CollectObjects.Children)
                sourcesBounds = CalculateWorldBounds(sources);

            return NavMeshBuilder.UpdateNavMeshDataAsync(data, GetBuildSettings(), sources, sourcesBounds);
        }

        private static void Register(NavMeshSurface surface)
        {
            if (s_NavMeshSurfaces.Count == 0)
                NavMesh.onPreUpdate += UpdateActive;

            if (!s_NavMeshSurfaces.Contains(surface))
                s_NavMeshSurfaces.Add(surface);
        }

        private static void Unregister(NavMeshSurface surface)
        {
            s_NavMeshSurfaces.Remove(surface);

            if (s_NavMeshSurfaces.Count == 0 && NavMesh.onPreUpdate != null)
                    NavMesh.onPreUpdate -= UpdateActive;
        }

        private static void UpdateActive()
        {
            for (var i = 0; i < s_NavMeshSurfaces.Count; ++i)
                s_NavMeshSurfaces[i].UpdateDataIfTransformChanged();
        }

        private void AppendModifierVolumes(ref List<NavMeshBuildSource> sources)
        {
            // Modifiers
            List<NavMeshModifierVolume> modifiers;
            if (m_CollectObjects == CollectObjects.Children)
            {
                modifiers =
                    new List<NavMeshModifierVolume>
                        (GetComponentsInChildren<NavMeshModifierVolume>());
                modifiers.RemoveAll(x => !x.isActiveAndEnabled);
            }
            else
                modifiers = NavMeshModifierVolume.activeModifiers;

            foreach (var m in modifiers)
            {
                if ((m_LayerMask & (1 << m.gameObject.layer)) == 0)
                    continue;
                if (!m.AffectsAgentType(m_AgentTypeID))
                    continue;
                var mcenter = m.transform.TransformPoint(m.center);
                var scale = m.transform.lossyScale;
                var msize =
                    new Vector3(
                        m.size.x * Mathf.Abs(scale.x),
                        m.size.y * Mathf.Abs(scale.y),
                        m.size.z * Mathf.Abs(scale.z));

                var src = new NavMeshBuildSource
                {
                    shape = NavMeshBuildSourceShape.ModifierBox,
                    transform =
                        Matrix4x4.TRS(
                            mcenter, m.transform.rotation, Vector3.one),
                    size = msize,
                    area = m.area
                };
                sources.Add(src);
            }
        }

        private List<NavMeshBuildSource> CollectSources()
        {
            var sources = new List<NavMeshBuildSource>();
            var markups = new List<NavMeshBuildMarkup>();

            List<NavMeshModifier> modifiers;
            if (m_CollectObjects == CollectObjects.Children)
            {
                modifiers = new List<NavMeshModifier>(GetComponentsInChildren<NavMeshModifier>());
                modifiers.RemoveAll(x => !x.isActiveAndEnabled);
            }
            else
                modifiers = NavMeshModifier.activeModifiers;

            foreach (var m in modifiers)
            {
                if ((m_LayerMask & (1 << m.gameObject.layer)) == 0)
                    continue;
                if (!m.AffectsAgentType(m_AgentTypeID))
                    continue;
                var markup = new NavMeshBuildMarkup
                {
                    root = m.transform,
                    overrideArea = m.overrideArea,
                    area = m.area,
                    ignoreFromBuild = m.ignoreFromBuild
                };
                markups.Add(markup);
            }

            switch (m_CollectObjects)
            {
                case CollectObjects.All:
                    NavMeshBuilder.CollectSources(
                        null, m_LayerMask, m_UseGeometry,
                        m_DefaultArea, markups, sources);
                    break;
                case CollectObjects.Children:
                    NavMeshBuilder.CollectSources(
                        transform, m_LayerMask, m_UseGeometry,
                        m_DefaultArea, markups, sources);
                    break;
                case CollectObjects.Volume:
                {
                    var localToWorld =
                        Matrix4x4.TRS(
                            transform.position, transform.rotation,
                            Vector3.one);
                    var worldBounds =
                        GetWorldBounds(localToWorld, new Bounds(m_Center, m_Size));
                    NavMeshBuilder.CollectSources(
                        worldBounds, m_LayerMask, m_UseGeometry,
                        m_DefaultArea, markups, sources);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (m_IgnoreNavMeshAgent)
                sources.RemoveAll(
                    (x) =>
                        (x.component != null &&
                         x.component.gameObject.GetComponent<NavMeshAgent>() != null));

            if (m_IgnoreNavMeshObstacle)
                sources.RemoveAll(
                    (x) =>
                        (x.component != null && 
                         x.component.gameObject.GetComponent<NavMeshObstacle>() != null));

            AppendModifierVolumes(ref sources);

            return sources;
        }

        private static Vector3 Abs(Vector3 v)
        {
            return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
        }

        private static Bounds GetWorldBounds(Matrix4x4 mat, Bounds bounds)
        {
            var absAxisX = Abs(mat.MultiplyVector(Vector3.right));
            var absAxisY = Abs(mat.MultiplyVector(Vector3.up));
            var absAxisZ = Abs(mat.MultiplyVector(Vector3.forward));
            var worldPosition = mat.MultiplyPoint(bounds.center);
            var worldSize =
                absAxisX * bounds.size.x +
                absAxisY * bounds.size.y +
                absAxisZ * bounds.size.z;
            return new Bounds(worldPosition, worldSize);
        }

        private Bounds CalculateWorldBounds(IEnumerable<NavMeshBuildSource> sources)
        {
            // Use the unscaled matrix for the NavMeshSurface
            var worldToLocal =
                Matrix4x4.TRS(
                    transform.position, transform.rotation, Vector3.one);
            worldToLocal = worldToLocal.inverse;

            var result = new Bounds();
            foreach (var src in sources)
            {
                switch (src.shape)
                {
                    case NavMeshBuildSourceShape.Mesh:
                    {
                        var m = src.sourceObject as Mesh;
                        result.Encapsulate(
                            GetWorldBounds(worldToLocal * src.transform, m.bounds));
                        break;
                    }
                    case NavMeshBuildSourceShape.Terrain:
                    {
                        // Terrain pivot is lower/left corner - shift bounds accordingly
                        var t = src.sourceObject as TerrainData;
                        if (t != null)
                            result.Encapsulate(
                                GetWorldBounds(
                                    worldToLocal * src.transform,
                                    new Bounds(0.5f * t.size, t.size)));
                        break;
                    }
                    case NavMeshBuildSourceShape.Box:
                    case NavMeshBuildSourceShape.Sphere:
                    case NavMeshBuildSourceShape.Capsule:
                    case NavMeshBuildSourceShape.ModifierBox:
                        result.Encapsulate(GetWorldBounds(
                            worldToLocal * src.transform,
                            new Bounds(Vector3.zero, src.size)));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            // Inflate the bounds a bit to avoid clipping co-planar sources
            result.Expand(0.1f);
            return result;
        }

        private bool HasTransformChanged()
        {
            return 
                (m_LastPosition != transform.position ||
                 m_LastRotation != transform.rotation);
        }

        private void UpdateDataIfTransformChanged()
        {
            if (!HasTransformChanged()) return;

            RemoveData();
            AddData();
        }

#if UNITY_EDITOR
        private bool UnshareNavMeshAsset()
        {
            // Nothing to unshare
            if (m_NavMeshData == null)
                return false;

            // Prefab parent owns the asset reference
            var prefabType = PrefabUtility.GetPrefabAssetType(this);
            if (prefabType == PrefabAssetType.Regular)
                return false;

            // An instance can share asset reference only with its prefab parent
            var prefab = GetCorrespondingObjectFromSource(this) as NavMeshSurface;
            if (prefab != null && prefab.navMeshData == navMeshData)
                return false;

            // Don't allow referencing an asset that's assigned to another surface
            return
                s_NavMeshSurfaces.Any(
                    surface =>
                        surface != this && surface.m_NavMeshData == m_NavMeshData);

            // Asset is not referenced by known surfaces
        }

        private void OnValidate()
        {
            if (UnshareNavMeshAsset())
            {
                Debug.LogWarning(
                    "Duplicating NavMeshSurface does not duplicate the referenced navmesh data",
                    this);
                m_NavMeshData = null;
            }

            var settings = NavMesh.GetSettingsByID(m_AgentTypeID);
            if (settings.agentTypeID == -1) return;

            // When unchecking the override control, revert to automatic value.
            const float kMinVoxelSize = 0.01f;
            if (!m_OverrideVoxelSize)
                m_VoxelSize = settings.agentRadius / 3.0f;
            if (m_VoxelSize < kMinVoxelSize)
                m_VoxelSize = kMinVoxelSize;

            // When unchecking the override control, revert to default value.
            const int kMinTileSize = 16;
            const int kMaxTileSize = 1024;
            const int kDefaultTileSize = 256;

            if (!m_OverrideTileSize)
                m_TileSize = kDefaultTileSize;
            // Make sure tile size is in sane range.
            Mathf.Clamp(m_TileSize, kMinTileSize, kMaxTileSize);
        }
#endif
    }
}
