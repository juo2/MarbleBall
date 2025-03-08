using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MidiCustomPlayer : MonoBehaviour
{
    public AudioSource audioSource;

    private Dictionary<int, AudioSource> midiAudioDic = new Dictionary<int, AudioSource>();

    private string font = "musicbox-";
    //private string font = "musicboxglockenspiel-";

    private Dictionary<int, string> midiResourcesDic = new Dictionary<int, string>() 
    {
        {32,"C2"},
        {33,"Dd2"},
        {34,"D2"},
        {35,"Ed2"},
        {36,"E2"},
        {37,"F2"},
        {38,"Gd2"},
        {39,"G2"},
        {40,"Ad2"},
        {41,"A2"},
        {42,"Bd2"},
        {43,"B2"},
        {44,"C3"},
        {45,"Dd3"},
        {46,"D3"},
        {47,"Ed3"},
        {48,"E3"},
        {49,"F3"},
        {50,"Gd3"},
        {51,"G3"},
        {52,"Ad3"},
        {53,"A3"},
        {54,"Bd3"},
        {55,"B3"},
        {56,"C4"},
        {57,"Dd4"},
        {58,"D4"},
        {59,"Ed4"},
        {60,"E4"},
        {61,"F4"},
        {62,"Gd4"},
        {63,"G4"},
        {64,"Ad4"},
        {65,"A4"},
        {66,"Bd4"},
        {67,"B4"},
        {68,"C5"},
        {69,"Dd5"},
        {70,"D5"},
        {71,"Ed5"},
        {72,"E5"},
        {73,"F5"},
        {74,"Gd5"},
        {75,"G5"},
        {76,"Ad5"},
        {77,"A5"},
        {78,"Bd5"},
        {79,"B5"},
        {80,"C6"},
        {81,"Dd6"},
        {82,"D6"},
        {83,"Ed6"},
        {84,"E6"},
        {85,"F6"},
        {86,"Gd6"},
        {87,"G6"},
        {88,"Ad6"},
        {89,"A6"},
        {90,"Bd6"},
        {91,"B6"},
        {92,"C7"} 
    }; 

    // Start is called before the first frame update
    void Start()
    {
        foreach(var key in midiResourcesDic.Keys)
        {
            AudioClip clip = Resources.Load<AudioClip>($"audio/{font}{midiResourcesDic[key]}");
            AudioSource source = Instantiate(audioSource);
            source.gameObject.SetActive(true);
            source.transform.SetParent(transform);
            source.clip = clip;
            midiAudioDic.Add(key, source);
        }
    }

    public void Play(int midi)
    {
        if(midiAudioDic.ContainsKey(midi))
        {
            midiAudioDic[midi].Play();
        }
        else
        {
            Debug.LogError($"�Ҳ���Midi:{midi}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
