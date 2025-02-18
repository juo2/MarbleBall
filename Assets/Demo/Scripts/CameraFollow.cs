using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // 需要跟随的目标对象
    public Transform target;

    // 相机与目标之间的偏移量
    public Vector3 offset;

    // 跟随平滑速度
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        // 如果没有设置目标，直接返回
        if (target == null)
            return;

        // 计算目标位置加上偏移量
        Vector3 desiredPosition = target.position + offset;

        // 使用Lerp平滑移动相机位置
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 更新相机位置
        transform.position = smoothedPosition;

        // 可选：如果需要让相机始终注视目标
        transform.LookAt(target);
    }
}
