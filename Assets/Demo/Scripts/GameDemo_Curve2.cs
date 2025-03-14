using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GameDemo : MonoBehaviour
{
    /// <summary>
    /// 尝试将 N 分解为若干候选数，使得最后剩下的数在 minLeftover 到 maxLeftover 之间
    /// 返回的 Tuple 中，Item1 是候选数字的列表，Item2 是剩余数
    /// </summary>
    Tuple<List<int>, int> Decompose(int N, int minLeftover, int maxLeftover)
    {
        List<int> result = new List<int>();
        if (Backtrack(N, result, minLeftover, maxLeftover))
        {
            int sum = 0;
            foreach (int i in result)
            {
                BarsCurve tempBarsCurve = FindBarsCurve(i, false);
                sum += tempBarsCurve.frame;
            }
            int leftover = N - sum;
            return Tuple.Create(result, leftover);
        }
        else
        {
            return null; // 没有合适的分解方案
        }
    }

    /// <summary>
    /// 递归回溯算法：从剩余的数中减去候选数字，直到剩余数在 minLeftover 到 maxLeftover 之间
    /// </summary>
    bool Backtrack(int remaining, List<int> result, int minLeftover, int maxLeftover)
    {
        // 基本情况：剩余数在允许的范围内，返回 true
        if (remaining >= minLeftover && remaining <= maxLeftover)
        {
            return true;
        }

        for (int i = barsDemoPrefabList.Count - 1; i > 2; i--)
        {
            BarsCurve tempBarsCurve = FindBarsCurve(i, false);
            int candidate = tempBarsCurve.frame;
            // 只有当剩余数减去 candidate 后不小于 minLeftover 时，才尝试分解
            if (remaining - candidate >= minLeftover)
            {
                result.Add(i);
                if (Backtrack(remaining - candidate, result, minLeftover, maxLeftover))
                {
                    return true;
                }
                // 如果此路径无解，回溯
                result.RemoveAt(result.Count - 1);
            }
        }

        return false;
    }

    void VerticalToVertical_SimulateSuccess(int offsetIndex, int offsetFrame, CurveType _curveType)
    {

        logger($"    尝试 curveType : {_curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  ---success!!!");

        UpdateCurveType(_curveType);
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

    void HorizontalCombineSuccess(int offsetIndex, int offsetFrame)
    {
        logger($"    尝试 curveType : {curveType} --- offsetIndex : {offsetIndex} --- offsetFrame : {offsetFrame} --- isVertical:{isVertical} --- ContinuousCount:{ContinuousCount}  ---success!!!");

        RecordBarsCombine();
        EndCameraMovement(true);

        UpdateCurveType(curveType);
    }

    void VerticalCombineSuccess(int offsetIndex, int offsetFrame)
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

        CurveType preCurveType = LastCurveType();

        if (preCurveType == CurveType.HorizontalToHorizontal && (_curveType == CurveType.VerticalToVertical || _curveType == CurveType.VerticalToVertical_Simulate))
        {
            EndCameraMovement();
        }
        else if ((preCurveType == CurveType.VerticalToVertical || preCurveType == CurveType.VerticalToVertical_Simulate) && _curveType == CurveType.HorizontalToHorizontal)
        {
            EndCameraMovement();
        }

        curveTypeList.Add(_curveType);

        return;


        //CurveType preCurveType = LastCurveType();
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
            else if (_curveType == CurveType.VerticalLine || _curveType == CurveType.VerticalLine2 || _curveType == CurveType.VerticalPipline || _curveType == CurveType.VerticalPipline2)
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
                    //if (tryingType == TryingType.Try_Vertical)
                    //{
                    //    nextCurveType = CurveType.VerticalToVertical;
                    //    isVertical = true;
                    //    ContinuousCount++;
                    //}
                    //else if (tryingType == TryingType.Try_Horizontal)
                    //{
                    //    nextCurveType = CurveType.HorizontalToHorizontal;
                    //    isVertical = false;
                    //}
                    //强制指定水平
                    tryingType = TryingType.Try_Horizontal;
                    nextCurveType = CurveType.HorizontalToHorizontal;
                    isVertical = false;
                }
            }
            else if (tryingType == TryingType.Try_Vertical && _curveType == CurveType.HorizontalToHorizontal)
            {
                ContinuousCount = 0;
                nextCurveType = CurveType.HorizontalToVertical;
            }
            else if (tryingType == TryingType.Try_Horizontal && _curveType == CurveType.VerticalToVertical)
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

        if (nextCurveType != CurveType.None)
            curveType = nextCurveType;
    }


    IEnumerator HorizontalCurveFail(int offsetIndex, int offsetFrame)
    {
        if (offsetFrame >= 23 && offsetFrame <= 27)
        {
            if (CurveTypeRecord.Contains(CurveType.HorizontalToHorizontal) && !CurveTypeRecord.Contains(CurveType.HorizontalPipline))
            {
                curveType = CurveType.HorizontalPipline;
                yield return TryCurve(offsetIndex, offsetFrame);
            }
            else if (CurveTypeRecord.Contains(CurveType.HorizontalPipline) && !CurveTypeRecord.Contains(CurveType.HorizontalToHorizontal))
            {
                curveType = CurveType.HorizontalToHorizontal;
                yield return TryCurve(offsetIndex, offsetFrame);
            }
            else
            {
                BackupPlan(currentFrame, offsetIndex, offsetFrame);
            }
        }
        else if (offsetFrame > 50)
        {
            curveType = CurveType.HorizontalPipline2;
            yield return TryCurve(offsetIndex, offsetFrame);
        }
        else
        {
            BackupPlan(currentFrame, offsetIndex, offsetFrame);
        }

    }

    IEnumerator VerticalCurveFail(int offsetIndex, int offsetFrame)
    {

        if (offsetFrame > 50)
        {
            curveType = CurveType.VerticalPipline2;
            yield return TryCurve(offsetIndex, offsetFrame);
        }
        else
        {
            BackupPlan(currentFrame, offsetIndex, offsetFrame);
        }

    }
}
