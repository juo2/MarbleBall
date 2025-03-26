using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class SkinManager : MonoBehaviour
{
    [System.Serializable]
    public class SkinConfig
    {
        public string midi;
        public int ballSkin;
        public int lightSkin;
        public int padSkin;
        public int barSkin;
        public int wallSkin;
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
    public List<Material> padPiplineMatList = new List<Material>();
    Pad selectPad;
    SkinConfigData skinConfigData;
    //int padMaterialSkin = 0;
    //public int currentLigthtSkin = 0;

    public SkinConfig currentSkinConfig;
    SkinConfig defalutSkinConfig;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0;i < barsParent.childCount;i++)
        {
            Transform barsTran = barsParent.GetChild(i);
            BarsDemo barsDemo = barsTran.GetComponent<BarsDemo>();
            barsDemoList.Add(barsDemo);
        }


        defalutSkinConfig = new SkinConfig();
        defalutSkinConfig.midi = "";
        defalutSkinConfig.lightSkin = 0;
        defalutSkinConfig.padMaterialSkin = 0;
        defalutSkinConfig.padSkin = 0;
        defalutSkinConfig.barSkin = 0;
        defalutSkinConfig.ballSkin = 0;
        defalutSkinConfig.wallSkin = 0;

        string path = Path.Combine(Application.streamingAssetsPath, "skinConfig.json");
        string json_str = File.ReadAllText(path);
        skinConfigData = JsonUtility.FromJson<SkinConfigData>(json_str);

        //SkinConfig skinConfig = new SkinConfig();
        //skinConfigData.skinConfigs = new List<SkinConfig>();

        ////黑夜
        //skinConfig.midi = "A-Lin - 天若有情";
        //skinConfig.lightSkin = 1;
        //skinConfig.padMaterialSkin = 0;
        //skinConfig.padSkin = 0;
        //skinConfig.barSkin = 0;
        //skinConfig.ballSkin = 0;
        //skinConfig.wallSkin = 0;

        //skinConfigData.skinConfigs.Add(skinConfig);

        //string path = Path.Combine(Application.streamingAssetsPath, "skinConfig.json");
        //string json_str = JsonUtility.ToJson(skinConfigData);
        //File.WriteAllText(path, json_str);
    }
    
    public void RefreshSkinByName(string midiName)
    {
        string nameWithoutExtension = Path.GetFileNameWithoutExtension(midiName);
        RefreshSkin(nameWithoutExtension);
    }


    public void RefreshSkin(string nameWithoutExtension)
    {
        currentSkinConfig = null;

        foreach (var sc in skinConfigData.skinConfigs)
        {
            if(sc.midi == nameWithoutExtension)
            {
                currentSkinConfig = sc;
                break;
            }
        }

        if(currentSkinConfig == null)
        {
            currentSkinConfig = defalutSkinConfig;
        }

        RefreshLight(currentSkinConfig.lightSkin);
        RefreshPad(currentSkinConfig.padSkin);
        RefreshBars(currentSkinConfig.barSkin);
        RefreshWall(currentSkinConfig.wallSkin);
        RefreshBall(currentSkinConfig.ballSkin);
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
                Light light = ballRenderList[i].transform.GetComponentInChildren<Light>();

                if(isDay())
                {
                    light.shadows = LightShadows.None;
                }
                else
                {
                    light.shadows = LightShadows.Soft;
                }
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

        RefreshSkyBox(_skin);
        RefreshVolume(_skin);

        Material material = padPiplineMatList[_skin];
        foreach (var pad in padList)
        {
            pad.padSkinInit(material);
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

    public bool isDay()
    {
        return currentSkinConfig.lightSkin == 1; 
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
