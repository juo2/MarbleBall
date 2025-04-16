using MidiPlayerTK;
using RenderHeads.Media.AVProMovieCapture;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PhysicsState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;
}

public class CameraMovement
{
    public int beiginFrame;
    public int endFrame;
    public bool isBar;
}

public class FrameVelocityOffset
{
    public float offsetX;
    public float offsetY;
    public float modifyVelocityX;
    public float modifyVelocityY;
    public Vector3 offsetRotation;
}

public partial class GameDemo : MonoBehaviour
{
    private bool isPlay = false;
    private bool isGening = false;

    private bool isEditorCamera = true;

    [SerializeField]
    private GameObject captureObj;

    [SerializeField]
    private CaptureGUI captureGUI;

    [SerializeField]
    private GameObject canvasObj;

    [SerializeField]
    private Cinemachine.CinemachineSmoothPath cinemachineSmoothPath;

    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera v_camera;

    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera v_camera_editor;

    [SerializeField]
    private Transform cameraFollowPos;

    [SerializeField]
    private Transform cameraPos;

    [SerializeField]
    private MidiHelper midiHelper;

    [SerializeField]
    private BallDemo ball;

    [SerializeField]
    private Text currentObjCountTxt;

    [SerializeField]
    private Text currentFrameTxt;

    [SerializeField]
    private Text currentOffsetFrameTxt;

    [SerializeField]
    private Text currentVelocityTxt;

    [SerializeField]
    private Text currentBouncessTxt;

    [SerializeField]
    private Text currentRotationTxt;

    [SerializeField]
    private Text currentTryingTypeTxt;

    [SerializeField]
    private Text currentContinuousCountTxt;

    [SerializeField]
    private Button loadData;

    [SerializeField]
    private Button beiginBtn;

    [SerializeField]
    private Button playBtn;


    [SerializeField]
    private Pad padSimulate;

    [SerializeField]
    private LineRendererDemo line;

    [SerializeField]
    private LineRenderer cameraProjection;

    [SerializeField]
    private LineRenderer cameraPathProjection;

    [SerializeField]
    private Transform obstaclesParent;

    [SerializeField]
    private Transform padPoolParent;

    [SerializeField]
    private Transform barPrefabParent;

    [SerializeField]
    private BarsDemo bar1;

    [SerializeField]
    private BarsCombineDemo barCombine;

    [SerializeField]
    private Transform simulateParent;

    [SerializeField]
    private Dropdown dropdown;

    [SerializeField]
    private BGMPlayer bGMPlayer;

    [SerializeField]
    List<BarsDemo> barsDemoPrefabList;

    List<BarsDemo> barsDemoList = new List<BarsDemo>();
    Stack<BarsDemo> barsDemoPool = new Stack<BarsDemo>();
    Dictionary<int ,Stack<BarsDemo>> barsLineDemoPool = new Dictionary<int, Stack<BarsDemo>>();

    private float currentTime = 0f;
    private float spacePlayTime = 3f;
    private bool spacePlay = false;

    private int m_currentFrame = 0;
    private int m_currentObjCount = -1;
    private int m_preObjCount = -1;

    private List<int> frameList = new List<int>();
    private List<CameraMovement> cameraMovementList = new List<CameraMovement>();
    private List<CameraMovement> cameraFollowMovementList = new List<CameraMovement>();

    private List<Vector3> cameraPathList = new List<Vector3>();

    private List<PhysicsState> physicsStateList = new List<PhysicsState>();

    private Dictionary<int, Pad> visiblePadDic = new Dictionary<int, Pad>();
    private int m_visibleCount = 40;

    private Stack<Pad> padPool = new Stack<Pad>();

    private Stack<BarsCombineDemo> barsCombineDemoPool = new Stack<BarsCombineDemo>();

    int playMusicFrame = 0;

    int currentOffsetIndex = 1;

    private string logPath = string.Empty;
    private string logJsonPath = string.Empty;

    Cinemachine.CinemachineTrackedDolly cinemachineTrackedDolly;

