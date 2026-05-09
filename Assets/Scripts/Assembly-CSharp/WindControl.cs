using TMPro;
using UnityEngine;

public class WindControl : MonoBehaviour
{
	private Player player;

	public TextMeshProUGUI text;

	public bool windOn;

	public bool varied;

	private void Update()
	{
		player = Object.FindObjectOfType<Player>();
		if (varied && intervalCounter(200))
		{
			variedWindUpdates();
		}
	}

	private void variedWindUpdates()
	{
		AreaEffector2D component = GetComponent<AreaEffector2D>();
		ParticleSystem componentInChildren = component.GetComponentInChildren<ParticleSystem>();
		if (windOn)
		{
			component.forceMagnitude = 0f;
			windOn = false;
			componentInChildren.Stop();
		}
		else
		{
			component.forceMagnitude = -50f;
			windOn = true;
			componentInChildren.Play();
		}
		if (!Object.FindObjectOfType<LevelManager>().checkPointing)
		{
			if (player.inWindZone && windOn)
			{
				text.text = "Wind Alert!!";
			}
			else
			{
				text.text = "";
			}
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
}
