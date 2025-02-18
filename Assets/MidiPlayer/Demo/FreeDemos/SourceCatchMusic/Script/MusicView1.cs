using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
using System;
using UnityEngine.Events;
using DemoMPTK;
using System.IO;

namespace MPTKDemoCatchMusic
{
    /// <summary>
    /// CatchMusic 示例的主类。
    /// - 启动 MidiFilePlayer 并定义 OnEventNotesMidi 来捕获每个 MIDI 事件（无需播放）。
    /// - 为每个 MIDI 事件创建 Unity 的 GameObject（仅针对 NoteOn 和乐器的 PatchChange）。
    /// </summary>
    public class MusicView1 : MonoBehaviour
    {

        private string logFilePath = Path.Combine(Application.dataPath, "TwinkleTwinkleLittleStar.txt");
        string logEntry = string.Empty;
        
        HashSet<float> hashMap = new HashSet<float>();

        //public static float Speed = 15f;
        //public Camera Cam;

        ///// <summary>
        ///// 在 Unity 编辑器的层级视图中必须存在一个 MidiFilePlayer 预制件，并在 MusicView GameObject 检视器中关联。
        ///// 在这个示例中，MidiFilePlayer 被静音（发送 MIDI 到合成器的功能被禁用）。
        ///// 它仅用于通过 MIDI 播放器的事件 OnEventNotesMidi 接收 MIDI 事件（见下文）。
        ///// </summary>
        public MidiFilePlayer midiFilePlayer;

        ///// <summary>
        ///// 在 Unity 编辑器的层级视图中必须存在一个 MidiStreamPlayer 预制件，并在 MusicView GameObject 检视器中关联。
        ///// MidiStreamPlayer 的引用会被关联到为每个 MIDI 事件创建的 NoteView 和 ControlView GameObject。
        ///// 当 GameObject 到达区域末尾时，MIDI 事件会通过 MidiStreamPlayer 播放。
        ///// </summary>
        //public MidiStreamPlayer midiStreamPlayer;

        //public static Color ButtonColor = new Color(.7f, .9f, .7f, 1f);

        ///// <summary>
        ///// 为从 MidiFilePlayer 播放器接收到的每个 MIDI 音符创建的 GameObject。使用 Instantiate<NoteView>() 创建。
        ///// 包含：
        ///// - 关联的音符
        ///// - 用于通过 MIDI 合成器播放音符的 MidiStreamPlayer
        ///// - 一些图形信息和当前的位置
        ///// </summary>
        //public NoteView NoteDisplay;

        ///// <summary>
        ///// 为从 MidiFilePlayer 播放器接收到的每个 MIDI 控制器创建的 GameObject。使用 Instantiate<NoteView>() 创建。
        ///// 包含：
        ///// - 关联的音符
        ///// - 用于通过 MIDI 合成器播放音符的 MidiStreamPlayer
        ///// - 一些图形信息和当前的位置
        ///// </summary>
        //public ControlView ControlDisplay;

        //public Collide Collider;
        //public GameObject Plane;
        //public float minZ, maxZ, minX, maxX;
        //public float LastTimeCollider;
        //public float DelayCollider = 5;
        //public float FirstDelayCollider = 20;
        //public Material MatNewNote;
        //public Material MatNewController;

        // 统计平面中每个 z 位置的 GameObject 数量。用于堆叠它们。
        int[] countZ;

        public void EndLoadingSF()
        {
            Debug.Log("SoundFont 加载完成，MPTK 准备就绪");
            Debug.Log("   加载 SoundFont 所需时间: " + Math.Round(MidiPlayerGlobal.MPTK_TimeToLoadSoundFont.TotalSeconds, 3).ToString() + " 秒");
            Debug.Log("   加载样本所需时间: " + Math.Round(MidiPlayerGlobal.MPTK_TimeToLoadWave.TotalSeconds, 3).ToString() + " 秒");
            Debug.Log("   预设加载数量: " + MidiPlayerGlobal.MPTK_CountPresetLoaded);
            Debug.Log("   样本加载数量: " + MidiPlayerGlobal.MPTK_CountWaveLoaded);
        }

        void Start()
        {
            //if (!HelperDemo.CheckSFExists()) return;

            //// Unity 平面的默认大小
            //float planSize = 10f;

            //minZ = Plane.transform.localPosition.z - Plane.transform.localScale.z * planSize / 2f;
            //maxZ = Plane.transform.localPosition.z + Plane.transform.localScale.z * planSize / 2f;

            //minX = Plane.transform.localPosition.x - Plane.transform.localScale.x * planSize / 2f;
            //maxX = Plane.transform.localPosition.x + Plane.transform.localScale.x * planSize / 2f;

            if (midiFilePlayer != null)
            {
                // 未定义监听器。通过脚本设置监听器。当 MIDI 文件中读取到新的音符时会调用 NotesToPlay。
                Debug.Log("MusicView: 通过脚本设置 MidiFilePlayer.OnEventNotesMidi 事件监听器（见 MusicView.cs）。也可以通过检视器设置。");
                midiFilePlayer.OnEventNotesMidi.AddListener(NotesToPlay);
            }
            else
                Debug.Log("未检测到 MidiFilePlayer 预制件。请将其添加到层级视图中，并在 MusicView 的检视器中定义。");


            //logEntry += "1111\n";

            logFilePath = Path.Combine(Application.dataPath, "TwinkleTwinkleLittleStar.txt");

            Debug.Log($"logFilePath:{logFilePath}");
            //File.WriteAllLines(logFilePath, new string[] { logEntry });
            hashMap = new HashSet<float>();
        }

