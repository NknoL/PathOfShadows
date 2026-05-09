using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	public Rigidbody2D rb;

	public Rigidbody2D projectile;

	public Transform foot;

	public Transform sword;

	public LayerMask layer;

	public LayerMask layer2;

	private SwordDash sd;

	public SpriteRenderer sr;

	private Animator a;

	public HealthBar hb;

	public Slider slider;

	public Vector3 respawnPoint;

	private LevelManager lm;

	public bool npcBehind;

	public bool canBeHit = true;

	private bool walkingSoundOn;

	public int dir = 1;

	public int health;

	public int shurikens;

	public float footCheckRadius;

	public float dashFinalX;

	public float dashReachMax;

	public float speedX;

	public float speedY;

	public float myPltY;

	public bool isDashing;

	public bool isHit;

	public bool inWindZone;

	public bool falling;

	public bool inBossRange;

	public bool registerNpcsForDashDamage = true;

	private bool onGround;

	private bool fallback = true;

	private bool dead;

	private float xMovement;

	private float shurikenIntervalTimeTemp;

	private float shurikenIntervalTime;

	public bool farRange;

	public bool onRange;

	private void Start()
	{
		lm = Object.FindObjectOfType<LevelManager>();
		respawnPoint = base.transform.position;
		hb.setMaxHealth(health);
		rb = GetComponent<Rigidbody2D>();
		a = GetComponent<Animator>();
		sd = Object.FindObjectOfType<SwordDash>();
		sr = GetComponent<SpriteRenderer>();
		shurikenIntervalTimeTemp = Time.time;
	}

	private void Update()
	{
		float num;
		float num2;
		if (SceneManager.GetActiveScene().name.EndsWith("1"))
		{
			num = 1f;
			num2 = 1.5f;
		}
		else
		{
			num = 0.5f;
			num2 = 1f;
		}
		if (health <= 0)
		{
			dead = true;
			a.SetInteger("health", 0);
		}
		else
		{
			onGround = Physics2D.OverlapCircle(foot.position, footCheckRadius, layer);
			if (!onGround)
			{
				onGround = Physics2D.OverlapCircle(foot.position, footCheckRadius, layer2);
			}
			if (Input.GetKeyDown("n") && !isDashing && sd.dashVal == 2f && !lm.paused)
			{
				registerNpcsForDashDamage = false;
				isDashing = true;

				// #4 - Activar animación de espada al principio
				a.SetBool("isAttacking", true);

				rb.velocity = new Vector2(0f, 0f);
				base.transform.position = Vector2.Lerp(base.transform.position, new Vector2(dashFinalX, base.transform.position.y), 200f * Time.deltaTime);
			}
			if (!sd.obstacleFound)
			{
				dashFinalX = base.transform.position.x + sr.bounds.extents.x * (float)dir + dashReachMax * (float)dir;
			}
			if (Input.GetKeyUp("m") && shurikens != 0 && !lm.paused)
			{
				Object.Instantiate(projectile, new Vector2(base.transform.position.x + 1.5f * (float)dir, base.transform.position.y - 0.2f), base.transform.rotation);
				shurikens--;
			}
			a.SetBool("onGround", onGround);
			a.SetBool("moving", xMovement != 0f);
			// #4 - Mejor control de animación de espada/dash
			if (isDashing)
			{
				a.SetBool("isAttacking", true);
				a.SetBool("moving", false);
			}
			else
			{
				a.SetBool("isAttacking", false);
			}
			a.SetBool("isHit", isHit);
			a.SetFloat("yVel", rb.velocity.y);
			shurikenIntervalTime = Time.time;
			float num3 = Mathf.Abs(shurikenIntervalTime - shurikenIntervalTimeTemp);
			if (num <= num3 && num3 <= num2)
			{
				if (shurikens != 10)
				{
					shurikens++;
				}
				shurikenIntervalTimeTemp = shurikenIntervalTime;
			}
			if (isHit && fallback)
			{
				Object.FindObjectOfType<AudioManager>().Play("playerHit");
				health--;
				if (!npcBehind)
				{
					base.transform.position = Vector2.MoveTowards(base.transform.position, new Vector2(base.transform.position.x - 2f, base.transform.position.y), 5f);
				}
				else
				{
					base.transform.position = Vector2.MoveTowards(base.transform.position, new Vector2(base.transform.position.x + 2f, base.transform.position.y), 5f);
				}
				fallback = false;
			}
			slider.value = shurikens;
			if (base.transform.position.x + 1f < Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect)
			{
				health = 0;
			}
			if (base.transform.position.x - 1f > Camera.main.transform.position.x + Camera.main.orthographicSize * Camera.main.aspect)
			{
				health = 0;
			}
		}
		hb.setHealth(health);
		if (rb.velocity.x != 0f && onGround)
		{
			if (!walkingSoundOn)
			{
				Object.FindObjectOfType<AudioManager>().Play("playerWalk");
			}
			walkingSoundOn = true;
		}
		else
		{
			if (walkingSoundOn)
			{
				Object.FindObjectOfType<AudioManager>().Pause("playerWalk");
			}
			walkingSoundOn = false;
		}
	}

	private void FixedUpdate()
	{
		if (!dead)
		{
			xMovement = Input.GetAxis("Horizontal");
			if (xMovement < 0f && !isDashing)
			{
				dir = -1;
				base.transform.localScale = new Vector2(-4.5882f, 4.5882f);
				rb.velocity = new Vector2(xMovement * speedX * Time.deltaTime, rb.velocity.y);
			}
			if (xMovement > 0f && !isDashing)
			{
				dir = 1;
				base.transform.localScale = new Vector2(4.5882f, 4.5882f);
				rb.velocity = new Vector2(xMovement * speedX * Time.deltaTime, rb.velocity.y);
			}
			if ((Input.GetKey("w") || Input.GetKeyDown("up")) && onGround)
			{
				rb.velocity = new Vector2(rb.velocity.x, speedY * Time.deltaTime);
			}
		}
	}

	public void hitRoutine()
	{
		isHit = false;
		fallback = true;
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other.name.StartsWith("CheckPoint"))
		{
			lm.checkPointReached();
			respawnPoint = new Vector3(other.transform.position.x, other.transform.position.y, base.transform.position.z);
			Object.Destroy(other.gameObject);
		}
	}

	public void OnTriggerStay2D(Collider2D other)
	{
		if (other.name == "BossShootRange")
		{
			inBossRange = true;
		}
		if (other.name == "FallDetector")
		{
			StartCoroutine("respawn");
		}
		if (other.name.StartsWith("Wind"))
		{
			if (!inWindZone)
			{
				Object.FindObjectOfType<AudioManager>().Play("wind");
			}
			inWindZone = true;
		}
		if (other.tag == "Collectible" && Mathf.Abs(base.transform.position.x - other.transform.position.x) <= 1f)
		{
			Object.FindObjectOfType<AudioManager>().Play("collected");
			if (health != 10)
			{
				health++;
			}
			Object.Destroy(other.gameObject);
		}
		if (other.name.StartsWith("strike") && Mathf.Abs(base.transform.position.x - other.transform.position.x) <= 1f)
		{
			isHit = true;
		}
	}

	public void OnTriggerExit2D(Collider2D other)
	{
		if (other.name.StartsWith("Wind"))
		{
			if (inWindZone)
			{
				Object.FindObjectOfType<AudioManager>().Pause("wind");
			}
			inWindZone = false;
		}
		if (other.name == "BossShootRange")
		{
			inBossRange = false;
		}
	}

	public void OnCollisionEnter2D(Collision2D other)
	{
		if (other.collider.name == "br1" || other.collider.name == "br4")
		{
			farRange = true;
			onRange = true;
		}
		if (other.collider.name == "br2" || other.collider.name == "br3")
		{
			farRange = false;
			onRange = true;
		}
	}

	public void OnCollisionExit2D(Collision2D other)
	{
		if (other.collider.name == "br1" || other.collider.name == "br4" || other.collider.name == "br2" || other.collider.name == "br3")
		{
			onRange = false;
		}
	}

	public IEnumerator respawn()
	{
		falling = true;
		base.enabled = false;
		canBeHit = false;
		yield return new WaitForSeconds(2f);
		if (!base.enabled)
		{
			health--;
			base.enabled = true;
			rb.velocity = new Vector2(0f, 0f);
			base.transform.position = new Vector3(respawnPoint.x, respawnPoint.y, base.transform.position.z);
			Camera.main.transform.position = new Vector3(respawnPoint.x, respawnPoint.y, Camera.main.transform.position.z);
			shurikenIntervalTime = Time.time;
			shurikenIntervalTimeTemp = Time.time;
			StartCoroutine("hittable");
			falling = false;
		}
	}

	public IEnumerator hittable()
	{
		yield return new WaitForSeconds(2.5f);
		canBeHit = true;
	}

	public void die()
	{
		lm.gameOver();
	}

	void OnCollisionStay2D(Collision2D col)
	{
		if (col.gameObject.CompareTag("npc") || col.gameObject.CompareTag("zombie") || col.gameObject.CompareTag("wolf"))
		{
			Vector2 pushDir = (transform.position - col.transform.position).normalized;
			rb.AddForce(pushDir * 5f, ForceMode2D.Impulse);
		}
	}

}
