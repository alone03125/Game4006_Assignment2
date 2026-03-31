using UnityEngine;
using System.Collections;
using TMPro;

public class DeliveryMachine : MonoBehaviour
{
    [Header("交货参数")]
    public Transform coinEjectPoint;
    public GameObject coinPrefab;
    public float deliveryRateMin = 20f;
    public float deliveryRateMax = 50f;

    [Header("交互范围")]
    public float interactionDistance = 3f;

    [Header("UI引用")]
    public DeliveryPanelUI deliveryPanel;

    [Header("玩家引用")]
    public PlayerController playerController;
    public PlayerResourcesUI playerResources;
    public CoinManager coinManager;

    [Header("显示玩家货物数量")]
    public TextMeshProUGUI[] playerResourceLabels = new TextMeshProUGUI[4];

    [Header("显示交货量")]
    public TextMeshProUGUI[] deliveryAmountLabels = new TextMeshProUGUI[4];

    private DeliveryTask currentTask;
    private bool isDeliveryInProgress = false;
    private bool canStartDelivery = true;
    private bool switchRequested = false;      // 按F切换资源标志
    private Coroutine deliveryRoutine = null;
    private PlayerResourcesUI resourcesUI;
    private bool wasInRange = false;   // 记录上次是否在范围内，避免每帧重复设置

    private void Start()
    {
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
        if (playerResources == null)
            playerResources = FindObjectOfType<PlayerResourcesUI>();
        if (coinManager == null)
            coinManager = FindObjectOfType<CoinManager>();
        if (resourcesUI == null)
            resourcesUI = FindObjectOfType<PlayerResourcesUI>();
        StartCoroutine(TaskCycle());
    }

    private void Update()
    {
        // 实时更新玩家货物显示
        UpdatePlayerResourceDisplay();
        // 实时更新交货量显示（文字）
        UpdateDeliveryAmountDisplay();

        // 处理交互输入
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isDeliveryInProgress && canStartDelivery && IsPlayerInRange())
            {
                StartDelivery();
            }
            else if (isDeliveryInProgress)
            {
                // 请求切换到下一个资源
                switchRequested = true;
            }
        }
            // 检测交货提示
        bool inRange = IsPlayerInRange();
        if (inRange != wasInRange)
        {
            wasInRange = inRange;
            if (resourcesUI != null)
                resourcesUI.ShowDeliveryPrompt(inRange);
        }
    }

    private bool IsPlayerInRange()
    {
        float distance = Vector3.Distance(playerController.GetPlayerPosition(), transform.position);
        return distance <= interactionDistance;
    }

    private void UpdatePlayerResourceDisplay()
    {
        if (playerResourceLabels == null || playerResourceLabels.Length != 4) return;
        for (int i = 0; i < 4; i++)
        {
            if (playerResourceLabels[i] != null)
            {
                float amount = playerResources.GetResource((ResourceType)i);
                playerResourceLabels[i].text = Mathf.FloorToInt(amount).ToString();
            }
        }
    }

    private void UpdateDeliveryAmountDisplay()
    {
        if (currentTask == null || deliveryAmountLabels == null || deliveryAmountLabels.Length != 4) return;
        for (int i = 0; i < 4; i++)
        {
            if (deliveryAmountLabels[i] != null)
            {
                deliveryAmountLabels[i].text = currentTask.delivered[i].ToString();
            }
        }
    }

    private IEnumerator TaskCycle()
    {
        while (true)
        {
            // 创建新任务
            currentTask = new DeliveryTask();
            currentTask.Initialize();   // 随机需求 50~200
            isDeliveryInProgress = false;
            canStartDelivery = true;
            switchRequested = false;
            if (deliveryRoutine != null) StopCoroutine(deliveryRoutine);
            deliveryRoutine = null;

            // 显示任务面板（需求进度条、计时器开始）
            deliveryPanel.ShowTask(currentTask);

            // 等待玩家按F开始交货
            while (!isDeliveryInProgress)
            {
                // 更新计时器显示（从任务创建到现在）
                deliveryPanel.UpdateTimer(currentTask.GetElapsedTime());
                yield return null;
            }

            // 等待交货协程完成（currentTask.isCompleted 变为 true）
            while (!currentTask.isCompleted)
            {
                deliveryPanel.UpdateTimer(currentTask.GetElapsedTime());
                yield return null;
            }

            // 任务完成，计算得分并生成金币
            int score = currentTask.CalculateScore();
            StartCoroutine(SpawnCoinsCoroutine(score));

            // 禁止继续交货，等待10秒发布新任务
            canStartDelivery = false;
            yield return new WaitForSeconds(10f);
        }
    }

    private void StartDelivery()
    {
        if (currentTask == null || isDeliveryInProgress) return;

        isDeliveryInProgress = true;
        canStartDelivery = false;   // 防止重复开始
        deliveryRoutine = StartCoroutine(DeliveryProcess());
    }

    private IEnumerator DeliveryProcess()
    {
        // 重置交货数据
        for (int i = 0; i < 4; i++)
            currentTask.delivered[i] = 0;

        // 重置交货面板的进度条
        deliveryPanel.ShowTask(currentTask);  // 会重置交货进度条为0

        // 按顺序处理四种资源
        for (int step = 0; step < 4; step++)
        {
            ResourceType currentType = (ResourceType)step;
            int required = currentTask.requirements[step];
            float delivered = 0;
            switchRequested = false;

            // 提交当前资源，直到达标、玩家切换或资源不足
            while (delivered < 300 && !switchRequested)
            {
                float playerAmount = playerResources.GetResource(currentType);
                if (playerAmount <= 0) break;  // 玩家资源不足，自动进入下一种

                // 计算本帧交货量（0.2~0.5），不超过剩余需求和玩家资源
                float delta = Random.Range(deliveryRateMin, deliveryRateMax) * Time.deltaTime;
                delta = Mathf.Min(delta, 300 - delivered);
                delta = Mathf.Min(delta, playerAmount);
                if (delta <= 0) break;

                // 扣除玩家资源，增加已交货量
                playerResources.RemoveResource(currentType, delta);
                delivered += delta;
                currentTask.delivered[step] = (int)delivered;

                // 更新UI：交货进度条（仅更新当前类型即可）
                deliveryPanel.UpdateDeliveryProgress(currentTask, step);
                // 更新机器上的文字显示
                UpdateDeliveryAmountDisplay();

                yield return null;
            }

            // 如果是因按F切换而退出，清除标志继续下一个资源
            if (switchRequested)
            {
                switchRequested = false;
                continue;
            }
            // 否则正常进入下一个资源
        }

        // 所有资源处理完毕，标记任务完成
        currentTask.isCompleted = true;
        isDeliveryInProgress = false;
        deliveryRoutine = null;
    }

    /// <summary>
    /// 逐个生成金币，每个之间间隔 0.04 秒
    /// </summary>
    private IEnumerator SpawnCoinsCoroutine(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // 随机方向（向上偏斜）
            Vector3 randomDir = Random.insideUnitSphere;
            randomDir.y = Mathf.Abs(randomDir.y);
            randomDir.Normalize();

            // 生成位置：抛出点附近
            Vector3 spawnPos = coinEjectPoint.position + randomDir * 0.5f;
            GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);

            // 施加初速度
            Rigidbody rb = coin.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = randomDir * 5f;
            }

            // 等待 0.04 秒再生成下一个
            yield return new WaitForSeconds(0.04f);
        }
    }
}