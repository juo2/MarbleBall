using MidiPlayerTK;
using RenderHeads.Media.AVProMovieCapture;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.CinemachineSmoothPath;

[System.Serializable]
public class PhysicsState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;
}

public class BarTransform
{
    public BarPrefabs barPrefab;
    public BarsDemo barsDemo;
}

public class CameraMovement
{
    public int beiginFrame;
    public int endFrame;
    public bool isBar;
}

[System.Serializable]
public class BarPrefabs
{
    public int prefabIndex;
    public int barsAppearFrame;
    public int barsMaxFrame;
    public int barsFrontFrame;
    public int barsFallFrame;
}


[System.Serializable]
public class BarPrefabsData
{
    public BarPrefabs[] barPrefabs;
}

public class FrameVelocityOffset
{
    public float offsetX;
    public float offsetY;
    public float modifyVelocityX;
    public float modifyVelocityY;
    public Vector3 offsetRotation;
}

[System.Serializable]
public class BarConfig
{
    public int prefabIndex;
    public float beiginOffset;
    public float endOffset;
    public float modifyVelocityX;
    public float modifyVelocityY;
    public int remainFrame;
}

[System.Serializable]
public class BarsConfig
{
    public List<BarConfig> BarConfigList;
}

[System.Serializable]
public class Config
{
    public float gravityY;
    public int continuousTryCount;
    public Vector3 initialVelocity;
    public List<float> h_bouncinessPriorityList;
    public List<float> v_bouncinessPriorityList;
    public List<float> h_rotationList;
    public List<float> v_rotationList1;
    public List<float> v_rotationList2;

    public Vector2 horizontalCenter1;
    public Vector2 horizontalCenter2;
    public Vector2 horizontalBestPos1;
    public Vector2 horizontalBestPos2;
    public Vector2 horizontalSize;

    public Vector2 verticalCenter1;
    public Vector2 verticalCenter2;
    public Vector2 verticalBestPos1;
    public Vector2 verticalBestPos2;
    public Vector2 verticalSize;
}

public partial class GameDemo : MonoBehaviour
{
    public class BestPoint
    {
        public List<PhysicsState> physicsStates;
        public float rotation;
        public float bounciness;
        public float minDistance = float.MaxValue;
        public CurveType curveType = CurveType.None;
        public Vector3 center;
        public Vector3 size;
        public Vector3 bestPos;
    }

    private bool isPlay = false;
    private bool isGening = false;

    private bool isEditorCamera = true;

    [SerializeField]
    private InputField inputField;

    [SerializeField]
    private GameObject captureObj;

    [SerializeField]
    private CaptureGUI captureGUI;

    [SerializeField]
    private GameObject canvasObj;

    [SerializeField]
    private Cinemachine.CinemachineSmoothPath cinemachineSmoothPath;

    [SerializeField]
    private Camera follow_camera;

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
    private Button startGenerating;

    [SerializeField]
    private Button loadData;

    [SerializeField]
    private Button saveData;

    [SerializeField]
    private Button cameraBtn;

    [SerializeField]
    private Button beiginBtn;

    [SerializeField]
    private Button playBtn;

    [SerializeField]
    private Button nextFrameBtn;

    [SerializeField]
    private Button preFrameBtn;

    [SerializeField]
    private Button nextObjBtn;

    [SerializeField]
    private Button preObjBtn;

    [SerializeField]
    private Button simulateBtn;

    [SerializeField]
    private Button simulateV2V;

    [SerializeField]
    private Button simulateV2H;

    [SerializeField]
    private Button simulateV2P;

    [SerializeField]
    private Button simulateH2H;

    [SerializeField]
    private Button simulateH2V;

    [SerializeField]
    private Button simulateH2P;

    [SerializeField]
    private Button boundsBtn;

    [SerializeField]
    private Pad padPrefab;

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
    public TextAsset physicBarPrefabJson;

    [SerializeField]
    private BarsDemo bar1;

    [SerializeField]
    private BarsCombineDemo barCombine;

    [SerializeField]
    private BarsScaleDemo barScale;

    [SerializeField]
    private Transform simulateParent;

    [SerializeField]
    private Dropdown dropdown;

    //List<BarsDemo> barsDemoPrefabList = new List<BarsDemo>();
    List<BarsDemo> barsDemoList = new List<BarsDemo>();
    Stack<BarsDemo> barsDemoPool = new Stack<BarsDemo>();
    Dictionary<int ,Stack<BarsDemo>> barsLineDemoPool = new Dictionary<int, Stack<BarsDemo>>();
    List<BarsScaleDemo> barsScaleDemoList = new List<BarsScaleDemo>();
    private BarsCombineDemo currentBarsCombine;
    private BarsDemo currentBars;
    private BarsDemo currentLineBars;

    private BarsScaleDemo currentBarsScale;

    private Vector3 initialVelocity = new Vector3(1f, 0, 0);
    private float barsAppearMinOffset = float.MaxValue;

    private float currentTime = 0f;
    private float spacePlayTime = 3f;
    private bool spacePlay = false;

    //private int barsAppearFrame = 40;
    //private int barsFrontFrame = 25;
    //private int barsFallFrame = 5;
    private int m_currentFrame = 0;
    private int m_currentObjCount = -1;
    private int m_preObjCount = -1;
    //private int m_invisibleCount = 20;
    //private int m_beiginInvisibleCount = 30;

    //private List<float> floatList = new List<float>();  // 用于存储读取的浮动数值
    private List<int> frameOffsetList = new List<int>();
    private List<int> frameList = new List<int>();
    private List<CameraMovement> cameraMovementList = new List<CameraMovement>();
    private List<CameraMovement> cameraFollowMovementList = new List<CameraMovement>();

    private List<Vector3> cameraPathList = new List<Vector3>();

    private List<PhysicsState> physicsStateList = new List<PhysicsState>();
    private List<BarsDemo> BarsDemoPrefabList = new List<BarsDemo>();

    private Dictionary<int, Pad> visiblePadDic = new Dictionary<int, Pad>();
    private int m_visibleCount = 40;

    private Stack<Pad> padPool = new Stack<Pad>();

    private List<Pad> showPadList = new List<Pad>();

    private Stack<BarsCombineDemo> barsCombineDemoPool = new Stack<BarsCombineDemo>();

    int playMusicFrame = 0;

    bool g_isPass = false;

    //int CorrectionTime = 11;
    float defalutBounciness = 0f;
    float verticalBounciness = 0f;

    int LOOPCOUNT = 95;
    int currentOffsetIndex = 1;
    bool isVertical = false;
    Pad currentPad = null;

    private string logPath = string.Empty;

    Vector2 horizontalCenter1 = new Vector2(0.15f, -0.05f);
    Vector2 horizontalCenter2 = new Vector2(-0.15f, -0.05f);
    Vector2 horizontalBestPos1 = new Vector2(0.15f, 0f);
    Vector2 horizontalBestPos2 = new Vector2(-0.15f, 0f);

    Vector2 horizontalCenter3 = new Vector2(0.15f,-0.13f);
    Vector2 horizontalCenter4 = new Vector2(-0.15f, -0.13f);
    Vector2 horizontalBestPos3 = new Vector2(0.15f,-0.08f);
    Vector2 horizontalBestPos4 = new Vector2(-0.15f, -0.08f);

    Vector2 horizontalSize = new Vector2(0.1f, 0.1f);

    Vector2 verticalCenter1 = new Vector2(0.2f, -0.16f);
    Vector2 verticalCenter2 = new Vector2(-0.2f, -0.16f);
    Vector2 verticalBestPos1 = new Vector2(0.17f, -0.13f);
    Vector2 verticalBestPos2 = new Vector2(-0.17f, -0.13f);

    //Vector2 verticalCenter3 = new Vector2(0.15f, -0.12f);
    //Vector2 verticalCenter4 = new Vector2(-0.15f, -0.12f);
    //Vector2 verticalBestPos3 = new Vector2(0.13f, -0.1f);
    //Vector2 verticalBestPos4 = new Vector2(-0.13f, -0.1f);

    Vector2 verticalSize = new Vector2(0.1f, 0.1f);

    CameraMovement currentCameraMovement = new CameraMovement();
    Cinemachine.CinemachineTrackedDolly cinemachineTrackedDolly;

    string config_path = string.Empty;

    Dictionary<int, List<BestPoint>> BestPointDic = new Dictionary<int, List<BestPoint>>();

    List<float> h_bouncinessPriorityList = new List<float>()
    {
        0.3f,
        0.4f,
        0.5f,
        0.2f,
        //0.6f,
        //0.7f,
        //0.8f,
        //0.9f,
        //1.0f,
        //0.1f,
        //0.0f,
    };

