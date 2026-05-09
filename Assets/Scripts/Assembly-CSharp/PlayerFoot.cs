using UnityEngine;

public class PlayerFoot : MonoBehaviour
{
	private Player player;

	private void Start()
	{
		player = Object.FindObjectOfType<Player>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "plt" || other.tag == "floor")
		{
			player.myPltY = other.transform.position.y;
		}
	}
}
