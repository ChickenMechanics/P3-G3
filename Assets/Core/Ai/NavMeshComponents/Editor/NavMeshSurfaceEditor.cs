using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Core.Ai.NavMeshComponents.Editor;
using Assets.Core.Ai.NavMeshComponents.Scripts;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine.AI;
using UnityEngine;
using static UnityEditor.EditorApplication;
using static UnityEditor.PrefabUtility;

namespace UnityEditor.AI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NavMeshSurface))]
    internal class NavMeshSurfaceEditor : Editor
    {
        private SerializedProperty m_AgentTypeID;
        private SerializedProperty m_BuildHeightMesh;
        private SerializedProperty m_Center;
        private SerializedProperty m_CollectObjects;
        private SerializedProperty m_DefaultArea;
        private SerializedProperty m_LayerMask;
        private SerializedProperty m_OverrideTileSize;
        private SerializedProperty m_OverrideVoxelSize;
        private SerializedProperty m_Size;
        private SerializedProperty m_TileSize;
        private SerializedProperty m_UseGeometry;
        private SerializedProperty m_VoxelSize;

        private class Styles
        {
            public readonly GUIContent m_LayerMask =
                new GUIContent("Include Layers");
            public readonly GUIContent m_ShowInputGeom =
                new GUIContent("Show Input Geom");
            public readonly GUIContent m_ShowVoxels =
                new GUIContent("Show Voxels");
            public readonly GUIContent m_ShowRegions =
                new GUIContent("Show Regions");
            public readonly GUIContent m_ShowRawContours =
                new GUIContent("Show Raw Contours");
            public readonly GUIContent m_ShowContours =
                new GUIContent("Show Contours");
            public readonly GUIContent m_ShowPolyMesh =
                new GUIContent("Show Poly Mesh");
            public readonly GUIContent m_ShowPolyMeshDetail =
                new GUIContent("Show Poly Mesh Detail");
        }

        private struct AsyncBakeOperation
        {
            public NavMeshSurface surface;
            public NavMeshData bakeData;
            public AsyncOperation bakeOperation;
        }

        private static List<AsyncBakeOperation> s_BakeOperations =
            new List<AsyncBakeOperation>();

        private static Styles s_Styles;

        private static bool s_ShowDebugOptions;

        private static Color s_HandleColor =
            new Color(127f, 214f, 244f, 100f) / 255;
        private static Color s_HandleColorSelected =
            new Color(127f, 214f, 244f, 210f) / 255;
        private static Color s_HandleColorDisabled =
            new Color(127f * 0.75f, 214f * 0.75f, 244f * 0.75f, 100f) / 255;

        private BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle();

        private bool editingCollider =>
            EditMode.editMode ==
            EditMode.SceneViewEditMode.Collider && EditMode.IsOwner(this);

        private void OnEnable()
        {
            m_AgentTypeID = serializedObject.FindProperty("m_AgentTypeID");
            m_BuildHeightMesh = serializedObject.FindProperty("m_BuildHeightMesh");
            m_Center = serializedObject.FindProperty("m_Center");
            m_CollectObjects = serializedObject.FindProperty("m_CollectObjects");
            m_DefaultArea = serializedObject.FindProperty("m_DefaultArea");
            m_LayerMask = serializedObject.FindProperty("m_LayerMask");
            m_OverrideTileSize = serializedObject.FindProperty("m_OverrideTileSize");
            m_OverrideVoxelSize = serializedObject.FindProperty("m_OverrideVoxelSize");
            m_Size = serializedObject.FindProperty("m_Size");
            m_TileSize = serializedObject.FindProperty("m_TileSize");
            m_UseGeometry = serializedObject.FindProperty("m_UseGeometry");
            m_VoxelSize = serializedObject.FindProperty("m_VoxelSize");

            NavMeshVisualizationSettings.showNavigation++;
        }

        private void OnDisable()
        {
            NavMeshVisualizationSettings.showNavigation--;
        }

        private static string GetAndEnsureTargetPath(Component surface)
        {
            // Create directory for the asset if it does not exist yet.
            var activeScenePath = surface.gameObject.scene.path;

            var targetPath = "Assets";
            if (!string.IsNullOrEmpty(activeScenePath))
                targetPath =
                    Path.Combine(
                        Path.GetDirectoryName(activeScenePath) ??
                        throw new InvalidOperationException(),
                        Path.GetFileNameWithoutExtension(activeScenePath));
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);
            return targetPath;
        }

        private static void CreateNavMeshAsset(NavMeshSurface surface)
        {
            var targetPath = GetAndEnsureTargetPath(surface);

            var combinedAssetPath =
                Path.Combine(targetPath, "NavMesh-" + surface.name + ".asset");
            combinedAssetPath = AssetDatabase.GenerateUniqueAssetPath(combinedAssetPath);
            AssetDatabase.CreateAsset(surface.navMeshData, combinedAssetPath);
        }

        private static NavMeshData GetNavMeshAssetToDelete(NavMeshSurface navSurface)
        {
            if (navSurface == null) return navSurface.navMeshData;

            var prefabType = GetPrefabType(navSurface);
            if (prefabType != PrefabType.PrefabInstance &&
                prefabType != PrefabType.DisconnectedPrefabInstance)
                return navSurface.navMeshData;
            // Don't allow deleting the asset belonging to the prefab parent
            var parentSurface =
                GetCorrespondingObjectFromSource(navSurface) as NavMeshSurface;
            if (parentSurface &&
                navSurface.navMeshData == parentSurface.navMeshData)
                return null;

            return navSurface.navMeshData;
        }

        private void ClearSurface(NavMeshSurface navSurface)
        {
            var assetToDelete = GetNavMeshAssetToDelete(navSurface);
            navSurface.RemoveData();
            navSurface.navMeshData = null;
            EditorUtility.SetDirty(navSurface);

            if (!assetToDelete) return;

            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(assetToDelete));
            EditorSceneManager.MarkSceneDirty(navSurface.gameObject.scene);
        }

        private Bounds GetBounds()
        {
            var navSurface = (NavMeshSurface)target;
            return new Bounds(navSurface.transform.position, navSurface.size);
        }

        public override void OnInspectorGUI()
        {
            if (s_Styles == null)
                s_Styles = new Styles();

            serializedObject.Update();

            var bs = NavMesh.GetSettingsByID(m_AgentTypeID.intValue);

            if (bs.agentTypeID != -1)
            {
                // Draw image
                const float diagramHeight = 80.0f;
                var agentDiagramRect =
                    EditorGUILayout.GetControlRect(false, diagramHeight);
                NavMeshEditorHelpers.DrawAgentDiagram(
                    agentDiagramRect, bs.agentRadius, bs.agentHeight,
                    bs.agentClimb, bs.agentSlope);
            }
            NavMeshComponentsGUIUtility.AgentTypePopup(
                "Agent Type", m_AgentTypeID);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_CollectObjects);
            if ((CollectObjects)m_CollectObjects.enumValueIndex ==
                CollectObjects.Volume)
            {
                EditorGUI.indentLevel++;

                EditMode.DoEditModeInspectorModeButton(
                    EditMode.SceneViewEditMode.Collider,
                    "Edit Volume",
                    EditorGUIUtility.IconContent("EditCollider"),
                    GetBounds,
                    this);
                EditorGUILayout.PropertyField(m_Size);
                EditorGUILayout.PropertyField(m_Center);

                EditorGUI.indentLevel--;
            }
            else if (editingCollider)
                EditMode.QuitEditMode();

            EditorGUILayout.PropertyField(m_LayerMask, s_Styles.m_LayerMask);
            EditorGUILayout.PropertyField(m_UseGeometry);

            EditorGUILayout.Space();

            EditorGUILayout.Space();

            m_OverrideVoxelSize.isExpanded =
                EditorGUILayout.Foldout(
                    m_OverrideVoxelSize.isExpanded, "Advanced");
            if (m_OverrideVoxelSize.isExpanded)
            {
                EditorGUI.indentLevel++;

                NavMeshComponentsGUIUtility.AreaPopup(
                    "Default Area", m_DefaultArea);

                // Override voxel size.
                EditorGUILayout.PropertyField(m_OverrideVoxelSize);

                using (
                    new EditorGUI.DisabledScope(
                        !m_OverrideVoxelSize.boolValue ||
                        m_OverrideVoxelSize.hasMultipleDifferentValues))
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(m_VoxelSize);

                    if (!m_OverrideVoxelSize.hasMultipleDifferentValues)
                    {
                        if (!m_AgentTypeID.hasMultipleDifferentValues)
                        {
                            var voxelsPerRadius =
                                m_VoxelSize.floatValue > 0.0f ?
                                    (bs.agentRadius / m_VoxelSize.floatValue) :
                                    0.0f;
                            EditorGUILayout.LabelField(
                                " ",
                                voxelsPerRadius.ToString("0.00") +
                                    " voxels per agent radius",
                                EditorStyles.miniLabel);
                        }

                        if (m_OverrideVoxelSize.boolValue)
                            EditorGUILayout.HelpBox(
                                "Voxel size controls how accurately the" +
                                " navigation mesh is generated from the level geometry." +
                                " A good voxel size is 2-4 voxels per agent radius." +
                                " Making voxel size smaller will increase build time.",
                                MessageType.None);
                    }
                    EditorGUI.indentLevel--;
                }

                // Override tile size
                EditorGUILayout.PropertyField(m_OverrideTileSize);

                using (
                    new EditorGUI.DisabledScope(
                        !m_OverrideTileSize.boolValue ||
                        m_OverrideTileSize.hasMultipleDifferentValues))
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(m_TileSize);

                    if (!m_TileSize.hasMultipleDifferentValues &&
                        !m_VoxelSize.hasMultipleDifferentValues)
                    {
                        var tileWorldSize =
                            m_TileSize.intValue * m_VoxelSize.floatValue;
                        EditorGUILayout.LabelField(
                            " ",
                            tileWorldSize.ToString("0.00") +
                                " world units",
                            EditorStyles.miniLabel);
                    }

                    if (!m_OverrideTileSize.hasMultipleDifferentValues &&
                        m_OverrideTileSize.boolValue)
                    { 
                        EditorGUILayout.HelpBox(
                            "Tile size controls the how local the changes" +
                            " to the world are (rebuild or carve)." +
                            " Small tile size allows more local changes," +
                            " while potentially generating more data in overall.",
                            MessageType.None);
                    }
                    EditorGUI.indentLevel--;
                }


                // Height mesh
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(m_BuildHeightMesh);
                }

                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

            var hadError = false;
            var multipleTargets = targets.Length > 1;
            foreach (var o in targets)
            {
                var navSurface = (NavMeshSurface) o;
                var settings = navSurface.GetBuildSettings();
                // Calculating bounds is potentially expensive when unbounded -
                // so here we just use the center/size.
                // It means the validation is not checking vertical voxel limit
                // correctly when the surface is set to something else than "in volume".
                var bounds = new Bounds(Vector3.zero, Vector3.zero);
                if (navSurface.collectObjects == CollectObjects.Volume)
                    bounds = new Bounds(navSurface.center, navSurface.size);

                var errors = settings.ValidationReport(bounds);
                if (errors.Length <= 0 || !multipleTargets) continue;

                EditorGUILayout.LabelField(navSurface.name);
                foreach (var err in errors)
                    EditorGUILayout.HelpBox(err, MessageType.Warning);
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUIUtility.labelWidth);
                if (GUILayout.Button(
                    "Open Agent Settings...", EditorStyles.miniButton))
                    NavMeshEditorHelpers.OpenAgentSettings(
                        navSurface.agentTypeID);
                GUILayout.EndHorizontal();
                hadError = true;
            }

            if (hadError)
                EditorGUILayout.Space();

            using (
                new EditorGUI.DisabledScope(
                    Application.isPlaying || m_AgentTypeID.intValue == -1))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUIUtility.labelWidth);
                if (GUILayout.Button("Clear"))
                {
                    foreach (var o in targets)
                    {
                        var s = (NavMeshSurface) o;
                        ClearSurface(s);
                    }

                    SceneView.RepaintAll();
                }

                if (GUILayout.Button("Bake"))
                {
                    // Remove first to avoid double registration of the callback
                    if (update != null) update -= UpdateAsyncBuildOperations;
                    update += UpdateAsyncBuildOperations;

                    foreach (var o in targets)
                    {
                        var surf = (NavMeshSurface) o;
                        var operation =
                            new AsyncBakeOperation
                            {
                                bakeData = InitializeBakeData(surf)
                            };

                        operation.bakeOperation =
                            surf.UpdateNavMesh(operation.bakeData);
                        operation.surface = surf;

                        s_BakeOperations.Add(operation);
                    }
                }

                GUILayout.EndHorizontal();
            }

            // Show progress for the selected targets
            for (var i = s_BakeOperations.Count - 1; i >= 0; --i)
            {
                if (!targets.Contains(s_BakeOperations[i].surface))
                    continue;

                var operation = s_BakeOperations[i].bakeOperation;
                if (operation == null)
                    continue;

                var p = operation.progress;
                if (operation.isDone)
                {
                    SceneView.RepaintAll();
                    continue;
                }

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Cancel", EditorStyles.miniButton))
                {
                    var bakeData = s_BakeOperations[i].bakeData;
                    UnityEngine.AI.NavMeshBuilder.Cancel(bakeData);
                    s_BakeOperations.RemoveAt(i);
                }

                EditorGUI.ProgressBar(
                    EditorGUILayout.GetControlRect(),
                    p,
                    "Baking: " + (int)(100 * p) + "%");
                if (p <= 1)
                    Repaint();

                GUILayout.EndHorizontal();
            }
        }

        private static NavMeshData InitializeBakeData(NavMeshSurface surface)
        {
            var emptySources = new List<NavMeshBuildSource>();
            var emptyBounds = new Bounds();
            return UnityEngine.AI.NavMeshBuilder.BuildNavMeshData(
                surface.GetBuildSettings(), emptySources, emptyBounds
                , surface.transform.position, surface.transform.rotation);
        }

        private static void UpdateAsyncBuildOperations()
        {
            foreach (var oper in s_BakeOperations)
            {
                if (oper.surface == null || oper.bakeOperation == null)
                    continue;

                if (!oper.bakeOperation.isDone) continue;

                var surface = oper.surface;
                var delete = GetNavMeshAssetToDelete(surface);
                if (delete != null)
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(delete));

                surface.RemoveData();
                surface.navMeshData = oper.bakeData;
                if (surface.isActiveAndEnabled)
                    surface.AddData();
                CreateNavMeshAsset(surface);
                EditorSceneManager.MarkSceneDirty(surface.gameObject.scene);
            }
            s_BakeOperations.RemoveAll(
                o => o.bakeOperation == null || o.bakeOperation.isDone);
            if (s_BakeOperations.Count == 0 && update != null)
                    update -= UpdateAsyncBuildOperations;
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.Active | GizmoType.Pickable)]
        private static void RenderBoxGizmoSelected(
            NavMeshSurface navSurface, GizmoType gizmoType)
        {
            RenderBoxGizmo(navSurface, gizmoType, true);
        }

        [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Pickable)]
        private static void RenderBoxGizmoNotSelected(
            NavMeshSurface navSurface, GizmoType gizmoType)
        {
            if (NavMeshVisualizationSettings.showNavigation > 0)
                RenderBoxGizmo(navSurface, gizmoType, false);
            else
                Gizmos.DrawIcon(
                    navSurface.transform.position,
                    "NavMeshSurface Icon",
                    true);
        }

        private static void RenderBoxGizmo(
            NavMeshSurface navSurface, GizmoType gizmoType, bool selected)
        {
            var color = selected ? s_HandleColorSelected : s_HandleColor;
            if (!navSurface.enabled)
                color = s_HandleColorDisabled;

            var oldColor = Gizmos.color;
            var oldMatrix = Gizmos.matrix;

            // Use the unscaled matrix for the NavMeshSurface
            var localToWorld =
                Matrix4x4.TRS(
                    navSurface.transform.position,
                    navSurface.transform.rotation,
                    Vector3.one);
            Gizmos.matrix = localToWorld;

            if (navSurface.collectObjects == CollectObjects.Volume)
            {
                Gizmos.color = color;
                Gizmos.DrawWireCube(navSurface.center, navSurface.size);

                if (selected && navSurface.enabled)
                {
                    var colorTrans =
                        new Color(
                            color.r * 0.75f,
                            color.g * 0.75f,
                            color.b * 0.75f,
                            color.a * 0.15f);
                    Gizmos.color = colorTrans;
                    Gizmos.DrawCube(navSurface.center, navSurface.size);
                }
            }
            else if (navSurface.navMeshData != null)
            {
                var bounds = navSurface.navMeshData.sourceBounds;
                Gizmos.color = Color.grey;
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        
            Gizmos.matrix = oldMatrix;
            Gizmos.color = oldColor;

            Gizmos.DrawIcon(
                navSurface.transform.position,
                "NavMeshSurface Icon",
                true);
        }

        private void OnSceneGUI()
        {
            if (!editingCollider)
                return;

            var navSurface = (NavMeshSurface)target;
            var color = navSurface.enabled ? s_HandleColor : s_HandleColorDisabled;
            var localToWorld =
                Matrix4x4.TRS(
                    navSurface.transform.position,
                    navSurface.transform.rotation,
                    Vector3.one);
            using (new Handles.DrawingScope(color, localToWorld))
            {
                m_BoundsHandle.center = navSurface.center;
                m_BoundsHandle.size = navSurface.size;

                EditorGUI.BeginChangeCheck();
                m_BoundsHandle.DrawHandle();
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(navSurface, "Modified NavMesh Surface");
                    var center = m_BoundsHandle.center;
                    var size = m_BoundsHandle.size;
                    navSurface.center = center;
                    navSurface.size = size;
                    EditorUtility.SetDirty(target);
                }
            }
        }

        [MenuItem("GameObject/AI/NavMesh Surface", false, 2000)]
        public static void CreateNavMeshSurface(MenuCommand menuCommand)
        {
            var parent = menuCommand.context as GameObject;
            var go =
                NavMeshComponentsGUIUtility.CreateAndSelectGameObject(
                    "NavMesh Surface", parent);
            go.AddComponent<NavMeshSurface>();
            var view = SceneView.lastActiveSceneView;
            if (view != null)
                view.MoveToView(go.transform);
        }
    }
}
