using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SkinManager : MonoBehaviour
{
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
    int padMaterialSkin = 0;
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
        skinConfig.skin = 0;
        skinConfig.ballSkin = 0;
        skinConfig.lightSkin = 0;
        skinConfig.padSkin = 0;
        skinConfig.barSkin = 0;
        skinConfig.wallSkin = 0;
        skinConfig.volumeSkin = 0;
        skinConfig.skyBoxSkin = 0;
        skinConfig.padMaterialSkin = 0;
        skinConfigData.skinConfigs = new List<SkinConfig>();
        skinConfigData.skinConfigs.Add(skinConfig);

        skinConfig = new SkinConfig();
        skinConfig.skin = 1;
        skinConfig.ballSkin = 1;
        skinConfig.lightSkin = 1;
        skinConfig.padSkin = 1;
        skinConfig.barSkin = 1;
        skinConfig.wallSkin = 1;
        skinConfig.volumeSkin = 1;
        skinConfig.skyBoxSkin = 1;
        skinConfig.padMaterialSkin = 1;


        skinConfigData.skinConfigs.Add(skinConfig);

        skinConfig = new SkinConfig();
        skinConfig.skin = 2;
        skinConfig.ballSkin = 2;
        skinConfig.lightSkin = 1;
        skinConfig.padSkin = 2;
        skinConfig.barSkin = 2;
        skinConfig.wallSkin = 2;
        skinConfig.volumeSkin = 1;
        skinConfig.skyBoxSkin = 1;
        skinConfig.padMaterialSkin = 2;


        skinConfigData.skinConfigs.Add(skinConfig);

        skinConfig = new SkinConfig();
        skinConfig.skin = 3;
        skinConfig.ballSkin = 3;
        skinConfig.lightSkin = 0;
        skinConfig.padSkin = 3;
        skinConfig.barSkin = 0;
        skinConfig.wallSkin = 3;
        skinConfig.volumeSkin = 0;
        skinConfig.skyBoxSkin = 0;
        skinConfig.padMaterialSkin = 0;


        skinConfigData.skinConfigs.Add(skinConfig);


        RefreshSkin();
    }


    public void RefreshSkin(int _skin = -1)
    {
        if(_skin != -1)
        {
            skin = _skin;
        }

        SkinConfig skinConfig = null;

        foreach (var sc in skinConfigData.skinConfigs)
        {
            if(sc.skin == skin)
            {
                skinConfig = sc;
                break;
            }
        }

        RefreshBall(skinConfig.ballSkin);
        RefreshLight(skinConfig.lightSkin);
        RefreshPad(skinConfig.padSkin);
        RefreshBars(skinConfig.barSkin);
        RefreshWall(skinConfig.wallSkin);
        RefreshVolume(skinConfig.volumeSkin);
        RefreshSkyBox(skinConfig.skyBoxSkin);

        padMaterialSkin = skinConfig.padMaterialSkin;
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
    }

    public Pad GetPadPrefab()
    {
        return selectPad;
    }

    public Material GetMaterial(bool on,int lightIndex)
    {
        if(on)
        {
            MaterialAsset materialAsset = onMaterialAssetList[padMaterialSkin];
            return materialAsset.materials[lightIndex];
        }
        else
        {
            MaterialAsset materialAsset = offMaterialAssetList[padMaterialSkin];
            return materialAsset.materials[lightIndex];
        }
    }

    public int GetMaterialCount()
    {
        MaterialAsset materialAsset = onMaterialAssetList[padMaterialSkin];
        return materialAsset.materials.Count;
    }
}
