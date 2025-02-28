using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public partial class GameDemo : MonoBehaviour
{
    [System.Serializable]
    public class SimulateData
    {
        public string simulateName;
        public int frame;
        public float rotation;
        public float bounciness;
        public float vOffsetX;
        public float vOffsetY;

        public Vector3 position;
        public Vector3 localScale;
        public Vector3 eulerAngles;
        public Vector3 debugCenter;
        public Vector3 debugSize;
        public Vector3 debugBestPos;

        public CurveType curveType;
        public DistanceType distanceType;

        public bool isBest = false;

        public List<PhysicsState> physicsStates;
    }

    [System.Serializable]
    private class BarsCurve
    {
        public int prefabIndex;
        public int frame;
        public bool inverse;
        public List<PhysicsState> physicsStates;
    }

    [System.Serializable]
    private class PadCurve
    {
        public float rotation;
        public int frame;
        public bool inverse;
        public List<PhysicsState> physicsStates;
    }

    bool isPass_Simulate = false;

    Dictionary<int, List<SimulateData>> simulateDataDic = new Dictionary<int, List<SimulateData>>();
    
    Dictionary<int, List<SimulateObj>> simulateDic = new Dictionary<int, List<SimulateObj>>();
    Dictionary<int, Transform> simulateTransformDic = new Dictionary<int, Transform>();

    Stack<SimulateObj> simulatePool = new Stack<SimulateObj>();
    Stack<Transform> simulateTransformPool = new Stack<Transform>();

    [SerializeField]
    SimulateObj simulateObj;

    [SerializeField]
    Transform simulatePoolTran;

    [SerializeField]
    Button simulateBarsDemoBtn;

    [SerializeField]
    BarsDemo simulateBarsDemo;

    [SerializeField]
    BarsDemo simulateBarsDemo2;

    [SerializeField]
    List<BarsDemo> barsDemoPrefabList;

    void SimulateV2V()
    {
        Vector3 center = Vector2.zero;
        Vector3 size = Vector2.zero;
        Vector3 bestPos = Vector2.zero;

        if (ball.velocity.x > 0)
        {
            center = verticalCenter2;
            bestPos = verticalBestPos2;
        }
        else
        {
            center = verticalCenter1;
            bestPos = verticalBestPos1;
        }

        size = verticalSize;

        SimulateV2V_Internal(center, size, bestPos);
    }

    void SimulateV2H()
    {
        Vector3 center = Vector2.zero;
        Vector3 size = Vector2.zero;
        Vector3 bestPos = Vector2.zero;

        if (ball.velocity.x > 0)
        {
            center = horizontalCenter1;
            bestPos = horizontalBestPos1;
        }
        else
        {
            center = horizontalCenter2;
            bestPos = horizontalBestPos2;
        }

        size = verticalSize;

        SimulateV2V_Internal(center, size, bestPos);
    }

    IEnumerator SimulateV2P()
    {
        int offsetFrame = frameOffsetList[currentOffsetIndex+1];

        List<float> rotationList = null;
        if (ball.velocity.x > 0)
        {
            rotationList = v_rotationList1;
        }
        else
        {
            rotationList = v_rotationList2;
        }

        foreach (var rotation in rotationList)
        {
            foreach (var bounciness in v_bouncinessPriorityList)
            {
                if (visiblePadDic.ContainsKey(currentOffsetIndex - 1))
                {
                    visiblePadDic[currentOffsetIndex - 1].gameObject.SetActive(false);
                }

                Pad pad_instance = InstantPad(ball.transform.position, ball.velocity, currentFrame, rotation, true);
                pad_instance.SetPhysicMaterialBounciness(bounciness);
                //pad_instance.SetBounds(center, size, bestPos);

                GameObject go = new GameObject($"rotation:{rotation}-bounciness:{bounciness}");
                pad_instance.transform.SetParent(go.transform);

                yield return DealWithBarCombine_Simluate(offsetFrame, go.transform);

                if (isPass_Simulate)
                {
                    go.name = "pass_" + go.name;
                }

                go.transform.SetParent(simulateParent);
                go.SetActive(false);
            }
        }
    }

    void SimulateV2V_Internal(Vector3 center, Vector3 size, Vector3 bestPos)
    {
        List<float> rotationList = null;

        if (ball.velocity.x > 0)
        {
            rotationList = v_rotationList1;
        }
        else
        {
            rotationList = v_rotationList2;
        }

        int backFrame = currentFrame;
        List<float> bouncinessPriorityList = v_bouncinessPriorityList;

        int offsetFrame = frameOffsetList[currentOffsetIndex+1];

        foreach (var rotation in rotationList)
        {
            foreach (var bounciness in bouncinessPriorityList)
            {
                simulateToFrame(backFrame);

                List<PhysicsState> currentList = new List<PhysicsState>();
                foreach (var ps in physicsStateList)
                {
                    currentList.Add(ps);
                }

                if (visiblePadDic.ContainsKey(currentOffsetIndex - 1))
                {
                    visiblePadDic[currentOffsetIndex - 1].gameObject.SetActive(false);
                }

                Pad pad_instance = InstantPad(ball.transform.position, ball.velocity, currentFrame, rotation, true);
                pad_instance.SetPhysicMaterialBounciness(bounciness);
                pad_instance.SetBounds(center, size, bestPos);

                GameObject go = new GameObject($"rotation:{rotation}-bounciness:{bounciness}");
                pad_instance.transform.SetParent(go.transform);

                LineRendererDemo lineRenderer = Instantiate<LineRendererDemo>(line, go.transform);
                lineRenderer.transform.position = Vector3.zero;
                lineRenderer.transform.rotation = Quaternion.identity;
                lineRenderer.transform.localScale = Vector3.one;

                for (int i = 0; i < offsetFrame; i++)
                {
                    Physics.Simulate(0.02f);
                    PhysicsState ps = new PhysicsState();
                    ps.position = ball.transform.position;
                    ps.rotation = ball.transform.rotation;
                    ps.velocity = ball.velocity;
                    ps.angularVelocity = ball.angularVelocity;
                    currentList.Add(ps);
                }

                lineRenderer.ResetLineRenderer(currentList);
                lineRenderer.gameObject.SetActive(true);
                go.transform.SetParent(simulateParent);
                go.SetActive(false);
            }
        }

        simulateToFrame(backFrame);
    }

    void SimulateH2H()
    {
        Vector3 center = Vector2.zero;
        Vector3 size = Vector2.zero;
        Vector3 bestPos = Vector2.zero;

        if (ball.velocity.x > 0)
        {
            center = horizontalCenter1;
            bestPos = horizontalBestPos1;
        }
        else
        {
            center = horizontalCenter2;
            bestPos = horizontalBestPos2;
        }
        size = horizontalSize;


        SimulateH2H_Internal(center, size, bestPos);
    }

    void SimulateH2V()
    {
        Vector3 center = Vector2.zero;
        Vector3 size = Vector2.zero;
        Vector3 bestPos = Vector2.zero;

        if (ball.velocity.x > 0)
        {
            center = verticalCenter1;
            bestPos = verticalBestPos1;
        }
        else
        {
            center = verticalCenter2;
            bestPos = verticalBestPos2;
        }
        size = verticalSize;

        SimulateH2H_Internal(center, size, bestPos);
    }

    void SimulateH2H_Internal(Vector3 center, Vector3 size, Vector3 bestPos)
    {
        int backFrame = currentFrame;

        List<float> bouncinessPriorityList = null;
        {
            bouncinessPriorityList = h_bouncinessPriorityList;
        }

        int offsetFrame = frameOffsetList[currentOffsetIndex+1];

        foreach (var bounciness in bouncinessPriorityList)
        {
            simulateToFrame(backFrame);

            List<PhysicsState> currentList = new List<PhysicsState>();
            foreach (var ps in physicsStateList)
            {
                currentList.Add(ps);
            }

            if (visiblePadDic.ContainsKey(currentOffsetIndex - 1))
            {
                visiblePadDic[currentOffsetIndex - 1].gameObject.SetActive(false);
            }

            Pad pad_instance = InstantPad(ball.transform.position, ball.velocity, currentFrame, 0, true);
            pad_instance.SetPhysicMaterialBounciness(bounciness);
            pad_instance.SetBounds(center, size, bestPos);

            GameObject go = new GameObject($"rotation:{0}-bounciness:{bounciness}");
            pad_instance.transform.SetParent(go.transform);

            LineRendererDemo lineRenderer = Instantiate<LineRendererDemo>(line, go.transform);
            lineRenderer.transform.position = Vector3.zero;
            lineRenderer.transform.rotation = Quaternion.identity;
            lineRenderer.transform.localScale = Vector3.one;

            for (int i = 0; i < offsetFrame; i++)
            {
                Physics.Simulate(0.02f);
                PhysicsState ps = new PhysicsState();
                ps.position = ball.transform.position;
                ps.rotation = ball.transform.rotation;
                ps.velocity = ball.velocity;
                ps.angularVelocity = ball.angularVelocity;
                currentList.Add(ps);
            }

            lineRenderer.ResetLineRenderer(currentList);
            lineRenderer.gameObject.SetActive(true);
            go.transform.SetParent(simulateParent);
            go.SetActive(false);
        }

        simulateToFrame(backFrame);
    }

    IEnumerator SimulateH2P()
    {
        int offsetFrame = frameOffsetList[currentOffsetIndex+1];

        foreach (var bounciness in h_bouncinessPriorityList)
        {
            if (visiblePadDic.ContainsKey(currentOffsetIndex - 1))
            {
                visiblePadDic[currentOffsetIndex - 1].gameObject.SetActive(false);
            }

            Pad pad_instance = InstantPad(ball.transform.position, ball.velocity, currentFrame, 0, true);
            pad_instance.SetPhysicMaterialBounciness(bounciness);
            //pad_instance.SetBounds(center, size, bestPos);

            GameObject go = new GameObject($"bounciness:{bounciness}");
            pad_instance.transform.SetParent(go.transform);

            yield return DealWithBarCombine_Simluate(offsetFrame, go.transform);
            
            if (isPass_Simulate)
            {
                go.name = "pass_" + go.name;
            }

            go.transform.SetParent(simulateParent);
            go.SetActive(false);
        }
    }

    IEnumerator DealWithBarCombine_Simluate(int offsetFrame,Transform parentTran)
    {
        isPass_Simulate = false;
        List<PhysicsState> currentList = new List<PhysicsState>();
        foreach (var ps in physicsStateList)
        {
            currentList.Add(ps);
        }

        LineRendererDemo lineRenderer = Instantiate<LineRendererDemo>(line, parentTran);
        lineRenderer.transform.position = Vector3.zero;
        lineRenderer.transform.rotation = Quaternion.identity;
        lineRenderer.transform.localScale = Vector3.one;

        void _simulate()
        {
            Physics.Simulate(0.02f);
            PhysicsState ps = new PhysicsState();
            ps.position = ball.transform.position;
            ps.rotation = ball.transform.rotation;
            ps.velocity = ball.velocity;
            ps.angularVelocity = ball.angularVelocity;
            currentList.Add(ps);
        }

        void _simulateToFrame(int _frame)
        {
            PhysicsState ps = currentList[_frame];
            ball.ResetState(ps);
            currentList.RemoveRange(_frame + 1, currentList.Count - 1 - _frame);
        }

        int backFrame = currentFrame;
        Vector3 prePos = ball.transform.position;

        //预测这一帧
        for (int i = 0; i < offsetFrame; i++)
        {
            _simulate();
        }

        Vector3 afterPos = ball.transform.position;
        if (afterPos.y < prePos.y - barCombine.beiginOffset)
        {
            int simlateFrame = 0;
            //回到之前
            _simulateToFrame(backFrame);

            while (true)
            {
                _simulate();
                simlateFrame++;
                //找到在beginOffset下方的点
                if (ball.transform.position.y < prePos.y - barCombine.beiginOffset)
                {
                    break;
                }
            }

            int frontFrame = simlateFrame;
            int lastFrame = offsetFrame - frontFrame;
            Vector3 bornPos = ball.transform.position;
            currentBarsCombine = InstantBarsCombine(bornPos, ball.velocity, currentFrame, parentTran);

            yield return new WaitForEndOfFrame();

            //阶段1
            for (int _f = 0; _f < lastFrame; _f++)
            {
                //模拟
                _simulate();
                simlateFrame++;
                if (currentBarsCombine.isPass1(ball.transform.position, ball.velocity))
                {
                    isPass_Simulate = true;
                    break;
                }
            }

            //阶段2
            if (isPass_Simulate)
            {
                isPass_Simulate = false;

                Vector3 stage1Pos = ball.transform.position;

                int stage1Frame = simlateFrame;
                int stage1BackFrame = simlateFrame + currentFrame;

                lastFrame = offsetFrame - stage1Frame;
                for (int i = 0; i < lastFrame; i++)
                {
                    _simulate();
                }

                currentBarsCombine.updateVirtualBars(false);
                currentBarsCombine.updateBar(stage1Pos, ball.transform.position);

                float barsCombineScale = currentBarsCombine.getScaleX();
                while (barsCombineScale > 0.1f)
                {
                    barsCombineScale = barsCombineScale - 0.1f;
                    currentBarsCombine.SetScaleX(barsCombineScale);

                    _simulateToFrame(stage1BackFrame);
                    for (int i = 0; i < lastFrame; i++)
                    {
                        _simulate();
                    }

                    if (currentBarsCombine.isPass2(ball.transform.position))
                    {
                        isPass_Simulate = true;
                        break;
                    }
                }
            }
        }

        lineRenderer.ResetLineRenderer(currentList);
        lineRenderer.gameObject.SetActive(true);

        simulateToFrame(backFrame);
    }

    void RecordPadInfo_Simluate(int frame, Vector3 debugCenter, Vector3 debugSize, Vector3 debugBestPos, float rotation,float bounciness, List<PhysicsState> physicsStates,float vOffsetX = 0f,float vOffsetY = 0f)
    {
        string _name = $"PAD_{frame}";
        RecordInfo_Simluate(_name, frame, rotation, bounciness, physicsStates, vOffsetX, vOffsetY);
    }

    void RecordBarsInfo_Simluate(int frame, float rotation, float bounciness, List<PhysicsState> physicsStates)
    {
        string _name = $"BARS_{frame}";
        RecordInfo_Simluate(_name, frame, rotation, bounciness, physicsStates);
    }

    Transform RecordInfo_Simluate(string simulateName, int frame, float rotation, float bounciness, List<PhysicsState> physicsStates, float vOffsetX = 0f, float vOffsetY = 0f)
    {
        string key = $"curveType:{curveType}-distanceType:{distanceType}-rotation:{rotation}-bounciness:{bounciness}-vOffsetX:{vOffsetX}-vOffsetY:{vOffsetY}";

        Transform parentTran = null;
        SimulateData simulateData = new SimulateData();
        simulateData.simulateName = simulateName;
        simulateData.frame = frame;
        simulateData.rotation = rotation;
        simulateData.bounciness = bounciness;
        simulateData.physicsStates = physicsStates;
        simulateData.vOffsetX = vOffsetX;
        simulateData.vOffsetY = vOffsetY;

        simulateData.isBest = false;

        simulateData.position = currentPad.transform.position;
        simulateData.localScale = currentPad.transform.localScale;
        simulateData.eulerAngles = currentPad.transform.eulerAngles;
        simulateData.debugCenter = currentPad.debugCenter;
        simulateData.debugSize = currentPad.debugSize;
        simulateData.debugBestPos = currentPad.debugBestPos;
        simulateData.curveType = curveType;
        simulateData.distanceType = distanceType;

        List<SimulateData> simulateDataList = null;
        List<SimulateObj> simulateObjList = null;
        if (simulateDataDic.ContainsKey(frame))
        {
            simulateDataList = simulateDataDic[frame];

            if (!isAutoGen)
                simulateObjList = simulateDic[frame];

            if (!isAutoGen)
                parentTran = simulateTransformDic[frame];
        }
        else
        {
            simulateDataList = new List<SimulateData>();
            simulateDataDic.Add(frame, simulateDataList);

            if (!isAutoGen)
            {
                simulateObjList = new List<SimulateObj>();
                simulateDic.Add(frame, simulateObjList);

                if (simulateTransformPool.Count > 0)
                {
                    parentTran = simulateTransformPool.Pop();
                }
                else
                {
                    GameObject go = new GameObject();
                    parentTran = go.transform;
                }

                parentTran.gameObject.name = simulateName;

                parentTran.SetParent(simulateParent);
                parentTran.localPosition = Vector3.zero;
                parentTran.localScale = Vector3.one;
                parentTran.rotation = Quaternion.identity;

                simulateTransformDic.Add(frame, parentTran);
            }
        }

        if (!isAutoGen)
        {
            SimulateObj so = null;
            if (simulatePool.Count > 0)
            {
                so = simulatePool.Pop();
            }
            else
            {
                so = Instantiate<SimulateObj>(simulateObj);
            }
            so.transform.SetParent(parentTran);
            so.Init(physicsStates, currentPad.transform.position,currentPad.transform.localScale,currentPad.transform.eulerAngles,currentPad.debugCenter,currentPad.debugSize,currentPad.debugBestPos);
            so.gameObject.name = $"{simulateName} {key}";
            so.gameObject.SetActive(false);
            simulateObjList.Add(so);
        }

        simulateDataList.Add(simulateData);

        return parentTran;
    }

    void RecordBestPoint_Simluate(int frame,float rotation,float bounciness,float vOffsetX = 0f,float vOffsetY = 0f)
    {
        if(simulateDataDic.ContainsKey(frame))
        {
            bool res = false;
            List<SimulateData> simulateDataList = simulateDataDic[frame];
            for(int i = 0;i < simulateDataList.Count;i++)
            {
                SimulateData sd = simulateDataList[i];
                if (sd.rotation == rotation && sd.bounciness == bounciness && sd.vOffsetX == vOffsetX && sd.vOffsetY == vOffsetY && sd.curveType == curveType && sd.distanceType == distanceType)
                {
                    simulateDataList[i].isBest = true;
                    res = true;

                    if (!isAutoGen)
                    {
                        List<SimulateObj> simulateObjList = simulateDic[frame];
                        SimulateObj simulateObj = simulateObjList[i];
                        simulateObj.name = "BEST --- " + simulateObj.name;
                    }

                    break;
                }
            }

            if(!res)
                Debug.LogError($"没找到 11111 frame:{frame} curveType:{curveType}  distanceType:{distanceType} rotation:{rotation} bounciness:{bounciness}");
        }
        else
        {
            Debug.LogError($"没找到 22222 frame:{frame} curveType:{curveType}  distanceType:{distanceType} rotation:{rotation} bounciness:{bounciness}");
        }
    }

    void Clear_Simulate()
    {
        simulateDataDic.Clear();

        foreach(var simulate in simulateDic)
        {
            List<SimulateObj> simulateObjList = simulate.Value;

            foreach(var s in simulateObjList)
            {
                s.transform.position = new Vector3(9999, 9999, 9999);
                s.transform.SetParent(simulatePoolTran);
                simulatePool.Push(s);
            }
        }

        foreach(var dic in simulateTransformDic)
        {
            dic.Value.transform.position = new Vector3(9999, 9999, 9999);
            dic.Value.transform.SetParent(simulatePoolTran);
            simulateTransformPool.Push(dic.Value);
        }

        simulateDic.Clear();
        simulateTransformDic.Clear();
    }


    void LoadSimulateData(List<KeyValuePair> SimulateDataDicList)
    {
        if (SimulateDataDicList.Count <= 0)
            return;

        Dictionary<int, List<SimulateData>> _simulateDataDic = new Dictionary<int, List<SimulateData>>();
        foreach (var pair in SimulateDataDicList)
        {
            _simulateDataDic.Add(pair.key, pair.value);
        }

        foreach (var dic in _simulateDataDic)
        {
            var list = dic.Value;

            List<SimulateObj> simulateObjList = new List<SimulateObj>();

            Transform parentTran = null;
            if (simulateTransformPool.Count > 0)
            {
                parentTran = simulateTransformPool.Pop();
            }
            else
            {
                GameObject go = new GameObject();
                parentTran = go.transform;
            }

            parentTran.localPosition = Vector3.zero;
            parentTran.localScale = Vector3.one;
            parentTran.rotation = Quaternion.identity;
            parentTran.gameObject.name = list[0].simulateName;
            parentTran.SetParent(simulateParent);

            simulateDic.Add(list[0].frame, simulateObjList);

            foreach (var sd in list)
            {
                string key = $"curveType:{sd.curveType}-distanceType:{sd.distanceType}-rotation:{sd.rotation}-bounciness:{sd.bounciness}-vOffsetX:{sd.vOffsetX}-vOffsetY:{sd.vOffsetY}";

                SimulateObj so = null;
                if (simulatePool.Count > 0)
                {
                    so = simulatePool.Pop();
                }
                else
                {
                    so = Instantiate<SimulateObj>(simulateObj);
                }
                so.transform.SetParent(parentTran);
                so.Init(sd.physicsStates, sd.position, sd.localScale, sd.eulerAngles, sd.debugCenter, sd.debugSize, sd.debugBestPos);
                
                if(sd.isBest)
                {
                    so.gameObject.name = $"BEST --- {sd.simulateName} {key}";
                }
                else
                {
                    so.gameObject.name = $"{sd.simulateName} {key}";
                }

                so.gameObject.SetActive(false);
                simulateObjList.Add(so);
            }
        }
    }


    void Pad_Simulate()
    {
        bool inverse = false;
        float rota = 60f;

        Vector3 v = new Vector3(1f, -1f, 0);

        int inv = 1;
        if (inverse)
            inv = -1;

        v = new Vector3(v.x * inv, v.y, v.z);

        ball.SetVelocity(v);
        physicsStateList[0].velocity = v;

        Pad _pad = InstantPad(ball.transform.position, ball.velocity, currentFrame, rota * inv, true);

        int simlateFrame = 0;
        for (int i = 0; i < 6; i++)
        {
            simulateNextFrame();
            simlateFrame++;
        }

        PadCurve padCurve = new PadCurve();
        padCurve.rotation = rota * inv;
        padCurve.frame = physicsStateList.Count - 1;
        padCurve.inverse = inverse;
        padCurve.physicsStates = physicsStateList.GetRange(1, physicsStateList.Count - 1);

        string json_str = JsonUtility.ToJson(padCurve);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "padCurve/temp.json"), json_str);
    }

    void Pad_Simulate2()
    {
        bool inverse = false;
        float rota = -10f;

        Vector3 v = new Vector3(-0.8f, -1.2f, 0);

        int inv = 1;
        if (inverse)
            inv = -1;

        v = new Vector3(v.x * inv, v.y, v.z);

        ball.SetVelocity(v);
        physicsStateList[0].velocity = v;

        Pad _pad = InstantPad(ball.transform.position, ball.velocity, currentFrame, rota * inv, true);
        _pad.SetPhysicMaterialBounciness(0.6f);

        int simlateFrame = 0;
        for (int i = 0; i < 32; i++)
        {
            simulateNextFrame();
            simlateFrame++;
        }

        //PadCurve padCurve = new PadCurve();
        //padCurve.rotation = rota * inv;
        //padCurve.frame = physicsStateList.Count - 1;
        //padCurve.inverse = inverse;
        //padCurve.physicsStates = physicsStateList.GetRange(1, physicsStateList.Count - 1);

        //string json_str = JsonUtility.ToJson(padCurve);
        //File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "padCurve/temp.json"), json_str);
    }

    void Bars_Simulate()
    {
        int backFrame = currentFrame;
        
        foreach (var rotation in h_rotationList1)
        {
            for (int j = 0; j < h_bouncinessPriorityList.Count; j++)
            {
                float bounciness = h_bouncinessPriorityList[j];
                simulateToFrame(backFrame, true, true);
                SetRotation(1, rotation);
                SetPhysicMaterialBounciness(1, bounciness);

                Vector3 prePos = ball.transform.position;

                int simlateFrame = 0;

                for (int i = 0; i < 999; i++)
                {
                    simulateNextFrame();
                    simlateFrame++;

                    Vector3 afterPos = ball.transform.position;

                    if (afterPos.y < prePos.y - 0.1f)
                    {
                        break;
                    }
                }

                BarsDemo barsDemo = InstantBars_Simulate(ball.transform.position, ball.velocity, simulateBarsDemo, false);

                for (int i = 0; i < 999; i++)
                {
                    simulateNextFrame();
                    simlateFrame++;

                    if (barsDemo.isPassXY(ball.transform.position, 0.1f))
                    {
                        break;
                    }
                }
                DestroyImmediate(barsDemo.gameObject);
                Debug.Log($"simlateFrame:{simlateFrame}");
            }
        }
    }


    IEnumerator Bars_Simulate2()
    {
        Physics.gravity = new Vector3(0, -5f, 0);

        int prefabIndex = 1;
        bool inverse = true;
        //Vector3 v = new Vector3(0.829f, -1.29f, 0);
        Vector3 v = new Vector3(0.35f, -0.5f, 0);
        //Vector3 v = new Vector3(-1f, -1f, 0);
        int inv = 1;
        if (inverse)
            inv = -1;

        v = new Vector3(v.x * inv, v.y, 0);
        //Vector3 v = new Vector3(2f, 0f, 0);

        int simlateFrame = 0;
        BarsDemo barsDemo = InstantBars_Simulate(ball.transform.position, ball.velocity, barsDemoPrefabList[prefabIndex], inverse);

        ball.SetVelocity(v);
        physicsStateList[0].velocity = v;

        yield return new WaitForSeconds(0.3f);

        ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x , ball.angularVelocity.y  -5f, ball.angularVelocity.z));
        physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;

         
        for (int i = 1; i < 20; i++)
        {
            ModifyFrameVelocity(i, 0.15f, 0f);
        }

        for (int i = 40; i < 55; i++)
        {
            ModifyFrameVelocity(i, 0.11f, 0f);
        }

        //for (int i = 30; i < 35; i++)
        //{
        //    ModifyFrameVelocity(i, 0.105f, 0f);
        //}

        //for (int i = 34; i < 40; i++)
        //{
        //    ModifyFrameVelocity(i, 0.1f, 0f);
        //}

        //for (int i = 0; i < 84; i++)
        for (int i = 0; i < 80; i++)
        {
            simulateNextFrame();
            simlateFrame++;

            ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x , ball.angularVelocity.y -5f , ball.angularVelocity.z));
            physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;
        }

        Debug.Log($"physicsStateList.Count:{physicsStateList.Count}");

        BarsCurve barsCurve = new BarsCurve();
        barsCurve.prefabIndex = prefabIndex;
        barsCurve.inverse = inverse;
        barsCurve.physicsStates = physicsStateList.GetRange(1, physicsStateList.Count - 1);
        barsCurve.frame = physicsStateList.Count - 1;

        string json_str = JsonUtility.ToJson(barsCurve);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "barCurve/temp.json"), json_str);

        logger($"simlateFrame:{simlateFrame}");
        Debug.Log($"simlateFrame:{simlateFrame}");
    }

    IEnumerator Bars_Simulate2_2()
    {
        Physics.gravity = new Vector3(0, -5f, 0);

        int prefabIndex = 1;
        bool inverse = true;
        //Vector3 v = new Vector3(0.829f, -1.29f, 0);
        Vector3 v = new Vector3(-0.35f, -0.5f, 0);
        //Vector3 v = new Vector3(-1f, -1f, 0);
        int inv = 1;
        if (inverse)
            inv = -1;

        v = new Vector3(v.x * inv, v.y, 0);
        //Vector3 v = new Vector3(2f, 0f, 0);

        int simlateFrame = 0;
        BarsDemo barsDemo = InstantBars_Simulate(ball.transform.position, ball.velocity, barsDemoPrefabList[prefabIndex], inverse);

        ball.SetVelocity(v);
        physicsStateList[0].velocity = v;

        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < 9; i++)
        {
            simulateNextFrame();
            simlateFrame++;

            if(barsDemo.isPassX(ball.transform.position))
            {
                break;
            }
        }

        for(int i = 10;i < 40;i++)
        {
            ModifyFrameVelocity(i, 0.2f, 0f);
        }

        for (int i = 9; i < 70; i++)
        {
            simulateNextFrame();
            simlateFrame++;

            if (barsDemo.isPassX(ball.transform.position))
            {
                break;
            }
        }


        Debug.Log($"physicsStateList.Count:{physicsStateList.Count}");

        BarsCurve barsCurve = new BarsCurve();
        barsCurve.prefabIndex = prefabIndex;
        barsCurve.inverse = inverse;
        barsCurve.physicsStates = physicsStateList.GetRange(1, physicsStateList.Count-1);
        barsCurve.frame = physicsStateList.Count-1;

        string json_str = JsonUtility.ToJson(barsCurve);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath,"barCurve/temp.json"), json_str);

        logger($"simlateFrame:{simlateFrame}");
        Debug.Log($"simlateFrame:{simlateFrame}");
    }

    void Bars_Simulate3()
    {
        int prefabIndex = 1;
        bool inverse = false;

        InstantBars_Simulate(ball.transform.position, ball.velocity, barsDemoPrefabList[prefabIndex], inverse);
        BarsCurve barsCurve = FindBarsCurve(prefabIndex, inverse);
        //List<PhysicsState> physicsStates = TransformPhysicsState_Simulate(barsCurve.physicsStates);
        List<PhysicsState> physicsStates = TransformPhysicsState_Simulate(barsCurve.physicsStates);
        AddPhysicsState(physicsStates);
        simulateToFrame(physicsStates.Count, true, true);

        barsCurve.physicsStates = physicsStateList.GetRange(1, physicsStateList.Count - 1);
        barsCurve.frame = physicsStateList.Count - 1;

        string json_str = JsonUtility.ToJson(barsCurve);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "barCurve/temp.json"), json_str);
    }

    IEnumerator Bars_Simulate4()
    {
        Physics.gravity = new Vector3(0, -5f, 0);

        int prefabIndex = 2;
        bool inverse = false;
        Vector3 v = new Vector3(0.5f, -1.0f, 0);
        int inv = 1;
        if (inverse)
            inv = -1;

        v = new Vector3(v.x * inv, v.y, 0);

        int simlateFrame = 0;
        BarsDemo barsDemo = InstantBars_Simulate(ball.transform.position, ball.velocity, simulateBarsDemo, inverse);

        ball.SetVelocity(v);
        physicsStateList[0].velocity = v;

        yield return new WaitForSeconds(0.3f);

        ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x, ball.angularVelocity.y - 5f, ball.angularVelocity.z));
        physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;

        for (int i = 2; i < 5; i++)
        {
            ModifyFrameVelocity(i, 0.4f, 0.4f);
        }

        for (int i = 5; i < 15; i++)
        {
            ModifyFrameVelocity(i, -0.2f, 0);
        }

        for (int i = 0; i < 15; i++)
        {
            simulateNextFrame(true,false);
            simlateFrame++;

            ball.SetAngularVelocity(new Vector3(ball.angularVelocity.x, ball.angularVelocity.y - 5f, ball.angularVelocity.z));
            physicsStateList[physicsStateList.Count - 1].angularVelocity = ball.angularVelocity;
        }

        BarsCurve barsCurve = new BarsCurve();
        barsCurve.prefabIndex = prefabIndex;
        barsCurve.inverse = inverse;
        barsCurve.physicsStates = physicsStateList.GetRange(1, physicsStateList.Count - 1);
        barsCurve.frame = physicsStateList.Count - 1;

        string json_str = JsonUtility.ToJson(barsCurve);
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "barCurve/temp.json"), json_str);

        logger($"simlateFrame:{simlateFrame}");
        Debug.Log($"simlateFrame:{simlateFrame}");
    }

    private List<PhysicsState> TransformPhysicsState_Simulate(List<PhysicsState> psList)
    {
        List<PhysicsState> physicsStates = new List<PhysicsState>();

        Vector3 prePos = Vector3.zero;
        Quaternion preRota = Quaternion.identity;

        for (int i = 0; i < psList.Count; i++)
        {
            PhysicsState physicsState = new PhysicsState();
            physicsState.velocity = psList[i].velocity;
            physicsState.angularVelocity = psList[i].angularVelocity;

            Vector3 nextPos = psList[i].position + ball.transform.position;
            Quaternion nextRota = psList[i].rotation * ball.transform.rotation;

            if (prePos != Vector3.zero)
            {
                Vector3 middlePos = Vector3.Lerp(prePos, nextPos, 0.5f);
                Quaternion middleRota = Quaternion.Lerp(preRota, nextRota, 0.5f);

                PhysicsState physicsState1 = new PhysicsState();
                physicsState1.velocity = Vector3.zero;
                physicsState1.angularVelocity = Vector3.zero;

                physicsState1.position = middlePos;
                physicsState1.rotation = middleRota;

                physicsStates.Add(physicsState1);
            }

            physicsState.position = psList[i].position + ball.transform.position;
            physicsState.rotation = psList[i].rotation * ball.transform.rotation;

            prePos = physicsState.position;
            preRota = physicsState.rotation;

            physicsStates.Add(physicsState);
        }

        return physicsStates;
    }

    void BarsScale_Simulate()
    {
        Vector3 prePos = ball.transform.position;

        int simlateFrame = 0;

        for (int i = 0; i < 999; i++)
        {
            simulateNextFrame();
            simlateFrame++;

            Vector3 afterPos = ball.transform.position;

            if (afterPos.y < prePos.y - 0.12f)
            {
                break;
            }
        }

        BarsScaleDemo barsDemo = InstantBarsScale_Simulate(ball.transform.position, ball.velocity);
        barsDemo.updateVirtualBars(true);

        int backFrame = currentFrame;
        int simlateFrame2 = simlateFrame;
    }

    int VirtualBarsScale_Simulate()
    {
        int barsIndex = -9999;
        Vector3 prePos = ball.transform.position;
        int simlateFrame = 0;

        for (int i = 0; i < 999; i++)
        {
            simulateNextFrame();
            simlateFrame++;

            Vector3 afterPos = ball.transform.position;

            if (afterPos.y < prePos.y - 0.12f)
            {
                break;
            }
            else if (ball.velocity.x > 0 && afterPos.x > prePos.x + 0.12f)
            {
                break;
            }
            else if (ball.velocity.x < 0 && afterPos.x < prePos.x - 0.12f)
            {
                break;
            }
        }

        int backFrame = currentFrame;
        float eulerAngles = -30f;
        float offsetE = -5f;
        //for (int i = 0;i < 10;i++)
        {
            //float euler = eulerAngles + offsetE * i;
            simulateToFrame(backFrame, true, true);
            BarsScaleDemo barsDemo = InstantBarsScale_Simulate(ball.transform.position, ball.velocity, eulerAngles);
            barsDemo.updateVirtualBars(true);

            for (int j = 0; j < 999; j++)
            {
                simulateNextFrame();
                simlateFrame++;

                if (barsDemo.isPassXY(ball.transform.position))
                {
                    break;
                }
            }

            //DestroyImmediate(barsDemo.gameObject);

            logger($"eulerAngles:{eulerAngles} simlateFrame:{simlateFrame}");
            Debug.Log($"eulerAngles:{eulerAngles} simlateFrame:{simlateFrame}");
        }

        return barsIndex;
    }


    private void SplineSample_Simulate(BarsDemo barsDemo,int n)
    {
        SampleCollection sampleCollection = new SampleCollection();
        //barsDemo.splineComputerVirtual.GetSamples(sampleCollection);

        SplineSample[] splineSamples = sampleCollection.samples;
        int _length = splineSamples.Length;
        List<SplinePoint> splinePointList = new List<SplinePoint>();

        Debug.Log($"SplineSample_Simulate n:{n}");

        for (int i = _length - 1; i >= n; i--)
        {
            SplineSample ss = splineSamples[i];
            SplinePoint splinePoint = new SplinePoint();
            splinePoint.position = barsDemo.transform.localToWorldMatrix.MultiplyPoint(ss.position);
            splinePoint.size = 1;
            splinePoint.normal = new Vector3(0, 0, 1);
            splinePointList.Add(splinePoint);

            if (i == n)
                barsDemo.endPoint.transform.position = new Vector3(splinePoint.position.x, splinePoint.position.y, splinePoint.position.z);
        }

        if(barsDemo.transform.eulerAngles.y == 180)
        {
            barsDemo.RefreshMesh(false);
        }
        else
        {
            barsDemo.RefreshMesh(true);
        }

        //barsDemo.splineComputer.SetPoints(splinePointList.ToArray());
        //barsDemo.splineComputer.ResampleTransform();
    }

    BarsScaleDemo InstantBarsScale_Simulate(Vector3 bornPos, Vector3 frameDir,float eulerAngles = -20f,int _frame = 0)
    {
        BarsScaleDemo bars_instance = Instantiate<BarsScaleDemo>(barScale);
        bars_instance.transform.SetParent(obstaclesParent.transform);
        bars_instance.transform.position = bornPos;
        bars_instance.transform.localScale = Vector3.one;

        if (frameDir.x > 0)
        {
            bars_instance.transform.eulerAngles = new Vector3(0, 0, eulerAngles);
        }
        else
        {
            bars_instance.transform.eulerAngles = new Vector3(0, 180, eulerAngles);
        }

        bars_instance.gameObject.SetActive(true);
        bars_instance.name = $"BARS_{_frame}";

        PhysicMaterial pm = new PhysicMaterial();
        pm.dynamicFriction = 0.0f;
        pm.staticFriction = 0.0f;
        pm.bounciness = 0.0f;
        pm.frictionCombine = PhysicMaterialCombine.Minimum;
        pm.bounceCombine = PhysicMaterialCombine.Minimum;
        pm.name = $"bounciness_{0}";

        bars_instance.virtualScaler.Find("SplineMesh1").GetComponent<BoxCollider>().material = pm;
        bars_instance.virtualScaler.Find("SplineMesh2").GetComponent<BoxCollider>().material = pm;

        return bars_instance;
    }

    BarsDemo InstantBars_Simulate(Vector3 framePos, Vector3 frameDir, BarsDemo barPerfab, bool inverse, int _frame = 0)
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

        bars_instance.transform.eulerAngles = inverse ? new Vector3(0,180,0) : Vector3.zero;
        bars_instance.inverse = inverse;
        if (bars_instance.followRotation)
        {
            float angle = Vector3.Angle(frameDir, Vector3.down);
            //if(angle > 0)
            //{
            //    bars_instance.transform.eulerAngles = new Vector3(bars_instance.transform.eulerAngles.x, bars_instance.transform.eulerAngles.y, 10);
            //}
            //else
            //{
            //    bars_instance.transform.eulerAngles = new Vector3(bars_instance.transform.eulerAngles.x, bars_instance.transform.eulerAngles.y, -10);
            //}
        }

        bars_instance.gameObject.SetActive(true);
        bars_instance.name = $"BARS_{_frame}_{barPerfab.name}";
        bars_instance.updateVirtualBars(true);
        //barsDemoList.Add(bars_instance);

        return bars_instance;
    }

}
