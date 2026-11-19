// SPDX-License-Identifier: MIT

using GaussianSplatting.Runtime;
using UnityEditor;
using UnityEngine;
using RenderMode = GaussianSplatting.Runtime.RenderMode;
using static GaussianSplatting.Runtime.GaussianSplatLocalization;

namespace GaussianSplatting.Editor
{
    [CustomEditor(typeof(GaussianSplatRenderingOptions))]
    [CanEditMultipleObjects]
    public class GaussianSplatRenderingOptionsEditor : UnityEditor.Editor
    {
        SerializedProperty m_PropScript;
        SerializedProperty m_PropTransparency;
        SerializedProperty m_PropTemporalFilter;
        SerializedProperty m_PropRenderMode;
        SerializedProperty m_PropPointSize;
        SerializedProperty m_PropSHOnly;

        void OnEnable()
        {
            m_PropScript = serializedObject.FindProperty("m_Script");
            m_PropTransparency = serializedObject.FindProperty("m_Transparency");
            m_PropTemporalFilter = serializedObject.FindProperty("m_TemporalFilter");
            m_PropRenderMode = serializedObject.FindProperty("m_RenderMode");
            m_PropPointSize = serializedObject.FindProperty("m_PointDisplaySize");
            m_PropSHOnly = serializedObject.FindProperty("m_SHOnly");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            using (new EditorGUI.DisabledScope(true))
                GaussianSplatEditorLocalization.PropertyField(m_PropScript);
            EditorGUILayout.Space();
            GUILayout.Label(Text("Options"), EditorStyles.boldLabel);
            GaussianSplatEditorLocalization.PropertyField(m_PropTransparency);
            if (!m_PropTransparency.hasMultipleDifferentValues && m_PropTransparency.intValue == (int)TransparencyMode.Stochastic)
                GaussianSplatEditorLocalization.PropertyField(m_PropTemporalFilter);
            EditorGUILayout.Space();
            GUILayout.Label(Text("Debugging Tweaks"), EditorStyles.boldLabel);
            GaussianSplatEditorLocalization.PropertyField(m_PropRenderMode);
            if (!m_PropRenderMode.hasMultipleDifferentValues && m_PropRenderMode.intValue == (int)RenderMode.DebugPoints)
                GaussianSplatEditorLocalization.PropertyField(m_PropPointSize);
            GaussianSplatEditorLocalization.PropertyField(m_PropSHOnly);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
