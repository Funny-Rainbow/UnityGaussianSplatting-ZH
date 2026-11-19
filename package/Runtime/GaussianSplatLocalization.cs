// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System;
using System.Reflection;
#endif

namespace GaussianSplatting.Runtime
{
#if UNITY_6000_5_OR_NEWER
    [Unity.Scripting.LifecycleManagement.NoAutoStaticsCleanup]
#endif
    public static class GaussianSplatLocalization
    {
#if UNITY_EDITOR
        // Unity exposes its editor language through an internal type in 2022.3 and Unity 6.
        static readonly Func<SystemLanguage> s_GetLanguage = EditorGetter<SystemLanguage>("currentEditorLanguage");
        static readonly Func<bool> s_GetEnabled = EditorGetter<bool>("enableEditorLocalization");

        static Func<T> EditorGetter<T>(string property)
        {
            var type = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.LocalizationDatabase");
            var getter = type?.GetProperty(property, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.GetGetMethod(true);
            return getter == null ? null : (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), getter);
        }
#endif

        public static bool isChinese
        {
            get
            {
#if UNITY_EDITOR
                if (s_GetLanguage != null && s_GetEnabled != null && s_GetEnabled())
                {
                    var language = s_GetLanguage();
                    return language == SystemLanguage.Chinese || language == SystemLanguage.ChineseSimplified ||
                           language == SystemLanguage.ChineseTraditional;
                }
#endif
                return false;
            }
        }

        public static string Text(string english) => isChinese && s_Chinese.TryGetValue(english, out var chinese) ? chinese : english;

        public static string Format(string english, params object[] args) => string.Format(Text(english), args);

