using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class BGMPlayer : MonoBehaviour
{
    // 相对于 StreamingAssets 目录下的子目录名
    public string bgmFolderName = "bgm";
    // 播放音频的 AudioSource（可在 Inspector 中指定）
    public AudioSource audioSource;

    Dictionary<string, AudioClip> audioClipDic = new Dictionary<string, AudioClip>();

    private void Start()
    {
        // 如果未挂载 AudioSource，则自动添加
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // 开始播放 BGM 列表（顺序播放）
        //StartCoroutine(PlayAllBGM());
    }

    IEnumerator PlayAllBGM()
    {
        // 组合出完整的目录路径
        string bgmPath = Path.Combine(Application.streamingAssetsPath, bgmFolderName);
        if (!Directory.Exists(bgmPath))
        {
            Debug.LogError("BGM目录不存在: " + bgmPath);
            yield break;
        }

        // 获取该目录下所有 .mp3 文件（不递归子目录，可根据需要调整）
        string[] mp3Files = Directory.GetFiles(bgmPath, "*.mp3", SearchOption.TopDirectoryOnly);

        foreach (string file in mp3Files)
        {
            Debug.Log("加载文件：" + file);
            // 使用 UnityWebRequest 加载音频
            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(file, AudioType.MPEG);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("加载 " + file + " 失败: " + request.error);
                continue;
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(request);


                string fileName = Path.GetFileName(file);
                string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

                Debug.Log($"nameWithoutExtension:{nameWithoutExtension}");

                audioClipDic.Add(nameWithoutExtension, clip);
            }
        }
    }


    public void PlayBgm(string midiName,float time = 0f)
    {
        //string nameWithoutExtension = Path.GetFileNameWithoutExtension(midiName);

        //if(audioClipDic.ContainsKey(nameWithoutExtension))
        //{
        //    AudioClip clip = audioClipDic[nameWithoutExtension];
        //    audioSource.clip = clip;
        //    audioSource.time = time;
        //    audioSource.Play();
        //}
    }

    public void StopBgm()
    {
        audioSource.Stop();
    }
}
