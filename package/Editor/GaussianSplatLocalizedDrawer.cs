// SPDX-License-Identifier: MIT

using UnityEditor;
using UnityEngine;
using static GaussianSplatting.Runtime.GaussianSplatLocalization;

namespace GaussianSplatting.Editor
{
    public class GaussianSplatLocalizedDrawer : MaterialPropertyDrawer
    {
        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            editor.DefaultShaderProperty(position, prop, Text(label));
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return MaterialEditor.GetDefaultPropertyHeight(prop);
        }
    }
}
