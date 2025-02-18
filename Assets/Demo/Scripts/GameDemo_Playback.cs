using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.CinemachineSmoothPath;

public partial class GameDemo : MonoBehaviour
{
    private List<PadData> padDataList = new List<PadData>();

    private List<BarCombineData> barCombineDataList = new List<BarCombineData>();
    private List<BarsCombineDemo> barsCombineDemoList = new List<BarsCombineDemo>();

    private List<BarLineData> barLineDataList = new List<BarLineData>();
    private List<BarsDemo> barsLineDemoList = new List<BarsDemo>();

    private List<BarData> barDataList = new List<BarData>();
    private List<BarsDemo> barsDemoList2 = new List<BarsDemo>();


    [System.Serializable]
    public class KeyValuePair
    {
        public int key;
        public List<SimulateData> value;

        public KeyValuePair(int key, List<SimulateData> value)
        {
            this.key = key;
            this.value = value;
        }
    }


    [System.Serializable]
    public class PadData
    {
        public Vector3 pos;
        public float rotation;
        public int lightIndex;
        public int frame;
        public int offsetFrame;
        public float bounciness;
        //public Vector2 miniBorder;
        //public Vector2 maxBorder;
        public Color debugColor;
        public Vector3 debugCenter;
        public Vector3 debugSize;
        public Vector3 debugBestPos;
        public TryingType tryingType;
        public int continuousCount;
        public bool inverse;
    }

    [System.Serializable]
    public class BarLineData
    {
        public Vector3 pos;
        public int frame;
        public Vector3 eulerAngles;
        public int prefabIndex;
    }

    [System.Serializable]
    public class BarData
    {
        public Vector3 pos;
        public int frame;
        public Vector3 eulerAngles;
        public int n;
    }


    [System.Serializable]
    public class BarCombineData
    {
        public Vector3 pos;
        public int frame;
        public Vector3 eulerAngles;
        public float scaleX;
    }

    [System.Serializable]
    private class SavePhysicsData
    {
        public int playMusicFrame;
        public int currentOffsetIndex;
        
        public List<int> FrameList;
        public List<PadData> PadDataList;
        public List<BarCombineData> BarCombineDataList;
        public List<BarLineData> BarLineDataList;

        public List<PhysicsState> PhysicsStateList;
        public List<Vector3> CameraPathList;
        public List<Vector3> CameraFollowList;
        public List<KeyValuePair> SimulateDataDicList;
    }

    void LoadData()
    {
        ResetBeigin();

        string path = Path.Combine(Application.streamingAssetsPath, currentMidi + ".json");
        string json_str = File.ReadAllText(path);

        SavePhysicsData savePhysicsData = JsonUtility.FromJson<SavePhysicsData>(json_str);
        playMusicFrame = savePhysicsData.playMusicFrame;
        currentOffsetIndex = savePhysicsData.currentOffsetIndex;
        frameList = savePhysicsData.FrameList;
        physicsStateList = savePhysicsData.PhysicsStateList;

        line.ResetLineRenderer(physicsStateList);

        padDataList = savePhysicsData.PadDataList;
        barCombineDataList = savePhysicsData.BarCombineDataList;
        barLineDataList = savePhysicsData.BarLineDataList;
        //镜头朝向
        cameraPathList = savePhysicsData.CameraPathList;
        cameraProjection.positionCount = cameraPathList.Count;
        for(int i = 0;i < cameraPathList.Count;i++)
        {
            cameraProjection.SetPosition(i, cameraPathList[i]);
        }
        cameraProjection.material.color = Color.white;

        //镜头移动
        cameraFollowList = savePhysicsData.CameraFollowList;
        cameraPathProjection.positionCount = cameraFollowList.Count;
        cameraPathProjection.SetPositions(cameraFollowList.ToArray());
        cameraPathProjection.material.color = Color.cyan;
        List<Waypoint> waypoints = new List<Waypoint>();
        foreach (var pos in cameraFollowList)
        {
            Waypoint waypoint = new Waypoint();
            waypoint.position = pos;
            waypoints.Add(waypoint);
        }
        cinemachineSmoothPath.m_Waypoints = waypoints.ToArray();

        //开始动画
        InstantBars(ball.transform.position, Vector3.one, bar1, 0);

        foreach (var bar in barCombineDataList)
        {
            BarsCombineDemo barCombine_instance = Instantiate<BarsCombineDemo>(barCombine);
            barCombine_instance.transform.SetParent(obstaclesParent.transform);
            barCombine_instance.transform.position = bar.pos;
            barCombine_instance.transform.localScale = Vector3.one;
            barCombine_instance.transform.eulerAngles = bar.eulerAngles;
            barCombine_instance.updateVirtualBars(false);
            barCombine_instance.SetScaleX(bar.scaleX);

            barCombine_instance.gameObject.SetActive(true);
            barCombine_instance.name = $"BARS_{bar.frame}";
        }

        foreach(var bar in barLineDataList)
        {
            BarsDemo bar_instance = Instantiate<BarsDemo>(barsDemoPrefabList[bar.prefabIndex]);
            bar_instance.transform.SetParent(obstaclesParent.transform);
            bar_instance.transform.position = bar.pos;
            bar_instance.transform.localScale = Vector3.one;
            bar_instance.transform.eulerAngles = bar.eulerAngles;

            bar_instance.gameObject.SetActive(true);
            bar_instance.name = $"BARS_{bar.frame}";
        }

        Debug.Log($"LoadData path:{path}");

        startGenerating.transform.Find("Text").GetComponent<Text>().text = "继续生成";

        LoadSimulateData(savePhysicsData.SimulateDataDicList);

        midiHelper.Load(currentMidi);
    }

