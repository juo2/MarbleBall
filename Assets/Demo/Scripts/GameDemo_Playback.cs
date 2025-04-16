using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Cinemachine.CinemachineSmoothPath;

public partial class GameDemo : MonoBehaviour
{
    private List<PadData> padDataList = new List<PadData>();

    private List<BarCombineData> barCombineDataList = new List<BarCombineData>();
    private List<BarsCombineDemo> barsCombineDemoList = new List<BarsCombineDemo>();

    private List<BarLineData> barLineDataList = new List<BarLineData>();
    private List<BarsDemo> barsLineDemoList = new List<BarsDemo>();
    
    [System.Serializable]
    public class PadData
    {
        public Vector3 pos;
        public float rotation;
        public int lightIndex;
        public int frame;
        public int offsetFrame;
        public float bounciness;
        public Color debugColor;
        public Vector3 debugCenter;
        public Vector3 debugSize;
        public Vector3 debugBestPos;
        public bool inverse;
    }

    [System.Serializable]
    public class BarLineData
    {
        public Vector3 pos;
        public int frame;
        public Vector3 eulerAngles;
        public int prefabIndex;
        public bool inverse;
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
        public bool inverse;
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
    }

    IEnumerator LoadData()
    {
        ResetBeigin();
        string json_str = string.Empty;
        string path = GameConfig.Instance.GetJsonPath();

#if UNITY_WEBGL
        path = Path.Combine(path, currentMidi + ".json");
        UnityWebRequest request = UnityWebRequest.Get(path);
        request.certificateHandler = new AcceptAllCertificatesSignedHandler();
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            json_str = request.downloadHandler.text;
        }
        else
        {
            Debug.LogError($"加载失败: {request.error}");
        }
#else
        path = Path.Combine(path, currentMidi + ".json");
        json_str = File.ReadAllText(path);
#endif

        SkinManager.Instance.RefreshSkinByName(currentMidi);

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
        cameraPathProjection.material.color = Color.cyan;
        List<Waypoint> waypoints = new List<Waypoint>();

        foreach (var pos in cameraPathList)
        //foreach (var pos in cameraFollowList)
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
            barCombine_instance.inverse = bar.inverse;
            barCombine_instance.gameObject.SetActive(true);

            if (bar.inverse)
            {
                barCombine_instance.transform.eulerAngles = new Vector3(0, 180f, 0);
            }
            else
            {
                barCombine_instance.transform.eulerAngles = Vector3.zero;
            }

            barCombine_instance.refreshBarDir();

            barCombine_instance.name = $"BARS_{bar.frame}";
        }

        foreach(var bar in barLineDataList)
        {
            BarsDemo bar_instance = Instantiate<BarsDemo>(barsDemoPrefabList[bar.prefabIndex]);
            bar_instance.transform.SetParent(obstaclesParent.transform);
            bar_instance.transform.position = bar.pos;
            bar_instance.transform.localScale = Vector3.one;
            bar_instance.transform.eulerAngles = bar.eulerAngles;
            bar_instance.inverse = bar.inverse;
            bar_instance.gameObject.SetActive(true);

            if (bar.inverse)
            {
                bar_instance.transform.eulerAngles = new Vector3(0, 180f, 0);
            }
            else
            {
                bar_instance.transform.eulerAngles = Vector3.zero;
            }

            bar_instance.refreshBarDir();

            bar_instance.name = $"BARS_{bar.frame}";
        }

        Debug.Log($"LoadData path:{path}");
        yield return midiHelper.Load(currentMidi);
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

        string json_str = JsonUtility.ToJson(savePhysicsData);

        File.WriteAllText(path, json_str);

        Debug.Log($"SaveData path:{path}");
    }

}
