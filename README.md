<div align="center">

# Decimater For Unity

<em><h5 align="center">(using Unity 2022.3.22f1)</h5></em>

![git](https://github.com/user-attachments/assets/4ce84a66-0117-4b19-8761-925cd3c5088f)

| **English** | [日本語](./README.jp.md) |

This plugin enables a functionality similar to Blender's decimate feature within Unity.

decimate functions are created by using **UnityMeshSimplifier**[[1]][UnityMeshSimplifier_github]

<div align="left">

### Installation
---

**How to Install**

1.  Download the file from [here][download_link] and extract it into your Assets folder.
  
2.  Next, download the latest release file (Unitypackage) from [here][download_link2] and import it to complete the setup.

**File Tree Structure**

tree have to looks like this

```shell
MeshDecimater_Unity
│  .gitignore
│  LICENSE
│  README.jp.md
│  README.md
│
├─.github
│  └─ISSUE_TEMPLATE
│          bugreport.yml
│          config.yml
│
├─Editor
│      DecimaterMain.cs
│      DecimaterMain.cs.meta
│      MeshInfoDisplay.cs
│      MeshInfoDisplay.cs.meta
│      MeshPreviewer.cs
│      MeshPreviewer.cs.meta
│      WireframeDrawer.cs
│      WireframeDrawer.cs.meta
│
├─Runtime
│      MeshDecimaterUtility.cs
│      MeshDecimaterUtility.cs.meta
│      MeshUtils.cs
│      MeshUtils.cs.meta
│
└─Shader
        Wireframe.shader
        Wireframe.shader.meta
```

### How to use
---
![13](https://github.com/user-attachments/assets/d4c8bba5-00c2-4a75-9b59-d4514d09990d)


Select an object with a mesh from the Hierarchy or choose it from the ObjectField to use the plugin.

Similar to Blender, adjust the `Decimate Level` and press `Apply Decimation` to execute the decimation.

Clicking `Revert` will restore the original file.

Please note that Revert will not work after clicking on a different object (the original mesh will remain, so you can replace it to restore).

```
work confirmed in

・Unity 2022.3.22f1

・Unity 2019.4.31f1
```

<!-- links -->
  [UnityMeshSimplifier_github]: https://github.com/Whinarn/UnityMeshSimplifier
  [download_link]: https://github.com/Whinarn/UnityMeshSimplifier/releases/tag/v3.1.0
  [download_link2]: https://github.com/refiaa/MeshDecimater_Unity/releases/latest

