using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    CharacterController characterController;
    PlayerAttack playerAttack;
    PlayerStatus playerStatus;

    float playerMoveSpeed;
    float playerCurrentSpeed;
    [Range(360f, 1080f)]
    public float playerRotateSpeed;
    float playerAxisH;
    float playerAxisZ;
    bool isDash;
    public bool lockKeyboard;
    Vector3 playerDir;

    public float PlayerMoveSpeed
    {
        get => playerMoveSpeed;
        set => playerMoveSpeed = Mathf.Clamp(value, 0, 10);
    }

    public Vector3 PlayerDir => playerDir;

    public bool IsDash
    {
        get => isDash;
        set => isDash = value;
    }

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerAttack = GetComponent<PlayerAttack>();
        playerStatus = GetComponent<PlayerStatus>();
    }

    void Start()
    {
        playerMoveSpeed = 5f;
        playerRotateSpeed = 360f;
    }

    void Update()
    {
        MapCheck();

        if (!lockKeyboard)
        {
            if (!playerAttack.IsAttacking)
            {
                PlayerMove();
                DashCheck();
            }
            PlayerAttackState();
        }
    }

    void PlayerMove()
    {
        playerAxisH = Input.GetAxisRaw("Horizontal");
        playerAxisZ = Input.GetAxisRaw("Vertical");
        playerDir = new Vector3(playerAxisH, 0, playerAxisZ).normalized; // ���� ����
        playerCurrentSpeed = IsDash ? PlayerMoveSpeed * 2f : PlayerMoveSpeed;

        // ���� ���Ͱ� 0�� �ƴ϶�� => �̵� ���̴� ���⿡ ���� �÷��̾� ȸ��
        if (playerDir != Vector3.zero)
        {
            Quaternion target = Quaternion.LookRotation(playerDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, playerRotateSpeed * Time.deltaTime);
        }

        // �̵�
        characterController.Move(playerDir * playerCurrentSpeed * Time.deltaTime);
    }

    void DashCheck()
    {
        IsDash = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    void PlayerAttackState()
    {
        // ��� ���콺 ������ �ٶ󺸰� �Ϸ��� �̰Ÿ� �ֱ�
        // if (!playerAttack.IsAttacking) LookAttackPoint();

        // ���� & ��ų
        if (Input.GetMouseButtonDown(0))
        {
            if (!playerAttack.IsAttacking) LookAttackPoint();
            playerAttack.Attack();
        }
        else if (Input.GetMouseButton(1))
        {
            if (playerStatus.Stack == playerStatus.MaxStack)
            {
                if (!playerAttack.IsAttacking)
                {
                    LookAttackPoint();
                    playerAttack.Skill();
                    playerStatus.ResetStack();
                    playerStatus.SkillBubbleUpdate();
                }
            }
            else
            {
                Debug.Log($"������ ������� �ʽ��ϴ�!! (���罺��:{playerStatus.Stack})");
            }
        }
    }

    void LookAttackPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 targetPoint = hitInfo.point;
            targetPoint.y = transform.position.y;
            Vector3 direction = (targetPoint - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void MapCheck()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 1.5f, Color.red);

        if (Physics.Raycast(ray, out hit, 1.5f))
        {
            if (hit.collider.gameObject.layer == LayerMask.GetMask("Ground"))
            {
                playerStatus.Die();
            }
        }
    }
}