        /// <summary>@brief
        /// 当 MIDI 阅读器准备好一组 MIDI 事件用于播放时调用。
        /// 播放事件会延迟，直到它们“掉落”。详见 NoteView.cs 和 MControlView.cs。
        /// </summary>
        /// <param name="notes"></param>
        public void NotesToPlay(List<MPTKEvent> notes)
        {
            // 统计平面中每个 z 位置的 GameObject 数量。用于堆叠它们。
            //countZ = new int[Convert.ToInt32(maxZ - minZ) + 1];

            //Debug.Log(midiFilePlayer.MPTK_PlayTime.ToString() + " count:" + notes.Count);
            foreach (MPTKEvent mptkEvent in notes)
            {
                switch (mptkEvent.Command)
                {
                    case MPTKCommand.NoteOn:
                        Debug.Log($"Time.frameCount:{Time.frameCount}  RealTime:{mptkEvent.RealTime}");
                        //Debug.Log($"NoteOn Channel:{mptkEvent.Channel}  Preset index:{midiStreamPlayer.MPTK_ChannelPresetGetIndex(mptkEvent.Channel)}  Preset name:{midiStreamPlayer.MPTK_ChannelPresetGetName(mptkEvent.Channel)}");
                        //if (mptkEvent.Value > 40 && mptkEvent.Value < 100)// && note.Channel==1)
                        //{
                        //    // Z 位置根据音符值设置：mptkEvent.Value
                        //    float z = Mathf.Lerp(minZ, maxZ, (mptkEvent.Value - 40) / 60f);
                        //    countZ[Convert.ToInt32(z - minZ)]++;
                        //    // Y 位置根据 z 位置的对象数量设置
                        //    Vector3 position = new Vector3(maxX, 2 + countZ[Convert.ToInt32(z - minZ)] * 4f, z);
                        //    // 实例化一个 GameObject，用于表示这个 MIDI 事件在 3D 世界中的存在
                        //    NoteView noteview = Instantiate<NoteView>(NoteDisplay, position, Quaternion.identity);
                        //    noteview.gameObject.SetActive(true);
                        //    noteview.hideFlags = HideFlags.HideInHierarchy;
                        //    noteview.midiStreamPlayer = midiStreamPlayer;
                        //    noteview.noteOn = mptkEvent; // MIDI 事件附加到 GameObject，将稍后播放
                        //    noteview.gameObject.GetComponent<Renderer>().material = MatNewNote;
                        //    // 详见 noteview.cs: update() 会沿着平面移动音符，直到它们掉落，然后播放
                        //    noteview.zOriginal = position.z;

                        //    if (!NoteView.FirstNotePlayed)
                        //        PlaySound();
                        //}

                        hashMap.Add(mptkEvent.RealTime);
                        logEntry = "";
                        foreach (var t in hashMap)
                        {
                            logEntry += $"{t}\n";
                        }

                        File.WriteAllLines(logFilePath,new string[] { logEntry });

                        break;

                    case MPTKCommand.PatchChange:
                        //{
                        //    //Debug.Log($"PatchChange Channel:{note.Channel}  Preset index:{note.Value}");
                        //    // Z 位置根据音符值设置：mptkEvent.Value
                        //    float z = Mathf.Lerp(minZ, maxZ, mptkEvent.Value / 127f);
                        //    // Y 位置根据 z 位置的对象数量设置
                        //    countZ[Convert.ToInt32(z - minZ)]++;
                        //    Vector3 position = new Vector3(maxX, 8f + countZ[Convert.ToInt32(z - minZ)] * 4f, z);
                        //    // 实例化一个 GameObject，用于表示这个 MIDI 事件在 3D 世界中的存在
                        //    ControlView patchview = Instantiate<ControlView>(ControlDisplay, position, Quaternion.identity);
                        //    patchview.gameObject.SetActive(true);
                        //    patchview.hideFlags = HideFlags.HideInHierarchy;
                        //    patchview.midiStreamPlayer = midiStreamPlayer;
                        //    patchview.instrumentChange = mptkEvent; // MIDI 事件附加到 GameObject，将稍后播放
                        //    patchview.gameObject.GetComponent<Renderer>().material = MatNewController;
                        //    patchview.zOriginal = position.z;
                        //}
                        break;
                }
            }
        }

        private void PlaySound()
        {
            // 在等待音符时播放一些声音，当第一个音符播放后将被禁用...
            //! [Example PlayNote]
            //midiStreamPlayer.MPTK_PlayEvent
            //(
            //    new MPTKEvent()
            //    {
            //        Channel = 9,
            //        Duration = 999999,
            //        Value = 48,
            //        Velocity = 100
            //    }
            //);
            //! [Example PlayNote]
        }

