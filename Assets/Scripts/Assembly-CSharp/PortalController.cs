using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
	public int vomitInterval = 300;

	public int health;

	private bool hbDestroyed;

	public bool destroyable;

	public bool canSpawn;

	public bool canSpawnParticularVomit;

	private LevelManager levelManager;

	public HealthBar hb;

	public GameObject vomit;

	private FinalGameManager fm;

	private SpriteRenderer sr;

	private Rigidbody2D vomitRb;

	private void Start()
	{
		levelManager = Object.FindObjectOfType<LevelManager>();
		canSpawnParticularVomit = true;
		if (base.name.StartsWith("portal"))
		{
			sr = GetComponent<SpriteRenderer>();
			hb.setMaxHealth(health);
			if (SceneManager.GetActiveScene().name.EndsWith("1"))
			{
				canSpawn = true;
			}
			else
			{
				fm = Object.FindObjectOfType<FinalGameManager>();
			}
		}
	}

	private void Update()
	{
		if (base.name.StartsWith("portal"))
		{
			if (health <= 0 && destroyable)
			{
				sr.color = new Color(1f, 1f, 1f, 1f);
				if (!hbDestroyed)
				{
					Object.Destroy(hb.gameObject);
				}
				hbDestroyed = true;
				if (SceneManager.GetActiveScene().name.EndsWith("2"))
				{
					fm.portalsDestroyed++;
					Object.Destroy(base.gameObject);
				}
			}
			else if (intervalCounter(vomitInterval) && canSpawn)
			{
				Object.Instantiate(vomit, base.transform.position, base.transform.rotation);
			}
		}
		if (base.name.StartsWith("Roller"))
		{
			vomitRb = GetComponent<Rigidbody2D>();
			vomitRb.velocity = new Vector2(-15f, vomitRb.velocity.y);
		}
	}

	private bool intervalCounter(int intervalFrame)
	{
		if (Time.frameCount % intervalFrame == 0)
		{
			return true;
		}
		return false;
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (base.name.StartsWith("Roller") && other.name == "FallDetector")
		{
			Object.Destroy(base.gameObject);
		}
		if (other.tag == "shuriken" && base.name.StartsWith("portal") && destroyable)
		{
			Object.FindObjectOfType<AudioManager>().Play("npcDamage");
			if (health > 0)
			{
				health--;
			}
			if (!hbDestroyed)
			{
				hb.setHealth(health);
			}
			Object.Destroy(other.gameObject);
		}
		if (other.name == "Player" && health <= 0)
		{
			Camera.main.GetComponent<AudioSource>().Pause();
			Object.FindObjectOfType<AudioManager>().Pause("wind");
			Object.FindObjectOfType<AudioManager>().Play("victory");
			Object.FindObjectOfType<Player>().enabled = false;
			levelManager.goToL2();
		}
	}

	public void OnTriggerStay2D(Collider2D other)
	{
		if (other.name.StartsWith("rolf") || other.name.StartsWith("heart"))
		{
			canSpawnParticularVomit = false;
		}
	}

	public void OnTriggerExit2D(Collider2D other)
	{
		if (other.name.StartsWith("rolf") || other.name.StartsWith("heart"))
		{
			canSpawnParticularVomit = true;
		}
	}
}
