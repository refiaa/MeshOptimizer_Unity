using UnityEngine;
using UnityEditor;

namespace CustomDecimater
{
    public class DecimaterMain
    {
        [MenuItem("Tools/Activate Decimater")]
        public static void ActivateDecimater()
        {
            DecimaterWindow.ShowWindow();
        }
    }

    public class DecimaterWindow : EditorWindow
    {
        private GameObject selectedGameObject;
        private Mesh originalMesh;
        private Mesh decimatedMesh;
        private MeshPreviewer meshPreviewer;
        private MeshInfoDisplay meshInfoDisplay;
        private float decimateLevel = 1.0f;

        private Material previewMaterial;
        private Material wireframeMaterial;
        private Shader previewShader;
        private Shader wireframeShader;

        private Mesh tempDecimatedMesh;
        private string tempAssetPath;

        public static void ShowWindow()
        {
            GetWindow<DecimaterWindow>("Decimater for Unity");
        }

        private void OnEnable()
        {
            LoadShaders();
            previewMaterial = CreatePreviewMaterial();
            wireframeMaterial = CreateWireframeMaterial();

            meshPreviewer = new MeshPreviewer(previewMaterial, wireframeMaterial);
            meshInfoDisplay = new MeshInfoDisplay();
        }

        private void LoadShaders()
        {
            previewShader = Shader.Find("Standard");
            wireframeShader = Shader.Find("Refiaa/Wireframe");
        }

        private void OnSelectionChange()
        {
            UpdateSelection(Selection.activeGameObject);
        }

        private void OnGUI()
        {
            GUILayout.Label("Select a GameObject", EditorStyles.boldLabel);

            GameObject newSelectedGameObject = (GameObject)EditorGUILayout.ObjectField("GameObject", selectedGameObject, typeof(GameObject), true);
            if (newSelectedGameObject != selectedGameObject)
            {
                UpdateSelection(newSelectedGameObject);
            }

            if (selectedGameObject == null)
            {
                EditorGUILayout.HelpBox("No GameObject selected. Please select a GameObject with a MeshFilter or SkinnedMeshRenderer component.", MessageType.Warning);
                return;
            }

            GUILayout.Space(10);
            GUILayout.Label("Mesh Preview", EditorStyles.boldLabel);
            Rect previewRect = GUILayoutUtility.GetRect(400, 400);
            meshPreviewer.PreviewMesh(selectedGameObject, previewRect);

            GUILayout.Space(10);
            GUILayout.Label("Decimate Level", EditorStyles.boldLabel);
            decimateLevel = EditorGUILayout.Slider("Decimate Level", decimateLevel, 0.1f, 1.0f);

            GUILayout.Space(10);
            if (GUILayout.Button("Apply Decimation"))
            {
                SafeApplyDecimation();
            }
            
            if (GUILayout.Button("Revert"))
            {
                RestoreOriginalMesh();
            }

            GUILayout.Space(10);
            meshInfoDisplay.DisplayMeshInfo(GetCurrentMesh());
        }

        private void UpdateSelection(GameObject newSelectedGameObject)
        {
            if (newSelectedGameObject != null)
            {
                MeshFilter meshFilter = newSelectedGameObject.GetComponent<MeshFilter>();
                SkinnedMeshRenderer skinnedMeshRenderer = newSelectedGameObject.GetComponent<SkinnedMeshRenderer>();

                if (meshFilter != null || skinnedMeshRenderer != null)
                {
                    selectedGameObject = newSelectedGameObject;
                    originalMesh = meshFilter != null ? meshFilter.sharedMesh : skinnedMeshRenderer.sharedMesh;
                    EnableReadWrite(originalMesh);
                    decimatedMesh = Object.Instantiate(originalMesh);
                    meshInfoDisplay.SetOriginalMesh(originalMesh);
                    Selection.activeObject = originalMesh;
                    Debug.Log($"Selected Mesh: {AssetDatabase.GetAssetPath(originalMesh)}");
                    Repaint();
                }
            }
        }

        private void SafeApplyDecimation()
        {
            tempDecimatedMesh = Object.Instantiate(originalMesh);
            EnableReadWrite(tempDecimatedMesh);

            MeshDecimaterUtility.DecimateMesh(originalMesh, tempDecimatedMesh, decimateLevel, IsSkinnedMesh());

            tempAssetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/TempDecimatedMesh.asset");
            AssetDatabase.CreateAsset(tempDecimatedMesh, tempAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ReplaceMeshWithDecimated();

            meshPreviewer.UpdatePreviewMesh(selectedGameObject);
        }

        private void ReplaceMeshWithDecimated()
        {
            if (IsSkinnedMesh())
            {
                SkinnedMeshRenderer renderer = selectedGameObject.GetComponent<SkinnedMeshRenderer>();
                renderer.sharedMesh = tempDecimatedMesh;
            }
            else
            {
                MeshFilter filter = selectedGameObject.GetComponent<MeshFilter>();
                filter.sharedMesh = tempDecimatedMesh;
            }
        }

        private void RestoreOriginalMesh()
        {
            if (IsSkinnedMesh())
            {
                SkinnedMeshRenderer renderer = selectedGameObject.GetComponent<SkinnedMeshRenderer>();
                renderer.sharedMesh = originalMesh;
            }
            else
            {
                MeshFilter filter = selectedGameObject.GetComponent<MeshFilter>();
                filter.sharedMesh = originalMesh;
            }

            if (!string.IsNullOrEmpty(tempAssetPath))
            {
                AssetDatabase.DeleteAsset(tempAssetPath);
                tempAssetPath = null;
            }

            tempDecimatedMesh = null;
            AssetDatabase.Refresh();

            decimateLevel = 1.0f;
            meshPreviewer.UpdatePreviewMesh(selectedGameObject);
        }

        private bool IsSkinnedMesh()
        {
            return selectedGameObject.GetComponent<SkinnedMeshRenderer>() != null;
        }

        private Mesh GetCurrentMesh()
        {
            if (selectedGameObject != null)
            {
                if (selectedGameObject.GetComponent<MeshFilter>() != null)
                {
                    return selectedGameObject.GetComponent<MeshFilter>().sharedMesh;
                }
                else if (selectedGameObject.GetComponent<SkinnedMeshRenderer>() != null)
                {
                    return selectedGameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                }
            }
            return null;
        }

        private void EnableReadWrite(Mesh mesh)
        {
            string path = AssetDatabase.GetAssetPath(mesh);
            if (string.IsNullOrEmpty(path)) return;

            ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
            if (modelImporter != null)
            {
                modelImporter.isReadable = true;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }

        private Material CreatePreviewMaterial()
        {
            if (previewShader == null)
            {
                Debug.LogError("Preview shader not found!");
                return null;
            }

            Material material = new Material(previewShader)
            {
                name = "Preview Material"
            };

            material.SetFloat("_Glossiness", 0.0f);
            material.SetFloat("_GlossyReflections", 0.0f);
            material.SetFloat("_Metallic", 0.0f);
            material.SetFloat("_SpecularHighlights", 0.0f);
            material.SetColor("_Color", Color.white);
            material.SetColor("_SpecColor", Color.white);

            return material;
        }

        private Material CreateWireframeMaterial()
        {
            if (wireframeShader == null)
            {
                Debug.LogError("Wireframe shader not found!");
                return null;
            }

            return new Material(wireframeShader)
            {
                name = "Wireframe Material"
            };
        }

        private void OnDisable()
        {
            RestoreOriginalMesh();
        }

        public void PrepareForVRCSDKUpload()
        {
            RestoreOriginalMesh();
        }
    }
}