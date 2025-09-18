using MidiPlayerTK;
using RenderHeads.Media.AVProMovieCapture;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
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
    public int remainFrame;
    public int targetMinFrame;
    public int targetMaxFrame;
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
    [System.Serializable]
    public class RecordJson
    {
        public List<CurveType> curveTypeList = new List<CurveType>();
        public int offsetIndex;
        public int offsetFrame;
        public int continuousCount;
        public TryingType tryingType;
        public bool isBackUp = false;
        public CurveType successCurveType;
    }

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
    private GameObject canvasTextObj;

    [SerializeField]
    private Cinemachine.CinemachineSmoothPath cinemachineSmoothPath;

    [SerializeField]
    private Camera follow_camera;

    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera v_3rd_camera;

    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera v_camera;

    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera v_camera_editor;

    [SerializeField]
    private Transform testCameraPos;

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

    [SerializeField]
    private BGMPlayer bGMPlayer;

    [SerializeField]
    private BarsDemo barBeigin0_1;

    [SerializeField]
    private BarsDemo barBeigin1_1;

    [SerializeField]
    private BarsCombineDemo barCombine_1;

    [SerializeField]
    private Light spotLight;

    [SerializeField]
    private KeepRotation keepRotation;

    Dictionary<int, Vector3> verCameraPointDic = new Dictionary<int, Vector3>();

    //List<BarsDemo> barsDemoPrefabList = new List<BarsDemo>();
    List<BarsDemo> barsDemoList = new List<BarsDemo>();
    Stack<BarsDemo> barsDemoPool = new Stack<BarsDemo>();

    Dictionary<int ,Stack<BarsDemo>> barsLineDemoPool = new Dictionary<int, Stack<BarsDemo>>();

    List<BarsScaleDemo> barsScaleDemoList = new List<BarsScaleDemo>();
    private BarsCombineDemo currentBarsCombine;
    private BarsDemo currentBars;
    private BarsDemo currentLineBars;

    private BarsScaleDemo currentBarsScale;

    //private Vector3 initialVelocity = new Vector3(0.8f, 0, 0);
    private Vector3 initialVelocity = new Vector3(0f, 0, 0);
    //private Vector3 initialRotation = new Vector3(0f, 90f, 0);
    private Vector3 initialRotation = new Vector3(0f, 0f, 0);
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
    private string logJsonPath = string.Empty;
    bool isRecordJson = true;
    List<RecordJson> recordJsons = new List<RecordJson>();
    Stack<RecordJson> recordJsonPool = new Stack<RecordJson>();
    RecordJson currentRecordJson;

    int lightIndex = 0;

    [System.Serializable]
    class RecordJsonData
    {
        public List<RecordJson> recordJsons;
    }


    //Vector2 verticalCenter3 = new Vector2(0.15f, -0.12f);
    //Vector2 verticalCenter4 = new Vector2(-0.15f, -0.12f);
    //Vector2 verticalBestPos3 = new Vector2(0.13f, -0.1f);
    //Vector2 verticalBestPos4 = new Vector2(-0.13f, -0.1f);

    Vector2 verticalSize = new Vector2(0.1f, 0.1f);

    CameraMovement currentCameraMovement = new CameraMovement();
    Cinemachine.CinemachineTrackedDolly cinemachineTrackedDolly;

    string config_path = string.Empty;

    Dictionary<int, List<BestPoint>> BestPointDic = new Dictionary<int, List<BestPoint>>();

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

    int spBarsCount = 4;
    float midiFacter = 1f;

    BarsCurve firstBarsCurve = null;

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

    float gravityY = -4.5f;

    HashSet<CurveType> CurveTypeRecord = new HashSet<CurveType>();
    List<CurveType> CurveTypePriority = new List<CurveType>();

    List<BarsCurve> barsCurveList = new List<BarsCurve>();
    List<PadCurve> padCurveList = new List<PadCurve>();

    bool isFirstAnim = false;
    bool isHoldSong = true;
    bool isUseAVPro = false;
    bool isTest = false;

    int barsCurveMinIndex = 0;
    int barsCurveMaxIndex = 11;

    int padCurveMinIndex = 1;
    int padCurveMaxIndex = 2;


    // Start is called before the first frame update
    void Start()
    {

        canvasTextObj.SetActive(isTest);
        canvasObj.SetActive(isTest);

        //LoadConfig();
        cinemachineTrackedDolly = v_camera.GetCinemachineComponent<Cinemachine.CinemachineTrackedDolly>();

        //defalutBounciness = h_bouncinessPriorityList1[0];
        //verticalBounciness = v_bouncinessPriorityList[0];


        Application.targetFrameRate = 60;
        Physics.gravity = new Vector3(0, gravityY, 0);
        Physics.autoSimulation = false;

        //ball.SetVelocity(initialVelocity);
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

        nextFrameBtn.onClick.AddListener(() =>
        {
            //Debug.Log($"speed:{ball.velocity.magnitude}");
        });

        preFrameBtn.onClick.AddListener(() =>
        {
        });

        nextObjBtn.onClick.AddListener(() =>
        {
        });

        preObjBtn.onClick.AddListener(() =>
        {
        });

        simulateBtn.onClick.AddListener(() =>
        {
            //DealFirstBars_Simulate();
        });

        //simulateV2V.onClick.AddListener(() =>
        //{
        //    SimulateV2V();
        //});

        //simulateV2H.onClick.AddListener(() =>
        //{
        //    SimulateV2H();
        //});

        //simulateV2P.onClick.AddListener(() =>
        //{
        //    StartCoroutine(SimulateV2P());
        //});

        //simulateH2H.onClick.AddListener(() =>
        //{
        //    SimulateH2H();
        //});

        //simulateH2V.onClick.AddListener(() =>
        //{
        //    SimulateH2V();
        //});

        //simulateH2P.onClick.AddListener(() =>
        //{
        //    StartCoroutine(SimulateH2P());
        //});

        startGenerating.onClick.AddListener(() =>
        {
        });

        loadData.onClick.AddListener(() =>
        {
           // StartCoroutine(LoadData());
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
                Pad padPrefab = SkinManager.Instance.GetPadPrefab();

                //Pad pad1 = Instantiate<Pad>(padPrefab);
                //pad1.childTransforms[1].gameObject.SetActive(true);
                //pad1.SetBounds(horizontalCenter1, horizontalSize, horizontalBestPos1);

                //Pad pad2 = Instantiate<Pad>(padPrefab);
                //pad2.childTransforms[1].gameObject.SetActive(true);
                //pad2.SetBounds(horizontalCenter2, horizontalSize, horizontalBestPos2);

                //Pad pad3 = Instantiate<Pad>(padPrefab);
                //pad3.childTransforms[1].gameObject.SetActive(true);
                //pad3.SetBounds(verticalCenter1, verticalSize, verticalBestPos1);

                //Pad pad4 = Instantiate<Pad>(padPrefab);
                //pad4.childTransforms[1].gameObject.SetActive(true);
                //pad4.SetBounds(verticalCenter2, verticalSize, verticalBestPos2);

                //showPadList.Add(pad1);
                //showPadList.Add(pad2);
                //showPadList.Add(pad3);
                //showPadList.Add(pad4);

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


        //for (int i = 0; i < barPrefabParent.childCount; i++)
        //{
        //    BarsDemo bar = barPrefabParent.Find("Bar" + (i + 1)).GetComponent<BarsDemo>();
        //    BarsDemoPrefabList.Add(bar);

        //    if (bar.beiginOffset < barsAppearMinOffset)
        //    {
        //        barsAppearMinOffset = bar.beiginOffset;
        //    }
        //}

        UpdateCamera();

#if UNITY_WEBGL
        currentMidi = "0105FC8C9CE6AB6B66C709F4E596C369";
#else
        string _path = Path.Combine(Application.streamingAssetsPath, "json");
        List<string> _files = CheckForJsonFiles(_path);

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

        inputField.text = "22";

        captureBase = captureObj.GetComponent<RenderHeads.Media.AVProMovieCapture.CaptureBase>();

        simulateBarsDemoBtn.onClick.AddListener(() => 
        {
            //Pad_Simulate2();
            //Bars_Simulate();
            //Pad_Simulate3();
 

        });
        StartCoroutine(LoadConfig());
        
        if(!isFirstAnim)
            //initialRotation = new Vector3(0f, 90f, 0);
            initialRotation = new Vector3(0f, 0f, 0);

        if(isUseAVPro)
        {
            follow_camera.gameObject.SetActive(true);
            testCameraPos.gameObject.SetActive(false);
        }
        else
        {
            follow_camera.gameObject.SetActive(false);
            testCameraPos.gameObject.SetActive(true);
        }
    }

    IEnumerator LoadConfig()
    {
        yield return LoadfirstBarCurve();
        yield return LoadBarsConfig();
        yield return LoadPadConfig();
        
        if(!isTest)
            yield return GenAll();
    }

    IEnumerator LoadfirstBarCurve()
    {
        yield return null;

        string firstBarCurvePath = GameConfig.Instance.GetFirstBarCurvePath();
        string json_str = string.Empty;
#if UNITY_WEBGL
        UnityWebRequest request = UnityWebRequest.Get(firstBarCurvePath);
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
#else
        json_str = File.ReadAllText(firstBarCurvePath);
#endif
        firstBarsCurve = JsonUtility.FromJson<BarsCurve>(json_str);
    }


    IEnumerator LoadBarsConfig()
    {
        yield return null;

        string barsConfigPath = GameConfig.Instance.GetBarsConfigPath();
        string json_str = string.Empty;

#if UNITY_WEBGL
        UnityWebRequest request = UnityWebRequest.Get(barsConfigPath);
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
#else
        json_str = File.ReadAllText(barsConfigPath);
#endif

        barsConfig = JsonUtility.FromJson<BarsConfig>(json_str);
        string barsCurvePath = GameConfig.Instance.GetBarCurvePath();

        for (int i = barsCurveMinIndex; i < barsCurveMaxIndex + 1; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                string file = Path.Combine(barsCurvePath, $"barsCurve{i}_{j + 1}.json");
#if UNITY_WEBGL
                request = UnityWebRequest.Get(file);
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
#else
                json_str = File.ReadAllText(file);
#endif
                BarsCurve barsCurve = JsonUtility.FromJson<BarsCurve>(json_str);
                barsCurveList.Add(barsCurve);
            }
        }
    }

    IEnumerator LoadPadConfig()
    {
        yield return null;

        string barsCurvePath = GameConfig.Instance.GetPadCurvePath();
        string json_str = string.Empty;

        for (int i = padCurveMinIndex; i < padCurveMaxIndex + 1; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                string file = Path.Combine(barsCurvePath, $"padCurve{i}_{j + 1}.json");
#if UNITY_WEBGL
                UnityWebRequest request = UnityWebRequest.Get(file);
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
#else
                json_str = File.ReadAllText(file);
#endif
                PadCurve padCurve = JsonUtility.FromJson<PadCurve>(json_str);
                padCurveList.Add(padCurve);
            }
        }
    }

    List<string> CheckForJsonFiles(string path)
    {
        List<string> outFiles = new List<string>();
        if (Directory.Exists(path))
        {
            // 获取路径下所有文件
            string[] files = Directory.GetFiles(path, "*.json");

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

            //if(!isFirstAnim)
            //    v_3rd_camera.gameObject.SetActive(true);
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

        if(logStr.Contains("BarLine"))
        {
            Debug.Log(logStr);
        }
    }

    RecordJson getRecordJson()
    {
        RecordJson rj = null;
        if (recordJsonPool.Count > 0)
        {
            rj = recordJsonPool.Pop();
            rj.isBackUp = false;
            rj.curveTypeList = new List<CurveType>();
        }
        else
        {
            rj = new RecordJson();
        }

        return rj;
    }

    void saveJsonLog()
    {
        if(isRecordJson)
        {
            RecordJsonData recordJsonData = new RecordJsonData();
            recordJsonData.recordJsons = recordJsons;

            string json_str = JsonUtility.ToJson(recordJsonData);
            File.WriteAllText(logJsonPath, json_str);

            foreach (var rj in recordJsons)
            {
                recordJsonPool.Push(rj);
            }

            recordJsons.Clear();
        }
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
                            //currentTryingTypeTxt.text = padData.tryingType.ToString();
                            //currentContinuousCountTxt.text = $"count:{padData.continuousCount}";
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
                    _pad.padTrigger(currentFrame);
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

   
    void RecycleBarsCombine(BarsCombineDemo bars = null)
    {
        if(bars == null)
        {
            bars = currentBarsCombine;
        }

        if(bars != null)
        {
            bars.transform.SetParent(padPoolParent);
            bars.transform.position = new Vector3(9999, 9999, 9999);
            bars.padCollider1.collisionAction = null;
            bars.padCollider2.collisionAction = null;
            bars.padCollider3.collisionAction = null;
            bars.updateVirtualBars(false);
            bars.SetScaleX(1.0f);

            barsCombineDemoPool.Push(bars);

            logger($"BarCombine 回收 {bars.GetInstanceID()}");
        }

        if(bars == currentBarsCombine)
            currentBarsCombine = null;
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
        //在不使用AVRro录屏的时候不进这里
        if(!isAutoGen || isUseAVPro)
            PlayBallAnim();
    }

    // Texture2D tex;

    int width = 1080;
    int height = 1920;
    byte[] bytes;

    private void Awake()
    {
    //    tex = new Texture2D(width, height, TextureFormat.RGB24, false);

    }

    IEnumerator PlayLoop()
    {
        string capDir = Path.Combine(Application.dataPath, "../Captures", midiHelper.currentMidi);
        if (!Directory.Exists(capDir))
        {
            Directory.CreateDirectory(capDir);
        }

        // 线性/URP 可靠配置
        var rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        rt.Create();
        // 线性色彩空间：最后一个参数 linear = true
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false, QualitySettings.activeColorSpace == ColorSpace.Linear);

        while (isPlay)
        {
            // 1) 推进一帧的状态/相机/文本
            PlayBallAnim();

            // 2) 等渲染完成（这样你记录的“帧号文本”已经出现在屏幕上了）
            yield return new WaitForEndOfFrame();

            ScreenCapture.CaptureScreenshotIntoRenderTexture(rt);

            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;

            var rect = new Rect(0, 0, width, height);
            tex.ReadPixels(rect, 0, 0, false);
            RenderTexture.active = prev;

            if (SystemInfo.graphicsUVStartsAtTop)
            {
                FlipYInPlace(tex);
            }
            tex.Apply(false, false);
            
            string filename = $"cap_{currentFrame}.jpg";    
            string path = Path.Combine(capDir, filename);
            // bytes = tex.EncodeToJPG(100);
            bytes = tex.EncodeToPNG();
            File.WriteAllBytes(path, bytes);

            // yield return new WaitForEndOfFrame();
        }
    }

    static void FlipYInPlace(Texture2D tex)
    {
        int w = tex.width;
        int h = tex.height;
        // RGBA32 → 4 字节每像素
        int stride = w * 4;

        var data = tex.GetRawTextureData<byte>(); // NativeArray<byte>
        byte[] rowBuf = new byte[stride];

        for (int y = 0; y < h / 2; y++)
        {
            int top = y * stride;
            int bot = (h - 1 - y) * stride;

            // 交换 top/bot 两行
            // 复制 top → rowBuf
            for (int i = 0; i < stride; i++) rowBuf[i] = data[top + i];
            // bot → top
            for (int i = 0; i < stride; i++) data[top + i] = data[bot + i];
            // rowBuf → bot
            for (int i = 0; i < stride; i++) data[bot + i] = rowBuf[i];
        }
        // 注意：我们直接改了 raw data，仍需要 Apply 把 CPU 改动上传到 GPU
        // （上面主流程里已经调用了 tex.Apply(false,false)）
    }

    void PlayBallAnim()
    {
        if (isPlay)
        {
            if (currentFrame < physicsStateList.Count)
            {
                ball.ResetState(physicsStateList[currentFrame]);

                if(isUseAVPro)
                    ball.PlayEffet();
                else
                    ball.StopEffect();

                currentFrame++;

                keepRotation.SetLabel(currentFrame);

                if (m_preObjCount != m_currentObjCount)
                {
                    if (m_currentObjCount - 1 == 0)
                    {
                        bGMPlayer.PlayBgm(midiHelper.currentMidi, midiHelper.beginTime);

                        logger($"offsetTime:{ Time.time - recordPreTime}");
                        logger($"beginTime:{midiHelper.beginTime}");
                    }
                    midiHelper.Play(m_currentObjCount - 1);
                }
                m_preObjCount = m_currentObjCount;
            }
            else
            {
                if (isHoldSong) 
                {
                    if (currentFrame < midiHelper.GetEndFrame())
                    {
                        //等结束
                        currentFrame++;
                    }
                    else
                    {
                        isPlay = false;
                        Text label = playBtn.transform.Find("Text (Legacy)").GetComponent<Text>();
                        label.text = "play";
                    }
                    ball.StopEffect();
                }
                else
                {
                    isPlay = false;
                    Text label = playBtn.transform.Find("Text (Legacy)").GetComponent<Text>();
                    label.text = "play";
                    ball.StopEffect();
                }
            }

            MoveCamera(currentFrame);

            
        }
        else
        {
            ball.StopEffect();
        }

        foreach (var pair in visiblePadDic)
        {
            pair.Value.PadFixedUpdate(currentFrame);
        }

        // if(isPlay)
        //     Debug.Log($"currentFrame:{currentFrame} 2222222222222222222");
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

        if(frame < cameraPathList.Count )
            cinemachineTrackedDolly.m_PathPosition = frame;

        if (!isUseAVPro)
            UpdateCameraSmooth();

        //if (currentFrame < cameraPathList.Count)
        //{
        //    Vector3 cf_pos = cameraPathList[currentFrame];
        //    testCameraPos.transform.position = new Vector3(cf_pos.x, cf_pos.y, testCameraPos.transform.position.z);
        //}

        //v_camera_editor.
    }

    int keyStride = 50;

    void UpdateCameraSmooth()
    {
        int N = cameraPathList.Count;
        if (N == 0) return;

        // 将 currentFrame 映射到“关键点区间”
        int maxKey = (N - 1) / keyStride;                               // 最后一个关键点索引
        int keyIdx = Mathf.Clamp(currentFrame / keyStride, 0, maxKey);   // 当前关键点
        int nextKey = Mathf.Min(keyIdx + 1, maxKey);                     // 下一个关键点

        int aIdx = keyIdx * keyStride;
        int bIdx = nextKey * keyStride;

        // 本区间内的归一化进度 t ∈ [0,1]
        float denom = Mathf.Max(1, keyStride);
        float t = Mathf.Clamp01((currentFrame - aIdx) / denom);

        Vector3 pos;
        if ( keyIdx > 0 && nextKey < maxKey)
        {
            // Catmull-Rom 样条：p1=a, p2=b，前后各取一个控制点
            int p0Idx = (keyIdx - 1) * keyStride;
            int p3Idx = Mathf.Min((nextKey + 1) * keyStride, N - 1);

            var p0 = cameraPathList[p0Idx];
            var p1 = cameraPathList[aIdx];
            var p2 = cameraPathList[bIdx];
            var p3 = cameraPathList[p3Idx];

            pos = CatmullRom(p0, p1, p2, p3, t);
        }
        else
        {
            // 边界或未启用样条：线性插值
            var a = cameraPathList[aIdx];
            var b = cameraPathList[bIdx];
            pos = Vector3.Lerp(a, b, t);
        }

        // 保持原来的 z
        pos.z = testCameraPos.transform.position.z;
        testCameraPos.transform.position = pos;
    }

    // Catmull–Rom 样条（均匀参数化，t ∈ [0,1]）
    static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;
        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

}