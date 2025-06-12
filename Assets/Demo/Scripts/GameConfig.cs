using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AcceptAllCertificatesSignedHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // 始终返回 true，即忽略所有证书错误
        return true;
    }
}

public class GameConfig : MonoBehaviour
{

    private static GameConfig instance;
    public static GameConfig Instance
    {
        get
        {
            if (instance == null)
            {
                // 尝试在场景中查找已存在的实例
                instance = FindObjectOfType<GameConfig>();

                // 如果场景中不存在，则创建新的 GameObject 并添加该组件
                if (instance == null)
                {
                    GameObject go = new GameObject("GameConfig");
                    instance = go.AddComponent<GameConfig>();
                }
            }
            return instance;
        }
    }

#if UNITY_WEBGL

    static string headUrl = "https://172.19.11.101:8777";

    string midiPath = headUrl + "/midi";
    string barsConfigPath = headUrl + "/barsConfig.json";
    string skinConfigPath = headUrl + "/skinConfig.json";
    string barCurvePath = headUrl + "/barCurve";
    string padCurvePath = headUrl + "/padCurve";
    string jsonPath = headUrl + "/json";
    string firstBarCurvePath = headUrl + "/firstBarCurve.json";
#else 
    string midiPath = Application.streamingAssetsPath + "/midi";
    string barsConfigPath = Application.streamingAssetsPath + "/barsConfig.json";
    string skinConfigPath = Application.streamingAssetsPath + "/skinConfig.json";
    string barCurvePath = Application.streamingAssetsPath + "/barCurve";
    string padCurvePath = Application.streamingAssetsPath + "/padCurve";
    string jsonPath = Application.streamingAssetsPath + "/json";
    string firstBarCurvePath = Application.streamingAssetsPath + "/firstBarCurve.json";
#endif

    public string GetMidiPath()
    {
        return midiPath;
    }

    public string GetBarsConfigPath()
    {
        return barsConfigPath;
    }

    public string GetSkinConfigPath()
    {
        return skinConfigPath;
    }

    public string GetBarCurvePath()
    {
        return barCurvePath;
    }

    public string GetPadCurvePath()
    {
        return padCurvePath;
    }

    public string GetJsonPath()
    {
        return jsonPath;
    }

    public string GetFirstBarCurvePath()
    {
        return firstBarCurvePath;
    }

}
