using UnityEngine;

public class Projectile : MonoBehaviour
{
	public GameObject explosion;

	public Rigidbody2D rb;

	private Player player;

	public float projectileSpeed;

	private int dir = -1;

	private int projMagnitude;

	private void Start()
	{
		player = Object.FindObjectOfType<Player>();
		rb = GetComponent<Rigidbody2D>();
		if (base.tag == "pzp")
		{
			if (player.gameObject.transform.position.x < base.transform.position.x)
			{
				dir = -1;
			}
			else
			{
				dir = 1;
			}
			if (rb.gravityScale != 0f)
			{
				if (player.farRange)
				{
					projMagnitude = 450;
				}
				else
				{
					projMagnitude = 150;
				}
				rb.AddForce(new Vector2(projMagnitude * dir, 0f));
			}
		}
		if (base.tag == "shuriken")
		{
			dir = player.dir;
		}
	}

	private void Update()
	{
		if (!base.name.StartsWith("explosion") && rb.gravityScale == 0f)
		{
			float x = Camera.main.transform.position.x;
			float num = Camera.main.aspect * Camera.main.orthographicSize;
			if (rb.gravityScale == 0f)
			{
				rb.velocity = new Vector2(projectileSpeed * (float)dir, 0f);
			}
			if (base.transform.position.x < x - num || base.transform.position.x > x + num)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (base.tag == "pzp")
		{
			if (other.tag == "Player" && player.canBeHit)
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
			if (other.tag != "npc" && other.tag != "Collectible" && other.tag != "checkPoint" && other.tag != "misc")
			{
				if (base.name.StartsWith("Orb"))
				{
					Object.Instantiate(explosion, base.transform.position, base.transform.rotation);
					Object.FindObjectOfType<AudioManager>().Play("explosion");
				}
				Object.Destroy(base.gameObject);
			}
		}
		if (base.tag == "shuriken" && other.tag != "Player" && other.tag != "Collectible" && other.tag != "checkPoint" && other.tag != "misc" && other.tag != "shuriken" && !other.name.StartsWith("portal"))
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void removeExplosion()
	{
		Object.Destroy(base.gameObject);
	}
}