    List<float> h_bouncinessPriorityList1 = new List<float>()
    {
        0.3f,
        0.4f,
        0.5f,
        0.2f,
        0.6f,
        //0.7f,
        //0.8f,
        //0.9f,
        //1.0f,
        //0.1f,
        //0.0f,
    };

    List<float> h_rotationList1 = new List<float>()
    {
         0f,
         2f,
        -2f,
         4f,
        -4f,
       //  6f,
       // -6f,
       //  8f,
       // -8f,
       // 10f,
       //-10f,
    };

    List<float> h_bouncinessPriorityList2 = new List<float>()
    {
        0.6f,
        0.7f,
        0.5f,
        //0.4f,
        //0.8f,
    };

    List<float> h_rotationList2 = new List<float>()
    {
         0f,
         2f,
        -2f,
         4f,
        -4f,
    };

    List<float> v_bouncinessPriorityList = new List<float>()
    {
        0.3f,
        0.4f,
        0.5f,
        0.6f,
        //0.7f,
        //0.8f,
        //0.9f,
        //1.0f,
        //0.2f,
        //0.1f,
        //0.0f,
    };

    List<float> v_rotationList1 = new List<float>()
    {
        30f,
        35f,
        40f,
        45f,
        50f,
        55f,
        60f,
        65f,
        70f,
        75f,
        80f,
        85f,
        90f,
    };

    List<float> v_rotationList2 = new List<float>()
    {
        -30f,
        -35f,
        -40f,
        -45f,
        -50f,
        -55f,
        -60f,
        -65f,
        -70f,
        -75f,
        -80f,
        -85f,
        -90f,
    };

    List<float> v_bouncinessPriorityList1 = new List<float>()
    {
        0.6f,
        0.7f,
        0.8f,
        0.9f,
        1.0f,
        0.5f,
        0.4f,
        0.3f,
        //0.2f,
        //0.1f,
        //0.0f,
    };

    List<float> v_rotationList3 = new List<float>()
    {
        60f,
        65f,
        70f,
        75f,
        80f,
        85f,
    };

    List<float> v_rotationList4 = new List<float>()
    {
        -60f,
        -65f,
        -70f,
        -75f,
        -80f,
        -85f,
    };

    /// <summary>
    /// 大类
    /// </summary>
    /// 
    [System.Serializable]
    public enum CurveType
    {
        None,
        HorizontalToHorizontal,
        VerticalToVertical,
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
    }

    [System.Serializable]
    public enum DistanceType
    {
        None,
        Near,
        Far,
        Speed
    }

    private enum PassType
    {
        None,
        greater,
        less,
        pass,
    }

    CurveType curveType = CurveType.None;
    DistanceType distanceType = DistanceType.None;
    PassType passType = PassType.None;

    Dictionary<int, FrameVelocityOffset> frameVelocityOffsetList = new Dictionary<int, FrameVelocityOffset>();
    Dictionary<int, Vector3> frameVelocityList = new Dictionary<int, Vector3>();
    Dictionary<int, int> cameraPosMap = new Dictionary<int, int>();
    List<Vector3> cameraFollowList = new List<Vector3>();

    int beforeBarbeiginFrame = -1;
    int beforeBarEndFrame = -1;
    int preBarBeginFrame = -1;
    int preBarEndFrame = -1;
    Vector3 preBarBeiginPos = Vector3.zero;
    Vector3 preBarEndPos = Vector3.zero;
    string currentMidi = string.Empty;
    BarsConfig barsConfig;

    int spBarsCount = 2;
    float midiFacter = 1f;

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

    float gravityY = -5f;

    HashSet<CurveType> CurveTypeRecord = new HashSet<CurveType>();
    List<CurveType> CurveTypePriority = new List<CurveType>();

    List<BarsCurve> barsCurveList = new List<BarsCurve>();
    List<PadCurve> padCurveList = new List<PadCurve>();

    // Start is called before the first frame update
    void Start()
    {
        //LoadConfig();

        cinemachineTrackedDolly = v_camera.GetCinemachineComponent<Cinemachine.CinemachineTrackedDolly>();

        defalutBounciness = h_bouncinessPriorityList1[0];
        verticalBounciness = v_bouncinessPriorityList[0];


        Application.targetFrameRate = 60;
        Physics.gravity = new Vector3(0, gravityY, 0);
        Physics.autoSimulation = false;

        PhysicsState ps = new PhysicsState();
        ps.position = ball.transform.position;
        ps.rotation = ball.transform.rotation;
        ps.velocity = ball.velocity;
        ps.angularVelocity = ball.angularVelocity;
        physicsStateList.Add(ps);
        line.AddLineRenderer(ball.transform.position);
        //ball.SetVelocity(initialVelocity);

        Text label = playBtn.transform.Find("Text (Legacy)").GetComponent<Text>();

        beiginBtn.onClick.AddListener(() =>
        {
            if(padDataList.Count > 0)
            {
                Physics.autoSimulation = false;
                isPlay = true;
                currentFrame = 0;
                visiblePadRange(0, 10);
                cinemachineTrackedDolly.m_PathPosition = 0;
                cameraPos.transform.position = Vector3.zero;
                midiHelper.Stop();

                m_currentObjCount = -1;
                m_preObjCount = -1;

                isEditorCamera = false;
                UpdateCamera();
            }
            else
            {
                isPlay = true;
                currentFrame = 0;
            }

        });

        playBtn.onClick.AddListener(() =>
        {
            Physics.autoSimulation = false;
            isPlay = !isPlay;
            if (isPlay)
            {
                label.text = "暂停";
                //midiHelper.Play();
            }
            else
            {
                label.text = "播放";
                midiHelper.Pause();
            }
        });

        nextFrameBtn.onClick.AddListener(() =>
        {
            Physics.autoSimulation = false;
            simulateNextFrame();

            //Debug.Log($"speed:{ball.velocity.magnitude}");
        });

        preFrameBtn.onClick.AddListener(() =>
        {
            Physics.autoSimulation = false;
            simulatePreviousFrame();
        });

        nextObjBtn.onClick.AddListener(() =>
        {
            Physics.autoSimulation = false;

            for (int i = 0; i < frameList.Count; i++)
            {
                if (currentFrame < frameList[i])
                {
                    simulateToFrame(frameList[i]);
                    break;
                }
                else if (currentFrame == frameList[i])
                {
                    if (i < frameList.Count)
                    {
                        simulateToFrame(frameList[i + 1]);
                        break;
                    }
                }
            }

        });

        preObjBtn.onClick.AddListener(() =>
        {
            Physics.autoSimulation = false;

            for (int i = 0; i < frameList.Count; i++)
            {
                if (currentFrame < frameList[i])
                {
                    if (i != 0)
                    {
                        simulateToFrame(frameList[i - 1]);
                        break;
                    }
                }
                else if (currentFrame == frameList[i])
                {
                    if (i != 0)
                    {
                        simulateToFrame(frameList[i - 1]);
                        break;
                    }
                }
            }
        });

        simulateBtn.onClick.AddListener(() =>
        {
            Physics.autoSimulation = true;
        });

        simulateV2V.onClick.AddListener(() =>
        {
            SimulateV2V();
        });

        simulateV2H.onClick.AddListener(() =>
        {
            SimulateV2H();
        });

        simulateV2P.onClick.AddListener(() =>
        {
            StartCoroutine(SimulateV2P());
        });

        simulateH2H.onClick.AddListener(() =>
        {
            SimulateH2H();
        });

        simulateH2V.onClick.AddListener(() =>
        {
            SimulateH2V();
        });

        simulateH2P.onClick.AddListener(() =>
        {
            StartCoroutine(SimulateH2P());
        });

        startGenerating.onClick.AddListener(() =>
        {
            ResetBeigin();

            StartCoroutine(StartGenerating(currentMidi, int.Parse(inputField.text)));
        });

        loadData.onClick.AddListener(() =>
        {
            LoadData();
        });

        saveData.onClick.AddListener(() =>
        {
            SaveData(currentMidi);
        });

        cameraBtn.onClick.AddListener(() =>
        {
            isEditorCamera = !isEditorCamera;
            UpdateCamera();
        });

        boundsBtn.onClick.AddListener(() => 
        {
            if(showPadList.Count == 0)
            {
                Pad pad1 = Instantiate<Pad>(padPrefab);
                pad1.childTransforms[1].gameObject.SetActive(true);
                pad1.SetBounds(horizontalCenter1, horizontalSize, horizontalBestPos1);

                Pad pad2 = Instantiate<Pad>(padPrefab);
                pad2.childTransforms[1].gameObject.SetActive(true);
                pad2.SetBounds(horizontalCenter2, horizontalSize, horizontalBestPos2);

                Pad pad3 = Instantiate<Pad>(padPrefab);
                pad3.childTransforms[1].gameObject.SetActive(true);
                pad3.SetBounds(verticalCenter1, verticalSize, verticalBestPos1);

                Pad pad4 = Instantiate<Pad>(padPrefab);
                pad4.childTransforms[1].gameObject.SetActive(true);
                pad4.SetBounds(verticalCenter2, verticalSize, verticalBestPos2);

                showPadList.Add(pad1);
                showPadList.Add(pad2);
                showPadList.Add(pad3);
                showPadList.Add(pad4);

                foreach (Pad pad in showPadList)
                {
                    pad.gameObject.SetActive(true);
                }
            }
            else 
            { 
                foreach(Pad pad in showPadList)
                {
                    pad.gameObject.SetActive(true);
                }
            }
        });

        genAllBtn.onClick.AddListener(() => 
        {
            StartCoroutine(GenAll());
        });


        for (int i = 0; i < barPrefabParent.childCount; i++)
        {
            BarsDemo bar = barPrefabParent.Find("Bar" + (i + 1)).GetComponent<BarsDemo>();
            BarsDemoPrefabList.Add(bar);

            if (bar.beiginOffset < barsAppearMinOffset)
            {
                barsAppearMinOffset = bar.beiginOffset;
            }
        }

        UpdateCamera();

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

        inputField.text = "0";

        captureBase = captureObj.GetComponent<RenderHeads.Media.AVProMovieCapture.CaptureBase>();

        simulateBarsDemoBtn.onClick.AddListener(() => 
        {
            //Pad_Simulate2();
            //Pad_Simulate();
            Bars_Simulate();
            //StartCoroutine(Bars_Curve(0,40,25));
            //VirtualBarsScale_Simulate();
            //StartCoroutine(Bars_Simulate2());
            //Bars_Simulate3();
        });

        LoadBarsConfig();
        LoadPadConfig();
        //BarsConfig barsConfig = new BarsConfig();
        //barsConfig.BarConfigList = new List<BarConfig>();

        //BarConfig barConfig = new BarConfig();
        //barConfig.prefabIndex = 0;
        //barConfig.beiginOffset = 0.1f;
        //barConfig.endOffset = 0.1f;
        //barConfig.modifyVelocityX = -9999;
        //barConfig.modifyVelocityY = -9999;

        //barsConfig.BarConfigList.Add(barConfig);

        //barConfig = new BarConfig();
        //barConfig.prefabIndex = 1;
        //barConfig.beiginOffset = 0.18f;
        //barConfig.endOffset = 0.3f;
        //barConfig.modifyVelocityX = -1.1f;
        //barConfig.modifyVelocityY = -0.48f;

        //barsConfig.BarConfigList.Add(barConfig);

        //string json_str = JsonUtility.ToJson(barsConfig);
        //File.WriteAllText(Path.Combine(Application.streamingAssetsPath,"barsConfig.json"), json_str);

        //Config config = new Config();
        //config.gravityY = gravityY;
        //config.continuousTryCount = ContinuousTryCount;
        //config.initialVelocity = initialVelocity;
        //config.h_bouncinessPriorityList = h_bouncinessPriorityList;
        //config.v_bouncinessPriorityList = v_bouncinessPriorityList;
        //config.h_rotationList = h_rotationList;
        //config.v_rotationList1 = v_rotationList1;
        //config.v_rotationList2 = v_rotationList2;
        //config.horizontalCenter1 = horizontalCenter1;
        //config.horizontalCenter2 = horizontalCenter2;
        //config.horizontalBestPos1 = horizontalBestPos1;
        //config.horizontalBestPos2 = horizontalBestPos2;
        //config.horizontalSize = horizontalSize;
        //config.verticalCenter1 = verticalCenter1;
        //config.verticalCenter2 = verticalCenter2;
        //config.verticalBestPos1 = verticalBestPos1;
        //config.verticalBestPos2 = verticalBestPos2;
        //config.verticalSize = verticalSize;

        //string json_str = JsonUtility.ToJson(config);
        //File.WriteAllText(config_path, json_str);
    }

