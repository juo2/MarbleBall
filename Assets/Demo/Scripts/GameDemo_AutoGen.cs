using RenderHeads.Media.AVProMovieCapture;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.CinemachineSmoothPath;

public partial class GameDemo : MonoBehaviour
{
    CaptureBase captureBase;

    bool isAutoGen = false;

    float recordPreTime = 0.0f;

    private void OnGUI()
    {
        if(isAutoGen)
        {
            // 设置GUI的样式，默认样式即可
            GUIStyle style = new GUIStyle();
            style.fontSize = 60;
            style.normal.textColor = Color.red;

            GUI.Label(new Rect(10, 10, 200, 30), captureBase.CaptureStats.FPS.ToString(), style);
        }
    }

    void ResetBeigin()
    {
        //清空bars
        ClearAllBars();
        
        //清空pad
        ClearAllPad();

        //临时清除
        DestoryAllBars();

        currentOffsetIndex = 1;

        //清空三条线
        line.ClearLineRenderer();
        cameraProjection.positionCount = 0;
        cameraPathProjection.positionCount = 0;
        cameraPathList.Clear();
        cameraFollowList.Clear();
        cameraMovementList.Clear();
        cameraFollowMovementList.Clear();

        physicsStateList.Clear();
        cameraPosMap.Clear();

        cinemachineTrackedDolly.m_PathPosition = 0;

        m_currentFrame = 0;
        m_currentObjCount = -1;
        m_preObjCount = -1;

        isEditorCamera = true;
        UpdateCamera();

        ball.ResetState();

        padDataList.Clear();

        frameVelocityOffsetList.Clear();
        frameVelocityList.Clear();

        barCombineDataList.Clear();
        barLineDataList.Clear();
    }

    void SpaceKeyDown()
    {
        //Time.timeScale = midiHelper.suitMidiFacter;

        spacePlay = true;
        
        m_currentObjCount = -1;
        m_preObjCount = -1;

        //重置位置
        currentFrame = 0;
        ball.ResetState(physicsStateList[currentFrame]);
        visiblePadRange(0, 10);
        midiHelper.Stop();
        isPlay = false;
        cinemachineTrackedDolly.m_PathPosition = 0;
        cameraPos.transform.position = Vector3.zero;

        isEditorCamera = false;
        UpdateCamera();

        bGMPlayer.StopBgm();
    }

    void DestoryAllBars()
    {
        while(barsDemoPool.Count > 0)
        {
            var bars = barsDemoPool.Pop();
            DestroyImmediate(bars.gameObject);
        }

        while (barsCombineDemoPool.Count > 0)
        {
            var bars = barsCombineDemoPool.Pop();
            DestroyImmediate(bars.gameObject);
        }

        foreach(var key in barsLineDemoPool.Keys)
        {
            var barPools = barsLineDemoPool[key];
            while (barPools.Count > 0)
            {
                var bars = barPools.Pop();
                DestroyImmediate(bars.gameObject);
            }
        }

        while(padPool.Count > 0)
        {
            var pad = padPool.Pop();
            DestroyImmediate(pad.gameObject);
        }

    }


    void ClearAllBars()
    {
        if(barsDemoList.Count > 0)
        {
            RecycleBars(barsDemoList[0]);
            barsDemoList.Clear();
        }

        foreach (var bar in barsCombineDemoList)
        {
            RecycleBarsCombine(bar);
        }

        barsCombineDemoList.Clear();

        foreach (var bar in barsLineDemoList)
        {
            RecycleLineBars(bar);
        }

        barsLineDemoList.Clear();
    }

    void ClearAllPad()
    {
        foreach(var item in visiblePadDic)
        {
            RecyclePad(item.Value);
        }

        visiblePadDic.Clear();
    }
}
