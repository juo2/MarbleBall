using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GameDemo : MonoBehaviour
{
    int ContinuousTryCount = 3;
    int ContinuousCount = 0;
    Bounds padsBounds = new Bounds();
    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    List<BarsCurve> combineFrameBarsList = new List<BarsCurve>(); 

    List<CurveType> curveTypeList = new List<CurveType>();

    [System.Serializable]
    public enum TryingType
    {
        None,
        Try_Vertical,
        Try_Horizontal,
    }

    TryingType tryingType = TryingType.None;

}
