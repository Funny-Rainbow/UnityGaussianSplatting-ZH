# Unity 高斯泼溅使用说明

在 Unity 中加载并显示三维高斯泼溅（3D Gaussian Splatting，简称 3DGS）模型。本说明适用于当前 **2.0.0** 版本，包含插件导入、渲染管线配置、模型加载和功能列表。英文菜单、组件名和字段名保留 Unity 界面中的写法，方便查找。

![高斯泼溅场景预览](docs/Images/shotOverview.jpg)

## 功能列表

| 功能 | 使用方式与说明 |
| --- | --- |
| 直接加载模型 | 直接读取 `.ply`、`.spz`，无需预先转换为专用高斯资产。 |
| 多种数据来源 | 支持相对于 `StreamingAssets` 的路径、相对于当前工作目录的路径、绝对路径和模型文件网址。 |
| SPZ 格式支持 | 支持 SPZ 1～4，包括使用 Zstandard 压缩的 SPZ 4；支持 0～3 阶球谐数据。 |
| 三种渲染管线 | 支持内置渲染管线、通用渲染管线（URP）和高清渲染管线（HDRP）。 |
| 多对象全局排序 | 在排序混合模式下，对多个高斯对象统一排序，正确处理它们相互交叠时的前后关系。 |
| 两种透明渲染方式 | 默认使用排序混合；也可选择无需排序的随机透明渲染。 |
| 时域降噪 | 随机透明模式支持无滤波、简单累积和重投影滤波。 |
| 场景统一设置 | 通过 `GaussianSplatRenderingOptions` 统一调整透明渲染、时域滤波和调试显示。 |
| 单个对象调整 | 支持位置、旋转、缩放，以及高斯点大小、不透明度和球谐阶数调整。 |
| 场景与相机预览 | 无需进入运行模式即可预览；支持透视和正交相机，也可读取配套相机文件切换视角。 |
| 深度遮挡 | 可与普通不透明物体共同显示，并根据深度进行遮挡。 |
| 调试显示 | 支持点模式和仅显示球谐贡献，方便检查模型。 |

## 环境要求

| 项目 | 要求 |
| --- | --- |
| 内置渲染管线 | Unity 2022.3 或兼容的较新版本；仓库示例使用 2022.3.62f3。 |
| URP / HDRP | Unity 6 或更新版本；仓库示例使用 6000.0.81f1。 |
| Windows | 使用 Direct3D 12 或 Vulkan，**不支持 Direct3D 11**。 |
| macOS | 使用 Metal。 |
| Linux | 使用 Vulkan。 |
| 显卡 | 需要支持计算着色器，并具备足够的显存容纳模型与渲染缓冲区。 |

推荐先在桌面平台验证。OpenGL、OpenGL ES 和 MSAA 抗锯齿不受支持；移动端、网页端和各类 XR 设备的兼容性需单独验证。

## 导入 Unity

### 方式一：导入已有项目

1. 打开 Unity 项目，进入包管理器（`Window > Package Manager`；部分版本位于 `Window > Package Management > Package Manager`）。
2. 点击左上角的 `+`，选择从 Git 网址添加包（`Add package from git URL...` 或 `Install package from git URL...`）。
3. 输入本仓库 `ZH` 分支中的包路径：

   ```text
   https://github.com/Funny-Rainbow/UnityGaussianSplatting-ZH.git?path=/package#ZH
   ```

4. 等待安装与脚本编译完成。包管理器会按包声明安装 Burst、Collections 和 Mathematics 依赖。
5. 按下文配置项目使用的渲染管线，然后加载模型。

使用 Git 网址安装需要本机已安装 Git，并且 Unity 可以访问 GitHub。

也可以先下载或克隆仓库，在包管理器的 `+` 菜单中选择从磁盘安装（`Add package from disk...` 或 `Install package from disk...`），选中仓库内的 **`package/package.json`**。使用这种方式时，请保留本地 `package` 目录。

### 方式二：打开示例项目

1. 克隆 `ZH` 分支，或在 GitHub 切换到该分支后，通过 `Code > Download ZIP` 下载并解压。

   ```bash
   git clone --branch ZH https://github.com/Funny-Rainbow/UnityGaussianSplatting-ZH.git
   ```