    /// <summary>
    /// 大类
    /// </summary>
    /// 
    [System.Serializable]
    public enum CurveType
    {
        None,
        HorizontalToHorizontal,
        HorizontalToHorizontal_Simulate,
        VerticalToVertical,
        VerticalToVertical_Simulate,
        HorizontalToVertical,
        VerticalToHorizontal,
        HorizontalLine,
        VerticalLine,
        HorizontalLine2,
        VerticalLine2,
        HorizontalPipline,
        VerticalPipline,
        HorizontalPipline2,
        VerticalPipline2,
        HorizontalCombine,
        VerticalCombine,
    }

    [System.Serializable]
    public enum DistanceType
    {
        None,
        Near,
        Far,
        Speed
    }

    Dictionary<int, FrameVelocityOffset> frameVelocityOffsetList = new Dictionary<int, FrameVelocityOffset>();
    Dictionary<int, Vector3> frameVelocityList = new Dictionary<int, Vector3>();
    Dictionary<int, int> cameraPosMap = new Dictionary<int, int>();
    List<Vector3> cameraFollowList = new List<Vector3>();
    string currentMidi = string.Empty;

    private int currentFrame
    {
        set
        {
            m_currentFrame = value;
            currentFrameTxt.text = $"frame:{m_currentFrame}";

            padStatusUpdate(m_currentFrame);
        }
        get
        {
            return m_currentFrame;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        cinemachineTrackedDolly = v_camera.GetCinemachineComponent<Cinemachine.CinemachineTrackedDolly>();

        Application.targetFrameRate = 60;
        Physics.autoSimulation = false;

        PhysicsState ps = new PhysicsState();
        ps.position = ball.transform.position;
        ps.rotation = ball.transform.rotation;
        ps.velocity = ball.velocity;
        ps.angularVelocity = ball.angularVelocity;
        physicsStateList.Add(ps);

        line.AddLineRenderer(ball.transform.position);
        Text label = playBtn.transform.Find("Text (Legacy)").GetComponent<Text>();

        beiginBtn.onClick.AddListener(() =>
        {
            SpaceKeyDown();

        });

        playBtn.onClick.AddListener(() =>
        {
            Physics.autoSimulation = false;
            isPlay = !isPlay;
            if (isPlay)
            {
                label.text = "pause";
                //midiHelper.Play();
            }
            else
            {
                label.text = "play";
                midiHelper.Pause();
            }
        });


        loadData.onClick.AddListener(() =>
        {
            initLogger(currentMidi);
            StartCoroutine(LoadData());
        });

        UpdateCamera();

#if UNITY_WEBGL
        currentMidi = "0105FC8C9CE6AB6B66C709F4E596C369";
#else
        string _path = Path.Combine(Application.streamingAssetsPath, "midi");
        List<string> _files = CheckForMidiFiles(_path);

        dropdown.onValueChanged.AddListener((int index) =>
        {
            currentMidi = _files[index];
        }); 

        dropdown.options = new List<Dropdown.OptionData>();
        for(int i = 0;i < _files.Count;i++)
        {
            Dropdown.OptionData optionData = new Dropdown.OptionData();
            optionData.text = _files[i];
            dropdown.options.Add(optionData);
        }
        dropdown.value = 0;
        dropdown.captionText.text = _files[0];
        currentMidi = _files[0];
#endif

        captureBase = captureObj.GetComponent<RenderHeads.Media.AVProMovieCapture.CaptureBase>();
    }


    List<string> CheckForMidiFiles(string path)
    {
        List<string> outFiles = new List<string>();
        if (Directory.Exists(path))
        {
            // 获取路径下所有文件
            string[] files = Directory.GetFiles(path, "*.mid");

            if (files.Length > 0)
            {
                // 输出所有找到的 .bytes 文件路径
                foreach (var file in files)
                {
                    Debug.Log("Found .bytes file: " + GetFileNameWithoutExtension(file));
                    outFiles.Add(GetFileNameWithoutExtension(file));
                }
            }
            else
            {
                Debug.Log("No .bytes files found in the specified directory.");
            }
        }
        else
        {
            Debug.LogError("The specified directory does not exist: " + path);
        }

        return outFiles;
    }

    string GetFileNameWithoutExtension(string filePath)
    {
        // 获取文件名（带扩展名）
        string fileName = Path.GetFileName(filePath);

        // 获取文件名并去掉扩展名（.bytes）
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

        return fileNameWithoutExtension;
    }

    void UpdateCamera()
    {
        if (isEditorCamera)
        {
            v_camera.gameObject.SetActive(false);
            v_camera_editor.gameObject.SetActive(true);
        }
        else
        {
            v_camera.gameObject.SetActive(true);
            v_camera_editor.gameObject.SetActive(false);
        }
    }

    void initLogger(string midi)
    {
        string folderPath = Path.Combine(Application.dataPath, "../1_logs");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        logPath = Path.Combine(folderPath, $"{midi}_log.txt");

        string folderPath2 = Path.Combine(Application.dataPath, "../1_jsons");
        if (!Directory.Exists(folderPath2))
        {
            Directory.CreateDirectory(folderPath2);
        }

        logJsonPath = Path.Combine(folderPath2, $"{midi}_log.json");

        File.WriteAllText(logPath,"");
    }

    void logger(string logStr)
    {
        if(logPath == string.Empty)
        {
            return;
        }
        File.AppendAllText(logPath, logStr + "\n");
    }

    void padStatusUpdate(int _frame)
    {
        int _currentCount = 0;
        bool isTrigger = false;

        currentVelocityTxt.text = string.Format("vx:{0:F4}\nvy:{1:F4}", ball.velocity.x, ball.velocity.y);

        if (frameList.Count > 0)
        {
            for(int i = 0;i < frameList.Count;i++)
            {
                if(_frame >= frameList[i])
                {
                    _currentCount++;
                    if(_frame == frameList[i])
                    {
                        isTrigger = true;
                    }

                    if(i < padDataList.Count)
                    {
                        if (visiblePadDic.TryGetValue(i,out Pad _pad))
                        {
                            PadData padData = padDataList[i];
                            ball.SetPointLight(_pad.lightIndex);

                            currentBouncessTxt.text = $"bouncess:{padData.bounciness}";
                            currentRotationTxt.text = $"rotation:{padData.rotation}";
                            currentOffsetFrameTxt.text = $"offsetFrame:{padData.offsetFrame}";
                        }
                        
                        currentObjCountTxt.text = $"count:{(i+1)}";

                        m_currentObjCount = i+1;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        if(padDataList.Count >= _currentCount)
        {
            for(int i =0;i < _currentCount;i++)
            {
                if(visiblePadDic.TryGetValue(i, out Pad _pad))
                {
                    _pad.padAfter();
                }
            }

            for(int i = _currentCount;i < padDataList.Count;i++)
            {
                if (visiblePadDic.TryGetValue(i, out Pad _pad))
                {
                    _pad.padBefore();
                }
            }

            if(isTrigger)
            {
                if (visiblePadDic.TryGetValue(_currentCount - 1, out Pad _pad))
                {
                    _pad.padTrigger();
                }
            }
        }

        if(isTrigger)
        {
            if (isGening)
            {
                if(m_currentObjCount >= m_visibleCount)
                {
                    for(int i = 0;i < m_currentObjCount - m_visibleCount; i++)
                    {
                        RecyclePadByIndex(i);
                    }

                    //Debug.Log($"m_currentObjCount - vCount:{m_currentObjCount - m_visibleCount}");
                }
            }
            else
            {
                int vCount = m_visibleCount / 2;
                int upperCount = m_currentObjCount + vCount;
                int lowCount = m_currentObjCount - vCount;

                if(upperCount > padDataList.Count)
                {
                    upperCount = padDataList.Count;
                }

                if(lowCount < 0)
                {
                    lowCount = 0;
                }

                visiblePadRange(lowCount, upperCount);
            }
        }
    }

    void visiblePadRange(int lowCount,int upperCount)
    {
        for (int i = 0; i < padDataList.Count; i++)
        {
            if (i >= lowCount && i <= upperCount)
            {
                if (!visiblePadDic.ContainsKey(i))
                {
                    PadData padData = padDataList[i];
                    Pad pad_instance = InstantByPadData(padData);
                    visiblePadDic.Add(i, pad_instance);
                }
            }
            else
            {
                RecyclePadByIndex(i);
            }
        }
    }

    Pad InstantByPadData(PadData padData,bool debug = false)
    {
        Pad pad_instance = null;
        if (padPool.Count > 0 && !debug)
        {
            pad_instance = padPool.Pop();
        }
        else
        {
            Pad padPrefab = SkinManager.Instance.GetPadPrefab();
            pad_instance = Instantiate<Pad>(padPrefab);
            pad_instance.gameObject.SetActive(true);
        }

        pad_instance.lightIndex = padData.lightIndex;
        pad_instance.inverse = padData.inverse;
        pad_instance.transform.SetParent(obstaclesParent.transform);
        pad_instance.transform.position = padData.pos;
        pad_instance.transform.localScale = Vector3.one;
        //pad_instance.wrapper.eulerAngles = new Vector3(0, 0, padData.rotation);
        pad_instance.SetRotation(padData.rotation);
        pad_instance.name = $"PAD_{padData.frame}";
        pad_instance.frame = padData.frame;
        pad_instance.mistakeCollisionTimes = 0;
        pad_instance.SetPhysicMaterialBounciness(padData.bounciness);

        pad_instance.padCollider.collisionAction = () =>
        {
            {
                if (pad_instance.frame != currentFrame)
                {
                    pad_instance.mistakeCollisionTimes++;
                    //Debug.Log($"pad_instance:{pad_instance.name}    currentFrame:{currentFrame}");
                }
            }
            
        };
        
        pad_instance.debugCenter = padData.debugCenter;
        pad_instance.debugSize = padData.debugSize;
        pad_instance.debugBestPos = padData.debugBestPos;

        pad_instance.InitLineRenderer();
        pad_instance.SetLineRenererColor(padData.debugColor);

        return pad_instance;
    }

    void RecyclePadByIndex(int _index)
    {
        if(visiblePadDic.TryGetValue(_index,out Pad _pad))
        {
            RecyclePad(_pad);
            visiblePadDic.Remove(_index);
        }
    }

    void RecyclePad(Pad _pad)
    {
        _pad.transform.SetParent(padPoolParent);
        _pad.transform.position = new Vector3(9999, 9999, 9999);
        _pad.padCollider.collisionAction = null;

        padPool.Push(_pad);
    }

    void RecycleBarsCombine(BarsCombineDemo _bars)
    {
        _bars.transform.SetParent(padPoolParent);
        _bars.transform.position = new Vector3(9999, 9999, 9999);
        _bars.padCollider1.collisionAction = null;
        _bars.padCollider2.collisionAction = null;
        _bars.padCollider3.collisionAction = null;
        _bars.updateVirtualBars(false);
        _bars.SetScaleX(1.0f);

        barsCombineDemoPool.Push(_bars);
    }

    void RecycleBars(BarsDemo _bars)
    {
        _bars.transform.SetParent(padPoolParent);
        _bars.transform.position = new Vector3(9999, 9999, 9999);
        barsDemoPool.Push(_bars);
    }

    void RecycleLineBars(BarsDemo _bars)
    {
        _bars.transform.SetParent(padPoolParent);
        _bars.transform.position = new Vector3(9999, 9999, 9999);

        if(!barsLineDemoPool.ContainsKey(_bars.prefabIndex))
        {
            barsLineDemoPool[_bars.prefabIndex] = new Stack<BarsDemo>();
        }
        barsLineDemoPool[_bars.prefabIndex].Push(_bars);
    }

    BarsDemo InstantBars(Vector3 framePos, Vector3 frameDir,BarsDemo barPerfab, int _frame = 0)
    {
        BarsDemo bars_instance = null;
        if (barsDemoPool.Count > 0)
        {
            bars_instance = barsDemoPool.Pop();
        }
        else
        {
            bars_instance = Instantiate<BarsDemo>(barPerfab);
            bars_instance.gameObject.SetActive(true);
        }

        bars_instance.transform.SetParent(obstaclesParent.transform);
        bars_instance.transform.position = framePos;
        bars_instance.transform.localScale = Vector3.one;

        if (frameDir.x > 0)
        {
            bars_instance.transform.eulerAngles = Vector3.zero;
        }
        else
        {
            bars_instance.transform.eulerAngles = new Vector3(0, 180f, 0);
        }

        if(bars_instance.followRotation)
        {
            float angle = Vector3.Angle(frameDir, Vector3.down);
            bars_instance.transform.eulerAngles = new Vector3(bars_instance.transform.eulerAngles.x, bars_instance.transform.eulerAngles.y, angle);
        }

        bars_instance.gameObject.SetActive(true);
        bars_instance.name = $"BARS_{_frame}_{barPerfab.name}";
        barsDemoList.Add(bars_instance);

        return bars_instance;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(canvasObj.activeSelf)
            {
                canvasObj.SetActive(false);
            }
            else
            {
                canvasObj.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (captureObj.activeSelf)
            {
                captureObj.SetActive(false);
            }
            else
            {
                captureObj.SetActive(true);
                captureGUI.enabled = true;
            }

        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (line.gameObject.activeSelf)
            {
                line.gameObject.SetActive(false);
            }
            else
            {
                line.gameObject.SetActive(true);
            }
            
        }


        if (Input.GetKeyDown(KeyCode.W))
        {
            if (cameraProjection.gameObject.activeSelf)
            {
                cameraProjection.gameObject.SetActive(false);
            }
            else
            {
                cameraProjection.gameObject.SetActive(true);
            }
           
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (cameraPathProjection.gameObject.activeSelf)
            {
                cameraPathProjection.gameObject.SetActive(false);
            }
            else
            {
                cameraPathProjection.gameObject.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpaceKeyDown();
        }

        if (spacePlay)
        {
            currentTime += Time.deltaTime;

            if(currentTime > spacePlayTime)
            {
                currentTime = 0;
                isPlay = true ;
                spacePlay = false;

                //bGMPlayer.PlayBgm1();
            }
        }

    }

    private void FixedUpdate()
    {
        if (isPlay)
        {
            if (currentFrame < physicsStateList.Count)
            {
                ball.ResetState(physicsStateList[currentFrame]);
                ball.PlayEffet();
                currentFrame++;
                
                if(m_preObjCount != m_currentObjCount)
                {
                    if(m_currentObjCount - 1 == 0)
                    {
                        bGMPlayer.PlayBgm(midiHelper.currentMidi, midiHelper.beginTime);
                        
                        logger($"offsetTime:{ Time.time - recordPreTime}");
                        logger($"beginTime:{midiHelper.beginTime}");
                    }
                    midiHelper.Play(m_currentObjCount-1);
                }
                m_preObjCount = m_currentObjCount;
            }
            else
            {
                isPlay = false;
                Text label = playBtn.transform.Find("Text (Legacy)").GetComponent<Text>();
                label.text = "play";
            }

            MoveCamera(currentFrame);
           
        }
        else
        {
            ball.StopEffect();
        }
    }

    void MoveCamera(int frame)
    {
        if (frame < cameraPathList.Count)
        {
            cameraPos.transform.position = cameraPathList[frame];
        }

        frame = frame + 40;

        if (frame < cameraFollowList.Count)
        {
            cameraFollowPos.transform.position = cameraFollowList[frame];
        }

        if(frame < cameraPathList.Count )
            cinemachineTrackedDolly.m_PathPosition = frame;
    }
}
