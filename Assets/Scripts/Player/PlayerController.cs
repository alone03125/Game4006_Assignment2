using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("移动参数")]
    public CharacterController charController;
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float groundDrag = 0.1f;

    [Header("采集参数")]
    public float maxCollectDuration = 3f;
    public float collectRateMin = 20f;
    public float collectRateMax = 50f;
    public float collectorHeightDuringAnimation = 0.7f;  // 新增：采集时的碰撞箱高度
    private float originalCharControllerHeight;           // 保存初始高度

    [Header("引用")]
    public PlayerStamina stamina;
    public PlayerAnimatorController animController;
    public PlayerResourcesUI resourcesUI;
    public CameraController cameraController;
    private int collectibleCount = 0;          // 当前在采集范围内的资源节点数量


    private float currentSpeed;
    private bool canMove = true;
    private bool isCollecting = false;
    private ResourceNode currentCollectingNode;
    private float collectTime = 0f;

    private void Start()
    {
        if (charController == null)
            charController = GetComponent<CharacterController>();

        gameObject.tag = "Player";

        // 保存初始碰撞箱高度
        originalCharControllerHeight = charController.height;

        // 如果没有手动设置，自动查找CameraController
        if (cameraController == null)
            cameraController = FindObjectOfType<CameraController>();
    }

    private void Update()
    {
        if (!isCollecting && canMove)
        {
            HandleMovement();
        }
    }

    /// <summary>
    /// 处理玩家移动（根据摄像机前方方向）
    /// </summary>
    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 获取摄像机前方方向（用于计算移动方向）
        Vector3 cameraForward = cameraController.GetCameraForward();
        Vector3 cameraRight = cameraController.GetCameraRight();

        // 基于摄像机方向计算移动方向
        Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;

        // 检查是否奔跑
        bool isRunning = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.R)) && stamina.CanRun();

        // 更新体力
        stamina.UpdateStamina(isRunning);

        // 计算当前速度
        float speedMultiplier = stamina.GetSpeedMultiplier();
        currentSpeed = isRunning ? runSpeed : walkSpeed;
        currentSpeed *= speedMultiplier;

        // 播放动画
        if (moveDirection.magnitude > 0)
        {
            animController.PlayMove(isRunning && stamina.CanRun());
            charController.Move(moveDirection * currentSpeed * Time.deltaTime);

            // 让玩家身体面向移动方向
            RotatePlayerTowardsDirection(moveDirection);
        }
        else
        {
            animController.PlayIdle();
        }

        // 添加重力
        charController.Move(Vector3.down * groundDrag * Time.deltaTime);
    }

    /// <summary>
    /// 让玩家身体平滑地转向移动方向
    /// </summary>
    private void RotatePlayerTowardsDirection(Vector3 direction)
    {
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    /// <summary>
    /// 开始采集资源
    /// </summary>
    public void BeginCollectResource(ResourceNode node)
    {
        if (isCollecting)
            return;

        currentCollectingNode = node;
        canMove = false;
        isCollecting = true;
        collectTime = 0f;

        // 缩短碰撞箱高度
        charController.height = collectorHeightDuringAnimation;

        animController.PlayCollect(node.type);
        StartCoroutine(CollectResourceRoutine(node));
    }

    /// <summary>
    /// 采集资源协程
    /// </summary>
    private IEnumerator CollectResourceRoutine(ResourceNode node)
    {
        while (isCollecting && collectTime < maxCollectDuration && Input.GetKey(KeyCode.Space))
        {
            float deltaAmount = UnityEngine.Random.Range(collectRateMin, collectRateMax) * Time.deltaTime;
            resourcesUI.AddResource(node.type, deltaAmount);
            collectTime += Time.deltaTime;
            yield return null;
        }

        // 采集完成 - 恢复碰撞箱高度
        charController.height = originalCharControllerHeight;

        isCollecting = false;
        collectTime = 0f;
        canMove = true;

        // 当前节点消失
        node.OnCollectComplete();

        // 恢复动画和移动
        animController.ResetToIdle();
    }

    /// <summary>
    /// 获取玩家当前背包
    /// </summary>
    public PlayerResourcesUI GetResourcesUI()
    {
        return resourcesUI;
    }

    /// <summary>
    /// 获取玩家位置（供摄像机使用）
    /// </summary>
    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }
    /// <summary>
    /// 进入一个采集物范围时调用
    /// </summary>
    public void EnterCollectibleRange()
    {
        collectibleCount++;
        if (collectibleCount == 1 && resourcesUI != null)
            resourcesUI.ShowCollectPrompt(true);
    }

    /// <summary>
    /// 离开一个采集物范围时调用
    /// </summary>
    public void ExitCollectibleRange()
    {
        collectibleCount--;
        if (collectibleCount <= 0)
        {
            collectibleCount = 0;
            if (resourcesUI != null)
                resourcesUI.ShowCollectPrompt(false);
        }
    }
}