using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class SkinManager : MonoBehaviour
{
    public Dictionary<string, int> skinMap = new Dictionary<string, int>()
    {
        { "爱意藏风里",0},
        { "春江花月夜",1},
        { "想问情深有几许(恰恰版)",2},
        { "八小时时差",3},
        { "你能不能不要离开我(莫叫语版)",4},
        { "生分",5},
        { "时间你慢些走",6},
        { "NewBoy",7},
        { "墨尔本的秋天",8},
        { "海与天",9},
        { "回音(男声版)",10},
        { "谢谢你的爱",11},
        { "眼中有泪心中有你",12},
        { "一路生花",13},
        { "无归期",14},
        { "热恋冰淇淋",15},
        { "请先说你好",16},
        { "盼君折枝",17},
        { "后来遇见他-原曲剪辑片段",18},
        { "我期待的不是雪-原曲剪辑片段",19},
    };

    [System.Serializable]
    public class SkinConfig
    {
        public int skin;

        public int ballSkin;
        public int lightSkin;
        public int padSkin;
        public int barSkin;
        public int wallSkin;
        public int volumeSkin;
        public int skyBoxSkin;
        public int padMaterialSkin;
    }

    
    [System.Serializable]
    public class SkinConfigData
    {
        public List<SkinConfig> skinConfigs;
    }

    private static SkinManager instance;
    public static SkinManager Instance
    {
        get
        {
            if (instance == null)
            {
                // 尝试在场景中查找已存在的实例
                instance = FindObjectOfType<SkinManager>();

                // 如果场景中不存在，则创建新的 GameObject 并添加该组件
                if (instance == null)
                {
                    GameObject go = new GameObject("SkinManager");
                    instance = go.AddComponent<SkinManager>();
                }
            }
            return instance;
        }
    }

    public int skin = 0;

    public List<MeshRenderer> ballRenderList = new List<MeshRenderer>();
    public List<Pad> padList = new List<Pad>();
    public List<Light> sunList = new List<Light>();
    public List<GameObject> wallList = new List<GameObject>();
    public BarsCombineDemo barsCombineDemo;

    List<BarsDemo> barsDemoList = new List<BarsDemo>();
    
    public Transform barsParent;
    public BarsDemo barBeigin;
    public List<MaterialAsset> onMaterialAssetList = new List<MaterialAsset>();
    public List<MaterialAsset> offMaterialAssetList = new List<MaterialAsset>();
    public List<Volume> volumeList = new List<Volume>();
    public List<Material> skyBoxMatList = new List<Material>();
    Pad selectPad;
    SkinConfigData skinConfigData = new SkinConfigData();
    //int padMaterialSkin = 0;
    //public int currentLigthtSkin = 0;

    public SkinConfig currentSkinConfig;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0;i < barsParent.childCount;i++)
        {
            Transform barsTran = barsParent.GetChild(i);
            BarsDemo barsDemo = barsTran.GetComponent<BarsDemo>();
            barsDemoList.Add(barsDemo);
        }


        SkinConfig skinConfig = new SkinConfig();
        skinConfigData.skinConfigs = new List<SkinConfig>();

        {
            skinConfig.skin = 0;
            //黑夜
            skinConfig.lightSkin = 0;
            skinConfig.padMaterialSkin = 0;
            skinConfig.skyBoxSkin = 0;
            skinConfig.barSkin = 0;
            skinConfig.volumeSkin = 0;

            skinConfig.ballSkin = 0;
            skinConfig.padSkin = 0;
            skinConfig.wallSkin = 0;

            skinConfigData.skinConfigs.Add(skinConfig);
            skinConfig = new SkinConfig();

            skinConfig.skin = 2;
            //黑夜
            skinConfig.lightSkin = 0;
            skinConfig.padMaterialSkin = 0;
            skinConfig.barSkin = 0;
            skinConfig.volumeSkin = 0;
            skinConfig.skyBoxSkin = 0;

            skinConfig.ballSkin = 0;
            skinConfig.padSkin = 1;
            skinConfig.wallSkin = 1;

            skinConfigData.skinConfigs.Add(skinConfig);
            skinConfig = new SkinConfig();

            skinConfig.skin = 3;
            //黑夜
            skinConfig.lightSkin = 0;
            skinConfig.padMaterialSkin = 0;
            skinConfig.barSkin = 0;
            skinConfig.volumeSkin = 0;
            skinConfig.skyBoxSkin = 0;

            skinConfig.ballSkin = 1;
            skinConfig.padSkin = 2;
            skinConfig.wallSkin = 0;

            skinConfigData.skinConfigs.Add(skinConfig);
            skinConfig = new SkinConfig();

            skinConfig.skin = 5;
            //黑夜
            skinConfig.lightSkin = 0;
            skinConfig.padMaterialSkin = 0;
            skinConfig.barSkin = 0;
            skinConfig.volumeSkin = 0;
            skinConfig.skyBoxSkin = 0;

            skinConfig.ballSkin = 2;
            skinConfig.padSkin = 3;
            skinConfig.wallSkin = 1;

            skinConfigData.skinConfigs.Add(skinConfig);
            skinConfig = new SkinConfig();

            skinConfig.skin = 6;
            //黑夜
            skinConfig.lightSkin = 0;
            skinConfig.padMaterialSkin = 0;
            skinConfig.barSkin = 0;
            skinConfig.volumeSkin = 0;
            skinConfig.skyBoxSkin = 0;

            skinConfig.ballSkin = 3;
            skinConfig.padSkin = 0;
            skinConfig.wallSkin = 0;


            skinConfigData.skinConfigs.Add(skinConfig);
            skinConfig = new SkinConfig();

            skinConfig.skin = 9;
            //黑夜
            skinConfig.lightSkin = 0;
            skinConfig.padMaterialSkin = 0;
            skinConfig.barSkin = 0;
            skinConfig.volumeSkin = 0;
            skinConfig.skyBoxSkin = 0;

            skinConfig.ballSkin = 0;
            skinConfig.padSkin = 1;
            skinConfig.wallSkin = 1;

            skinConfigData.skinConfigs.Add(skinConfig);
            skinConfig = new SkinConfig();

            skinConfig.skin = 11;
            //黑夜
            skinConfig.lightSkin = 0;
            skinConfig.padMaterialSkin = 0;
            skinConfig.barSkin = 0;
            skinConfig.volumeSkin = 0;
            skinConfig.skyBoxSkin = 0;

            skinConfig.ballSkin = 1;
            skinConfig.padSkin = 3;
            skinConfig.wallSkin = 0;


            skinConfigData.skinConfigs.Add(skinConfig);
            skinConfig = new SkinConfig();

            skinConfig.skin = 12;
            //黑夜
            skinConfig.lightSkin = 0;
            skinConfig.padMaterialSkin = 0;
            skinConfig.barSkin = 0;
            skinConfig.volumeSkin = 0;
            skinConfig.skyBoxSkin = 0;

            skinConfig.ballSkin = 2;
            skinConfig.padSkin = 2;
            skinConfig.wallSkin = 1;


            skinConfigData.skinConfigs.Add(skinConfig);
            skinConfig = new SkinConfig();

            skinConfig.skin = 14;
            //黑夜
            skinConfig.lightSkin = 0;
            skinConfig.padMaterialSkin = 0;
            skinConfig.barSkin = 0;
            skinConfig.volumeSkin = 0;
            skinConfig.skyBoxSkin = 0;

            skinConfig.ballSkin = 3;
            skinConfig.padSkin = 3;
            skinConfig.wallSkin = 0;

            skinConfigData.skinConfigs.Add(skinConfig);
            skinConfig = new SkinConfig();

            skinConfig.skin = 19;
            //黑夜
            skinConfig.lightSkin = 0;
            skinConfig.padMaterialSkin = 0;
            skinConfig.barSkin = 0;
            skinConfig.volumeSkin = 0;
            skinConfig.skyBoxSkin = 0;

            skinConfig.ballSkin = 0;
            skinConfig.padSkin = 2;
            skinConfig.wallSkin = 1;

            skinConfigData.skinConfigs.Add(skinConfig);

        }


        {
            skinConfig = new SkinConfig();
            skinConfig.skin = 1;
            //白天
            skinConfig.lightSkin = 1;
            skinConfig.volumeSkin = 1;
            skinConfig.skyBoxSkin = 1;

            skinConfig.padMaterialSkin = 2;
            skinConfig.barSkin = 1;
            
            skinConfig.wallSkin = 2;
            skinConfig.ballSkin = 4;
            skinConfig.padSkin = 0;

            skinConfigData.skinConfigs.Add(skinConfig);


            skinConfig = new SkinConfig();
            skinConfig.skin = 4;
            //白天
            skinConfig.lightSkin = 1;
            skinConfig.volumeSkin = 1;
            skinConfig.skyBoxSkin = 1;

            skinConfig.padMaterialSkin = 1;
            skinConfig.barSkin = 1;

            skinConfig.wallSkin = 3;
            skinConfig.ballSkin = 5;
            skinConfig.padSkin = 1;

            skinConfigData.skinConfigs.Add(skinConfig);

            skinConfig = new SkinConfig();
            skinConfig.skin = 7;
            //白天
            skinConfig.lightSkin = 1;
            skinConfig.volumeSkin = 1;
            skinConfig.skyBoxSkin = 1;

            skinConfig.padMaterialSkin = 2;
            skinConfig.barSkin = 2;

            skinConfig.wallSkin = 3;
            skinConfig.ballSkin = 6;
            skinConfig.padSkin = 2;

            skinConfigData.skinConfigs.Add(skinConfig);

            skinConfig = new SkinConfig();
            skinConfig.skin = 8;
            //白天
            skinConfig.lightSkin = 1;
            skinConfig.volumeSkin = 1;
            skinConfig.skyBoxSkin = 1;

            skinConfig.padMaterialSkin = 1;
            skinConfig.barSkin = 1;

            skinConfig.wallSkin = 2;
            skinConfig.ballSkin = 5;
            skinConfig.padSkin = 0;

            skinConfigData.skinConfigs.Add(skinConfig);

            skinConfig = new SkinConfig();
            skinConfig.skin = 10;
            //白天
            skinConfig.lightSkin = 1;
            skinConfig.volumeSkin = 1;
            skinConfig.skyBoxSkin = 1;

            skinConfig.padMaterialSkin = 2;
            skinConfig.barSkin = 1;

            skinConfig.wallSkin = 3;
            skinConfig.ballSkin = 6;
            skinConfig.padSkin = 1;

            skinConfigData.skinConfigs.Add(skinConfig);

            skinConfig = new SkinConfig();
            skinConfig.skin = 13;
            //白天
            skinConfig.lightSkin = 1;
            skinConfig.volumeSkin = 1;
            skinConfig.skyBoxSkin = 1;

            skinConfig.padMaterialSkin = 1;
            skinConfig.barSkin = 2;

            skinConfig.wallSkin = 2;
            skinConfig.ballSkin = 4;
            skinConfig.padSkin = 2;

            skinConfigData.skinConfigs.Add(skinConfig);

            skinConfig = new SkinConfig();
            skinConfig.skin = 15;
            //白天
            skinConfig.lightSkin = 1;
            skinConfig.volumeSkin = 1;
            skinConfig.skyBoxSkin = 1;

            skinConfig.padMaterialSkin = 2;
            skinConfig.barSkin = 1;

            skinConfig.wallSkin = 3;
            skinConfig.ballSkin = 5;
            skinConfig.padSkin = 3;

            skinConfigData.skinConfigs.Add(skinConfig);

            skinConfig = new SkinConfig();
            skinConfig.skin = 16;
            //白天
            skinConfig.lightSkin = 1;
            skinConfig.volumeSkin = 1;
            skinConfig.skyBoxSkin = 1;

            skinConfig.padMaterialSkin = 1;
            skinConfig.barSkin = 1;

            skinConfig.wallSkin = 2;
            skinConfig.ballSkin = 6;
            skinConfig.padSkin = 0;

            skinConfigData.skinConfigs.Add(skinConfig);

            skinConfig = new SkinConfig();
            skinConfig.skin = 17;
            //白天
            skinConfig.lightSkin = 1;
            skinConfig.volumeSkin = 1;
            skinConfig.skyBoxSkin = 1;

            skinConfig.padMaterialSkin = 2;
            skinConfig.barSkin = 1;

            skinConfig.wallSkin = 3;
            skinConfig.ballSkin = 6;
            skinConfig.padSkin = 1;

            skinConfigData.skinConfigs.Add(skinConfig);

            skinConfig = new SkinConfig();
            skinConfig.skin = 18;
            //白天
            skinConfig.lightSkin = 1;
            skinConfig.volumeSkin = 1;
            skinConfig.skyBoxSkin = 1;

            skinConfig.padMaterialSkin = 1;
            skinConfig.barSkin = 1;

            skinConfig.wallSkin = 3;
            skinConfig.ballSkin = 4;
            skinConfig.padSkin = 3;

            skinConfigData.skinConfigs.Add(skinConfig);
        }

    }
    
    public void RefreshSkinByName(string midiName)
    {
        string nameWithoutExtension = Path.GetFileNameWithoutExtension(midiName);
        skin = skinMap[nameWithoutExtension];

        RefreshSkin(skin);
    }


    public void RefreshSkin(int _skin = -1)
    {
        if(_skin != -1)
        {
            skin = _skin;
        }

        foreach (var sc in skinConfigData.skinConfigs)
        {
            if(sc.skin == skin)
            {
                currentSkinConfig = sc;
                break;
            }
        }

        RefreshBall(currentSkinConfig.ballSkin);
        RefreshLight(currentSkinConfig.lightSkin);
        RefreshPad(currentSkinConfig.padSkin);
        RefreshBars(currentSkinConfig.barSkin);
        RefreshWall(currentSkinConfig.wallSkin);
        RefreshVolume(currentSkinConfig.volumeSkin);
        RefreshSkyBox(currentSkinConfig.skyBoxSkin);
    }

    public void RefreshVolume(int _skin)
    {
        for (int i = 0; i < volumeList.Count; i++)
        {
            if (i == _skin)
            {
                volumeList[i].gameObject.SetActive(true);
            }
            else
            {
                volumeList[i].gameObject.SetActive(false);
            }
        }
    }

    public void RefreshBall(int _skin)
    {
        for(int i = 0;i < ballRenderList.Count;i++)
        {
            if(i == _skin)
            {
                ballRenderList[i].gameObject.SetActive(true);
            }
            else
            {
                ballRenderList[i].gameObject.SetActive(false);
            }
        }
    }

    public void RefreshLight(int _skin)
    {
        for (int i = 0; i < sunList.Count; i++)
        {
            if (i == _skin)
            {
                sunList[i].gameObject.SetActive(true);
            }
            else
            {
                sunList[i].gameObject.SetActive(false);
            }
        }
    }

    public void RefreshPad(int _skin)
    {
        selectPad = padList[_skin];
    }

    public void RefreshBars(int _skin)
    {
        foreach(var bar in barsDemoList)
        {
            bar.refreshSkin(_skin);
        }

        barBeigin.refreshSkin(_skin);
        barsCombineDemo.refreshSkin(_skin);
    }

    public void RefreshWall(int _skin)
    {
        for (int i = 0; i < wallList.Count; i++)
        {
            if (i == _skin)
            {
                wallList[i].gameObject.SetActive(true);
            }
            else
            {
                wallList[i].gameObject.SetActive(false);
            }
        }
    }

    public void RefreshSkyBox(int _skin)
    {
        RenderSettings.skybox = skyBoxMatList[_skin];
        DynamicGI.UpdateEnvironment();
    }

    public Pad GetPadPrefab()
    {
        return selectPad;
    }

    public Material GetMaterial(bool on,int lightIndex)
    {
        if(on)
        {
            MaterialAsset materialAsset = onMaterialAssetList[currentSkinConfig.padMaterialSkin];
            return materialAsset.materials[lightIndex];
        }
        else
        {
            MaterialAsset materialAsset = offMaterialAssetList[currentSkinConfig.padMaterialSkin];
            return materialAsset.materials[lightIndex];
        }
    }

    public int GetMaterialCount()
    {
        MaterialAsset materialAsset = onMaterialAssetList[currentSkinConfig.padMaterialSkin];
        return materialAsset.materials.Count;
    }
}
