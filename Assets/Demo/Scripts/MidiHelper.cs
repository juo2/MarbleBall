using MidiPlayerTK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MidiHelper : MonoBehaviour
{
    [SerializeField]
    public MidiFilePlayer midiFilePlayer;

    [SerializeField]
    public MidiStreamPlayer midiStreamPlayer;

    [SerializeField]
    public MidiCustomPlayer midiCustomPlayer;

    public List<MPTKEvent> AllMidiEventsSet = new List<MPTKEvent>();
    public int playChannel = 1;

    private HashSet<float> timeSet = new HashSet<float>();
    public string currentMidi = string.Empty;

    public float suitMidiFacter = 1.0f;

    public float beginTime = 0;
    public float endTime = 0;
    public int beiginFrame = 0;
    AudioClip currentClip;
    private bool isLoadBgm = false;
    public List<int> frameList;
    public Dictionary<int, string> lyricDic = new Dictionary<int, string>();

    // Start is called before the first frame update
    void Awake()
    {

    }

    public IEnumerator Load(string _name)
    {
        if(currentMidi != _name)
        {
            beginTime = 0;
            timeSet.Clear();
            AllMidiEventsSet.Clear();

            string _path = Path.Combine(GameConfig.Instance.GetMidiPath(), _name + ".mid");
            byte[] _bytes = null;
#if UNITY_WEBGL
            UnityWebRequest request = UnityWebRequest.Get(_path);
            request.certificateHandler = new AcceptAllCertificatesSignedHandler();
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                _bytes = request.downloadHandler.data;
            } 
            else
            {
                Debug.LogError($"加载失败: {request.error}");
            }
#elif UNITY_ANDROID
            UnityWebRequest request = UnityWebRequest.Get(_path);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                _bytes = request.downloadHandler.data;
            }
            else
            {
                Debug.LogError($"加载失败: {request.error}");
            }
#else
            yield return null;
            _bytes = File.ReadAllBytes(_path);
#endif

            midiFilePlayer.midiLoaded = new MidiLoad();
            midiFilePlayer.midiLoaded.MPTK_KeepNoteOff = midiFilePlayer.MPTK_KeepNoteOff;
            midiFilePlayer.midiLoaded.MPTK_KeepEndTrack = midiFilePlayer.MPTK_KeepEndTrack;
            midiFilePlayer.midiLoaded.MPTK_LogLoadEvents = midiFilePlayer.MPTK_LogLoadEvents;
            midiFilePlayer.midiLoaded.MPTK_EnableChangeTempo = midiFilePlayer.MPTK_EnableChangeTempo;
            midiFilePlayer.midiLoaded.MPTK_Load(_bytes);

            midiStreamPlayer.MPTK_CorePlayer = false;
            List<MPTKEvent> AllMidiEvents = midiFilePlayer.MPTK_ReadMidiEvents();
            string contents = string.Empty;
            int _lastFrame = 0;
            //float lastTime = 0;
            int ind = 0;
            foreach (var mptkEvent in AllMidiEvents)
            {
                if (mptkEvent.Command == MPTKCommand.NoteOn)
                {
                    if(beginTime == 0)
                    {
                        beginTime = mptkEvent.RealTime / 1000f;
                    }

                    ind = ind + 1;
                    //contents += mptkEvent.RealTime + "\n";

                    //Debug.Log($"mptkEvent.RealTime - lastTime:{mptkEvent.RealTime - lastTime}");

                    //lastTime = mptkEvent.RealTime;

                    if (!timeSet.Contains(mptkEvent.RealTime))
                    {
                        if (mptkEvent.Channel == playChannel)
                        {
                            timeSet.Add(mptkEvent.RealTime);

                            int _frame = Mathf.RoundToInt(mptkEvent.RealTime / 1000.0f / 0.02f);
                            int _offsetFrame = _frame - _lastFrame;

                            if(_offsetFrame > 5)
                            {
                                AllMidiEventsSet.Add(mptkEvent);
                            }

                            _lastFrame = _frame;

                            endTime = mptkEvent.RealTime;
                        }
                    }
                }
            }

            beiginFrame = Mathf.RoundToInt(beginTime / 0.02f);

            //File.WriteAllText(Path.Combine(Application.dataPath, "test.txt"), contents);
            Debug.Log($"AllMidiEvents.Count:{AllMidiEvents.Count}");
            //Debug.Log($"ind:{ind}");
            midiFilePlayer.MPTK_StartPlayAtFirstNote = true;
            midiFilePlayer.MPTK_MidiAutoRestart = false;
        }

        currentMidi = _name;

        //yield return LoadLyric(_name);

        if (isLoadBgm)
        {
            string bgmPath = Path.Combine(Application.streamingAssetsPath, Path.Combine("bgm", currentMidi + ".mp3"));
            UnityWebRequest bgmRequest = UnityWebRequestMultimedia.GetAudioClip(bgmPath, AudioType.MPEG);
            yield return bgmRequest.SendWebRequest();
            if (bgmRequest.result == UnityWebRequest.Result.Success)
            {
                currentClip = DownloadHandlerAudioClip.GetContent(bgmRequest);
            }
            else
            {
                Debug.LogError($"加载失败: {bgmRequest.error}");
            }

        }
    }

    IEnumerator LoadLyric(string _name)
    {
        string jsonStr = string.Empty;
        string _path = Path.Combine(GameConfig.Instance.GetLyricPath(), _name + ".krc");
        UnityWebRequest request = UnityWebRequest.Get(_path);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            jsonStr = request.downloadHandler.text;
        }
        else
        {
            Debug.LogError($"加载失败: {request.error}");
        }

        var strArray = jsonStr.Split('\n');

        int beiginFrame0 = 0;
        int beiginFrame = frameList[0];
        int worldCount = 0;
        int currentFrameIndex = 1;

        

        for (int n = 0;n < strArray.Length;n++)
        {
            //string input = "[19170,6130]<0,230,0>今<230,310,0>天<540,1050,0>我 <3490,190,0>寒<3680,170,0>夜<3850,300,0>里<4150,170,0>看<4320,500,0>雪<4820,400,0>飘<5220,910,0>过";
            {
                int beiginIndex = 0;
                string input = strArray[n];
                var firstNumber = ExtractFirstNumber(input);
                List<(int x, int y, int z, string word)> parsedData = ParseText(input);
                if (n == 0)
                {
                    beiginFrame0 = Mathf.RoundToInt((parsedData[0].x + firstNumber) / 1000.0f / 0.02f);
                    beiginIndex = 1;

                    lyricDic.Add(0, parsedData[0].word);
                }

                for (int i = beiginIndex; i < parsedData.Count; i++)
                {
                    var item = parsedData[i];
                    int beiginFrame_temp = Mathf.RoundToInt( (item.x + firstNumber) / 1000.0f / 0.02f);
                    int lastFrame_temp = Mathf.RoundToInt(item.y / 1000.0f / 0.02f);
                    worldCount++;

                    int leftBound = beiginFrame_temp - beiginFrame0;
                    int rightBound = (beiginFrame_temp + lastFrame_temp) - beiginFrame0;

                    Debug.Log($"frame_temp {worldCount} : {leftBound}  lastFrame : {rightBound}  word: {item.word}");

                    List<int> offsetFrameList = new List<int>();

                    for (int j = currentFrameIndex; j < frameList.Count; j++)
                    {
                        if(j == 70)
                        {
                            Debug.Log("1111111");
                        }

                        int currentFrame = frameList[j] - beiginFrame;
                        int offsetFrame = Math.Abs(leftBound - currentFrame);
                        offsetFrameList.Add(offsetFrame);

                        if (leftBound <= currentFrame && currentFrame <= rightBound)
                        {
                            lyricDic.Add(j, item.word);
                            currentFrameIndex = j + 1;
                            break;
                        }
                        else if(currentFrame > rightBound)
                        {
                            Debug.Log("超出范围");

                            int minIndex = 0;
                            int minOffset = int.MaxValue;
                            for(int offsetIndex = 0; offsetIndex < offsetFrameList.Count; offsetIndex++)
                            {
                                if(offsetFrameList[offsetIndex] < minOffset)
                                {
                                    minIndex = offsetIndex;
                                    minOffset = offsetFrameList[offsetIndex];
                                }
                            }

                            currentFrameIndex = currentFrameIndex + minIndex;
                            lyricDic.Add(currentFrameIndex, item.word);
                            currentFrameIndex = currentFrameIndex + 1;

                            break;
                        }
                    }

                    offsetFrameList.Clear();
                }
            }
        }

        foreach(var key in lyricDic.Keys)
        {
            Debug.Log($"lyricDic KEY:{key}  VALUE:{lyricDic[key]}");
        } 
    }

    public int ExtractFirstNumber(string input)
    {
        // Extract the first number in square brackets using regex
        var match = Regex.Match(input, @"\[(\d+),\d+\]");
        return match.Success ? int.Parse(match.Groups[1].Value) : 0;
    }

    public List<(int, int, int, string)> ParseText(string input)
    {
        List<(int, int, int, string)> result = new List<(int, int, int, string)>();

        // 匹配格式：`[数字,数字]<数字,数字,数字>文字`
        string pattern = @"<(\d+),(\d+),(\d+)>(\S)";
        Regex regex = new Regex(pattern);

        MatchCollection matches = regex.Matches(input);

        foreach (Match match in matches)
        {
            int x = int.Parse(match.Groups[1].Value);
            int y = int.Parse(match.Groups[2].Value);
            int z = int.Parse(match.Groups[3].Value);
            string word = match.Groups[4].Value;

            // 记录解析出的数据
            result.Add((x, y, z, word)); // 选择后面的坐标作为最终位置
        }

        return result;
    }


    public void Play(int index)
    {

        MPTKEvent NotePlaying = AllMidiEventsSet[index];

        midiCustomPlayer.Play(NotePlaying.Value);
    }

    public int GetEndFrame()
    {
        if(currentClip != null)
        {
            return Mathf.RoundToInt((currentClip.length - (endTime / 1000.0f)) / 0.02f);
        }
        else
        {
            return Mathf.RoundToInt((255f - (endTime / 1000.0f)) / 0.02f);
        }
    }

    public void Pause()
    {
        //midiFilePlayer.MPTK_Pause();

    }

    public void Stop()
    {
        //midiFilePlayer.MPTK_Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