    void LoadConfig()
    {
        config_path = Path.Combine(Application.streamingAssetsPath, "config.json");
        string json_str = File.ReadAllText(config_path);
        Config config = JsonUtility.FromJson<Config>(json_str);

        gravityY = config.gravityY;
        ContinuousTryCount = config.continuousTryCount;
        initialVelocity = config.initialVelocity;
        h_bouncinessPriorityList1 = config.h_bouncinessPriorityList;
        v_bouncinessPriorityList = config.v_bouncinessPriorityList;
        h_rotationList1 = config.h_rotationList;
        v_rotationList1 = config.v_rotationList1;
        v_rotationList2 = config.v_rotationList2;
        horizontalCenter1 = config.horizontalCenter1;
        horizontalCenter2 = config.horizontalCenter2;
        horizontalBestPos1 = config.horizontalBestPos1;
        horizontalBestPos2 = config.horizontalBestPos2;
        horizontalSize = config.horizontalSize;
        verticalCenter1 = config.verticalCenter1;
        verticalCenter2 = config.verticalCenter2;
        verticalBestPos1 = config.verticalBestPos1;
        verticalBestPos2 = config.verticalBestPos2;
        verticalSize = config.verticalSize;
    }

    void LoadBarsConfig()
    {
        string barsConfigPath = Path.Combine(Application.streamingAssetsPath, "barsConfig.json");
        string json_str = File.ReadAllText(barsConfigPath);
        barsConfig = JsonUtility.FromJson<BarsConfig>(json_str);

        string barsCurvePath = Path.Combine(Application.streamingAssetsPath, "barCurve");
        List<string> outFiles = new List<string>();
        if (Directory.Exists(barsCurvePath))
        {
            // 获取路径下所有文件
            string[] files = Directory.GetFiles(barsCurvePath, "*.json");

            if (files.Length > 0)
            {
                foreach (var file in files)
                {
                    json_str = File.ReadAllText(file);
                    BarsCurve barsCurve = JsonUtility.FromJson<BarsCurve>(json_str);
                    barsCurveList.Add(barsCurve);
                }
            }
        }
    }

