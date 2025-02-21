using MidiPlayerTK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
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
    string currentMidi = string.Empty;

    // Start is called before the first frame update
    void Awake()
    {
        //midiFilePlayer.MPTK_Channels[1].ForcedPreset = 10;
        //midiFilePlayer.MPTK_MidiName = "audio.mid";
        //midiFilePlayer.MPTK_MidiName = "1_蒲公英的约定";
        //midiFilePlayer.MPTK_MidiName = "2_一路生花-片段";
        //midiFilePlayer.MPTK_MidiName = "2_离别开出花-片段";
        //midiFilePlayer.MPTK_MidiName = "2_audio";

        //midiFilePlayer.MPTK_MidiName = "2_今天"; //209
        //midiFilePlayer.MPTK_MidiName = "2_十七岁"; //173
        //midiFilePlayer.MPTK_MidiName = "2_守望";
        //midiFilePlayer.MPTK_MidiName = "2_小城夏天";
        //midiFilePlayer.MPTK_MidiName = "2_小苹果";
        //midiFilePlayer.MPTK_MidiName = "2_青藏高原";
        //midiFilePlayer.MPTK_Load();

        //Load("2_今天.bytes");
    }

    public void Load(string _name)
    {
        if(currentMidi != _name)
        {
            timeSet.Clear();
            AllMidiEventsSet.Clear();

            string _path = Path.Combine(Application.streamingAssetsPath + "/midi", _name + ".mid");
            byte[] _bytes = File.ReadAllBytes(_path);

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
                        }
                    }
                }
            }

            //File.WriteAllText(Path.Combine(Application.dataPath, "test.txt"), contents);
            Debug.Log($"AllMidiEvents.Count:{AllMidiEvents.Count}");
            //Debug.Log($"ind:{ind}");
            midiFilePlayer.MPTK_StartPlayAtFirstNote = true;
            midiFilePlayer.MPTK_MidiAutoRestart = false;
        }

        currentMidi = _name;
    }


    public void Play(int index)
    {
        //midiFilePlayer.MPTK_Play(true);
        //midiFilePlayer.MPTK_Channels[playChannel].ForcedPreset = 11;

        //MPTKEvent NotePlaying = new MPTKEvent()
        //{
        //    Command = MPTKCommand.NoteOn,
        //    Value = 60, // note to played, ex 60=C5. Use the method from class HelperNoteLabel to convert to string
        //    Channel = 0,
        //    Duration = Convert.ToInt64(1 * 1000f), // millisecond, -1 to play indefinitely
        //    Velocity = 100, // Sound can vary depending on the velocity
        //    Delay = Convert.ToInt64(0 * 1000f),
        //};

        MPTKEvent NotePlaying = AllMidiEventsSet[index];

        //Debug.Log($"NotePlaying:{NotePlaying}");

        //midiStreamPlayer.MPTK_Channels[playChannel].ForcedPreset = 11;
        //midiStreamPlayer.MPTK_PlayEvent(NotePlaying);
        midiCustomPlayer.Play(NotePlaying.Value);
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
