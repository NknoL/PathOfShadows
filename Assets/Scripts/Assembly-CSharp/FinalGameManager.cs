using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalGameManager : MonoBehaviour
{
	public int portalsDestroyed;

	private bool rPDestroyed;

	public GameObject strikeDown1;

	public GameObject strikeDown2;

	public PortalController zKingPortal;

	public PortalController zPortal;

	public PortalController wPortal;

	public PortalController rP;

	public PortalController hP1;

	public PortalController hP2;

	private LevelManager lm;

	private void Start()
	{
		lm = Object.FindObjectOfType<LevelManager>();
	}

	private void Update()
	{
		if (intervalCounter(900))
		{
			lm.alert.text = "StrikeDown Alert!!";
		}
		if (intervalCounter(1000))
		{
			StartCoroutine("strikeDownRoutine");
		}
		if (portalsDestroyed < 3)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				if (rootGameObjects[i].name.StartsWith("iceZombieKing"))
				{
					num++;
				}
				else if (rootGameObjects[i].name.StartsWith("iceZombie"))
				{
					num2++;
				}
				else if (rootGameObjects[i].name.StartsWith("wolf"))
				{
					num3++;
				}
			}
			if (num < 1)
			{
				zKingPortal.canSpawn = true;
			}
			else
			{
				zKingPortal.canSpawn = false;
			}
			if (num2 < 2)
			{
				zPortal.canSpawn = true;
			}
			else
			{
				zPortal.canSpawn = false;
			}
			if (num3 < 1)
			{
				wPortal.canSpawn = true;
			}
			else
			{
				wPortal.canSpawn = false;
			}
			if (rP.canSpawnParticularVomit)
			{
				rP.canSpawn = true;
			}
			else
			{
				rP.canSpawn = false;
			}
		}
		else if (!rPDestroyed)
		{
			Object.Destroy(rP.gameObject);
			rPDestroyed = true;
		}
		if (hP1.canSpawnParticularVomit)
		{
			hP1.canSpawn = true;
		}
		else
		{
			hP1.canSpawn = false;
		}
		if (hP2.canSpawnParticularVomit)
		{
			hP2.canSpawn = true;
		}
		else
		{
			hP2.canSpawn = false;
		}
	}

	public IEnumerator strikeDownRoutine()
	{
		Object.FindObjectOfType<AudioManager>().Play("strikeDown");
		strikeDown1.SetActive(value: true);
		strikeDown2.SetActive(value: true);
		yield return new WaitForSeconds(5f);
		Object.FindObjectOfType<AudioManager>().Pause("strikeDown");
		strikeDown1.SetActive(value: false);
		strikeDown2.SetActive(value: false);
		lm.alert.text = "";
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
