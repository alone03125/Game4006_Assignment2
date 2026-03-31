using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("鼠标控制")]
    public bool hideMouse = true;
    public float mouseSensitivity = 1f;
    public float rotationSpeed = 100f;

    [Header("摄像机跟随")]
    public PlayerController player;
    public float followDistance = 5f;      // 距玩家的距离
    public float followHeight = 1.5f;      // 相对玩家的高度
    public float followSmoothness = 5f;   // 跟随平滑度

    private float yaw = 0f;      // 水平旋转角度
    private float pitch = 0f;    // 竖直旋转角度

    private void Start()
    {
        // 如果没有手动设置，自动查找玩家
        if (player == null)
            player = FindObjectOfType<PlayerController>();

        // 控制鼠标显示/隐藏
        Cursor.visible = !hideMouse;
        Cursor.lockState = hideMouse ? CursorLockMode.Locked : CursorLockMode.None;

        // 初始化摄像机角度
        Vector3 playerPos = player.GetPlayerPosition();
        Vector3 directionToPlayer = (transform.position - playerPos).normalized;
        yaw = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg;
        pitch = Mathf.Asin(directionToPlayer.y) * Mathf.Rad2Deg;
    }

    private void Update()
    {
        HandleRotation();

        // 允许运行时切换鼠标状态（按ESC键）
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            hideMouse = !hideMouse;
            Cursor.visible = !hideMouse;
            Cursor.lockState = hideMouse ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

    private void LateUpdate()
    {
        // 在LateUpdate中更新摄像机位置，保证在玩家移动之后
        UpdateCameraPosition();
    }

    /// <summary>
    /// 处理鼠标旋转（更新yaw和pitch）
    /// </summary>
    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * mouseSensitivity * Time.deltaTime;

        // 更新yaw和pitch
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
    }

    /// <summary>
    /// 更新摄像机位置和朝向
    /// </summary>
    private void UpdateCameraPosition()
    {
        Vector3 playerPos = player.GetPlayerPosition();

        // 基于yaw和pitch计算摄像机位置
        Vector3 offset = new Vector3(
            Mathf.Sin(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad) * followDistance,
            Mathf.Sin(pitch * Mathf.Deg2Rad) * followDistance + followHeight,
            Mathf.Cos(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad) * followDistance
        );

        Vector3 targetCameraPos = playerPos + offset;

        // 平滑跟随
        transform.position = Vector3.Lerp(transform.position, targetCameraPos, Time.deltaTime * followSmoothness);

        // 摄像机看向玩家
        Vector3 lookTarget = playerPos + Vector3.up * (followHeight * 0.5f);
        transform.LookAt(lookTarget);
    }

    /// <summary>
    /// 获取摄像机前方方向（供玩家移动使用）
    /// </summary>
    public Vector3 GetCameraForward()
    {
        Vector3 forward = transform.forward;
        forward.y = 0;  // 忽略竖直分量，只用水平方向
        return forward.normalized;
    }

    /// <summary>
    /// 获取摄像机右方方向（供玩家移动使用）
    /// </summary>
    public Vector3 GetCameraRight()
    {
        Vector3 right = transform.right;
        right.y = 0;  // 忽略竖直分量，只用水平方向
        return right.normalized;
    }
}