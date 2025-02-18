using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MidiCustomPlayer : MonoBehaviour
{
    public AudioSource audioSource;

    private Dictionary<int, AudioSource> midiAudioDic = new Dictionary<int, AudioSource>();

    private Dictionary<int, string> midiResourcesDic = new Dictionary<int, string>() 
    {
        {32,"musicbox-C2"},
        {33,"musicbox-Dd2"},
        {34,"musicbox-D2"},
        {35,"musicbox-Ed2"},
        {36,"musicbox-E2"},
        {37,"musicbox-F2"},
        {38,"musicbox-Gd2"},
        {39,"musicbox-G2"},
        {40,"musicbox-Ad2"},
        {41,"musicbox-A2"},
        {42,"musicbox-Bd2"},
        {43,"musicbox-B2"},
        {44,"musicbox-C3"},
        {45,"musicbox-Dd3"},
        {46,"musicbox-D3"},
        {47,"musicbox-Ed3"},
        {48,"musicbox-E3"},
        {49,"musicbox-F3"},
        {50,"musicbox-Gd3"},
        {51,"musicbox-G3"},
        {52,"musicbox-Ad3"},
        {53,"musicbox-A3"},
        {54,"musicbox-Bd3"},
        {55,"musicbox-B3"},
        {56,"musicbox-C4"},
        {57,"musicbox-Dd4"},
        {58,"musicbox-D4"},
        {59,"musicbox-Ed4"},
        {60,"musicbox-E4"},
        {61,"musicbox-F4"},
        {62,"musicbox-Gd4"},
        {63,"musicbox-G4"},
        {64,"musicbox-Ad4"},
        {65,"musicbox-A4"},
        {66,"musicbox-Bd4"},
        {67,"musicbox-B4"},
        {68,"musicbox-C5"},
        {69,"musicbox-Dd5"},
        {70,"musicbox-D5"},
        {71,"musicbox-Ed5"},
        {72,"musicbox-E5"},
        {73,"musicbox-F5"},
        {74,"musicbox-Gd5"},
        {75,"musicbox-G5"},
        {76,"musicbox-Ad5"},
        {77,"musicbox-A5"},
        {78,"musicbox-Bd5"},
        {79,"musicbox-B5"},
        {80,"musicbox-C6"},
        {81,"musicbox-Dd6"},
        {82,"musicbox-D6"},
        {83,"musicbox-Ed6"},
        {84,"musicbox-E6"},
        {85,"musicbox-F6"},
        {86,"musicbox-Gd6"},
        {87,"musicbox-G6"},
        {88,"musicbox-Ad6"},
        {89,"musicbox-A6"},
        {90,"musicbox-Bd6"},
        {91,"musicbox-B6"},
        {92,"musicbox-C7"} 
    }; 

    // Start is called before the first frame update
    void Start()
    {
        foreach(var key in midiResourcesDic.Keys)
        {
            AudioClip clip = Resources.Load<AudioClip>($"audio/{midiResourcesDic[key]}");
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
            Debug.LogError($"’“≤ªµΩMidi:{midi}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