2. 在 Unity Hub 中添加下列某一个项目目录，并使用对应版本的编辑器打开：

   | 渲染管线 | 项目目录 | 编辑器版本 |
   | --- | --- | --- |
   | 内置渲染管线 | `projects/GaussianExample` | Unity 2022.3 |
   | URP | `projects/GaussianExample-URP` | Unity 6 |
   | HDRP | `projects/GaussianExample-HDRP` | Unity 6 |

3. 保留仓库目录结构。示例通过相对路径引用根目录中的 `package`，不能只复制示例项目文件夹。
4. 等待导入完成，打开项目内的 `Assets/GSTestScene.unity`。
5. 自行准备模型文件，并按“加载与使用模型”一节替换场景中渲染组件的模型路径。

## 配置渲染管线

### Windows 图形接口

打开 `Edit > Project Settings > Player > Other Settings > Rendering`，在 Windows 的图形接口设置中关闭自动选择，将 **Direct3D12** 或 **Vulkan** 设为首选；按编辑器提示重启。不要使用 Direct3D11。

### 内置渲染管线

无需添加额外渲染通道，在场景中添加 `GaussianSplatRenderer` 即可。

### URP 通用渲染管线

1. 确认项目使用 Unity 6 或更新版本。
2. 找到当前相机实际使用的 URP Renderer Data 资源，在 Inspector 的 `Add Renderer Feature` 中添加 **`GaussianSplatURPFeature`**。
3. 在项目的 URP / Render Graph 设置中关闭 **`Compatibility Mode`**，启用渲染图路径。
4. 关闭 MSAA。使用 OpenXR 时，将立体渲染模式设为 **`Multi-Pass`**。

### HDRP 高清渲染管线

1. 确认项目使用 Unity 6 或更新版本。
2. 在场景中创建带有 **`Custom Pass Volume`** 组件的对象；需要全场景生效时启用 `Is Global`。
3. 在自定义通道列表中添加 **`GaussianSplatHDRPPass`**。
4. 注入点可选择透明物体之前（`Before Transparent`）或后处理之后（`After Post Process`）。遇到自动曝光导致的亮度异常时，优先尝试后处理之后。
5. 确保相机的帧设置允许自定义通道，并关闭 MSAA。

## 加载与使用模型

### 准备模型文件

插件负责显示已有模型，不包含训练流程。请准备以下任意一种文件：

- **PLY**：包含三维高斯属性的 `binary_little_endian 1.0` 二进制 PLY，相关高斯属性使用 `float`。普通点云 PLY 或 ASCII PLY 不能直接作为高斯模型使用。原始 3DGS 模型通常使用 `point_cloud/iteration_*/point_cloud.ply`。
- **SPZ**：支持 1～4 版和 0～3 阶球谐数据。带抗锯齿标记的文件会产生警告，显示可能不完全正确；暂不支持 SPZ 扩展。

### 在场景中显示模型

1. 在 Unity 项目的 `Assets` 下创建 `StreamingAssets` 文件夹，把模型放入其中，例如：

   ```text
   Assets/
   └── StreamingAssets/
       └── bicycle.ply
   ```

2. 在场景中创建一个空对象，通过 `Add Component` 添加 **`GaussianSplatRenderer`**。
3. 将模型文件拖到组件的 **`PLY / SPZ`** 字段，或点击旁边的文件夹按钮选择文件。也可以直接填入 `bicycle.ply`。
4. 等待 `Loading...` 结束；加载成功后，Inspector 会显示高斯点数量和球谐阶数。加载失败时，查看组件提示和 Console。
5. 选中模型对象，在场景视图按 `F` 聚焦，用对象的 Transform 调整位置、旋转和整体缩放，再把相机移到合适的位置。
6. 在场景视图或游戏视图检查效果。源文件更新后，点击组件的 **`Reload`** 重新加载。

需要放置多个模型时，为每个对象分别添加一个 `GaussianSplatRenderer` 并指定文件。

### 调整单个模型

| Inspector 字段 | 作用 |
| --- | --- |
| `PLY / SPZ` | 指定模型文件路径或网址。 |
| `Splat Scale` | 调整每个高斯点的大小；整个模型的尺寸请使用 Transform 的缩放。 |
| `Opacity Scale` | 调整不透明度。 |
| `SH Order` | 设置用于显示的球谐阶数（0～3），实际效果取决于源文件包含的数据。 |
| `Reload` | 重新读取当前模型文件。 |

### 使用配套相机视角

