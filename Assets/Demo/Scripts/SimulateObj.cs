using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateObj : MonoBehaviour
{
    [SerializeField]
    LineRendererDemo projection;

    [SerializeField]
    Pad padSim;

    public void Init(List<PhysicsState> physicsStates,Vector3 position,Vector3 localScale,Vector3 eulerAngles,Vector3 debugCenter,Vector3 debugSize,Vector3 debugBestPos)
    {
        projection.ResetLineRenderer(physicsStates);

        padSim.transform.position = position;
        padSim.transform.localScale = localScale;
        padSim.transform.eulerAngles = eulerAngles;
        padSim.debugCenter = debugCenter;
        padSim.debugSize = debugSize;
        padSim.debugBestPos = debugBestPos;

        padSim.InitLineRenderer();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
