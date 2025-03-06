using UnityEngine;
using UnityEditor;
using System.IO;
using Dreamteck.Splines;
using System.Collections.Generic;

public class Test : EditorWindow
{
    private string folderPath = "Assets/Art/Bars"; // 默认路径

    private GameObject fbxAsset1;

    private GameObject fbxAsset2;

    [MenuItem("Tools/ReCalulateBars")]
    public static void ShowWindow()
    {
        GetWindow<Test>("Prefab Image Replacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab Image Replacer", EditorStyles.boldLabel);

        // 选择文件夹
        GUILayout.Label("Prefabs Folder Path:", EditorStyles.label);
        folderPath = EditorGUILayout.TextField(folderPath);


        GUILayout.Label("FBX Asset1:", EditorStyles.label);
        fbxAsset1 = (GameObject)EditorGUILayout.ObjectField(fbxAsset1, typeof(GameObject), false);

        GUILayout.Label("FBX Asset2:", EditorStyles.label);
        fbxAsset2 = (GameObject)EditorGUILayout.ObjectField(fbxAsset2, typeof(GameObject), false);

        // 替换按钮
        if (GUILayout.Button("Replace Sprites in Prefabs"))
        {
            ReplaceSpritesInPrefabs();
        }
    }

    private void ReplaceSpritesInPrefabs()
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError("指定的文件夹路径不存在：" + folderPath);
            return;
        }

        if (fbxAsset1 == null)
        {
            Debug.LogError("请指定一个 FBX Asset 以提取 Mesh！");
            return;
        }

        if (fbxAsset2 == null)
        {
            Debug.LogError("请指定一个 FBX Asset 以提取 Mesh！");
            return;
        }

        MeshFilter fbxMeshFilter1 = fbxAsset1.GetComponentInChildren<MeshFilter>();
        MeshFilter fbxMeshFilter2 = fbxAsset2.GetComponentInChildren<MeshFilter>();
        Mesh fbxMesh1 = fbxMeshFilter1.sharedMesh;
        Mesh fbxMesh2 = fbxMeshFilter2.sharedMesh;

        string[] prefabFiles = Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories);
        int modifiedCount = 0;

        foreach (string prefabPath in prefabFiles)
        {
            // 只处理名称包含 "Bar3" 的 Prefab
            if (!prefabPath.Contains("Bar7") && !prefabPath.Contains("Bar6") && !prefabPath.Contains("Bar5") && !prefabPath.Contains("Bar4") && !prefabPath.Contains("Bar3") && !prefabPath.Contains("Bar2"))
            {
                continue;
            }

            // 使用 LoadPrefabContents 加载 Prefab 的临时实例
            GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);
            if (prefabInstance == null) 
                continue;

            bool modified = false;
            SplineComputer[] SCArray = prefabInstance.GetComponentsInChildren<SplineComputer>();
            foreach (var sc in SCArray)
            {
                // 创建新的子对象并挂载到当前 SplineComputer 下
                GameObject go = new GameObject("SplineMesh");
                // 第二个参数 false 表示保持局部坐标不变
                go.transform.SetParent(sc.transform, false);
                SplineMesh sm = go.AddComponent<SplineMesh>();

                MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
                if(meshRenderer == null)
                {
                    meshRenderer = go.AddComponent<MeshRenderer>();
                }


                List<Material> materialList = new List<Material>();
                materialList.Add(AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Bars/BarsFBX2/_Bars.mat"));
                materialList.Add(AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Bars/BarsFBX2/_Bars_Support.mat"));

                meshRenderer.materials = materialList.ToArray();
                sm.spline = sc;

                {
                    SplineMesh.Channel channel = sm.GetChannel(0);
                    channel.type = SplineMesh.Channel.Type.Extrude;
                    channel.count = 30;

                    SplineMesh.Channel.MeshDefinition meshDefinition = new SplineMesh.Channel.MeshDefinition(fbxMesh1);
                    meshDefinition.offset = new Vector3(0.015f, 0, 0);
                    meshDefinition.rotation = new Vector3(90, 90, 0);
                    meshDefinition.scale = new Vector3(15, 15, 15);
                    channel.AddMesh(meshDefinition);
                    channel.minOffset = new Vector3(-0.01f, -0.005f);
                }

                {
                    SplineMesh.Channel channel = sm.AddChannel("Channel 3");
                    channel.type = SplineMesh.Channel.Type.Extrude;
                    channel.count = 1;
                    channel.clipFrom = 0.43f;
                    channel.clipTo = 0.65f;
                    SplineMesh.Channel.MeshDefinition meshDefinition = new SplineMesh.Channel.MeshDefinition(fbxMesh2);
                    meshDefinition.offset = new Vector3(0, -0.04f, -0.15f);
                    meshDefinition.rotation = new Vector3(90, 0, 0);
                    meshDefinition.scale = new Vector3(20, 20, 20);
                    channel.AddMesh(meshDefinition);
                    channel.minOffset = new Vector3(0f, 0);
                    channel.minRotation = new Vector3(180, 0, 0);

                    channel.overrideMaterialID = true;
                    channel.targetMaterialID = 1;
                }

                //{
                //    SplineMesh.Channel channel = sm.GetChannel(0);
                //    channel.type = SplineMesh.Channel.Type.Extrude;
                //    channel.count = 30;

                //    SplineMesh.Channel.MeshDefinition meshDefinition = new SplineMesh.Channel.MeshDefinition(fbxMesh1);
                //    meshDefinition.rotation = new Vector3(90, 0, 0);
                //    meshDefinition.scale = new Vector3(15, 15, 15);
                //    channel.AddMesh(meshDefinition);
                //    channel.minOffset = new Vector3(0.04f, 0.05f);
                //}

                //{
                //    SplineMesh.Channel channel = sm.AddChannel("Channel 2");
                //    channel.type = SplineMesh.Channel.Type.Extrude;
                //    channel.count = 30;

                //    SplineMesh.Channel.MeshDefinition meshDefinition = new SplineMesh.Channel.MeshDefinition(fbxMesh1);
                //    meshDefinition.rotation = new Vector3(90, 0, 0);
                //    meshDefinition.scale = new Vector3(15, 15, 15);
                //    channel.AddMesh(meshDefinition);
                //    channel.minOffset = new Vector3(0.025f, -0.05f);
                //}


                //{
                //    SplineMesh.Channel channel = sm.AddChannel("Channel 3");
                //    channel.type = SplineMesh.Channel.Type.Extrude;
                //    channel.count = 1;
                //    channel.clipFrom = 0.48f;
                //    channel.clipTo = 0.8f;
                //    SplineMesh.Channel.MeshDefinition meshDefinition = new SplineMesh.Channel.MeshDefinition(fbxMesh2);
                //    meshDefinition.offset = new Vector3(0, 0.1f, 0);
                //    meshDefinition.rotation = new Vector3(90, 90, 0);
                //    meshDefinition.scale = new Vector3(20, 20, 20);
                //    channel.AddMesh(meshDefinition);
                //    channel.minOffset = new Vector3(0.018f, 0);

                //    channel.overrideMaterialID = true;
                //    channel.targetMaterialID = 1;
                //}


                //{
                //    SplineMesh.Channel channel = sm.AddChannel("Channel 4");
                //    channel.type = SplineMesh.Channel.Type.Extrude;
                //    channel.count = 30;

                //    SplineMesh.Channel.MeshDefinition meshDefinition = new SplineMesh.Channel.MeshDefinition(fbxMesh1);
                //    meshDefinition.rotation = new Vector3(90, 0, 0);
                //    meshDefinition.scale = new Vector3(15, 15, 15);
                //    channel.AddMesh(meshDefinition);
                //    channel.minOffset = new Vector3(-0.01f, 0);

                //}


                modified = true;
            }

            if (modified)
            {
                // 保存修改到原Prefab资源中
                PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
                modifiedCount++;
            }

            // 卸载临时实例
            PrefabUtility.UnloadPrefabContents(prefabInstance);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (modifiedCount > 0)
        {
            Debug.Log($"成功更新 {modifiedCount} 个 Prefab！");
        }
        else
        {
            Debug.Log("没有找到需要替换的 Prefab。");
        }
    }
}
