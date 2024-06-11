<div align="center">

# Decimater For Unity

<em><h5 align="center">(using Unity 2022.3.22f1)</h5></em>

| **English** | [日本語](./README.jp.md) |

This plugin enables a functionality similar to Blender's decimate feature within Unity.

It is created using **UnityMeshSimplifier**[[1]][UnityMeshSimplifier_github]

<div align="left">

### Installation
---

1.  Download the file from [here][download_link] and extract it into your Assets folder.
  
2.  Next, download the latest release file (Unitypackage) from [here][download_link2] and import it to complete the setup.


### How to use
---

Select an object with a mesh from the Hierarchy or choose it from the ObjectField to use the plugin.

![image](https://github.com/refiaa/MeshDecimater_Unity/assets/112306763/e6651c4f-357f-457a-a3a3-e87d0a38b75e)

Similar to Blender, adjust the `Decimate Level` and press `Apply Decimation` to execute the decimation.

Clicking `Revert` will restore the original file.

Please note that Revert will not work after clicking on a different object (the original mesh will remain, so you can replace it to restore).

<!-- links -->
  [UnityMeshSimplifier_github]: https://github.com/Whinarn/UnityMeshSimplifier
  [download_link]: https://github.com/Whinarn/UnityMeshSimplifier/releases/tag/v3.1.0
  [download_link2]: https://github.com/refiaa/MeshDecimater_Unity/releases/
