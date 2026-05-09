using System.Collections;
using UnityEngine;

public class NpcControl : MonoBehaviour
{
    public Rigidbody2D projectile;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Player player;
    public HealthBar hb;
    private Animator a;

    public int health;
    public int dir;
    public int healthDeduction;
    public float xSpeed;
    public float yScale;
    public float xScale;
    private bool awake;
    public float myPltY;

    private bool isDamaged = false;

    // ==================== #3: Rangos de visión ====================
    public float aggroRangeX = 12f;
    public float aggroRangeY = 4f;

    private void Start()
    {
        hb.setMaxHealth(health);
        player = Object.FindObjectOfType<Player>();
        sprite = GetComponent<SpriteRenderer>();
        a = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        awake = playerAccessible();

        if (player.gameObject.transform.position.x <= base.transform.position.x)
        {
            base.transform.localScale = new Vector2(0f - xScale, yScale);
            dir = -1;
        }
        else
        {
            base.transform.localScale = new Vector2(xScale, yScale);
            dir = 1;
        }

        if (health <= 0)
        {
            Object.Destroy(base.gameObject);
        }

        if ((base.tag == "zombie" || base.tag == "wolf") && !player.isHit && awake && !isDamaged)
        {
            if (rb != null)
                rb.velocity = new Vector2(xSpeed * (float)dir, rb.velocity.y);
        }

        if (!base.name.StartsWith("plantz"))
        {
            a.SetBool("moving", awake && !isDamaged);
        }

        hb.setHealth(health);
    }

    // ==================== #3: Lógica de visión mejorada ====================
    private bool playerAccessible()
    {
        if (player == null) return false;

        float distX = Mathf.Abs(player.transform.position.x - transform.position.x);
        float distY = Mathf.Abs(player.transform.position.y - transform.position.y);

        if (distX > aggroRangeX || distY > aggroRangeY)
            return false;

        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "shuriken")
        {
            healthDeduction = 1;
            damageRoutine();
        }
        if (other.name.StartsWith("strike"))
        {
            healthDeduction = 3;
            damageRoutine();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player" && !player.isDashing && player.canBeHit)
        {
            if (player.transform.position.x > base.transform.position.x)
            {
                player.npcBehind = true;
            }
            else
            {
                player.npcBehind = false;
            }
            player.isHit = true;
        }
    }

    private void projectiler()
    {
        if (awake)
        {
            Object.Instantiate(projectile, new Vector2(base.transform.position.x + 1.5f * (float)dir, base.transform.position.y - 0.1f), base.transform.rotation);
        }
    }

    public void damageRoutine()
    {
        StartCoroutine("damageRoutineActual");
    }

    public IEnumerator damageRoutineActual()
    {
        isDamaged = true;
        Object.FindObjectOfType<AudioManager>().Play("npcDamage");
        sprite.color = new Color(1f, 0f, 0f, 1f);

        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.velocity = new Vector2(0f, rb.velocity.y * 0.4f);
        }

        yield return new WaitForSeconds(0.15f);

        sprite.color = new Color(1f, 1f, 1f, 1f);
        health -= healthDeduction;
        isDamaged = false;
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (rb != null)
            {
                Vector2 pushDir = (transform.position - col.transform.position).normalized;
                rb.AddForce(pushDir * 4f, ForceMode2D.Impulse);
            }
        }
    }
}