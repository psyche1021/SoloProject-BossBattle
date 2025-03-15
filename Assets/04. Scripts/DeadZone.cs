using UnityEngine;

public class DeadZone : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }
        else
        {
            PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
            playerStatus.Die();
        }
    }
}
