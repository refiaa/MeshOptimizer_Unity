<div align="center">

# Decimater For Unity

<em><h5 align="center">(using Unity 2022.3.22f1)</h5></em>

![img](https://github.com/refiaa/MeshDecimater_Unity/assets/112306763/e4747f15-c537-4d83-a6d1-5e69100c244c)

| **English** | [日本語](./README.jp.md) |

This plugin enables a functionality similar to Blender's decimate feature within Unity.

decimate functions are created by using **UnityMeshSimplifier**[[1]][UnityMeshSimplifier_github]

<div align="left">

## Compatibility Issue with VRCSDK3

Currently, there is a significant compatibility issue between this Decimater tool and VRCSDK3-AVATAR. The following symptoms have been observed:

1. **Abnormal Behavior During Upload**:
   - When attempting to upload an avatar with Decimater imported into the project, VRCSDK exhibits unexpected behavior.
   - Specifically, instead of referencing the prefab in the Hierarchy, VRCSDK generates a new Prefab in the Assets folder named after the Blueprint ID, and attempts to use this for the upload.

2. **Error Occurrence**:
   - As a result of the above behavior, a `FileNotFoundException` occurs, causing the upload to fail.

### Problem Details

- This issue occurs simply by importing the Decimater tool into the project.
- The problem is reproducible even without executing any Decimating operations.
- While the cause is believed to be related to interactions with VRCSDK, addressing the issue is challenging due to inability to access the internal implementations of VRCSDK's `VRCAvatarBuilder` class and the `VRC.SDK3.Builder.VRCAvatarBuilder.ExportCurrentAvatarResource` method.


### Installation
---

**How to Install**

1.  Download the file from [here][download_link] and extract it into your Assets folder.
  
2.  Next, download the latest release file (Unitypackage) from [here][download_link2] and import it to complete the setup.

**File Tree Structure**

tree have to looks like this

```shell
Assets
├─MeshDecimater_Unity
│  ├─Material
│  ├─Shader
│  └─src
└─UnityMeshSimplifier
    ├─.circleci
    │  ├─ProjectSettings
    │  └─scripts
    ├─.github
    │  ├─ISSUE_TEMPLATE
    │  └─workflows
    ├─Editor
    ├─Runtime
    │  ├─Components
    │  ├─Exceptions
    │  ├─Internal
    │  ├─Math
    │  └─Utility
    └─Tests
        └─Editor
```

### How to use
---
![src](https://github.com/refiaa/MeshDecimater_Unity/assets/112306763/1830fee5-2ae0-49d0-bac4-929a3e42ab4a)

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

