using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class CornerDetector : MonoBehaviour
{
    public LineRenderer line;

    [Header("按顺序拖入所有折线的顶点（按连接顺序）")]
    public List<Vector3> points = new List<Vector3>();

    [Header("判定为角点的夹角阈值（度）")]
    [Range(0f, 180f)]
    public float angleThreshold = 10f;

    [Header("Gizmo 绘制时角点的大小")]
    public float gizmoSize = 0.1f;

    private void Awake()
    {
        for (int i = 0; i < line.positionCount; i++)
        {
            Vector3 pos = line.GetPosition(i);
            points.Add(pos);
        }

    }

    void OnDrawGizmos()
    {
        if (points == null || points.Count < 3)
            return;

        // 1) 先把整条折线用白线画一下（World 空间下）
        Gizmos.color = Color.white;
        for (int i = 0; i < points.Count - 1; i++)
        {
            if (points[i] == null || points[i + 1] == null) continue;
            Gizmos.DrawLine(points[i], points[i + 1]);
        }

        // 2) 获取主摄像机 (你也可以强制指定某个 Camera)
        Camera cam = Camera.main;
        if (cam == null)
            return;

        // 3) 对每个中间顶点 i (1 <= i <= Count - 2)，分别把它和前后两个点投影到屏幕空间
        for (int i = 1; i < points.Count - 1; i++)
        {
            if (points[i - 1] == null || points[i] == null || points[i + 1] == null)
                continue;

            Vector3 worldA = points[i - 1]; // 前一个点
            Vector3 worldB = points[i];     // 当前点 (候选角点)
            Vector3 worldC = points[i + 1]; // 后一个点

            // 投影到屏幕 (返回值的 z 分量用于判断点是否在摄像机前面)
            Vector3 screenA = cam.WorldToScreenPoint(worldA);
            Vector3 screenB = cam.WorldToScreenPoint(worldB);
            Vector3 screenC = cam.WorldToScreenPoint(worldC);

            // 如果当前点在摄像机视线后面 (z < 0)，就跳过 (因为看不到)
            if (screenB.z < 0)
                continue;

            // 4) 在屏幕平面 (2D) 上计算两段的单位方向向量
            Vector2 dirPrev2D = (new Vector2(screenB.x, screenB.y) - new Vector2(screenA.x, screenA.y)).normalized;
            Vector2 dirNext2D = (new Vector2(screenC.x, screenC.y) - new Vector2(screenB.x, screenB.y)).normalized;

            // 5) 计算屏幕空间下的夹角
            float screenAngle = Vector2.Angle(dirPrev2D, dirNext2D);

            // 6) 用这个角度与阈值比较，若超过阈值，则认为是“屏幕上的拐点”
            if (screenAngle > angleThreshold)
            {
                // 在对应的 worldB 位置画一个红色小立方体 (Gizmo)
                Gizmos.color = Color.red;
                Gizmos.DrawCube(worldB, Vector3.one * gizmoSize);

                // 可选：在场景里显示数值标签 (Editor 环境下)
#if UNITY_EDITOR
                Handles.Label(worldB + Vector3.up * gizmoSize * 0.5f,
                              screenAngle.ToString("F1") + "°");
#endif
            }
        }
    }
}
