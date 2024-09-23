using UnityEngine;

public class EnemyDoDamage : MonoBehaviour
{
    private bool attacking = false;

    [SerializeField] public EnemyAttackArea attackArea; // Reference to the EnemyAttackArea script
    [SerializeField] private float timeToAttack = 0.25f;
    [SerializeField] private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the child GameObject has the EnemyAttackArea component
        if (attackArea == null)
        {
            GameObject child = transform.GetChild(0).gameObject;
            attackArea = child.GetComponent<EnemyAttackArea>();
            if (attackArea == null)
            {
                Debug.LogError("EnemyAttackArea component not found on child object.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // attacks every frame it can
        Attack();

        if (attacking)
        {
            timer += Time.deltaTime;

            if (timer >= timeToAttack)
            {
                timer = 0;
                attacking = false;
                attackArea.gameObject.SetActive(attacking);
            }
        }
    }

    void Attack()
    {
        if (!attacking) // Prevent multiple activations
        {
            attacking = true;
            attackArea.gameObject.SetActive(attacking);
        }
    }
}
