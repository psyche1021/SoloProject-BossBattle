using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator animator;
    PlayerController playercontroller;

    float currentParameter;
    float targetParameter;
    [Range(0.05f, 0.1f)]
    public float lerpTime; // �����ӵ� (�������� �ε巯��)

    public float Parameter => currentParameter;

    void Awake()
    {
        animator = GetComponent<Animator>();
        playercontroller = GetComponent<PlayerController>();

        lerpTime = 0.1f;
        currentParameter = 0;
    }

    void Update()
    {
        PlayerState();
    }

    void PlayerState()
    {
        if (playercontroller.PlayerDir != Vector3.zero) // ���� ���Ͱ� �������̸�
        {
            targetParameter = playercontroller.IsDash ? 1.0f : 0.5f; // ��� ���ο� ���� �Ķ���� ����
        }
        else
        {
            targetParameter = 0; // ������ 0
        }

        currentParameter = Mathf.Lerp(currentParameter, targetParameter, lerpTime); // ���� �Ķ���͸� �ε巴�� ����
        animator.SetFloat("Parameter", currentParameter); // ������ ������ ��������
    }
}