        static readonly Dictionary<string, string> s_Chinese = new()
        {
            { "Color", "颜色" },
            { "Src Blend", "源混合因子" },
            { "Dst Blend", "目标混合因子" },
            { "ZWrite", "深度写入" },
            { "Source File", "源文件" },
            { "Gaussian Splat source file is not assigned", "未指定高斯泼溅源文件" },
            { "Source file is missing: ", "源文件不存在：" },
            { "Render Options", "渲染选项" },
            { "Shader resources are not set up", "尚未设置着色器资源" },
            { "Reload", "重新加载" },
            { "Loading...", "正在加载..." },
            { "Splats", "高斯点" },
            { "SH Degree", "球谐阶数" },
            { "Splat Data", "高斯点数据" },
            { "Cameras", "相机" },
            { "Camera", "相机" },
            { "Splat Scale", "高斯点缩放" },
            { "Opacity Scale", "不透明度缩放" },
            { "SH Order", "球谐阶数" },
            { "Source Path", "源路径" },
            { "Script", "脚本" },
            { "Options", "选项" },
            { "Transparency", "透明度" },
            { "Temporal Filter", "时间滤波" },
            { "Debugging Tweaks", "调试调整" },
            { "Render Mode", "渲染模式" },
            { "Point Display Size", "点显示大小" },
            { "SH Only", "仅显示球谐贡献" },
            { "Sorted Blended", "排序混合" },
            { "Stochastic", "随机" },
            { "Debug Points", "调试点" },
            { "None", "无" },
            { "Simple Accumulation", "简单累积" },
            { "Reprojected", "重投影" },
            { "Path relative to StreamingAssets or current directory, an absolute path, or a URL", "相对于 StreamingAssets 或当前目录的路径、绝对路径或 URL" },
            { "Additional scaling factor for the splats", "高斯点的额外缩放系数" },
            { "Additional scaling factor for opacity", "不透明度的额外缩放系数" },
            { "Spherical Harmonics order to use", "使用的球谐阶数" },
            { "Gaussian splat transparency rendering algorithm", "高斯泼溅的透明度渲染算法" },
            { "Stochastic temporal filtering", "随机渲染的时间滤波" },
            { "Show only Spherical Harmonics contribution, using gray color", "使用灰色，仅显示球谐贡献" },
            { "Select splat file", "选择高斯泼溅文件" },
            { "Gaussian splats", "高斯泼溅" },
            { "Choose a splat file", "选择高斯泼溅文件" },
            { "Choose a PLY/SPZ file", "选择 PLY/SPZ 文件" },
            { "OK", "确定" },
            { "Captured {0}", "已截取 {0}" },
            { "Validating Gaussian splat rendering", "正在验证高斯泼溅渲染" },
            { "Validation needs an active GaussianSplatRenderer and a main camera", "验证需要一个处于活动状态的 GaussianSplatRenderer 和一台主相机" },
            { "Did not find validation source Assets/StreamingAssets/{0}", "未找到验证源文件 Assets/StreamingAssets/{0}" },
            { "Could not load validation item {0}: {1}", "无法加载验证项 {0}：{1}" },
            { "Missing splat data or camera {0} for validation item {1}", "验证项 {1} 缺少高斯点数据或相机 {0}" },
            { "Did not find reference image for validation item {0} at {1}", "未在 {1} 找到验证项 {0} 的参考图像" },
            { "{0} {1} cam {2}: RMSE {3:F2} PSNR {4:F2} diff pixels {5:N0}", "{0} {1} 相机 {2}：RMSE {3:F2} PSNR {4:F2} 差异像素数 {5:N0}" },
            { "{0} bytes/splat ({1:N1} MiB)", "{0} 字节/高斯点（{1:N1} MiB）" },
            { "Can't render '{0}': view preparation supports at most {1:N0} splats per object", "无法渲染“{0}”：视图准备阶段每个对象最多支持 {1:N0} 个高斯点" },
            { "Cannot render all {0:N0} splats: shared view buffer needs {1:N0} bytes (max graphics buffer size {2:N0})", "无法渲染全部 {0:N0} 个高斯点：共享视图缓冲区需要 {1:N0} 字节（图形缓冲区最大大小为 {2:N0}）" },
            { "Cannot sort all {0:N0} splats: we support at most {1:N0} splats", "无法对全部 {0:N0} 个高斯点排序：最多支持 {1:N0} 个高斯点" },
            { "Shader resources are missing or compute shaders are unsupported", "缺少着色器资源或不支持计算着色器" },
            { "Cannot load splats '{0}': {1}", "无法加载高斯泼溅“{0}”：{1}" },
            { "Choose a PLY/SPZ file path or URL", "选择 PLY/SPZ 文件路径或 URL" },
            { "Invalid source URL", "源 URL 无效" },
            { "Source must be a .ply or .spz file", "源文件必须为 .ply 或 .spz 文件" },
            { "Expected a 3DGS camera array", "应为 3DGS 相机数组" },
            { "Camera {0} needs a position and 3x3 rotation", "相机 {0} 需要位置和 3x3 旋转矩阵" },
            { "Splat count must be between 1 and 16,000,000", "高斯点数量必须介于 1 和 16,000,000 之间" },
            { "Only SH degrees 0 through 3 are supported", "仅支持 0 到 3 阶球谐" },
            { "Could not read SPZ compressed data", "无法读取 SPZ 压缩数据" },
            { "Could not decompress SPZ stream {0}: {1}", "无法解压 SPZ 数据流 {0}：{1}" },
            { "Could not load optional cameras '{0}': {1}", "无法加载可选相机文件“{0}”：{1}" },
            { "SPZ compression does not match its version", "SPZ 压缩方式与其版本不匹配" },
            { "Expected binary PLY or SPZ (versions 1–4)", "应为二进制 PLY 或 SPZ（版本 1–4）" },
            { "Truncated SPZ compressed data", "SPZ 压缩数据被截断" },
            { "Truncated SPZ data", "SPZ 数据被截断" },
            { "Unexpected SPZ trailing data", "SPZ 尾部存在意外数据" },
            { "Truncated PLY data", "PLY 数据被截断" },
            { "Could not read PLY vertex data", "无法读取 PLY 顶点数据" },
            { "Invalid or unrepresentable values at splat {0}", "高斯点 {0} 存在无效或无法表示的值" },
            { "Incomplete PLY header", "PLY 文件头不完整" },
            { "PLY header line too long", "PLY 文件头行过长" },
            { "Invalid PLY magic", "PLY 魔数无效" },
            { "Invalid PLY element", "PLY 元素无效" },
            { "Negative PLY element count", "PLY 元素数量为负数" },
            { "Duplicate vertex element", "顶点元素重复" },
            { "PLY vertices must be the first nonempty element", "PLY 顶点必须是第一个非空元素" },
            { "Unsupported PLY vertex property", "不支持的 PLY 顶点属性" },
            { "Duplicate PLY property: ", "PLY 属性重复：" },
            { "Expected binary_little_endian 1.0 PLY with vertices", "应为包含顶点的 binary_little_endian 1.0 格式 PLY" },
            { "Missing float Gaussian PLY property: ", "缺少 float 类型的高斯 PLY 属性：" },
            { "PLY SH must contain 0, 9, 24 or 45 properties", "PLY 球谐必须包含 0、9、24 或 45 个属性" },
            { "Truncated PLY vertex data", "PLY 顶点数据被截断" },
            { "Invalid SPZ magic", "SPZ 魔数无效" },
            { "SPZ version {0} is not supported (supported: 1–4)", "不支持 SPZ 版本 {0}（支持的版本：1–4）" },
            { "Invalid SPZ fractional bits", "SPZ 小数位数无效" },
            { "Unsupported SPZ flags: 0x{0:X2}", "不支持的 SPZ 标志：0x{0:X2}" },
            { "This SPZ was trained with antialiasing; rendering will not be fully correct", "此 SPZ 使用抗锯齿训练；渲染结果将不完全正确" },
            { "Nonzero SPZ reserved header bytes", "SPZ 文件头的保留字节非零" },
            { "Invalid SPZ stream count", "SPZ 数据流数量无效" },
            { "Invalid SPZ table offset", "SPZ 表偏移量无效" },
            { "Invalid SPZ stream size", "SPZ 数据流大小无效" },
            { "SPZ compressed data size mismatch", "SPZ 压缩数据大小不匹配" },
            { "InvalidData", "数据无效" },
            { "UnsupportedDictionary", "不支持的字典" },
            { "DestinationTooSmall", "目标缓冲区过小" },
            { "ScratchTooSmall", "临时缓冲区过小" },
            { "Tools/Gaussian Splats/Debug/Capture Screenshot", "Tools/高斯泼溅/调试/截取屏幕截图" },
            { "Tools/Gaussian Splats/Debug/Validate Render against SBIR", "Tools/高斯泼溅/调试/对照 SBIR 验证渲染" },
            { "Tools/Gaussian Splats/Debug/Validate Render against Full Precision Metal", "Tools/高斯泼溅/调试/对照全精度 Metal 验证渲染" },
        };
    }
}
