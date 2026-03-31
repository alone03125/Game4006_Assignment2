using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public ResourceType type;
    private bool canCollect = true;
    private bool isBeingCollected = false;
    private PlayerController currentPlayer;   // 记录当前触发的玩家

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayer = other.GetComponent<PlayerController>();
            if (currentPlayer != null)
                currentPlayer.EnterCollectibleRange();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (currentPlayer != null)
                currentPlayer.ExitCollectibleRange();
            currentPlayer = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && canCollect && Input.GetKey(KeyCode.Space))
        {
            if (!isBeingCollected)
            {
                isBeingCollected = true;
                var playerController = other.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.BeginCollectResource(this);
                    canCollect = false;
                }
            }
        }
    }

    /// <summary>
    /// 采集完成，节点消失并重新生成
    /// </summary>
    public void OnCollectComplete()
    {
        isBeingCollected = false;

        // 采集完成，手动通知玩家离开范围（避免因对象禁用而无法触发OnTriggerExit）
        if (currentPlayer != null)
        {
            currentPlayer.ExitCollectibleRange();
            currentPlayer = null;
        }

        // 将自身归还对象池
        ResourcePool.Instance.ReturnResource(type, gameObject);
        // 延迟后生成新的同类型资源
        Invoke(nameof(RespawnDelayed), 0.5f);
    }

    private void RespawnDelayed()
    {
        ResourcePool.Instance.GetResource(type);
    }

    /// <summary>
    /// 重置节点状态
    /// </summary>
    public void ResetNode()
    {
        canCollect = true;
        isBeingCollected = false;
        currentPlayer = null;   // 重置时清除玩家引用
    }
}