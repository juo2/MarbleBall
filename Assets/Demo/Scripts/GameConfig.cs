using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
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
    string skinConfigPath = headUrl + "/skinConfig.json";
    string jsonPath = headUrl + "/json";
    string lyricPath = headUrl + "/lyric";
#else 
    string midiPath = Application.streamingAssetsPath + "/midi";
    string skinConfigPath = Application.streamingAssetsPath + "/skinConfig.json";
    string jsonPath = Application.streamingAssetsPath + "/json";
    string lyricPath = Application.streamingAssetsPath + "/lyric";
#endif

    public SkinManager.SkinConfigData skinConfigData = null;
    public UnityAction callBack;

    private void Awake()
    {
        StartCoroutine(LoadConfig());
    }

    IEnumerator LoadConfig()
    {
        string json_str = string.Empty;
        string path = GameConfig.Instance.GetSkinConfigPath();
#if UNITY_WEBGL
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
#elif UNITY_ANDROID
        UnityWebRequest request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            json_str = request.downloadHandler.text;
            Debug.Log($"json_str:{json_str}");
        }
        else
        {
            Debug.LogError($"加载失败: {request.error}");
        }
#else
        yield return null;
        json_str = File.ReadAllText(path);
#endif
        skinConfigData = JsonUtility.FromJson<SkinManager.SkinConfigData>(json_str);
        callBack?.Invoke();
    }

    public string GetMidiPath()
    {
        return midiPath;
    }

    public string GetLyricPath()
    {
        return lyricPath;
    }

    public string GetSkinConfigPath()
    {
        return skinConfigPath;
    }

    public string GetJsonPath()
    {
        return jsonPath;
    }

}
