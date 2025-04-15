using MidiPlayerTK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
                Debug.LogError($"º”‘ÿ ß∞‹: {request.error}");
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
                Debug.LogError($"º”‘ÿ ß∞‹: {bgmRequest.error}");
            }

        }
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
