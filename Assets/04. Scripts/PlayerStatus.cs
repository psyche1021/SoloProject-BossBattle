using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStatus : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    int stack;
    const int maxStack = 5;
    bool isDamaged;

    public int Stack => stack;

    public int MaxStack => maxStack;

    public GameObject healthPrefab;
    public Transform healthContainer;
    public Sprite[] skillBubbleSprites;
    public Image skillBubbleImage;

    SkinnedMeshRenderer[] skinMeshs;
    MeshRenderer[] armorMeshs;
    List<Image> healthImage;
    Animator animator;
    PlayerController playerController;

    void Awake()
    {
        healthImage = new List<Image>();
        skinMeshs = GetComponentsInChildren<SkinnedMeshRenderer>();
        armorMeshs = GetComponentsInChildren<MeshRenderer>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    void Start()
    {
        maxHealth = 4;
        currentHealth = maxHealth;

        Initialize();
    }

    void Initialize()
    {
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject health = Instantiate(healthPrefab, healthContainer);
            healthImage.Add(health.GetComponent<Image>());
        }
    }

    void UpdateStatus()
    {
        for (int i = 0; i < healthImage.Count; i++)
        {
            if (i < currentHealth)
            {
                healthImage[i].enabled = true;
            }
            else
            {
                healthImage[i].enabled = false;
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDamaged) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        StartCoroutine(HitEffect());
        UpdateStatus();
    }

    IEnumerator HitEffect() 
    {
        // ���ݺ��� ����
        isDamaged = true;
        
        animator.SetBool("doHit", true);

        // �¾����� ���� ����
        foreach (SkinnedMeshRenderer mesh in skinMeshs) mesh.material.color = Color.yellow;
        foreach (MeshRenderer mesh in armorMeshs) mesh.material.color = Color.yellow;

        yield return new WaitForSeconds(0.5f);

        animator.SetBool("doHit", false);

        // ���󺹱�
        foreach (SkinnedMeshRenderer mesh in skinMeshs) mesh.material.color = Color.white;
        foreach (MeshRenderer mesh in armorMeshs) mesh.material.color = Color.white;

        yield return new WaitForSeconds(0.5f);

        // ���� ��!
        isDamaged = false; 
    }

    public void SkillBubbleUpdate()
    {
        if (stack >= 0 && stack < skillBubbleSprites.Length)
        {
            skillBubbleImage.sprite = skillBubbleSprites[stack];
        }
    }

    public void OnAttackHit()
    {
        if (stack < maxStack)
        {
            stack++;
            SkillBubbleUpdate();
        }
    }

    public void OnSkillHit()
    {
        SkillBubbleUpdate();
    }

    public void ResetStack()
    {
        stack = 0;
    }

    public void Die()
    {
        TakeDamage(currentHealth);
        
        // ���� ���Ŀ� �� ��...
        animator.SetTrigger("doDie"); // �ִϸ��̼� ���
        playerController.lockKeyboard = true; // Ű ����
        Invoke("ReloadSceneToGM", 3f); // �� ���ε�
    }

    void ReloadSceneToGM()
    {
        GameManager.instance.ReloadScene();
    }
}