        /// <summary>
        /// 快速 UI
        /// </summary>
        void OnGUI()
        {
            //int startx = 5;
            //int starty = 90;
            //int maxwidth = Screen.width;

            //if (!HelperDemo.CheckSFExists()) return;

            //if (midiFilePlayer != null)
            //{
            //    GUILayout.BeginArea(new Rect(startx, starty, maxwidth, 200));

            //    GUILayout.BeginHorizontal();
            //    if (GUILayout.Button(new GUIContent("上一首", ""), GUILayout.Width(150)))
            //    {
            //        Clear();
            //        midiFilePlayer.MPTK_Previous();
            //    }
            //    if (GUILayout.Button(new GUIContent("下一首", ""), GUILayout.Width(150)))
            //    {
            //        Clear();
            //        midiFilePlayer.MPTK_Next();
            //    }
            //    if (GUILayout.Button(new GUIContent("清除", ""), GUILayout.Width(150)))
            //        Clear();
            //    GUILayout.EndHorizontal();
            //    GUILayout.Label("MIDI '" + midiFilePlayer.MPTK_MidiName + (midiFilePlayer.MPTK_IsPlaying ? "' 正在播放" : " 未播放"));
            //    GUILayout.BeginHorizontal();
            //    GUILayout.Label("MIDI 位置:", GUILayout.Width(100));
            //    double currentposition = Math.Round(midiFilePlayer.MPTK_Position / 1000d, 2);
            //    double newposition = Math.Round(GUILayout.HorizontalSlider((float)currentposition, 0f, (float)midiFilePlayer.MPTK_DurationMS / 1000f, GUILayout.Width(200)), 2);
            //    if (newposition != currentposition)
            //        midiFilePlayer.MPTK_Position = newposition * 1000d;
            //    GUILayout.EndHorizontal();

            //    GUILayout.BeginHorizontal();
            //    GUILayout.Label("音乐速度:", GUILayout.Width(100));
            //    float speed = GUILayout.HorizontalSlider(midiFilePlayer.MPTK_Speed, 0.1f, 5f, GUILayout.Width(200));
            //    if (speed != midiFilePlayer.MPTK_Speed) midiFilePlayer.MPTK_Speed = speed;
            //    GUILayout.EndHorizontal();

            //    GUILayout.BeginHorizontal();
            //    GUILayout.Label("音符速度:", GUILayout.Width(100));
            //    Speed = GUILayout.HorizontalSlider(Speed, 5f, 20f, GUILayout.Width(200));
            //    GUILayout.EndHorizontal();

            //    GUILayout.BeginHorizontal();
            //    GUILayout.Label("相机 Y:", GUILayout.Width(100));
            //    float y = GUILayout.HorizontalSlider(Cam.transform.position.y, 50f, 150f, GUILayout.Width(200));
            //    if (y != Cam.transform.position.y)
            //        Cam.transform.Translate(new Vector3(0, y - Cam.transform.position.y, 0), Space.World);
            //    GUILayout.EndHorizontal();

            //    GUILayout.BeginHorizontal();
            //    GUILayout.Label("相机 X:", GUILayout.Width(100));
            //    float x = GUILayout.HorizontalSlider(Cam.transform.position.x, -50f, 50f, GUILayout.Width(200));
            //    if (x != Cam.transform.position.x)
            //        Cam.transform.Translate(new Vector3(x - Cam.transform.position.x, 0, 0), Space.World);
            //    GUILayout.EndHorizontal();

            //    GUILayout.Label("注意音符交通拥堵！！！");

            //    GUILayout.EndArea();
            //}
        }

        /// <summary>@brief
        /// 移除屏幕上的所有 GameObject 音符
        /// </summary>
        public void Clear()
        {
            NoteView[] components = GameObject.FindObjectsByType<NoteView>(FindObjectsSortMode.None);
            foreach (NoteView noteview in components)
            {
                if (noteview.enabled)
                    //Debug.Log("destroy " + ut.name);
                    DestroyImmediate(noteview.gameObject);
            }
        }

        void Update()
        {
            //if (midiFilePlayer != null && midiFilePlayer.MPTK_IsPlaying)
            //{
            //    // 随机生成碰撞体以改变音乐，增加趣味性！
            //    float time = Time.realtimeSinceStartup - LastTimeCollider;
            //    if (time > DelayCollider + FirstDelayCollider)
            //    {
            //        FirstDelayCollider = 0;
            //        LastTimeCollider = Time.realtimeSinceStartup;

            //        float zone = 10;
            //        Vector3 position = new Vector3(UnityEngine.Random.Range(minX + zone, maxX - zone), -5, UnityEngine.Random.Range(minZ + zone, maxZ - zone));
            //        // 实例化碰撞体（球体），与音符和补丁变化交互。
            //        Collide n = Instantiate<Collide>(Collider, position, Quaternion.identity);
            //        n.gameObject.SetActive(true);
            //        n.hideFlags = HideFlags.HideInHierarchy;
            //    }
            //}
        }
    }
}
