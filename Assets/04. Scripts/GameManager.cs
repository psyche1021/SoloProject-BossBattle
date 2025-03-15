using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public CinemachineCamera cinemachineCamera;
    public GameObject player;
    public BossMonster boss;
    public GameObject victoryPanel;

    PlayerController playerController;
    Animator animator;
    float currentFOV; // 현재 FOV
    float targetFOV; // 목표 FOV
    [Range(2f, 4f)]
    public float lerpTime; // 보간시간 (낮을수록 빠름)

    void Awake()
    {
        instance = this;

        playerController = player.GetComponent<PlayerController>();
        animator = player.GetComponent<Animator>();
    }
    void Start()
    {
        currentFOV = 40f;
        lerpTime = 2f;
        cinemachineCamera.Lens.FieldOfView = currentFOV;
    }

    void Update()
    {
        GameVictory();
    }

    void LateUpdate()
    {
        SetFOV();
    }

    void SetFOV()
    {
        LerpFOV();

        if (playerController.IsDash && animator.GetFloat("Parameter") > 0.5f) 
        {
            targetFOV = 50f;
        }
        else
        {
            targetFOV = 40f;
        }
    }

    void LerpFOV()
    {
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * lerpTime);
        cinemachineCamera.Lens.FieldOfView = currentFOV;
    }

    void GameVictory()
    {
        if (boss.IsDead)
        {
            player.GetComponent<PlayerStatus>().currentHealth = player.GetComponent<PlayerStatus>().maxHealth;
            Invoke("OnPanel", 2f);
        }
    }

    void OnPanel()
    {
        Time.timeScale = 0;
        victoryPanel.SetActive(true);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
