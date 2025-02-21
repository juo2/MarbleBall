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
    List<CurveType> curveTypeList = new List<CurveType>();

    [System.Serializable]
    public enum TryingType
    {
        None,
        Try_Vertical,
        Try_Horizontal,
    }

    TryingType tryingType = TryingType.None;

    IEnumerator TryCurve(int offsetIndex, int offsetFrame)
    {
        if(offsetIndex == 219)
        {
            logger($"开始尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical}");
        }
        else
        {
            logger($"开始尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical}");
        }
        CurveTypeRecord.Add(curveType);

        if (curveType == CurveType.VerticalToVertical)
        {

            stopwatch.Restart();

            VerticalToVertical(offsetIndex, offsetFrame);

            stopwatch.Stop();

            logger($"    运行 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount} ---运行时间:{stopwatch.ElapsedMilliseconds / 1000f}");

            if (g_isPass)
            {
                VerticalToVerticalSuccess(offsetIndex, offsetFrame, curveType);
            }
            else
            {
                logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  --- passType:{passType} --- fail...");

                yield return VerticalCurveFail(offsetIndex, offsetFrame);
            }
        }
        else if (curveType == CurveType.VerticalToHorizontal)
        {

            stopwatch.Restart();

            VerticalToHorizontal(offsetIndex, offsetFrame);

            stopwatch.Stop();

            logger($"    运行 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount} ---运行时间:{stopwatch.ElapsedMilliseconds / 1000f}");


            if (g_isPass)
            {
                VerticalToHorizontalSuccess(offsetIndex, offsetFrame, curveType);
            }
            else
            {
                logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  --- passType:{passType} --- fail...");

                yield return VerticalCurveFail(offsetIndex, offsetFrame);
            }
        }
        else if (curveType == CurveType.VerticalLine)
        {
            stopwatch.Restart();

            VerticalLine(offsetIndex, offsetFrame, spBarsCount, barsDemoPrefabList.Count - 2);

            stopwatch.Stop();

            logger($"    运行 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount} ---运行时间:{stopwatch.ElapsedMilliseconds / 1000f}");

            if (g_isPass)
            {
                VerticalLineSuccess(offsetIndex, offsetFrame);
            }
            else
            {
                logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  --- passType:{passType} --- fail...");

                yield return VerticalCurveFail(offsetIndex, offsetFrame);
            }
        }
        else if (curveType == CurveType.VerticalLine2)
        {
            stopwatch.Restart();

            VerticalLine(offsetIndex, offsetFrame, barsDemoPrefabList.Count - 2, barsDemoPrefabList.Count);

            stopwatch.Stop();

            logger($"    运行 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount} ---运行时间:{stopwatch.ElapsedMilliseconds / 1000f}");

            if (g_isPass)
            {
                VerticalLineSuccess(offsetIndex, offsetFrame);
            }
            else
            {
                logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  --- passType:{passType} --- fail...");

                yield return VerticalCurveFail(offsetIndex, offsetFrame);
            }
        }
        else if (curveType == CurveType.VerticalPipline)
        {
            VerticalPipline(offsetIndex, offsetFrame);

            if (g_isPass)
            {
                VerticalPiplineSuccess(offsetIndex, offsetFrame);
            }
            else
            {
                logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  --- passType:{passType} --- fail...");


                yield return VerticalCurveFail(offsetIndex, offsetFrame);
            }
        }
        else if (curveType == CurveType.VerticalPipline2)
        {
            stopwatch.Restart();

            VerticalPipline2(offsetIndex, offsetFrame);

            stopwatch.Stop();

            logger($"    运行 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount} ---运行时间:{stopwatch.ElapsedMilliseconds / 1000f}");


            if (g_isPass)
            {
                VerticalPipline2Success(offsetIndex, offsetFrame);
            }
            else
            {
                logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  --- passType:{passType} --- fail...");

                yield return VerticalCurveFail(offsetIndex, offsetFrame);
            }
        }
        else if (curveType == CurveType.HorizontalToHorizontal)
        {
            stopwatch.Restart();

            HorizontalToHorizontal(offsetIndex, offsetFrame);

            stopwatch.Stop();

            logger($"    运行 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount} ---运行时间:{stopwatch.ElapsedMilliseconds / 1000f}");

            if (g_isPass)
            {
                HorizontalToHorizontalSuccess(offsetIndex, offsetFrame, curveType);
            }
            else
            {
                logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  --- passType:{passType} --- fail...");

                yield return HorizontalCurveFail(offsetIndex, offsetFrame);
            }
        }
        else if (curveType == CurveType.HorizontalToVertical)
        {
            stopwatch.Restart();

            HorizontalToVertical(offsetIndex, offsetFrame);

            stopwatch.Stop();

            logger($"    运行 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount} ---运行时间:{stopwatch.ElapsedMilliseconds / 1000f}");

            if (g_isPass)
            {
                HorizontalToVerticalSuccess(offsetIndex, offsetFrame, curveType);
            }
            else
            {
                logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  --- passType:{passType} --- fail...");

                yield return HorizontalCurveFail(offsetIndex, offsetFrame);
            }
        }
        else if (curveType == CurveType.HorizontalLine)
        {
            stopwatch.Restart();

            HorizontalLine(offsetIndex, offsetFrame, spBarsCount, barsDemoPrefabList.Count - 2);

            stopwatch.Stop();

            logger($"    运行 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount} ---运行时间:{stopwatch.ElapsedMilliseconds / 1000f}");


            if (g_isPass)
            {
                HorizontalLineSuccess(offsetIndex, offsetFrame);
            }
            else
            {
                logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  --- passType:{passType} --- fail...");
                yield return HorizontalCurveFail(offsetIndex, offsetFrame);
            }
        }
        else if (curveType == CurveType.HorizontalLine2)
        {
            stopwatch.Restart();

            HorizontalLine(offsetIndex, offsetFrame, barsDemoPrefabList.Count - 2, barsDemoPrefabList.Count);

            stopwatch.Stop();

            logger($"    运行 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount} ---运行时间:{stopwatch.ElapsedMilliseconds / 1000f}");


            if (g_isPass)
            {
                HorizontalLineSuccess(offsetIndex, offsetFrame);
            }
            else
            {
                logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  --- passType:{passType} --- fail...");
                yield return HorizontalCurveFail(offsetIndex, offsetFrame);
            }
        }
        else if (curveType == CurveType.HorizontalPipline)
        {
            HorizontalPipline(offsetIndex, offsetFrame);

            if (g_isPass)
            {
                HorizontalPiplineSuccess(offsetIndex, offsetFrame);
            }
            else
            {
                logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  --- passType:{passType} --- fail...");

                yield return HorizontalCurveFail(offsetIndex, offsetFrame);
            }
        }
        else if (curveType == CurveType.HorizontalPipline2)
        {
            stopwatch.Restart();

            HorizontalPipline2(offsetIndex, offsetFrame);

            stopwatch.Stop();

            logger($"    运行 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount} ---运行时间:{stopwatch.ElapsedMilliseconds / 1000f}");

            if (g_isPass)
            {
                HorizontalPipline2Success(offsetIndex, offsetFrame);
            }
            else
            {
                logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  --- passType:{passType} --- fail...");
                yield return HorizontalCurveFail(offsetIndex, offsetFrame);
            }
        }
    }

    void HorizontalToHorizontal(int offsetIndex, int offsetFrame)
    {
        Vector3 center = Vector2.zero;
        Vector3 size = Vector2.zero;
        Vector3 bestPos = Vector2.zero;

        if (ball.velocity.x > 0)
        {
            center = horizontalCenter3;
            bestPos = horizontalBestPos3;
        }
        else
        {
            center = horizontalCenter4;
            bestPos = horizontalBestPos4;
        }
        size = horizontalSize;

        SetBounds(offsetIndex - 1, center, size, bestPos);

        distanceType = DistanceType.Far;

        HorizontalToHorizontal_Internal(offsetIndex, offsetFrame, h_rotationList2, h_bouncinessPriorityList2);

        if (!g_isPass)
        {
            logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  --- passType:{passType} --- fail...");

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

            SetBounds(offsetIndex - 1, center, size, bestPos);

            distanceType = DistanceType.Near;

            HorizontalToHorizontal_Internal(offsetIndex, offsetFrame, h_rotationList1, h_bouncinessPriorityList1);
        }
    }

    void HorizontalToVertical(int offsetIndex, int offsetFrame)
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

        SetPhysicMaterialBounciness(offsetIndex - 1, defalutBounciness);
        SetBounds(offsetIndex - 1, center, size, bestPos);

        distanceType = DistanceType.Far;

        HorizontalToHorizontal_Internal(offsetIndex, offsetFrame, h_rotationList1, h_bouncinessPriorityList);
    }

    void HorizontalLine(int offsetIndex, int offsetFrame,int startIndex, int endIndex)
    {
        currentLineBars = null;

        int backFrame = currentFrame;

        foreach (var rotation in h_rotationList1)
        {
            for (int j = 0; j < h_bouncinessPriorityList.Count; j++)
            {
                float bounciness = h_bouncinessPriorityList[j];
                simulateToFrame(backFrame, true, true);
                SetRotation(offsetIndex - 1, rotation);
                SetPhysicMaterialBounciness(offsetIndex - 1, bounciness);

                for(int i = startIndex; i < endIndex; i++)
                {
                    BarsLine_Curve(offsetIndex, offsetFrame,i);

                    if (g_isPass)
                    {
                        break;
                    }
                    else
                    {
                        if (currentLineBars != null)
                        {
                            RecycleLineBars(currentLineBars);
                            currentLineBars = null;
                            //回到之前
                            simulateToFrame(backFrame, true, true);
                        }
                    }
                }

                if (g_isPass)
                {
                    break;
                }
            }

            if (g_isPass)
            {
                break;
            }
        }
    }

    void HorizontalPipline(int offsetIndex, int offsetFrame)
    {
        HorizontalLine(offsetIndex, offsetFrame, 0, spBarsCount);
    }

    void HorizontalPipline2(int offsetIndex, int offsetFrame)
    {
        currentBarsCombine = null;

        int backFrame = currentFrame;

        foreach (var rotation in h_rotationList1)
        {
            for (int j = 0; j < h_bouncinessPriorityList.Count; j++)
            {
                float bounciness = h_bouncinessPriorityList[j];
                simulateToFrame(backFrame, true, true);
                SetRotation(offsetIndex - 1, rotation);
                SetPhysicMaterialBounciness(offsetIndex - 1, bounciness);

                DealWithBarCombine(offsetIndex, offsetFrame);

                if (g_isPass)
                {
                    break;
                }
                else
                {
                    if (currentBarsCombine != null)
                    {
                        RecycleBarsCombine(currentBarsCombine);
                        currentBarsCombine = null;
                        //回到之前
                        simulateToFrame(backFrame, true, true);
                    }
                }

                
            }

            if (g_isPass)
            {
                break;
            }
        }
       
    }

    void VerticalToVertical(int offsetIndex, int offsetFrame)
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

        SetPhysicMaterialBounciness(offsetIndex - 1, verticalBounciness);
        SetBounds(offsetIndex - 1, center, size, bestPos);

        distanceType = DistanceType.Far;

        VerticalToVertical_Internal(offsetIndex, offsetFrame);
    }

    void VerticalToHorizontal(int offsetIndex, int offsetFrame)
    {
        Vector3 center = Vector2.zero;
        Vector3 size = Vector2.zero;
        Vector3 bestPos = Vector2.zero;
        {
            logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  --- passType:{passType} --- fail...");

            if (ball.velocity.x > 0)
            {
                center = horizontalCenter4;
                bestPos = horizontalBestPos4;
            }
            else
            {
                center = horizontalCenter3;
                bestPos = horizontalBestPos3;
            }

            size = horizontalSize;

            SetPhysicMaterialBounciness(offsetIndex - 1, verticalBounciness);
            SetBounds(offsetIndex - 1, center, size, bestPos);

            distanceType = DistanceType.Far;

            VerticalToVertical_Internal(offsetIndex, offsetFrame);
        }
    }

    void VerticalLine(int offsetIndex, int offsetFrame, int startIndex, int endIndex)
    {
        currentLineBars = null;

        int backFrame = currentFrame;

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
            for (int j = 0; j < v_bouncinessPriorityList.Count; j++)
            {
                float bounciness = v_bouncinessPriorityList[j];
                simulateToFrame(backFrame, true, true);

                SetRotation(offsetIndex - 1, rotation);
                SetPhysicMaterialBounciness(offsetIndex - 1, bounciness);

                for (int i = startIndex; i < endIndex; i++)
                {
                    ClearMistakeCollisionTimes();

                    BarsLine_Curve(offsetIndex, offsetFrame, i);

                    if (g_isPass)
                    {
                        bool mistakeCollision = DetectMistakeCollisionTimes();
                        if (!mistakeCollision)
                            break;
                        else
                            g_isPass = false;
                    }

                    if (!g_isPass)
                    {
                        if (currentLineBars != null)
                        {
                            RecycleLineBars(currentLineBars);
                            currentLineBars = null;
                            //回到之前
                            simulateToFrame(backFrame, true, true);
                        }
                    }
                }

                if (g_isPass)
                {
                    break;
                }
            }

            if (g_isPass)
            {
                break;
            }
        }
    }


    void VerticalPipline(int offsetIndex, int offsetFrame)
    {
        VerticalLine(offsetIndex, offsetFrame, 0, spBarsCount);
    }

    void VerticalPipline2(int offsetIndex, int offsetFrame)
    {
        currentBarsCombine = null;

        int backFrame = currentFrame;

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
            for (int j = 0; j < v_bouncinessPriorityList.Count; j++)
            {
                float bounciness = v_bouncinessPriorityList[j];
                simulateToFrame(backFrame, true, true);

                SetRotation(offsetIndex - 1, rotation);
                SetPhysicMaterialBounciness(offsetIndex - 1, bounciness);

                ClearMistakeCollisionTimes();

                DealWithBarCombine(offsetIndex, offsetFrame);

                if (g_isPass)
                {
                    bool mistakeCollision = DetectMistakeCollisionTimes();
                    if (!mistakeCollision)
                        break;
                    else
                        g_isPass = false;
                }

                if (!g_isPass)
                {
                    if (currentBarsCombine != null)
                    {
                        RecycleBarsCombine(currentBarsCombine);
                        currentBarsCombine = null;
                        //回到之前
                        simulateToFrame(backFrame, true, true);
                    }
                }

            }

            if (g_isPass)
            {
                break;
            }
        }
    }

    void HorizontalToHorizontal_Internal(int offsetIndex, int offsetFrame, List<float> rotationList, List<float> bouncinessPriorityList)
    {
        passType = PassType.None;

        int backFrame = currentFrame;

        g_isPass = false;
        List<PhysicsState> minPhysicsState = null;
        Vector3 minPos = Vector3.zero;
        float minBounciness = defalutBounciness;
        float minRotation = float.MaxValue;
        float minDistance = float.MaxValue;

        foreach (var rotation in rotationList)
        {
            foreach (var bounciness in bouncinessPriorityList)
            {
                simulateToFrame(backFrame, true, true);
                SetRotation(offsetIndex - 1, rotation);
                SetPhysicMaterialBounciness(offsetIndex - 1, bounciness);

                ClearMistakeCollisionTimes();

                for (int i = 0; i < offsetFrame; i++)
                {
                    simulateNextFrame();
                }

                bool mistakeCollision = DetectMistakeCollisionTimes();

                List<PhysicsState> _list = GetCurrentPhysicsState(backFrame);
                RecordPadInfo_Simluate(backFrame, currentPad.debugCenter, currentPad.debugSize, currentPad.debugBestPos, rotation, bounciness, _list);

                if (!mistakeCollision)
                {
                    if (!g_isPass)
                    {
                        float distance = Vector3.Distance(ball.transform.position, currentPad.bestPos);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            minPhysicsState = _list;
                            minBounciness = bounciness;
                            minRotation = rotation;
                            minPos = ball.transform.position;
                        }

                        if (currentPad.bounds.Contains(ball.transform.position))
                        {
                            g_isPass = true;

                            minDistance = distance;
                            minPhysicsState = _list;
                            minBounciness = bounciness;
                            minRotation = rotation;
                            minPos = ball.transform.position;
                        }
                    }
                    else
                    {
                        if (currentPad.bounds.Contains(ball.transform.position))
                        {
                            float distance = Vector3.Distance(ball.transform.position, currentPad.bestPos);
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                minPhysicsState = _list;
                                minBounciness = bounciness;
                                minRotation = rotation;
                                minPos = ball.transform.position;
                            }
                        }
                    }
                }
            }

            if (g_isPass)
            {
                break;
            }
        }

        if (minRotation != float.MaxValue && minDistance != float.MaxValue)
        {
            RecordBestPoint_Simluate(backFrame, minRotation, minBounciness);
            RecordBestPoint_Curve(backFrame, minRotation, minBounciness, minDistance, currentPad.bounds, minPhysicsState);
        }

        if (g_isPass)
        {
            SetPhysicMaterialBounciness(offsetIndex - 1, minBounciness);
            SetRotation(offsetIndex - 1, minRotation);
            OverwritePhysicsState(minPhysicsState);
            simulateToFrame(backFrame + offsetFrame, true, true);

            passType = PassType.pass;
        }
        else
        {
            passType = PassType.less;

            if (ball.velocity.x > 0 && minPos.x > currentPad.bounds.max.x)
            {
                passType = PassType.greater;
            }
            else if (ball.velocity.x < 0 && minPos.x < currentPad.bounds.min.x)
            {
                passType = PassType.greater;
            }

            if (minPos.y < currentPad.bounds.min.y)
            {
                passType = PassType.greater;
            }

            simulateToFrame(backFrame, true, true);
        }
    }

    void VerticalToVertical_Internal(int offsetIndex, int offsetFrame)
    {
        g_isPass = false;
        passType = PassType.None;
        SetColor(offsetIndex - 1, Color.red);

        int backFrame = currentFrame;

        List<float> rotationList = null;
        if (ball.velocity.x > 0)
        {
            rotationList = v_rotationList1;
        }
        else
        {
            rotationList = v_rotationList2;
        }

        List<PhysicsState> minPhysicsState = null;
        Vector3 minPos = Vector3.zero;
        float minRotation = 90f;
        float minBounciness = verticalBounciness;
        float minDistance = float.MaxValue;

        foreach (var rotation in rotationList)
        {
            foreach (var bounciness in v_bouncinessPriorityList)
            {
                simulateToFrame(backFrame, true, true);
                SetRotation(offsetIndex - 1, rotation);
                SetPhysicMaterialBounciness(offsetIndex - 1, bounciness);

                ClearMistakeCollisionTimes();

                for (int i = 0; i < offsetFrame; i++)
                {
                    simulateNextFrame();
                }

                bool mistakeCollision = DetectMistakeCollisionTimes();
                List<PhysicsState> _list = GetCurrentPhysicsState(backFrame);
                RecordPadInfo_Simluate(backFrame, currentPad.debugCenter, currentPad.debugSize, currentPad.debugBestPos, rotation, bounciness, _list);

                if (!mistakeCollision)
                {
                    if (!g_isPass)
                    {
                        float distance = Vector3.Distance(ball.transform.position, currentPad.bestPos);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            minPhysicsState = _list;
                            minBounciness = bounciness;
                            minRotation = rotation;
                            minPos = ball.transform.position;
                        }

                        if (currentPad.bounds.Contains(ball.transform.position))
                        {
                            g_isPass = true;

                            minDistance = distance;
                            minPhysicsState = _list;
                            minBounciness = bounciness;
                            minRotation = rotation;
                            minPos = ball.transform.position;
                        }
                    }
                    else
                    {
                        if (currentPad.bounds.Contains(ball.transform.position))
                        {
                            float distance = Vector3.Distance(ball.transform.position, currentPad.bestPos);
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                minPhysicsState = _list;
                                minBounciness = bounciness;
                                minRotation = rotation;
                                minPos = ball.transform.position;
                            }
                        }
                    }
                }
            }
        }

        if (minRotation != float.MaxValue && minDistance != float.MaxValue)
        {
            RecordBestPoint_Simluate(backFrame, minRotation, minBounciness);
            RecordBestPoint_Curve(backFrame, minRotation, minBounciness, minDistance, currentPad.bounds, minPhysicsState);
        }


        if (g_isPass)
        {
            SetRotation(offsetIndex - 1, minRotation);
            SetPhysicMaterialBounciness(offsetIndex - 1, minBounciness);
            OverwritePhysicsState(minPhysicsState);
            simulateToFrame(backFrame + offsetFrame, true, true);
            passType = PassType.pass;
        }

        if (!g_isPass)
        {
            simulateToFrame(backFrame, true, true);
        }
    }


    void VerticalToVerticalSuccess(int offsetIndex, int offsetFrame, CurveType _curveType)
    {
        logger($"    尝试 curveType : {_curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  ---success!!!");


        if (tryingType == TryingType.Try_Vertical)
            ContinuousCount++;

        UpdateCurveType(_curveType);

    }

    void VerticalToHorizontalSuccess(int offsetIndex, int offsetFrame, CurveType _curveType)
    {
        logger($"    尝试 curveType : {_curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  ---success!!!");

        if (tryingType == TryingType.Try_Horizontal)
            ContinuousCount++;

        UpdateCurveType(_curveType);

        EndCameraMovement();
    }

    void HorizontalToHorizontalSuccess(int offsetIndex, int offsetFrame, CurveType _curveType)
    {
        logger($"    尝试 curveType : {_curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  --- success!!!");
        
        if (tryingType == TryingType.Try_Horizontal)
            ContinuousCount++;

        UpdateCurveType(_curveType);

    }

    void HorizontalToVerticalSuccess(int offsetIndex, int offsetFrame, CurveType _curveType)
    {
        logger($"    尝试 curveType : {_curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} ---  ContinuousCount:{ContinuousCount}  --- success!!!");

        if (tryingType == TryingType.Try_Vertical)
            ContinuousCount++;

        EndCameraMovement();

        UpdateCurveType(_curveType);

    }

    void VerticalLineSuccess(int offsetIndex, int offsetFrame)
    {
        logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} ---  ContinuousCount:{ContinuousCount}  --- success!!!");

        RecordBarsLine();
        EndCameraMovement(true);

        UpdateCurveType(curveType);

    }

    void HorizontalLineSuccess(int offsetIndex, int offsetFrame)
    {
        logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} ---  ContinuousCount:{ContinuousCount}  --- success!!!");

        RecordBarsLine();
        EndCameraMovement(true);

        UpdateCurveType(curveType);

    }

    void HorizontalPiplineSuccess(int offsetIndex, int offsetFrame)
    {
        logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} ---  ContinuousCount:{ContinuousCount}  --- success!!!");

        RecordBarsLine();
        EndCameraMovement(true);

        UpdateCurveType(curveType);

    }

    void VerticalPiplineSuccess(int offsetIndex, int offsetFrame)
    {
        logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  ---success!!!");

        RecordBarsLine();
        EndCameraMovement(true);

        UpdateCurveType(curveType);

    }
   
    void HorizontalPipline2Success(int offsetIndex, int offsetFrame)
    {
        logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  ---success!!!");

        RecordBarsCombine();
        EndCameraMovement(true);

        UpdateCurveType(curveType);

    }

    void VerticalPipline2Success(int offsetIndex, int offsetFrame)
    {
        logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  ---success!!!");

        RecordBarsCombine();
        EndCameraMovement(true);

        UpdateCurveType(curveType);
    }


    void UpdateCurveType(CurveType _curveType)
    {
        currentRecordJson.successCurveType = _curveType;
        curveTypeList.Add(_curveType);

        CurveType preCurveType = LastCurveType();
        CurveType nextCurveType = CurveType.None;

        UpdatePadsBounds();

        if (_curveType == CurveType.HorizontalToVertical)
        {
            if (ContinuousCount >= ContinuousTryCount)
            {
                nextCurveType = CurveType.VerticalToVertical;
                tryingType = TryingType.Try_Vertical;

                ContinuousCount = 0;
                isVertical = true;
            }
            else
            {
                isVertical = true;
                if (tryingType == TryingType.Try_Vertical)
                {
                    nextCurveType = CurveType.VerticalToVertical;
                }
                else
                {
                    if (IsCanTurn())
                    {
                        nextCurveType = CurveType.VerticalToHorizontal;
                    }
                    else
                    {
                        nextCurveType = CurveType.VerticalToVertical;
                    }
                }
            }
        }
        else if (_curveType == CurveType.VerticalToHorizontal)
        {
            if (ContinuousCount >= ContinuousTryCount)
            {
                nextCurveType = CurveType.HorizontalToHorizontal;
                tryingType = TryingType.Try_Horizontal;

                ContinuousCount = 0;
                isVertical = false;
            }
            else
            {
                isVertical = false;
                if (tryingType == TryingType.Try_Vertical)
                {
                    nextCurveType = CurveType.HorizontalToVertical;
                }
                else if (tryingType == TryingType.Try_Horizontal)
                {
                    nextCurveType = CurveType.HorizontalToHorizontal;
                }
            }
        }
        else
        {
            if (_curveType == CurveType.HorizontalLine || _curveType == CurveType.HorizontalLine2 || _curveType == CurveType.HorizontalPipline || _curveType == CurveType.HorizontalPipline2)
            {

                if (ContinuousCount >= ContinuousTryCount)
                {
                    nextCurveType = CurveType.VerticalToVertical;
                    tryingType = TryingType.Try_Vertical;

                    ContinuousCount = 0;
                    isVertical = true;
                }
                else
                {
                    if (tryingType == TryingType.Try_Vertical)
                    {
                        nextCurveType = CurveType.VerticalToVertical;
                        isVertical = true;
                    }
                    else if (tryingType == TryingType.Try_Horizontal)
                    {
                        nextCurveType = CurveType.HorizontalToHorizontal;
                        isVertical = false;
                        ContinuousCount++;
                    }
                }
            }
            else if(_curveType == CurveType.VerticalLine || _curveType == CurveType.VerticalLine2 || _curveType == CurveType.VerticalPipline || _curveType == CurveType.VerticalPipline2)
            {
                if (ContinuousCount >= ContinuousTryCount)
                {
                    nextCurveType = CurveType.HorizontalToHorizontal;
                    tryingType = TryingType.Try_Horizontal;

                    ContinuousCount = 0;
                    isVertical = false;
                }
                else
                {
                    if (tryingType == TryingType.Try_Vertical)
                    {
                        nextCurveType = CurveType.VerticalToVertical;
                        isVertical = true;
                        ContinuousCount++;
                    }
                    else if (tryingType == TryingType.Try_Horizontal)
                    {
                        nextCurveType = CurveType.HorizontalToHorizontal;
                        isVertical = false;
                    }
                }
            }
            else if(tryingType == TryingType.Try_Vertical && _curveType == CurveType.HorizontalToHorizontal)
            {
                ContinuousCount = 0;
                nextCurveType = CurveType.HorizontalToVertical;
            }
            else if(tryingType == TryingType.Try_Horizontal && _curveType == CurveType.VerticalToVertical)
            {
                ContinuousCount = 0;
                nextCurveType = CurveType.VerticalToHorizontal;
            }
            else
            {
                if (ContinuousCount >= ContinuousTryCount)
                {
                    if (isVertical)
                    {
                        if (IsCanTurn())
                        {
                            nextCurveType = CurveType.VerticalToHorizontal;
                            tryingType = TryingType.Try_Horizontal;
                            ContinuousCount = 0;
                        }
                    }
                    else
                    {
                        nextCurveType = CurveType.HorizontalToVertical;
                        tryingType = TryingType.Try_Vertical;
                        ContinuousCount = 0;
                    }
                }
                else
                {
                    nextCurveType = _curveType;
                }
            }
        }

        if(nextCurveType != CurveType.None)
            curveType = nextCurveType;
    }


    bool IsCanBars(Vector3 prePos,Vector3 afterPos,float offset = 0.12f)
    {
        bool res = false;
        if (afterPos.y < prePos.y - offset)
        {
            res = true;
        }
        //else if (ball.velocity.x > 0 && afterPos.x > prePos.x + offset)
        //{
        //    res = true;
        //}
        //else if (ball.velocity.x < 0 && afterPos.x < prePos.x - offset)
        //{
        //    res = true;
        //}
        return res;
    }

    int VirtualBars_Curve(int _offsetIndex, int offsetFrame)
    {
        int barsIndex = -9999;
        Vector3 prePos = ball.transform.position;
        int simlateFrame = 0;
        int backFrame = currentFrame;

        for (int i = 0; i < 100; i++)
        {
            simulateNextFrame();
            simlateFrame++;

            Vector3 afterPos = ball.transform.position;

            if (IsCanBars(prePos, afterPos, 0.1f))
            {
                break;
            }
        }

        int lastFrame = offsetFrame - simlateFrame;
        BarsDemo barsDemo = null;
        if (lastFrame > 0)
        {
            //barsDemo = InstantBars_Simulate(ball.transform.position, ball.velocity, simulateBarsDemo);
            barsDemo.updateVirtualBars(true);
            for (int i = 0; i < lastFrame; i++)
            {
                simulateNextFrame();
                simlateFrame++;
                
                if (barsDemo.isPassXY(ball.transform.position,0.1f))
                {
                    if(offsetFrame - simlateFrame <= 5)
                    {
                        barsIndex = 0;
                    }
                    else //太长了
                    {
                        barsIndex = -1;
                    }
                    break;
                }
            }
        }

        if (barsIndex == -9999)
        {
            if(barsDemo != null)
            {
                barsIndex = barsDemo.SplineSampleIndex(ball.transform.position);
            }
            else
            {
                barsIndex = -1;
            }
        }

        if (barsDemo != null)
        {
            if (barsDemo != null)
                RecycleBars(barsDemo);

            barsDemo = null;
            //回到之前
            simulateToFrame(backFrame, true, true);
        }

        return barsIndex;
    }

    IEnumerator Bars_Curve(int _offsetIndex, int offsetFrame,int n)
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

        if(IsCanBars(prePos,afterPos,0.1f))
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
                if (IsCanBars(prePos, ball.transform.position, 0.1f))
                {
                    g_isPass = true;
                    break;
                }
            }

            if (g_isPass)
            {
                bool mistakeCollision = DetectMistakeCollisionTimes();
                g_isPass = !mistakeCollision;
            }

            //currentBars = InstantBars_Simulate(ball.transform.position, ball.velocity, simulateBarsDemo);
            SplineSample_Simulate(currentBars, n);
            currentBars.updateVirtualBars(false);
            yield return new WaitForSeconds(0.3f);

            if (g_isPass)
            {
                g_isPass = false;

                int frontFrame = simlateFrame;
                int lastFrame = offsetFrame - frontFrame;

                for (int _f = 0; _f < lastFrame; _f++)
                {
                    //模拟
                    simulateNextFrame();
                    simlateFrame++;
                    if (currentBars.isPassXY(ball.transform.position,0.1f))
                    {
                        if(offsetFrame - simlateFrame <= 2)
                        {
                            g_isPass = true;
                        }
                        else
                        {
                            g_isPass = false;
                        }
                    }
                }
            }
        }

        if (!g_isPass)
        {
            if (currentBars != null)
                RecycleBars(currentBars);

            currentBars = null;
            //回到之前
            simulateToFrame(backFrame, true, true);
        }
    }

    BarsCurve FindBarsCurve(int prefabIndex,bool inverse)
    {
        foreach(var barsCurve in barsCurveList)
        {
            if(barsCurve.prefabIndex == prefabIndex && barsCurve.inverse == inverse)
            {
                return barsCurve;
            }
        }

        return null;
    }


    int prefabIndex_Curve = 0;

    void BarsLine_Curve(int _offsetIndex, int offsetFrame, int prefabIndex)
    {
        Debug.Log($"prefabIndex:{prefabIndex}");
        BarConfig barConfig = barsConfig.BarConfigList[prefabIndex];

        distanceType = DistanceType.None;

        int remainFrame = -1;

        int backFrame = currentFrame;
        Vector3 prePos = ball.transform.position;

        g_isPass = false;

        //预测这一帧
        for (int i = 0; i < offsetFrame; i++)
        {
            simulateNextFrame();
        }

        Vector3 afterPos = ball.transform.position;

        if (IsCanBars(prePos, afterPos, barConfig.beiginOffset))
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

                if (IsCanBars(prePos, ball.transform.position, barConfig.beiginOffset))
                {
                    g_isPass = true;
                    break;
                }
            }

            if (g_isPass)
            {
                bool mistakeCollision = DetectMistakeCollisionTimes();
                g_isPass = !mistakeCollision;
            }

            currentLineBars = InstantBarsLine(ball.transform.position, ball.velocity, prefabIndex , backFrame, backFrame + offsetFrame);

            if (g_isPass)
            {
                g_isPass = false;
                BarsCurve barsCurve = FindBarsCurve(prefabIndex, currentLineBars.inverse);
                if (barsCurve != null)
                {
                    if (offsetFrame == 100 && prefabIndex == prefabIndex_Curve)
                    {
                        g_isPass = true;
                        List<PhysicsState> physicsStates = TransformPhysicsState(barsCurve.physicsStates);
                        AddPhysicsState(physicsStates);
                        simulateToFrame(backFrame + simlateFrame + physicsStates.Count, true, true);

                        for (int _f = 0; _f < 999; _f++)
                        {
                            simulateNextFrame();
                            simlateFrame++;

                            if (currentLineBars.isPassXY(ball.transform.position, barConfig.endOffset))
                            {
                                break;
                            }
                        }

                        int totalFrame = simlateFrame + physicsStates.Count;
                        UpdateFrameList(currentOffsetIndex, totalFrame);

                        if (prefabIndex_Curve == 1)
                        {
                            prefabIndex_Curve = 0;
                        }
                        else if (prefabIndex_Curve == 0)
                        {
                            prefabIndex_Curve = 1;
                        }
                    }
                }
                else
                {
                    int frontFrame = simlateFrame;
                    int lastFrame = offsetFrame - frontFrame;

                    for (int _f = 0; _f < lastFrame; _f++)
                    {
                        //模拟
                        simulateNextFrame();
                        simlateFrame++;
                        if (currentLineBars.isPassXY(ball.transform.position, barConfig.endOffset))
                        {
                            remainFrame = offsetFrame - simlateFrame;
                            if (remainFrame <= barConfig.remainFrame)
                            {
                                g_isPass = true;
                            }
                            else
                            {
                                g_isPass = false;
                            }
                            break;
                        }
                    }
                }
            }
        }

        if (!g_isPass)
        {
            if (currentLineBars != null)
                RecycleLineBars(currentLineBars);

            currentLineBars = null;
            //回到之前
            simulateToFrame(backFrame, true, true);
        }
        else
        {
            for(int i = 0;i < remainFrame;i++)
            {
                simulateNextFrame();
            }
        }
    }

    CurveType LastCurveType()
    {
        if(curveTypeList.Count - 1 >= 0)
        {
            return curveTypeList[curveTypeList.Count - 1];
        }
        else
        {
            return CurveType.None;
        }
    }

    bool IsCanTurn()
    {
        bool isCanTurn = false;
        
        if(currentPad.transform.position.x < padsBounds.center.x && ball.velocity.x < 0)
        {
            isCanTurn = true;
        }
        else if(currentPad.transform.position.x > padsBounds.center.x && ball.velocity.x > 0)
        {
            isCanTurn = true;
        }

        return isCanTurn;
    }


    //true 左边
    //false 右边
    bool TurningDir()
    {
        return currentPad.transform.position.x < padsBounds.center.x;
    }


    void UpdatePadsBounds()
    {
        if(padDataList.Count > 1)
        {
            if(padDataList.Count == 2)
            {
                PadData pd0 = padDataList[0];
                PadData pd1 = padDataList[1];
                Vector3 _center = (pd0.pos + pd1.pos) / 2;
                Vector3 _size = new Vector3(Mathf.Abs(pd0.pos.x - pd1.pos.x), Mathf.Abs(pd0.pos.y - pd1.pos.y), 0);
                padsBounds = new Bounds(_center, _size);
            }
            else
            {
                PadData pd = padDataList[currentOffsetIndex - 1];
                padsBounds.Encapsulate(pd.pos);
            }
        }
    }

    void BackupPlan(int backFrame, int offsetIndex, int offsetFrame)
    {
        currentRecordJson.isBackUp = true;

        int v_frame = -1;
        if (offsetIndex == 74)
        {
            v_frame = -1;
        }

        if(isVertical && offsetFrame < 12)
        {
            logger($"    启动 BackupPlan padCurve");

            PadCurve padCurve = null;
            foreach(var pc in padCurveList)
            {
                if(pc.inverse == currentPad.inverse)
                {
                    padCurve = pc;
                    break;
                }
            }

            List<PhysicsState> physicsStates = TransformPhysicsState(padCurve.physicsStates);
            AddPhysicsState(physicsStates);
            simulateToFrame(currentFrame + physicsStates.Count, true, true);
            int remainFrame = offsetFrame - physicsStates.Count;

            for(int i = 0;i < remainFrame;i++)
            {
                simulateNextFrame();
            }

            SetRotation(offsetIndex - 1, padCurve.rotation);

            VerticalToHorizontalSuccess(offsetIndex, offsetFrame, CurveType.VerticalToHorizontal);

            g_isPass = true;
        }
        else if (BestPointDic.ContainsKey(backFrame))
        {
            List<BestPoint> bestPointList = BestPointDic[backFrame];
            BestPoint bestPoint = bestPointList[0];
            SetBounds(offsetIndex - 1, bestPoint.center, bestPoint.size, bestPoint.bestPos);
            SetPhysicMaterialBounciness(offsetIndex - 1, bestPoint.bounciness);
            SetRotation(offsetIndex - 1, bestPoint.rotation);

            logger($"    启动 BackupPlan bestPoint.curveType:{bestPoint.curveType}");

            Vector3 minPos = bestPoint.physicsStates[bestPoint.physicsStates.Count - 1].position;

            bool isH = false;
            bool isV = false;
            if (ball.velocity.x > 0 && minPos.x < currentPad.bounds.min.x)
            {
                isH = true;
            }
            else if (ball.velocity.x < 0 && minPos.x > currentPad.bounds.max.x)
            {
                isH = true;
            }

            if (minPos.y > currentPad.bounds.max.y)
            {
                isV = true;
            }

            int LengthX = 1;
            int LengthY = 1;
            if (isH)
            {
                LengthX = 50;
            }
            if (isV)
            {
                LengthY = 50;
            }

            float vOffsetY = 0.0f;
            float vOffsetX = 0.0f;

            if (isV || isH)
            {
                for (int y = 0; y < LengthY; y++)
                {
                    vOffsetY = -y * 0.1f;
                    simulateToFrame(backFrame, true, true);
                    v_frame = backFrame + 1;
                    ModifyFrameVelocity(backFrame + 1, vOffsetX, vOffsetY);
                    ClearMistakeCollisionTimes();

                    for (int i = 0; i < offsetFrame; i++)
                    {
                        simulateNextFrame();
                    }

                    bool mistakeCollision = DetectMistakeCollisionTimes();

                    if (ball.transform.position.y < currentPad.bounds.max.y && !mistakeCollision)
                    {
                        break;
                    }
                }

                for (int x = 0; x < LengthX; x++)
                {
                    vOffsetX = x * 0.05f;
                    simulateToFrame(backFrame, true, true);
                    v_frame = backFrame + 1;
                    ModifyFrameVelocity(backFrame + 1, vOffsetX, vOffsetY);
                    ClearMistakeCollisionTimes();

                    for (int i = 0; i < offsetFrame; i++)
                    {
                        simulateNextFrame();
                    }

                    bool mistakeCollision = DetectMistakeCollisionTimes();

                    if (ball.velocity.x > 0 && ball.transform.position.x > currentPad.bounds.min.x && !mistakeCollision)
                    {
                        break;
                    }
                    else if (ball.velocity.x < 0 && ball.transform.position.x < currentPad.bounds.max.x && !mistakeCollision)
                    {
                        break;
                    }
                }
            }
            else
            {
                AddPhysicsState(bestPoint.physicsStates);
                simulateToFrame(backFrame + offsetFrame, true, true);
            }

            if (v_frame != -1)
                RemoveFrameVelocityRecord(v_frame);


            if (bestPoint.curveType == CurveType.VerticalToVertical)
            {
                VerticalToVerticalSuccess(offsetIndex, offsetFrame, bestPoint.curveType);
            }
            else if (bestPoint.curveType == CurveType.VerticalToHorizontal)
            {
                VerticalToHorizontalSuccess(offsetIndex, offsetFrame, bestPoint.curveType);
            }
            else if (bestPoint.curveType == CurveType.HorizontalToHorizontal)
            {
                HorizontalToHorizontalSuccess(offsetIndex, offsetFrame, bestPoint.curveType);
            }
            else if(bestPoint.curveType == CurveType.HorizontalToVertical)
            {
                HorizontalToVerticalSuccess(offsetIndex, offsetFrame, bestPoint.curveType);
            }

            g_isPass = true;
        }
        else
        {
            Debug.LogError($"BestPointDic 的 frame:{backFrame} 不存在");
        }
    }

    void RecordBestPoint_Curve(int backFrame,float minRotation,float minBounciness,float minDistance,Bounds bounds,List<PhysicsState> minPhysicsState)
    {
        BestPoint bestPoint = null;
        List<BestPoint> bestPointList = null;

        if (!BestPointDic.ContainsKey(backFrame))
        {
            bestPointList = new List<BestPoint>();
            BestPointDic.Add(backFrame, bestPointList);
        }
        else
        {
            bestPointList = BestPointDic[backFrame];
        }

        bestPoint = new BestPoint();
        bestPoint.bounciness = minBounciness;
        bestPoint.rotation = minRotation;
        bestPoint.physicsStates = minPhysicsState;
        bestPoint.minDistance = minDistance;
        bestPoint.curveType = curveType;
        bestPoint.center = currentPad.debugCenter;
        bestPoint.size = currentPad.debugSize;
        bestPoint.bestPos = currentPad.debugBestPos;

        bestPointList.Add(bestPoint);
        bestPointList.Sort((a, b) => a.minDistance.CompareTo(b.minDistance));
    }

    void ClearMistakeCollisionTimes()
    {
        //重置计数器
        foreach (var _tempPad in visiblePadDic)
        {
            _tempPad.Value.mistakeCollisionTimes = 0;
        }

        foreach (var _bars in barsCombineDemoList)
        {
            _bars.mistakeCollisionTimes = 0;
        }
    }

    bool DetectMistakeCollisionTimes()
    {
        bool mistakeCollision = false;
        foreach (var _tempPad in visiblePadDic)
        {
            if (_tempPad.Value.mistakeCollisionTimes > 0)
            {
                mistakeCollision = true;
                break;
            }
        }

        if(!mistakeCollision)
        {
            foreach (var _bars in barsCombineDemoList)
            {
                if (_bars.mistakeCollisionTimes > 0)
                {
                    mistakeCollision = true;
                    break;
                }
            }
        }
        
        return mistakeCollision;
    }

    IEnumerator HorizontalCurveFail(int offsetIndex, int offsetFrame)
    {
        currentRecordJson.curveTypeList.Add(curveType);

        CurveType result = CurveTypePriority.FirstOrDefault(c => !CurveTypeRecord.Contains(c));

        if(result != CurveType.None)
        {
            curveType = result;
            yield return TryCurve(offsetIndex, offsetFrame);
        }
        else
        {
            if (!CurveTypeRecord.Contains(CurveType.HorizontalToHorizontal))
            {
                curveType = CurveType.HorizontalToHorizontal;
                yield return TryCurve(offsetIndex, offsetFrame);
            }
            else if (!CurveTypeRecord.Contains(CurveType.HorizontalToVertical))
            {
                curveType = CurveType.HorizontalToVertical;
                yield return TryCurve(offsetIndex, offsetFrame);
            }
            else if (!CurveTypeRecord.Contains(CurveType.HorizontalPipline2))
            {
                curveType = CurveType.HorizontalPipline2;
                yield return TryCurve(offsetIndex, offsetFrame);
            }
            else if (!CurveTypeRecord.Contains(CurveType.HorizontalLine2))
            {
                curveType = CurveType.HorizontalLine2;
                yield return TryCurve(offsetIndex, offsetFrame);
            }
            else
            {
                BackupPlan(currentFrame, offsetIndex, offsetFrame);
            }
        }
    }

    IEnumerator VerticalCurveFail(int offsetIndex, int offsetFrame)
    {
        currentRecordJson.curveTypeList.Add(curveType);

        CurveType result = CurveTypePriority.FirstOrDefault(c => !CurveTypeRecord.Contains(c));

        if (result != CurveType.None)
        {
            curveType = result;
            yield return TryCurve(offsetIndex, offsetFrame);
        }
        else
        {
            if (!CurveTypeRecord.Contains(CurveType.VerticalToVertical))
            {
                curveType = CurveType.VerticalToVertical;
                yield return TryCurve(offsetIndex, offsetFrame);
            }
            else if (!CurveTypeRecord.Contains(CurveType.VerticalToHorizontal))
            {
                curveType = CurveType.VerticalToHorizontal;
                yield return TryCurve(offsetIndex, offsetFrame);
            }
            else if (!CurveTypeRecord.Contains(CurveType.VerticalPipline2))
            {
                curveType = CurveType.VerticalPipline2;
                yield return TryCurve(offsetIndex, offsetFrame);
            }
            else if (!CurveTypeRecord.Contains(CurveType.VerticalLine2))
            {
                curveType = CurveType.VerticalLine2;
                yield return TryCurve(offsetIndex, offsetFrame);
            }
            else
            {
                BackupPlan(currentFrame, offsetIndex, offsetFrame);
            }
        }
            
    }
}
