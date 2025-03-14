using RenderHeads.Media.AVProMovieCapture;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.CinemachineSmoothPath;

public partial class GameDemo : MonoBehaviour
{
    [SerializeField]
    Button genAllBtn;

    CaptureBase captureBase;

    bool isAutoGen = false;

    float recordPreTime = 0.0f;

    IEnumerator GenAll()
    {
        canvasObj.SetActive(false);
        captureGUI.enabled = false;
        isAutoGen = true;
        string _path = Path.Combine(Application.streamingAssetsPath, "midi");
        List<string> _files = CheckForMidiFiles(_path);

        foreach(var file in _files)
        {
            ResetBeigin();

            Debug.Log($"正在生成 {file} 中......");

            yield return new WaitForSeconds(1f);

            yield return StartGenerating(file, 0);

            yield return new WaitForSeconds(3f);

            Debug.Log("生成完毕！！！！！！！！！！！！！");

            SaveData(file);

            SpaceKeyDown();

            captureObj.SetActive(true);
            captureBase.FilenamePrefix = file;

            yield return new WaitForSeconds(spacePlayTime-1);

            Debug.Log("开始录像！！！！！！！！！！！！！");

            captureBase.StartCapture();

            recordPreTime = Time.time;

            yield return new WaitForSeconds(2f);

            Debug.Log("开始检测 isPlay！！！！！！！！！！！！！");

            yield return new WaitUntil(() => isPlay == false);
            yield return new WaitForSeconds(1f);

            Debug.Log("停止录像 ！！！！！！！！！！！！！");

            captureBase.StopCapture();
            captureObj.SetActive(false);
        }
    }

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

    IEnumerator StartGenerating(string midi,int count)
    {
        initLogger(midi);

        foreach (Pad pad in showPadList)
        {
            pad.gameObject.SetActive(false);
        }

        if (currentOffsetIndex != 1)
        {
            PhysicsState ps = physicsStateList[physicsStateList.Count - 1];
            ball.ResetState(ps);
            currentFrame = physicsStateList.Count - 1;

            int _key = padDataList.Count - 1;
            //最后一个踢掉
            padDataList.RemoveAt(_key);

            if (visiblePadDic.ContainsKey(_key))
            {
                visiblePadDic.Remove(_key);
            }
        }

        midiHelper.Load(midi);
        logger($"midiHelper.suitMidiFacter = {midiHelper.suitMidiFacter}");

        SkinManager.Instance.RefreshSkinByName(midiHelper.currentMidi);

        LOOPCOUNT = count;

        yield return GameInit();
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

        Clear_Simulate();

        isEditorCamera = true;
        UpdateCamera();

        ball.ResetState();

        isVertical = false;
        ContinuousCount = 0;

        padDataList.Clear();
        BestPointDic.Clear();

        frameVelocityOffsetList.Clear();
        frameVelocityList.Clear();

        barCombineDataList.Clear();
        barLineDataList.Clear();

        currentCameraMovement = new CameraMovement();
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
