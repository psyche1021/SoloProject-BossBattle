using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator animator;
    PlayerController playercontroller;

    float currentParameter;
    float targetParameter;
    [Range(0.05f, 0.1f)]
    public float lerpTime; // 보간속도 (낮을수록 부드러움)

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
        if (playercontroller.PlayerDir != Vector3.zero) // 방향 벡터가 유동적이면
        {
            targetParameter = playercontroller.IsDash ? 1.0f : 0.5f; // 대시 여부에 따라 파라미터 조절
        }
        else
        {
            targetParameter = 0; // 정지시 0
        }

        currentParameter = Mathf.Lerp(currentParameter, targetParameter, lerpTime); // 블렌드 파라미터를 부드럽게 보간
        animator.SetFloat("Parameter", currentParameter); // 보간된 값으로 최종지정
    }
}