    void LoadPadConfig()
    {
        string padCurvePath = Path.Combine(Application.streamingAssetsPath, "padCurve");
        List<string> outFiles = new List<string>();
        if (Directory.Exists(padCurvePath))
        {
            // 获取路径下所有文件
            string[] files = Directory.GetFiles(padCurvePath, "*.json");

            if (files.Length > 0)
            {
                foreach (var file in files)
                {
                    string json_str = File.ReadAllText(file);
                    PadCurve padCurve = JsonUtility.FromJson<PadCurve>(json_str);
                    padCurveList.Add(padCurve);
                }
            }
        }
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
            cameraBtn.transform.Find("Text").GetComponent<Text>().text = "运行时镜头";
            v_camera.gameObject.SetActive(false);
            //follow_camera.gameObject.SetActive(false);
            v_camera_editor.gameObject.SetActive(true);
        }
        else
        {
            cameraBtn.transform.Find("Text").GetComponent<Text>().text = "编辑器镜头";
            v_camera.gameObject.SetActive(true);
            //follow_camera.gameObject.SetActive(true);
            v_camera_editor.gameObject.SetActive(false);
        }
    }

    void simulateNextFrame(bool record = true)
    {
        if (currentFrame + 1 < physicsStateList.Count)
        {
            currentFrame = currentFrame + 1;
            PhysicsState ps = physicsStateList[currentFrame];
            ball.ResetState(ps);
        }
        else
        {
            Physics.Simulate(0.02f);
            currentFrame = currentFrame + 1;

            if (record)
            {
                PhysicsState ps = new PhysicsState();
                ps.position = ball.transform.position;
                ps.rotation = ball.transform.rotation;
                ps.velocity = ball.velocity;
                ps.angularVelocity = ball.angularVelocity;

                physicsStateList.Add(ps);

                ModifyVelocity();

                line.AddLineRenderer(ball.transform.position);
            }
        }
    }

    void simulateToFrame(int _frame, bool record = true,bool clear = false)
    {
        if (_frame <= 0)
            return;

        if (_frame < physicsStateList.Count)
        {
            PhysicsState ps = physicsStateList[_frame];
            ball.ResetState(ps);
            currentFrame = _frame;

            if (clear)
            {
                physicsStateList.RemoveRange(_frame + 1, physicsStateList.Count - 1 - _frame);
                line.ResetLineRenderer(physicsStateList);
            }
        }
        else
        {
            PhysicsState ps = physicsStateList[physicsStateList.Count - 1];
            ball.ResetState(ps);
            currentFrame = physicsStateList.Count - 1;
            int tempFrame = currentFrame;
            for (int i = 0; i < _frame - tempFrame; i++)
            {
                simulateNextFrame(record);
            }
        }
    }

    void simulatePreviousFrame()
    {
        if (currentFrame > 0)
        {
            currentFrame = currentFrame - 1;
            PhysicsState ps = physicsStateList[currentFrame];
            ball.ResetState(ps);
        }
    }

    void initLogger(string midi)
    {
        logPath = Path.Combine(Application.dataPath, $"../{midi}_log.txt");
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
                            currentTryingTypeTxt.text = padData.tryingType.ToString();
                            currentContinuousCountTxt.text = $"count:{padData.continuousCount}";
                        }
                        
                        currentObjCountTxt.text = $"pad:{(i+1)}";

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

    IEnumerator DealFirstBars()
    {
        playMusicFrame = 0;

        InstantBars(ball.transform.position, Vector3.one, bar1, 0);

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < 9999; i++)
        {
            simulateNextFrame();
            playMusicFrame++;

            if (barsDemoList[0].isPass(ball.transform.position))
            {
                break;
            }
        }

        //额外simulate
        for (int i = 0; i < 15; i++)
        {
            simulateNextFrame();
            playMusicFrame++;
        }
    }

    int CalculateLightIndex(int _index)
    {
        int _FrameIndex = _index % 10;
        if (_FrameIndex < 6)
        {
            return 0;
        }
        else if (_FrameIndex == 6 || _FrameIndex == 7)
        {
            return 1;
        }
        else if (_FrameIndex == 8 || _FrameIndex == 9)
        {
            return 2;
        }

        return 0;
    }

    float angularFactor = 2f;  // 反比强度系数
    float minSpeed = 0.1f;      // 防止除以零的最小速度值

    void RefreshBallVelocity(int _index, bool isVertical)
    {
        if (_index == 1)
        {
            ball.velocity = new Vector3(0.5f, ball.velocity.y, ball.velocity.z);
            physicsStateList[physicsStateList.Count - 1].velocity = ball.velocity;
        }
        else
        {
            // 计算反比增量（速度越大增量越小） u
            float inverseIncrement = angularFactor / Mathf.Max(ball.speed, minSpeed);
            //Debug.Log($"inverseIncrement:{inverseIncrement} ball.speed:{ball.speed}");
            ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x - inverseIncrement, ball.angularVelocity.y - inverseIncrement, ball.angularVelocity.z - inverseIncrement));
            physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;

            if (!isVertical)
            {
                float LowBorder = 0.4f;
                float LowFix = 0.5f;
                float HighBorder = 0.7f;
                float HightFix = 0.6f;

                if (Mathf.Abs(ball.velocity.x) < LowBorder)
                {
                    if (ball.velocity.x > 0)
                    {
                        ball.velocity = new Vector3(LowFix, ball.velocity.y, ball.velocity.z);
                        physicsStateList[physicsStateList.Count - 1].velocity = ball.velocity;
                    }
                    else
                    {
                        ball.velocity = new Vector3(-LowFix, ball.velocity.y, ball.velocity.z);
                        physicsStateList[physicsStateList.Count - 1].velocity = ball.velocity;
                    }
                }

                if (Mathf.Abs(ball.velocity.x) > HighBorder)
                {
                    if (ball.velocity.x > 0)
                    {
                        ball.velocity = new Vector3(HightFix, ball.velocity.y, ball.velocity.z);
                        physicsStateList[physicsStateList.Count - 1].velocity = ball.velocity;
                    }
                    else
                    {
                        ball.velocity = new Vector3(-HightFix, ball.velocity.y, ball.velocity.z);
                        physicsStateList[physicsStateList.Count - 1].velocity = ball.velocity;
                    }
                }
            }

            if (ball.velocity.y < -1.5f)
            {
                ball.velocity = new Vector3(ball.velocity.x, -0.8f, ball.velocity.z);
                physicsStateList[physicsStateList.Count - 1].velocity = ball.velocity;
            }
        }
    }

    IEnumerator GameInit()
    {
        logger("GameInit");

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Restart();

        frameList.Clear();
        frameOffsetList.Clear();

        isGening = true;
        int _lastFrame = 0;
        bool isFirst = true;
        foreach (var mptkEvent in midiHelper.AllMidiEventsSet)
        {
            int _frame = Mathf.RoundToInt(mptkEvent.RealTime / 1000.0f / 0.02f * midiFacter);
            //Debug.Log($"_frame:{_frame}");

            int offsetFrame = _frame - _lastFrame;

            if (offsetFrame > 100)
            {
                offsetFrame = 100;
            }

            //if(offsetFrame > 5)
            {
                if (isFirst)
                {
                    //playMusicOffsetFrame = _frame;
                    isFirst = false;
                    offsetFrame = 0;
                }

                frameOffsetList.Add(offsetFrame);
                _lastFrame = _frame;
            }
        }
        
        if(currentOffsetIndex == 1)
        {
            yield return DealFirstBars();
            logger("已完成DealFirstBars");

            cameraProjection.positionCount = playMusicFrame;
            for (int i = 0; i < playMusicFrame; i++)
            {
                cameraProjection.SetPosition(i, physicsStateList[i].position);

                cameraPathList.Add(physicsStateList[i].position);

                cameraPosMap.Add(i, i);
            }
        }

        _lastFrame = playMusicFrame;

        foreach (var offsetFrame in frameOffsetList)
        {
            frameList.Add(_lastFrame + offsetFrame);
            _lastFrame = _lastFrame + offsetFrame;
        }

        currentCameraMovement.beiginFrame = frameList[currentOffsetIndex-1];
            
        int loopCount = LOOPCOUNT;
        if (LOOPCOUNT <= 0)
        {
            loopCount = frameOffsetList.Count;
        }
        else if (LOOPCOUNT > frameOffsetList.Count)
        {
            loopCount = frameOffsetList.Count;
        }

        curveType = CurveType.HorizontalToHorizontal;
        isVertical = false;
        tryingType = TryingType.Try_Horizontal;

        logger($"循环次数loopCount:{loopCount}");

        while (currentOffsetIndex < loopCount)
        {
            CurveTypePriority.Clear();

            int offsetIndex = currentOffsetIndex;

            logger($"第 {offsetIndex} 个开始 ++++++++++++");

            int offsetFrame = frameOffsetList[offsetIndex];
            RefreshBallVelocity(offsetIndex, isVertical);

            float rotation = 0f;
            if (isVertical)
                rotation = 30f;

            currentPad = InstantPad(ball.transform.position, ball.velocity, currentFrame, rotation);
            int lightIndex = CalculateLightIndex(offsetIndex);
            currentPad.lightIndex = lightIndex;

            CurveTypeRecord.Clear();

            if(offsetFrame >= 20)
            {
                if (offsetFrame == 100)
                {
                    if (isVertical)
                    {
                        curveType = CurveType.VerticalPipline;
                    }
                    else
                    {
                        curveType = CurveType.HorizontalPipline;
                    }
                }
                else if (offsetFrame < 40)
                {
                    CurveType preCurveType = LastCurveType();

                    if (isVertical)
                    {
                        if (preCurveType == CurveType.VerticalLine)
                        {
                            curveType = CurveType.VerticalPipline2;
                            CurveTypePriority.Add(CurveType.VerticalLine2);
                        }
                        else if (preCurveType == CurveType.VerticalPipline2)
                        {
                            curveType = CurveType.VerticalLine;
                            CurveTypePriority.Add(CurveType.VerticalLine2);
                            CurveTypePriority.Add(CurveType.VerticalPipline2);
                        }
                        else
                        {
                            curveType = CurveType.VerticalPipline2;
                            CurveTypePriority.Add(CurveType.VerticalLine2);
                        }

                    }
                    else
                    {
                        if (preCurveType == CurveType.HorizontalLine)
                        {
                            curveType = CurveType.HorizontalPipline2;
                            CurveTypePriority.Add(CurveType.HorizontalLine2);
                        }
                        else if (preCurveType == CurveType.HorizontalPipline2)
                        {
                            curveType = CurveType.HorizontalLine;
                            CurveTypePriority.Add(CurveType.HorizontalLine2);
                            CurveTypePriority.Add(CurveType.HorizontalPipline2);
                        }
                        else
                        {
                            curveType = CurveType.HorizontalPipline2;
                            CurveTypePriority.Add(CurveType.HorizontalLine2);
                        }
                    }
                }
                else
                {
                    if (isVertical)
                    {
                        curveType = CurveType.VerticalPipline2;
                    }
                    else
                    {
                        curveType = CurveType.HorizontalPipline2;
                    }
                }
            }

            yield return TryCurve(offsetIndex, offsetFrame);
            
            if(!g_isPass)
            {
                logger($"第 {offsetIndex} 个发生错误 已退出循环!!!!");
                Debug.Log($"第 {offsetIndex} 个发生错误 已退出循环!!!!");
                break;
            }

            logger($"第 {offsetIndex} 个结束 ------------");
            //Debug.Log($"第 {offsetIndex} 个结束 ------------");


            currentOffsetIndex++;
        }

        //创建最后一个
        float r = 0;
        if (isVertical)
        {
            if (ball.velocity.x > 0)
            {
                r = 30;
            }
            else
            {
                r = -30;
            }
        }

        Pad _pad = InstantPad(ball.transform.position, ball.velocity, currentFrame, r);
        _pad.lightIndex = CalculateLightIndex(currentOffsetIndex);

        //计算镜头参数
        currentCameraMovement.endFrame = frameList[currentOffsetIndex-1];
        cameraMovementList.Add(currentCameraMovement);

        int movementIndex = cameraProjection.positionCount;

        cameraFollowList.Clear();

        foreach(var c in cameraPathList)
        {
            cameraFollowList.Add(c);
        }

        beforeBarbeiginFrame = -1;
        beforeBarEndFrame = -1;
        preBarBeginFrame = -1;
        preBarEndFrame = -1;
        preBarBeiginPos = Vector3.zero;
        preBarEndPos = Vector3.zero;

        List<CameraMovement> barMovementList = new List<CameraMovement>();

        foreach (var cameraMovement in cameraMovementList)
        {
            int bf = cameraMovement.beiginFrame;
            int ef = cameraMovement.endFrame;

            if (cameraMovement.isBar)
            {
                int beiginIndex = cameraProjection.positionCount;
                cameraProjection.positionCount = cameraProjection.positionCount + (ef - bf);
                for (int i = bf; i < ef; i++)
                {
                    Vector3 pos = physicsStateList[i].position;

                    if (!cameraPosMap.ContainsKey(i))
                    {
                        cameraPathList.Add(pos);
                        cameraPosMap.Add(i, cameraPathList.Count - 1);

                    }

                    cameraProjection.SetPosition(beiginIndex + i - bf, pos);
                }


                if (!cameraPosMap.ContainsKey(ef))
                {
                    Vector3 pos = physicsStateList[ef].position;
                    cameraPathList.Add(pos);
                    cameraPosMap.Add(ef, cameraPathList.Count - 1);
                }

                movementIndex = cameraProjection.positionCount;
            }
            else
            {
                Vector3 pos1 = physicsStateList[bf].position;
                Vector3 pos2 = physicsStateList[ef].position;
                movementIndex = movementIndex + 2;

                cameraProjection.positionCount = movementIndex;

                cameraProjection.SetPosition(movementIndex - 2, pos1);
                cameraProjection.SetPosition(movementIndex - 1, pos2);

                if (!cameraPosMap.ContainsKey(bf))
                {
                    cameraPathList.Add(pos1);
                    cameraPosMap.Add(bf, cameraPathList.Count - 1);
                }

                for (int i = bf + 1; i < ef; i++)
                {
                    Vector3 pos = Vector3.Lerp(pos1, pos2, (float)(i - bf) / (float)(ef - bf));

                    if (!cameraPosMap.ContainsKey(i))
                    {
                        cameraPathList.Add(pos);
                        cameraPosMap.Add(i, cameraPathList.Count - 1);
                    }
                }

                if (!cameraPosMap.ContainsKey(ef))
                {
                    cameraPathList.Add(pos2);
                    cameraPosMap.Add(ef, cameraPathList.Count - 1);
                }
            }

            //预处理 isBar
            if (cameraMovement.isBar)
            {
                barMovementList.Add(cameraMovement);
            }
            else
            {
                if(barMovementList.Count > 0)
                {
                    var m1 = barMovementList[0];
                    var m2 = barMovementList[barMovementList.Count-1];

                    int beiginFrame = m1.beiginFrame;
                    int endFrame = m2.endFrame;

                    CameraMovement movement = new CameraMovement();
                    movement.beiginFrame = beiginFrame;
                    movement.endFrame = endFrame;
                    movement.isBar = true;

                    cameraFollowMovementList.Add(movement);

                    barMovementList.Clear();
                }
                cameraFollowMovementList.Add(cameraMovement);
            }
        }

        if(cameraFollowMovementList[0].isBar)
        {
            beforeBarbeiginFrame = 119;
            beforeBarEndFrame = 119;
        }

        foreach(var cameraMovement in cameraFollowMovementList)
        {
            int bf = cameraMovement.beiginFrame;
            int ef = cameraMovement.endFrame;

            if (cameraMovement.isBar)
            {
                CalculateCameraPath(bf, ef);

                cameraPathProjection.positionCount = cameraFollowList.Count;
                cameraPathProjection.SetPositions(cameraFollowList.ToArray());
                cameraPathProjection.material.color = Color.cyan;
            }
            else
            {
                if (beforeBarbeiginFrame == -1)
                    beforeBarbeiginFrame = bf;

                beforeBarEndFrame = ef;
            }
        }

        //处理最后一个
        CalculateCameraPath(-1, -1);

        cameraPathProjection.positionCount = cameraFollowList.Count;
        cameraPathProjection.SetPositions(cameraFollowList.ToArray());
        cameraPathProjection.material.color = Color.cyan;

        cameraProjection.material.color = Color.white;
        //cameraPosMap
        List<Waypoint> waypoints = new List<Waypoint>();
        //foreach(var pos in cameraFollowList)
        foreach(var pos in cameraPathList)
        {
            Waypoint waypoint = new Waypoint();
            waypoint.position = pos;
            waypoints.Add(waypoint);
        }

        cinemachineSmoothPath.m_Waypoints = waypoints.ToArray();

        stopwatch.Stop();

        logger($"运行时间:{stopwatch.ElapsedMilliseconds / 1000f}");

        isGening = false;
    }

    private void UpdateFrameList(int offsetIndex,int frame)
    {
        frameList.Clear();

        frameOffsetList[offsetIndex] = frame;
        int _lastFrame = playMusicFrame;

        foreach (var offsetFrame in frameOffsetList)
        {
            frameList.Add(_lastFrame + offsetFrame);
            _lastFrame = _lastFrame + offsetFrame;
        }
    }

    private void CalculateCameraPath(int bf,int ef)
    {
        float left = float.MaxValue;
        float right = float.MinValue;
        bool beiginDetetion = false;
        for (int f = 0; f < frameList.Count; f++)
        {
            if (beforeBarbeiginFrame == frameList[f])
            {
                beiginDetetion = true;
            }

            if (beiginDetetion)
            {
                PadData pd = padDataList[f];
                if (pd.pos.x < left)
                {
                    left = pd.pos.x;
                }

                if (pd.pos.x > right)
                {
                    right = pd.pos.x;
                }
            }

            if (beforeBarEndFrame == frameList[f])
            {
                beiginDetetion = false;
            }
        }

        Debug.Log($"physicsStateList.Count:{physicsStateList.Count}    beforeBarbeiginFrame:{beforeBarbeiginFrame}    beforeBarEndFrame:{beforeBarEndFrame}");

        Vector3 posPath1 = new Vector3((left + right) / 2, physicsStateList[beforeBarbeiginFrame].position.y, physicsStateList[beforeBarbeiginFrame].position.z);
        Vector3 posPath2 = new Vector3((left + right) / 2, physicsStateList[beforeBarEndFrame].position.y, physicsStateList[beforeBarEndFrame].position.z);

        if (preBarBeginFrame != -1 && preBarEndFrame != -1)
        {
            preBarEndPos = posPath1;

            for (int i = preBarBeginFrame + 1; i < preBarEndFrame; i++)
            {
                Vector3 posPath = Vector3.Lerp(preBarBeiginPos, preBarEndPos, (float)(i - preBarBeginFrame) / (float)(preBarEndFrame - preBarBeginFrame));
                cameraFollowList.Add(posPath);
            }
        }

        cameraFollowList.Add(posPath1);
        for (int i = beforeBarbeiginFrame + 1; i < beforeBarEndFrame; i++)
        {
            Vector3 posPath = Vector3.Lerp(posPath1, posPath2, (float)(i - beforeBarbeiginFrame) / (float)(beforeBarEndFrame - beforeBarbeiginFrame));
            cameraFollowList.Add(posPath);
        }

        if(beforeBarEndFrame - beforeBarbeiginFrame > 0)
            cameraFollowList.Add(posPath2);

        preBarBeginFrame = bf;
        preBarEndFrame = ef;
        //重置
        beforeBarbeiginFrame = -1;
        beforeBarEndFrame = -1;

        preBarBeiginPos = posPath2;
    }

    private List<PhysicsState> GetCurrentPhysicsState(int frame,int offsetFrame = -1)
    {
        if(frame + offsetFrame > physicsStateList.Count)
        {
            if (offsetFrame == -1)
                offsetFrame = physicsStateList.Count - frame;

            return physicsStateList.GetRange(frame, offsetFrame);
        }

        return null;
    }

    void EndCameraMovement(bool isBar = false)
    {
        if(isBar)
        {
            //CameraMovement preMovement = cameraMovementList[cameraMovementList.Count - 1];
            int preEnd1 = currentCameraMovement.beiginFrame;
            int preEnd2 = frameList[currentOffsetIndex - 1];

            //若中间没有空余pad
            if(preEnd1 == preEnd2)
            {
                currentCameraMovement.isBar = isBar;
                currentCameraMovement.endFrame = frameList[currentOffsetIndex];
                cameraMovementList.Add(currentCameraMovement);

                currentCameraMovement = new CameraMovement();
                currentCameraMovement.beiginFrame = frameList[currentOffsetIndex];
            }
            else
            {
                currentCameraMovement.isBar = false;
                currentCameraMovement.endFrame = preEnd2;
                cameraMovementList.Add(currentCameraMovement);

                //插入直线
                currentCameraMovement = new CameraMovement();
                currentCameraMovement.isBar = isBar;
                currentCameraMovement.beiginFrame = preEnd2;
                currentCameraMovement.endFrame = frameList[currentOffsetIndex];
                cameraMovementList.Add(currentCameraMovement);

                currentCameraMovement = new CameraMovement();
                currentCameraMovement.beiginFrame = frameList[currentOffsetIndex];
            }
        }
        else
        {
            currentCameraMovement.isBar = isBar;
            currentCameraMovement.endFrame = frameList[currentOffsetIndex];
            cameraMovementList.Add(currentCameraMovement);

            currentCameraMovement = new CameraMovement();
            currentCameraMovement.beiginFrame = frameList[currentOffsetIndex];
        }
       
    }

    private List<PhysicsState> GetCurrentPhysicsState(int _index)
    {
        if(_index < physicsStateList.Count)
        {
            int frameCount = physicsStateList.Count - _index;
            return physicsStateList.GetRange(_index, frameCount);
        }

        return null;
    }

    private void OverwritePhysicsState(List<PhysicsState> psList)
    {
        int overwriteCount = physicsStateList.Count - psList.Count;
        physicsStateList.RemoveRange(overwriteCount, psList.Count);
        physicsStateList.AddRange(psList);
    }

    private void AddPhysicsState(List<PhysicsState> psList)
    {
        physicsStateList.AddRange(psList);
    }

    private List<PhysicsState> TransformPhysicsState(List<PhysicsState> psList)
    {
        List<PhysicsState> physicsStates = new List<PhysicsState>();
        for (int i = 0;i < psList.Count;i++)
        {
            PhysicsState physicsState = new PhysicsState();
            physicsState.velocity = psList[i].velocity;
            physicsState.angularVelocity = psList[i].angularVelocity;
            physicsState.position = psList[i].position + ball.transform.position;
            physicsState.rotation = psList[i].rotation * ball.transform.rotation;
            physicsStates.Add(physicsState);
        }
        return physicsStates;
    }

    void DealWithBarCombine(int offsetIndex, int offsetFrame)
    //IEnumerator DealWithBarCombine(int offsetIndex, int offsetFrame)
    {
        distanceType = DistanceType.None;

        int backFrame = currentFrame; 
        Vector3 prePos = ball.transform.position;

        g_isPass = false;

        //预测这一帧
        for (int i = 0; i < offsetFrame; i++)
        {
            simulateNextFrame();
        }

        Vector3 afterPos = ball.transform.position;

        if(IsCanBars(prePos,afterPos, barCombine.beiginOffset))
        {
            int simlateFrame = 0;
            //回到之前
            simulateToFrame(backFrame, true, true);

            ClearMistakeCollisionTimes();

            while (true)
            {
                simulateNextFrame();
                simlateFrame++;

                //找到在beginOffset下方的点
                if(IsCanBars(prePos, ball.transform.position, barCombine.beiginOffset))
                {
                    g_isPass = true;
                    break;
                }
            }

            if(g_isPass)
            {
                bool mistakeCollision = DetectMistakeCollisionTimes();
                g_isPass = !mistakeCollision;
            }

            if(g_isPass)
            {
                g_isPass = false;

                int frontFrame = simlateFrame;
                int lastFrame = offsetFrame - frontFrame;
                Vector3 bornPos = ball.transform.position;

                int backFrame_create = currentFrame;

                currentBarsCombine = InstantBarsCombine(bornPos, ball.velocity, currentFrame,null, backFrame, backFrame + offsetFrame);

                //yield return new WaitForEndOfFrame();

                //阶段1
                for (int _f = 0; _f < lastFrame; _f++)
                {
                    //模拟
                    simulateNextFrame();
                    simlateFrame++;
                    if (currentBarsCombine.isPass1(ball.transform.position, ball.velocity))
                    {
                        g_isPass = true;
                        break;
                    }
                }

                //阶段2
                if (g_isPass)
                {
                    g_isPass = false;

                    Vector3 stage1Pos = ball.transform.position;

                    int stage1Frame = simlateFrame;
                    int stage1BackFrame = currentFrame;

                    int lastFrame2 = offsetFrame - stage1Frame;
                    for (int i = 0; i < lastFrame2; i++)
                    {
                        simulateNextFrame();
                    }
                    currentBarsCombine.updateBar(stage1Pos, ball.transform.position);
                     
                    float barsCombineScale = currentBarsCombine.getScaleX();
                    if (barsCombineScale > 5f && currentBarsCombine.transform.eulerAngles.z != 0)
                    {
                        currentBarsCombine.transform.eulerAngles = new Vector3(currentBarsCombine.transform.eulerAngles.x, currentBarsCombine.transform.eulerAngles.y, 0);

                        simlateFrame = frontFrame;

                        //重新回到平放
                        simulateToFrame(backFrame_create, true, true);

                        //阶段1
                        for (int _f = 0; _f < lastFrame; _f++)
                        {
                            //模拟
                            simulateNextFrame();
                            simlateFrame++;
                            if (currentBarsCombine.isPass1(ball.transform.position, ball.velocity))
                            {
                                g_isPass = true;
                                break;
                            }
                        }

                        if (g_isPass)
                        {
                            g_isPass = false;

                            stage1Pos = ball.transform.position;

                            stage1Frame = simlateFrame;
                            stage1BackFrame = currentFrame;

                            lastFrame2 = offsetFrame - stage1Frame;
                            for (int i = 0; i < lastFrame2; i++)
                            {
                                simulateNextFrame();
                            }
                            currentBarsCombine.updateBar(stage1Pos, ball.transform.position);
                            barsCombineScale = currentBarsCombine.getScaleX();

                        }
                    }

                    currentBarsCombine.updateVirtualBars(false);
                    while (barsCombineScale > 0.5f)
                    {
                        barsCombineScale = barsCombineScale - 0.5f;
                        currentBarsCombine.SetScaleX(barsCombineScale);

                        simulateToFrame(stage1BackFrame, true, true);
                        for (int i = 0; i < lastFrame2; i++)
                        {
                            simulateNextFrame();
                        }

                        if (currentBarsCombine.isPass2(ball.transform.position))
                        {
                            g_isPass = true;
                            break;
                        }
                    }
                }
            }
        }

        List<PhysicsState> _list = GetCurrentPhysicsState(backFrame);
        RecordBarsInfo_Simluate(backFrame, currentPad.transform.eulerAngles.z, currentPad.currentBounciness, _list);

        if (!g_isPass)
        {
            if (currentBarsCombine != null)
                RecycleBarsCombine(currentBarsCombine);

            currentBarsCombine = null;
            //回到之前
            simulateToFrame(backFrame, true, true);
        }
        
    }

    IEnumerator DealWithBarScale(int offsetIndex, int offsetFrame)
    {
        int backFrame = currentFrame;
        Vector3 prePos = ball.transform.position;

        g_isPass = false;

        //预测这一帧
        for (int i = 0; i < offsetFrame; i++)
        {
            simulateNextFrame();
        }

        Vector3 afterPos = ball.transform.position;

        if (afterPos.y < prePos.y - barScale.beiginOffset)
        {
            int simlateFrame = 0;
            //回到之前
            simulateToFrame(backFrame, true, true);

            while (true)
            {
                simulateNextFrame();
                simlateFrame++;

                //找到在beginOffset下方的点
                if (ball.transform.position.y < prePos.y - barScale.beiginOffset)
                {
                    break;
                }
            }
            //int frontFrame = simlateFrame;

            //int lastFrame = offsetFrame - frontFrame;
            Vector3 bornPos = ball.transform.position;

            currentBarsScale = InstantBarsScale(bornPos, ball.velocity, currentFrame);

            yield return new WaitForEndOfFrame();

            Vector3 stage1Pos = ball.transform.position;

            int stage1Frame = simlateFrame;
            int stage1BackFrame = currentFrame;

            int lastFrame = offsetFrame - stage1Frame;
            for (int i = 0; i < lastFrame; i++)
            {
                simulateNextFrame();
            }

            currentBarsScale.updateVirtualBars(false);
            currentBarsScale.updateBar(stage1Pos, ball.transform.position);

            float barsCombineScale = currentBarsScale.getScaleX();
            while (barsCombineScale > 1.5f)
            {
                barsCombineScale = barsCombineScale - 0.1f;
                currentBarsScale.SetScaleX(barsCombineScale);

                simulateToFrame(stage1BackFrame, true, true);
                for (int i = 0; i < lastFrame; i++)
                {
                    simulateNextFrame();
                }

                if (currentBarsScale.isPass2(ball.transform.position))
                {
                    g_isPass = true;
                    break;
                }
            }
        }

        if (!g_isPass)
        {
            DestroyImmediate(currentBarsScale.gameObject);
            currentBarsScale = null;
            //回到之前
            simulateToFrame(backFrame, true, true);
        }
    }

    IEnumerator DealWithBars(int _offsetIndex,int offsetFrame)
    {
        int backFrame = currentFrame;
        Vector3 prePos = ball.transform.position;
        
        g_isPass = false;

        //预测这一帧
        for (int i = 0; i < offsetFrame; i++)
        {
            simulateNextFrame();
        }

        Vector3 afterPos = ball.transform.position;

        if (afterPos.y < prePos.y - barsAppearMinOffset)
        {
            //yield return new WaitForEndOfFrame();
            foreach (var bt in BarsDemoPrefabList)
            {
                //判断位置是否在上一个点的下方
                if (afterPos.y < prePos.y - bt.beiginOffset)
                {
                    int frontFrame = 0;
                    int simlateFrame = 0;

                    //回到之前
                    simulateToFrame(backFrame,true,true);

                    while (true)
                    {
                        simulateNextFrame();
                        frontFrame++;
                        simlateFrame++;

                        //找到在beginOffset下方的点
                        if (ball.transform.position.y < prePos.y - bt.beiginOffset)
                        {
                            break;
                        }
                    }

                    int lastFrame = offsetFrame - frontFrame;
                    BarsDemo bars_instance = InstantBars(ball.transform.position, ball.velocity, bt, currentFrame);

                    yield return new WaitForEndOfFrame();
                    for (int _f = 0; _f < lastFrame; _f++)
                    {
                        //模拟
                        simulateNextFrame();
                        simlateFrame++;
                        if (bars_instance.isPass(ball.transform.position))
                        {
                            //暂定剩余时间为5帧内
                            if (offsetFrame - simlateFrame <= 10)
                            {
                                g_isPass = true;
                            }
                            break;
                        }
                    }

                    if (g_isPass)
                    {
                        //回到之前
                        //simulateToFrame(backFrame, true, true);

                        for (int i = 0; i < offsetFrame - simlateFrame; i++)
                        {
                            simulateNextFrame();
                        }

                        break;
                    }
                    else
                    {
                        //回到之前
                        simulateToFrame(backFrame, true, true);
                        barsDemoList.Remove(bars_instance);
                        DestroyImmediate(bars_instance.gameObject);
                    }
                }
            }
        }
    }

    Pad InstantPad(Vector3 framePos, Vector3 frameDir,int _frame = 0,float rotation = 0,bool debug = false)
    {
        PadData padData = new PadData();
        padData.pos = framePos;
        padData.frame = _frame;
        padData.tryingType = tryingType;
        padData.continuousCount = ContinuousCount;

        if (currentOffsetIndex < frameOffsetList.Count)
        {
            padData.offsetFrame = frameOffsetList[currentOffsetIndex];
        }
        else
        {
            padData.offsetFrame = 0;
        }

        padData.rotation = rotation;
        padData.debugColor = Color.red;

        if (rotation == 0)
            padData.bounciness = defalutBounciness;
        else
            padData.bounciness = verticalBounciness;

        if(rotation == 0)
        {
            if(frameDir.x > 0)
            {
                padData.debugCenter = horizontalCenter1;
                padData.debugBestPos = horizontalBestPos1;
                padData.inverse = false;
            }
            else
            {
                padData.debugCenter = horizontalCenter2;
                padData.debugBestPos = horizontalBestPos2;
                padData.inverse = true;
            }

            padData.debugSize = horizontalSize;
        }
        else
        {
            if (frameDir.x > 0)
            {
                padData.debugCenter = verticalCenter2;
                padData.debugBestPos = verticalBestPos2;
                padData.inverse = true;
            }
            else
            {
                padData.debugCenter = verticalCenter1;
                padData.debugBestPos = verticalBestPos1;
                padData.inverse = false;
            }
            padData.debugSize = verticalSize;
        }

        Pad pad_instance = InstantByPadData(padData,debug);

        if(!debug)
        {
            visiblePadDic.Add(padDataList.Count, pad_instance);
            padDataList.Add(padData);
        }

        return pad_instance;
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
            pad_instance = Instantiate<Pad>(padPrefab);
            pad_instance.gameObject.SetActive(true);
        }

        pad_instance.inverse = padData.inverse;
        pad_instance.transform.SetParent(obstaclesParent.transform);
        pad_instance.transform.position = padData.pos;
        pad_instance.transform.localScale = Vector3.one;
        pad_instance.transform.eulerAngles = new Vector3(0, 0, padData.rotation);
        pad_instance.name = $"PAD_{padData.frame}";
        pad_instance.frame = padData.frame;
        pad_instance.mistakeCollisionTimes = 0;
        pad_instance.SetPhysicMaterialBounciness(padData.bounciness);

        pad_instance.padCollider.collisionAction = () =>
        {
            if (pad_instance.frame != currentFrame)
            {
                pad_instance.mistakeCollisionTimes++;
                //Debug.Log($"pad_instance:{pad_instance.name}    currentFrame:{currentFrame}");
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

    BarsCombineDemo InstantBarsCombine(Vector3 bornPos, Vector3 frameDir, int _frame = 0,Transform parentTran = null, int beiginFrame = -1,int endFrame = -1)
    {
        BarsCombineDemo barCombine_instance = null;
        if (barsCombineDemoPool.Count > 0)
        {
            barCombine_instance = barsCombineDemoPool.Pop();
        }
        else
        {
            barCombine_instance = Instantiate<BarsCombineDemo>(barCombine);
            barCombine_instance.gameObject.SetActive(true);
        }

        if(parentTran == null)
        {
            barCombine_instance.transform.SetParent(obstaclesParent.transform);
        }
        else
        {
            barCombine_instance.transform.SetParent(parentTran);
        }

        barCombine_instance.beginFrame = beiginFrame;
        barCombine_instance.endFrame = endFrame;

        barCombine_instance.transform.position = bornPos;
        barCombine_instance.transform.localScale = Vector3.one;

        if (frameDir.x > 0)
        {
            barCombine_instance.transform.eulerAngles = Vector3.zero;
        }
        else
        {
            barCombine_instance.transform.eulerAngles = new Vector3(0, 180f, 0);
        }

        float angle = Vector3.Angle(frameDir, Vector3.down);
        barCombine_instance.transform.eulerAngles = new Vector3(barCombine_instance.transform.eulerAngles.x, barCombine_instance.transform.eulerAngles.y, angle);

        //barCombine_instance.updateBar(stage1Pos, framePos);
        barCombine_instance.updateVirtualBars(true);
        barCombine_instance.SetScaleX(1.0f);
        //barsCombineDemoList.Add(barCombine_instance);

        barCombine_instance.gameObject.SetActive(true);
        barCombine_instance.name = $"BARS_{_frame}";

        return barCombine_instance;
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

    BarsScaleDemo InstantBarsScale(Vector3 bornPos, Vector3 frameDir, int _frame = 0)
    {
        BarsScaleDemo bars_instance = Instantiate<BarsScaleDemo>(barScale);
        bars_instance.transform.SetParent(obstaclesParent.transform);
        bars_instance.transform.position = bornPos;
        bars_instance.transform.localScale = Vector3.one;

        if (frameDir.x > 0)
        {
            bars_instance.transform.eulerAngles = new Vector3(0, 180, -20f);
        }
        else
        {
            bars_instance.transform.eulerAngles = new Vector3(0, 0, -20f);
        }

        bars_instance.gameObject.SetActive(true);
        bars_instance.name = $"BARS_{_frame}";

        return bars_instance;
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

    BarsDemo InstantBarsLine(Vector3 framePos, Vector3 frameDir, int prefabIndex, int beiginFrame = -1, int endFrame = -1)
    {
        BarsDemo bars_instance = null;
        if (barsLineDemoPool.ContainsKey(prefabIndex) && barsLineDemoPool[prefabIndex].Count > 0)
        {
            bars_instance = barsLineDemoPool[prefabIndex].Pop();
        }
        else
        {
            bars_instance = Instantiate<BarsDemo>(barsDemoPrefabList[prefabIndex]);
            bars_instance.gameObject.SetActive(true);
        }

        bars_instance.transform.SetParent(obstaclesParent.transform);
        bars_instance.transform.position = framePos;
        bars_instance.transform.localScale = Vector3.one;

        bars_instance.beginFrame = beiginFrame;
        bars_instance.endFrame = endFrame;

        if (frameDir.x > 0)
        {
            bars_instance.transform.eulerAngles = Vector3.zero;
            bars_instance.inverse = false;
        }
        else
        {
            bars_instance.transform.eulerAngles = new Vector3(0, 180f, 0);
            bars_instance.inverse = true;
        }

        if (bars_instance.followRotation)
        {
            float angle = Vector3.Angle(frameDir, Vector3.down);
            bars_instance.transform.eulerAngles = new Vector3(bars_instance.transform.eulerAngles.x, bars_instance.transform.eulerAngles.y, angle);
        }

        bars_instance.gameObject.SetActive(true);
        bars_instance.name = $"BARS_{beiginFrame}_{endFrame}_{barsDemoPrefabList[prefabIndex].name}";
        bars_instance.prefabIndex = prefabIndex;

        return bars_instance;
    }

    LineRendererDemo linePadTest = null;

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
            }
        }

        //if(padsBounds != null)
        //{
        //    Vector3 leftUp = new Vector3(padsBounds.min.x, padsBounds.min.y, 0);
        //    Vector3 rightUp = new Vector3(padsBounds.max.x, padsBounds.min.y, 0);
        //    Vector3 rightDown = new Vector3(padsBounds.max.x, padsBounds.max.y, 0);
        //    Vector3 leftDown = new Vector3(padsBounds.min.x, padsBounds.max.y, 0);

        //    if(linePadTest == null)
        //        linePadTest = Instantiate(line);

        //    linePadTest._line.positionCount = 5;
        //    linePadTest._line.SetPosition(0, leftUp);
        //    linePadTest._line.SetPosition(1, rightUp);
        //    linePadTest._line.SetPosition(2, rightDown);
        //    linePadTest._line.SetPosition(3, leftDown);
        //    linePadTest._line.SetPosition(4, leftUp);
        //    linePadTest.SetWidth(0.3f);
        //    linePadTest.SetColor(Color.cyan);

        //    linePadTest.gameObject.SetActive(true);
        //}

    }

    private void FixedUpdate()
    {
        if (isPlay)
        {
            if (currentFrame == playMusicFrame)
            {
                //midiHelper.Play();
            }

            if (currentFrame < physicsStateList.Count)
            {
                ball.ResetState(physicsStateList[currentFrame]);
                ball.PlayEffet();
                currentFrame++;
                
                if(m_preObjCount != m_currentObjCount)
                {
                    midiHelper.Play(m_currentObjCount-1);
                }
                m_preObjCount = m_currentObjCount;
            }
            else
            {
                isPlay = false;
                Text label = playBtn.transform.Find("Text (Legacy)").GetComponent<Text>();
                label.text = "播放";
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

        //if (cameraPosMap.ContainsKey(frame))
        //{
        //    cinemachineTrackedDolly.m_PathPosition = cameraPosMap[frame];
        //    //v_camera.transform.position = new Vector3(cameraFollowList[frame].x, cameraFollowList[frame].y , -2f);
        //    //Cinemachine.CinemachineTrackedDolly trackedDolly = v_camera_editor.GetCinemachineComponent<Cinemachine.CinemachineTrackedDolly>();
        //    //trackedDolly.m_PathPosition = cameraPosMap[frame];
        //}

        if(frame < cameraFollowList.Count )
            cinemachineTrackedDolly.m_PathPosition = frame;

        //v_camera_editor.
    }

    void ModifyFrameVelocity(int frame,float offsetX,float offsetY,float modifyVelocityX = -9999, float modifyVelocityY = -9999,Vector3 offsetRotation = default(Vector3))
    {
        if (offsetRotation == default(Vector3))
        {
            offsetRotation = Vector3.zero;
        }

        if (frameVelocityOffsetList.ContainsKey(frame))
        {
            FrameVelocityOffset frameVelocityOffset = frameVelocityOffsetList[frame];
            frameVelocityOffset.offsetX = offsetX;
            frameVelocityOffset.offsetY = offsetY;
            frameVelocityOffset.modifyVelocityX = modifyVelocityX;
            frameVelocityOffset.modifyVelocityY = modifyVelocityY;
            frameVelocityOffset.offsetRotation = offsetRotation;
        }
        else
        {
            FrameVelocityOffset frameVelocityOffset = new FrameVelocityOffset();
            frameVelocityOffset.offsetX = offsetX;
            frameVelocityOffset.offsetY = offsetY;
            frameVelocityOffset.modifyVelocityX = modifyVelocityX;
            frameVelocityOffset.modifyVelocityY = modifyVelocityY;
            frameVelocityOffset.offsetRotation = offsetRotation;
            frameVelocityOffsetList.Add(frame, frameVelocityOffset);
        }
    }

    void RemoveFrameVelocityRecord(int frame)
    {
        if(frameVelocityOffsetList.ContainsKey(frame))
        {
            frameVelocityOffsetList.Remove(frame);
        }

        if(frameVelocityList.ContainsKey(frame))
        {
            frameVelocityList.Remove(frame);
        }
    }

    void ModifyVelocity()
    {
        //在修改列表中存在
        if(frameVelocityOffsetList.ContainsKey(currentFrame))
        {
            Vector3 v = ball.velocity;
            if(frameVelocityList.ContainsKey(currentFrame))
            {
                v = frameVelocityList[currentFrame];
            }
            else
            {
                frameVelocityList.Add(currentFrame, v);
            }

            FrameVelocityOffset fvo = frameVelocityOffsetList[currentFrame];
            PhysicsState physicsState = physicsStateList[physicsStateList.Count - 1];

            float offsetX = fvo.offsetX;
            float offsetY = fvo.offsetY;

            float modifyVelocityX = fvo.modifyVelocityX;
            float modifyVelocityY = fvo.modifyVelocityY;

            //if(offsetX < 0 && ball.velocity.x > 0)
            //{
            //    offsetX = -offsetX;
            //}
            //else if(offsetX > 0 && ball.velocity.x < 0)
            //{
            //    offsetX = -offsetX;
            //}

            if (offsetY < 0 && ball.velocity.y > 0)
            {
                offsetY = -offsetY;
            }
            else if(offsetY > 0 && ball.velocity.y < 0)
            {
                offsetY = -offsetY;
            }

            if (modifyVelocityX < -9998 && modifyVelocityY < -9998)
            {
                ball.velocity = new Vector3(v.x + v.x * offsetX, v.y + v.y * fvo.offsetY, v.z);
            }
            else
            {
                if (modifyVelocityX < -9998)
                {
                    modifyVelocityX = v.x;
                }

                if (modifyVelocityY < -9998)
                {
                    modifyVelocityY = v.y;
                }

                ball.velocity = new Vector3(modifyVelocityX, modifyVelocityY, v.z);
            }

            if(fvo.offsetRotation != Vector3.zero)
            {
                ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x + fvo.offsetRotation.x, ball.angularVelocity.y + fvo.offsetRotation.y, ball.angularVelocity.z + fvo.offsetRotation.z));
                physicsState.angularVelocity = ball.angularVelocity;
            }

            physicsState.velocity = ball.velocity;
            
        }
    }

}