可在模型旁放置同名 JSON 文件，例如 `garden.ply` 配 `garden.json`。JSON 需使用原始 3DGS 的相机描述格式。加载成功后，组件中的 **`Cameras`** 滑块可将带有 `MainCamera` 标签的主相机切换到对应视角，视角相对于高斯对象的变换计算。

## 设置全局渲染效果

在场景中的一个对象上添加 **`GaussianSplatRenderingOptions`**，统一设置场景内所有高斯对象。未添加该组件时使用默认设置。

| 设置 | 可选值与用途 |
| --- | --- |
| 透明渲染方式（`Transparency`） | **`Sorted Blended`**：默认方式，对高斯点全局排序后进行透明混合。**`Stochastic`**：随机透明测试并写入深度，无需排序，单帧会有明显噪点。 |
| 时域滤波（`Temporal Filter`） | 仅在随机透明模式下生效。**`None`**：关闭滤波；**`Simple Accumulation`**：累积多帧，适合静止视角；**`Reprojected`**：使用重投影滤波，也是默认滤波选项。 |
| 渲染模式（`Render Mode`） | **`Splats`**：正常显示高斯；**`Debug Points`**：以点显示，便于检查分布。 |
| 点显示大小（`Point Display Size`） | 调整调试点模式下的点大小。 |
| 仅显示球谐贡献（`SH Only`） | 使用灰色基底检查球谐数据带来的视角相关变化。 |

首次使用建议保持 `Sorted Blended`。尝试 `Stochastic` 时同时启用时域滤波，观察连续多帧后的效果。

## 路径与打包

`PLY / SPZ` 字段支持以下来源：

| 来源 | 示例 | 说明 |
| --- | --- | --- |
| 相对于 `StreamingAssets` | `bicycle.ply` 或 `models/garden.spz` | 推荐用于随应用发布的模型。 |
| 相对于当前工作目录 | `data/garden.ply` | 优先查找 `StreamingAssets` 中的同路径文件，找不到时再按当前工作目录解析。 |
| 绝对路径 | `D:/Models/garden.ply` | 适合本机调试；换设备后需确保对应文件存在。 |
| 网络地址 | `https://example.com/models/garden.spz` | 填写可直接下载模型的地址，网址路径需保留 `.ply` 或 `.spz` 后缀。 |

`Assets/StreamingAssets` 中的文件会随 Unity 构建一起打包。放在其他位置的模型不会因为组件引用了路径就自动进入构建，需要自行部署文件或通过网络加载。构建后也应确认目标平台使用受支持的图形接口。

## 常见问题

| 问题 | 检查方法 |
| --- | --- |
| 找不到渲染组件 | 确认包已安装完成，Console 没有脚本编译错误；组件名为 `GaussianSplatRenderer`。 |
| 模型没有显示 | 检查路径和加载错误、对象是否启用、相机位置及裁剪范围；Windows 确认使用 DX12 / Vulkan，URP / HDRP 确认已添加对应渲染通道。 |
| 编辑器能显示，打包后找不到文件 | 把模型放入 `Assets/StreamingAssets` 并使用相对路径，或在目标设备上部署外部文件。 |
| 随机透明模式噪点明显 | 启用 `Simple Accumulation` 或 `Reprojected`，等待多帧累积；需要稳定的默认效果时选择 `Sorted Blended`。 |
| 与场景中的透明物体混合异常 | 普通半透明物体与高斯对象之间仍有混合限制；多个高斯对象的全局排序不能解决普通透明材质的排序问题。 |
| 灯光或阴影不起作用 | 高斯模型使用自身颜色和球谐数据，不受 Unity 灯光、阴影、反射探针或光照贴图影响。 |
| 旧教程中的资产创建或编辑入口消失 | 2.0.0 已改为直接加载 PLY/SPZ，并移除了高斯点编辑、合并、裁剪及编辑结果导出工具；需要编辑时，可先使用外部高斯编辑器处理模型。 |
| 显示或抗锯齿异常 | 关闭 MSAA；HDRP 还可尝试调整自定义通道注入点。 |

以上直接加载、SPZ 4、全局排序、场景统一设置和随机透明渲染功能，可参阅[上游 2.0.0 版本说明](https://github.com/aras-p/UnityGaussianSplatting/releases/tag/v2.0.0)。

**中文说明贡献者：** [Funny-Rainbow](https://github.com/Funny-Rainbow) · **代码许可证：** [MIT](LICENSE.md)
