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
        public int minFrame;
        public int maxFrame;
        public bool isVertical;
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

}
