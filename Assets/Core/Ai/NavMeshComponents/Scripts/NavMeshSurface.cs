using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using static UnityEditor.PrefabUtility;
#endif

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
        [field: SerializeField] public int agentTypeID { get; set; }

        [field: SerializeField]
        public CollectObjects collectObjects { get; set; } = CollectObjects.All;

        [field: SerializeField] public Vector3 size { get; set; } = new Vector3(10.0f, 10.0f, 10.0f);

        [field: SerializeField] public Vector3 center { get; set; } = new Vector3(0, 2.0f, 0);

        [field: SerializeField] public LayerMask layerMask { get; set; } = ~0;

        [field: SerializeField]
        public NavMeshCollectGeometry useGeometry { get; set; } = NavMeshCollectGeometry.RenderMeshes;

        [field: SerializeField]
        public int defaultArea { get; set; }

        [field: SerializeField]
        public bool ignoreNavMeshAgent { get; set; } = true;

        [field: SerializeField]
        public bool ignoreNavMeshObstacle { get; set; } = true;

        [field: SerializeField]
        public bool overrideTileSize { get; set; }

        [field: SerializeField]
        public int tileSize { get; set; } = 256;


        [field: SerializeField]
        public bool overrideVoxelSize { get; set; }

        [field: SerializeField]
        public float voxelSize { get; set; }

        // Currently not supported advanced options
        [field: SerializeField]
        public bool buildHeightMesh { get; set; }

        // Reference to whole scene navmesh data asset.
        [field: UnityEngine.Serialization.FormerlySerializedAs("m_BakedNavMeshData")]
        [field: SerializeField]
        public NavMeshData navMeshData { get; set; }

        // Do not serialize - runtime only state.
        private NavMeshDataInstance m_NavMeshDataInstance;
        private Vector3 m_LastPosition = Vector3.zero;
        private Quaternion m_LastRotation = Quaternion.identity;

        public static List<NavMeshSurface> activeSurfaces { get; } = new List<NavMeshSurface>();

        private void OnEnable() { Register(this); AddData(); }

        private void OnDisable() { RemoveData(); Unregister(this); }

        public void AddData()
        {
            if (m_NavMeshDataInstance.valid)
                return;

            if (navMeshData != null)
            {
                m_NavMeshDataInstance =
                    NavMesh.AddNavMeshData(
                        navMeshData, transform.position, transform.rotation);
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
            var buildSettings = NavMesh.GetSettingsByID(agentTypeID);
            if (buildSettings.agentTypeID == -1)
            {
                Debug.LogWarning("No build settings for agent type ID " + agentTypeID, this);
                buildSettings.agentTypeID = agentTypeID;
            }

            if (overrideTileSize)
            {
                buildSettings.overrideTileSize = true;
                buildSettings.tileSize = tileSize;
            }

            if (!overrideVoxelSize) return buildSettings;

            buildSettings.overrideVoxelSize = true;
            buildSettings.voxelSize = voxelSize;

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
            var sourcesBounds = new Bounds(center, Abs(size));
            if (collectObjects == CollectObjects.All || collectObjects == CollectObjects.Children)
                sourcesBounds = CalculateWorldBounds(sources);

            var data = NavMeshBuilder.BuildNavMeshData(GetBuildSettings(),
                    sources, sourcesBounds, transform.position, transform.rotation);

            if (data == null) return;

            data.name = gameObject.name;
            RemoveData();
            navMeshData = data;
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
            var sourcesBounds = new Bounds(center, Abs(size));
            if (collectObjects == CollectObjects.All ||
                collectObjects == CollectObjects.Children)
                sourcesBounds = CalculateWorldBounds(sources);

            return NavMeshBuilder.UpdateNavMeshDataAsync(
                data, GetBuildSettings(), sources, sourcesBounds);
        }

        private static void Register(NavMeshSurface surface)
        {
            if (activeSurfaces.Count == 0)
                NavMesh.onPreUpdate += UpdateActive;

            if (!activeSurfaces.Contains(surface))
                activeSurfaces.Add(surface);
        }

        private static void Unregister(NavMeshSurface surface)
        {
            activeSurfaces.Remove(surface);

            if (activeSurfaces.Count == 0 && NavMesh.onPreUpdate != null)
                    NavMesh.onPreUpdate -= UpdateActive;
        }

        private static void UpdateActive()
        {
            foreach (var t in activeSurfaces)
                t.UpdateDataIfTransformChanged();
        }

        private void AppendModifierVolumes(ref List<NavMeshBuildSource> sources)
        {
            // Modifiers
            List<NavMeshModifierVolume> modifiers;
            if (collectObjects == CollectObjects.Children)
            {
                modifiers =
                    new List<NavMeshModifierVolume>
                        (GetComponentsInChildren<NavMeshModifierVolume>());
                modifiers.RemoveAll(x => !x.isActiveAndEnabled);
            }
            else
                modifiers = NavMeshModifierVolume.activeModifiers;

            sources.AddRange(
                from m
                    in modifiers
                where (layerMask & (1 << m.gameObject.layer)) != 0
                where m.AffectsAgentType(agentTypeID)
                let mcenter = m.transform.TransformPoint(m.center)
                let scale = m.transform.lossyScale
                let msize =
                    new Vector3(
                        m.size.x * Mathf.Abs(scale.x),
                        m.size.y * Mathf.Abs(scale.y),
                        m.size.z * Mathf.Abs(scale.z))
                select new NavMeshBuildSource
                {
                    shape =
                        NavMeshBuildSourceShape.ModifierBox,
                        transform = Matrix4x4.TRS(
                            mcenter,
                            m.transform.rotation,
                            Vector3.one),
                        size = msize,
                        area = m.area
                });
        }

        private List<NavMeshBuildSource> CollectSources()
        {
            var sources = new List<NavMeshBuildSource>();

            List<NavMeshModifier> modifiers;
            if (collectObjects == CollectObjects.Children)
            {
                modifiers =
                    new List<NavMeshModifier>(
                        GetComponentsInChildren<NavMeshModifier>());
                modifiers.RemoveAll(x => !x.isActiveAndEnabled);
            }
            else
                modifiers = NavMeshModifier.activeModifiers;

            var markups =
                (from m
                    in modifiers
                    where (layerMask & (1 << m.gameObject.layer)) != 0
                    where m.AffectsAgentType(agentTypeID)
                    select new NavMeshBuildMarkup
                    {
                        root = m.transform,
                        overrideArea = m.overrideArea,
                        area = m.area,
                        ignoreFromBuild = m.ignoreFromBuild
                    }).ToList();

            switch (collectObjects)
            {
                case CollectObjects.All:
                    NavMeshBuilder.CollectSources(
                        null, layerMask, useGeometry,
                        defaultArea, markups, sources);
                    break;
                case CollectObjects.Children:
                    NavMeshBuilder.CollectSources(
                        transform, layerMask, useGeometry,
                        defaultArea, markups, sources);
                    break;
                case CollectObjects.Volume:
                {
                    var localToWorld =
                        Matrix4x4.TRS(
                            transform.position, transform.rotation,
                            Vector3.one);
                    var worldBounds =
                        GetWorldBounds(localToWorld, new Bounds(center, size));
                    NavMeshBuilder.CollectSources(
                        worldBounds, layerMask, useGeometry,
                        defaultArea, markups, sources);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (ignoreNavMeshAgent)
                sources.RemoveAll(
                    (x) =>
                        (x.component != null &&
                         x.component.gameObject.GetComponent<NavMeshAgent>() != null));

            if (ignoreNavMeshObstacle)
                sources.RemoveAll(
                    (x) =>
                        (x.component != null && 
                         x.component.gameObject.GetComponent<NavMeshObstacle>() != null));

            AppendModifierVolumes(ref sources);

            return sources;
        }

        private static Vector3 Abs(Vector3 v)
        {
            return new Vector3(
                Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
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
                            GetWorldBounds(
                                worldToLocal * src.transform, m.bounds));
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
            if (navMeshData == null)
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
                activeSurfaces.Any(
                    surface =>
                        surface != this && surface.navMeshData == navMeshData);

            // Asset is not referenced by known surfaces
        }

        private void OnValidate()
        {
            if (UnshareNavMeshAsset())
            {
                Debug.LogWarning(
                    "Duplicating NavMeshSurface does not" +
                    " duplicate the referenced navmesh data",
                    this);
                navMeshData = null;
            }

            var settings = NavMesh.GetSettingsByID(agentTypeID);
            if (settings.agentTypeID == -1) return;

            // When unchecking the override control, revert to automatic value.
            const float kMinVoxelSize = 0.01f;
            if (!overrideVoxelSize)
                voxelSize = settings.agentRadius / 3.0f;
            if (voxelSize < kMinVoxelSize)
                voxelSize = kMinVoxelSize;

            // When unchecking the override control, revert to default value.
            const int kMinTileSize = 16;
            const int kMaxTileSize = 1024;
            const int kDefaultTileSize = 256;

            if (!overrideTileSize)
                tileSize = kDefaultTileSize;
            // Make sure tile size is in sane range.
            Mathf.Clamp(tileSize, kMinTileSize, kMaxTileSize);
        }
#endif
    }
}
