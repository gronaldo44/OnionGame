using UnityEngine;

public class PlayerDoDamage : MonoBehaviour
{

    bool attacking = false;

    private GameObject attackArea = default;
    Animator animator;
    private float timeToAttack = 0.25f;
    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        attackArea = transform.GetChild(0).gameObject;
        attackArea.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("Attack Animation");
            animator.SetTrigger(AnimationStrings.isAttacking);
            Attack();
        }

        if (attacking)
        {
            timer += Time.deltaTime;

            if (timer >= timeToAttack)
            {
                timer = 0;
                attacking = false;
                attackArea.SetActive(attacking);
            }

        }
    }

    void Attack()
    {
        Debug.Log("Player Attack");
        attacking = true;
        attackArea.SetActive(attacking);
    }
}
