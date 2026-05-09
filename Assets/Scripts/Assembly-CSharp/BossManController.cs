using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossManController : MonoBehaviour
{
	private Player player;

	private FinalGameManager fm;

	private LevelManager lm;

	private Rigidbody2D rb;

	public Rigidbody2D projectile;

	private SpriteRenderer sprite;

	public HealthBar hb;

	private Animator a;

	public GameObject bp1;

	public GameObject bp2;

	public GameObject bp3;

	public GameObject bp4;

	public GameObject bossPortalObject;

	public GameObject bsp1;

	public GameObject bsp2;

	public GameObject bsp3;

	public GameObject bsp4;

	public GameObject w;

	public GameObject z;

	public GameObject iz;

	public GameObject r;

	private bool jumpedDown;

	private bool canTeleport = true;

	public int dir;

	public int health;

	public int healthDeduction;

	private void Start()
	{
		bossPortalObject.gameObject.tag = "misc";
		a = GetComponent<Animator>();
		hb.setMaxHealth(health);
		player = Object.FindObjectOfType<Player>();
		lm = Object.FindObjectOfType<LevelManager>();
		fm = Object.FindObjectOfType<FinalGameManager>();
		sprite = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		if (health > 0)
		{
			if (player.gameObject.transform.position.x <= base.transform.position.x)
			{
				base.transform.localScale = new Vector2(-3.15587f, 3.15587f);
				dir = -1;
			}
			else
			{
				base.transform.localScale = new Vector2(3.15587f, 3.15587f);
				dir = 1;
			}
			if (fm.portalsDestroyed < 3)
			{
				projectile.gravityScale = 1f;
				if (intervalCounter(75) && player.onRange)
				{
					StartCoroutine("attackWithBallRoutine");
				}
			}
			else
			{
				projectile.gravityScale = 0f;
				if (!jumpedDown)
				{
					base.transform.localScale = new Vector2(-3.15587f, 3.15587f);
					jumpedDown = true;
					rb.AddForce(new Vector2(-8500f, 0f));
					dir = -1;
					Object.Instantiate(bossPortalObject, bp1.transform.position, bp1.transform.rotation);
					Object.Instantiate(bossPortalObject, bp2.transform.position, bp2.transform.rotation);
					Object.Instantiate(bossPortalObject, bp3.transform.position, bp3.transform.rotation);
					Object.Instantiate(bossPortalObject, bp4.transform.position, bp4.transform.rotation);
				}
				base.transform.localScale = new Vector2(3.15587f * (float)dir, 3.15587f);
				if (player.inBossRange && intervalCounter(30))
				{
					StartCoroutine("attackWithBallRoutine");
				}
				int num = 0;
				GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
				for (int i = 0; i < rootGameObjects.Length; i++)
				{
					if (rootGameObjects[i].name.StartsWith("rolf"))
					{
						num++;
					}
					else if (rootGameObjects[i].name.StartsWith("iceZombie"))
					{
						num++;
					}
					else if (rootGameObjects[i].name.StartsWith("zombie"))
					{
						num++;
					}
					else if (rootGameObjects[i].name.StartsWith("wolf"))
					{
						num++;
					}
				}
				if (intervalCounter(150) && num < 4)
				{
					StartCoroutine("attackWithSpawnRoutine");
				}
				if (canTeleport)
				{
					canTeleport = false;
					Invoke("teleporter", 5f);
				}
			}
			hb.setHealth(health);
			a.SetBool("moving", rb.velocity.x != 0f);
		}
		else
		{
			Object.FindObjectOfType<AudioManager>().Pause("strikeDown");
			Camera.main.GetComponent<AudioSource>().Pause();
			Object.FindObjectOfType<AudioManager>().Play("victory");
			a.SetBool("dying", value: true);
			lm.destroyAllNpcs();
		}
	}

	public void teleporter()
	{
		StartCoroutine("teleport");
	}

	public IEnumerator teleport()
	{
		yield return new WaitForSeconds(5f);
		int num = Random.Range(1, 5);
		canTeleport = true;
		if (num == 1)
		{
			base.transform.position = bp1.transform.position;
		}
		if (num == 2)
		{
			base.transform.position = bp2.transform.position;
		}
		if (num == 3)
		{
			base.transform.position = bp3.transform.position;
		}
		if (num == 4)
		{
			base.transform.position = bp4.transform.position;
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "shuriken")
		{
			Object.FindObjectOfType<AudioManager>().Play("npcDamage");
			healthDeduction = 1;
			damageRoutine();
		}
	}

	public IEnumerator attackWithBallRoutine()
	{
		a.SetBool("attacking", value: true);
		Object.Instantiate(projectile, new Vector2(base.transform.position.x + 2f * (float)dir, base.transform.position.y - 0.2f), base.transform.rotation);
		yield return new WaitForSeconds(0.5f);
		a.SetBool("attacking", value: false);
	}

	public IEnumerator attackWithSpawnRoutine()
	{
		a.SetBool("attacking", value: true);
		Vector3 position;
		switch (Random.Range(1, 5))
		{
		case 1:
			position = bp1.transform.position;
			break;
		case 2:
			position = bp2.transform.position;
			break;
		case 3:
			position = bp3.transform.position;
			break;
		default:
			position = bp4.transform.position;
			break;
		}
		int num = Random.Range(1, 4);
		if (num == 1)
		{
			Object.Instantiate(w, position, bp1.transform.rotation);
		}
		if (num == 2)
		{
			Object.Instantiate(z, position, bp1.transform.rotation);
		}
		if (num == 3)
		{
			Object.Instantiate(iz, position, bp1.transform.rotation);
		}
		if (num == 4)
		{
			Object.Instantiate(r, position, bp1.transform.rotation);
		}
		yield return new WaitForSeconds(0.5f);
		a.SetBool("attacking", value: false);
	}

	public void damageRoutine()
	{
		StartCoroutine("damageRoutineActual");
	}

	public IEnumerator damageRoutineActual()
	{
		sprite.color = new Color(1f, 0f, 0f, 1f);
		yield return new WaitForSeconds(0.2f);
		sprite.color = new Color(1f, 1f, 1f, 1f);
		health -= healthDeduction;
	}

	private bool intervalCounter(int intervalFrame)
	{
		if (Time.frameCount % intervalFrame == 0)
		{
			return true;
		}
		return false;
	}

	public void gameComplete()
	{
		lm.gameComplete();
	}

	public void nullFunction()
	{
	}
}
