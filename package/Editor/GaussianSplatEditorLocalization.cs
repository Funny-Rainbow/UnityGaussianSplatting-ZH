// SPDX-License-Identifier: MIT

using System;
using System.Reflection;
using GaussianSplatting.Runtime;
using UnityEditor;
using UnityEngine;
using static GaussianSplatting.Runtime.GaussianSplatLocalization;

namespace GaussianSplatting.Editor
{
#if UNITY_6000_5_OR_NEWER
    [Unity.Scripting.LifecycleManagement.NoAutoStaticsCleanup]
#endif
    [InitializeOnLoad]
    static class GaussianSplatEditorLocalization
    {
        const BindingFlags kStaticFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        static readonly MethodInfo s_AddMenuItem = typeof(Menu).GetMethod("AddMenuItem", kStaticFlags);
        static readonly MethodInfo s_RemoveMenuItem = typeof(Menu).GetMethod("RemoveMenuItem", kStaticFlags);
        static readonly string[] s_MenuPaths =
        {
            "Tools/Gaussian Splats/Debug/Capture Screenshot",
            "Tools/Gaussian Splats/Debug/Validate Render against SBIR",
            "Tools/Gaussian Splats/Debug/Validate Render against Full Precision Metal",
        };
        static readonly Action[] s_MenuActions =
        {
            Utils.CaptureScreenshot.CaptureShot,
            GaussianSplatValidator.ValidateSBIR,
            GaussianSplatValidator.ValidateFullPrecMetal,
        };
        static string[] s_RegisteredPaths;
        static bool? s_WasChinese;

        static GaussianSplatEditorLocalization()
        {
            // Attribute-based English menus remain available if Unity changes its internal menu API.
            EditorApplication.delayCall += UpdateMenus;
            EditorApplication.update += CheckLanguage;
            AssemblyReloadEvents.beforeAssemblyReload += RemoveMenus;
        }

        static void CheckLanguage()
        {
            if (s_WasChinese != isChinese)
                UpdateMenus();
        }

        static void UpdateMenus()
        {
            if (s_AddMenuItem == null || s_RemoveMenuItem == null)
                return;
            RemoveMenus();
            s_RegisteredPaths = new string[s_MenuPaths.Length];
            for (int i = 0; i < s_MenuPaths.Length; ++i)
            {
                s_RemoveMenuItem.Invoke(null, new object[] { s_MenuPaths[i] });
                string path = Text(s_MenuPaths[i]);
                s_AddMenuItem.Invoke(null, new object[] { path, i == 0 ? "%g" : "", false, 1000, s_MenuActions[i], null });
                s_RegisteredPaths[i] = path;
            }
            s_WasChinese = isChinese;
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        static void RemoveMenus()
        {
            if (s_RegisteredPaths == null || s_RemoveMenuItem == null)
                return;
            foreach (string path in s_RegisteredPaths)
                s_RemoveMenuItem.Invoke(null, new object[] { path });
            s_RegisteredPaths = null;
        }

        public static GUIContent PropertyLabel(SerializedProperty property)
        {
            var field = property.serializedObject.targetObject.GetType().GetField(property.name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            string tooltip = field?.GetCustomAttribute<TooltipAttribute>()?.tooltip;
            return new GUIContent(Text(property.displayName), string.IsNullOrEmpty(tooltip) ? "" : Text(tooltip));
        }

        public static void PropertyField(SerializedProperty property)
        {
            var label = PropertyLabel(property);
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorGUILayout.PropertyField(property, label);
                return;
            }

            var names = property.enumDisplayNames;
            var choices = new GUIContent[names.Length];
            for (int i = 0; i < names.Length; ++i)
                choices[i] = new GUIContent(Text(names[i]));
            Rect rect = EditorGUILayout.GetControlRect();
            EditorGUI.BeginProperty(rect, label, property);
            bool wasMixed = EditorGUI.showMixedValue;
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            int selected = EditorGUI.Popup(rect, label, property.enumValueIndex, choices);
            if (EditorGUI.EndChangeCheck())
                property.enumValueIndex = selected;
            EditorGUI.showMixedValue = wasMixed;
            EditorGUI.EndProperty();
        }
    }
}