    void SaveData(string midiName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, midiName + ".json");

        SavePhysicsData savePhysicsData = new SavePhysicsData();
        savePhysicsData.playMusicFrame = playMusicFrame;
        savePhysicsData.currentOffsetIndex = currentOffsetIndex;

        savePhysicsData.PadDataList = padDataList;
        savePhysicsData.BarCombineDataList = barCombineDataList;
        savePhysicsData.BarLineDataList = barLineDataList;
        savePhysicsData.FrameList = frameList;
        
        savePhysicsData.PhysicsStateList = physicsStateList;
        savePhysicsData.CameraPathList = cameraPathList;
        savePhysicsData.CameraFollowList = cameraFollowList;

        List<KeyValuePair> keyValueList = new List<KeyValuePair>();
        foreach (var pair in simulateDataDic)
        {
            keyValueList.Add(new KeyValuePair(pair.Key, pair.Value));
        }

        savePhysicsData.SimulateDataDicList = keyValueList;

        string json_str = JsonUtility.ToJson(savePhysicsData);

        File.WriteAllText(path, json_str);

        Debug.Log($"SaveData path:{path}");
    }

    void RecordBarsLine()
    {
        BarLineData barLineData = new BarLineData();
        barLineData.pos = currentLineBars.transform.position;
        barLineData.frame = currentFrame;
        barLineData.eulerAngles = currentLineBars.transform.eulerAngles;
        barLineData.prefabIndex = currentLineBars.prefabIndex;

        barLineDataList.Add(barLineData);
        barsLineDemoList.Add(currentLineBars);

        BarsDemo bars = currentLineBars;

        bars.padCollider.collisionAction = () => 
        {
            if (currentFrame < bars.beginFrame || currentFrame > bars.endFrame)
            {
                bars.mistakeCollisionTimes++;
            }
        };
    }

    void RecordBars()
    {
        BarData barData = new BarData();
        barData.pos = currentBars.transform.position;
        barData.frame = currentFrame;
        barData.eulerAngles = currentBars.transform.eulerAngles;

        barDataList.Add(barData);
        barsDemoList2.Add(currentBars);

        BarsDemo bars = currentBars;

        bars.padCollider.collisionAction = () =>
        {
            if (currentFrame < bars.beginFrame || currentFrame > bars.endFrame)
            {
                bars.mistakeCollisionTimes++;
            }
        };
    }

    void RecordBarsCombine()
    {
        BarCombineData barCombineData = new BarCombineData();
        barCombineData.pos = currentBarsCombine.transform.position;
        barCombineData.frame = currentFrame;
        barCombineData.eulerAngles = currentBarsCombine.transform.eulerAngles;
        barCombineData.scaleX = currentBarsCombine.getScaleX();

        barCombineDataList.Add(barCombineData);
        barsCombineDemoList.Add(currentBarsCombine);

        BarsCombineDemo barsCombine = currentBarsCombine;

        //添加事件
        barsCombine.padCollider1.collisionAction = () =>
        {
            if (currentFrame < barsCombine.beginFrame || currentFrame > barsCombine.endFrame)
            {
                barsCombine.mistakeCollisionTimes++;
            }
        };

        barsCombine.padCollider2.collisionAction = () =>
        {
            if (currentFrame < barsCombine.beginFrame || currentFrame > barsCombine.endFrame)
            {
                barsCombine.mistakeCollisionTimes++;
            }
        };

        barsCombine.padCollider3.collisionAction = () =>
        {
            if (currentFrame < barsCombine.beginFrame || currentFrame > barsCombine.endFrame)
            {
                barsCombine.mistakeCollisionTimes++;
            }
        };
    }

    void SetPhysicMaterialBounciness(int _index, float bounciness)
    {
        if (visiblePadDic.TryGetValue(_index, out Pad pad))
        {
            pad.SetPhysicMaterialBounciness(bounciness);
        }

        if (_index < padDataList.Count)
        {
            padDataList[_index].bounciness = bounciness;
        }
    }

    void SetBounds(int _index, Vector3 center, Vector3 size, Vector3 bestPos)
    {
        if (visiblePadDic.TryGetValue(_index, out Pad pad))
        {
            pad.SetBounds(center, size, bestPos);
        }

        if (_index < padDataList.Count)
        {
            padDataList[_index].debugCenter = center;
            padDataList[_index].debugSize = size;
            padDataList[_index].debugBestPos = bestPos;
        }
    }


    void SetColor(int _index, Color color)
    {
        if (visiblePadDic.TryGetValue(_index, out Pad pad))
        {
            pad.SetLineRenererColor(color);
        }

        if (_index < padDataList.Count)
        {
            padDataList[_index].debugColor = color;
        }

    }

    void SetRotation(int _index, float rotation)
    {
        if (visiblePadDic.TryGetValue(_index, out Pad pad))
        {
            pad.SetRotation(rotation);
        }

        if (_index < padDataList.Count)
        {
            padDataList[_index].rotation = rotation;
        }
    }

}
