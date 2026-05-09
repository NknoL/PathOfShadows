using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwordDash : MonoBehaviour
{
    private Player player;
    private BossManController boss;
    private ArrayList npcs = new ArrayList();

    public Slider slider;
    public bool obstacleFound;
    public float dashVal = 2f;
    private bool dashStarted;
    private bool bossInRange;
    private float playerPreDashX;

    private void Start()
    {
        boss = Object.FindObjectOfType<BossManController>();
        player = Object.FindObjectOfType<Player>();
    }

    private void Update()
    {
        dashVal = slider.value;

        if (slider.value < 2f && Time.frameCount % 12 == 0)
        {
            slider.value += 0.25f;
        }

        if (!player.isDashing)
        {
            playerPreDashX = player.transform.position.x;
        }

        if (player.isDashing && !dashStarted)
        {
            player.GetComponent<Animator>().SetBool("isAttacking", true);
            slider.value = 0f;
            StartCoroutine("damageNpc");
            dashStarted = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!player.registerNpcsForDashDamage)
        {
            return;
        }
        if (isObstacle(other.tag))
        {
            try
            {
                SpriteRenderer component = other.gameObject.GetComponent<SpriteRenderer>();
                player.dashFinalX = other.transform.position.x - (float)player.dir * component.bounds.extents.x - (float)player.dir;
                obstacleFound = true;
            }
            catch (MissingComponentException)
            {
            }
        }
        if ((other.tag == "zombie" || (other.tag == "npc" && other.name != "BossMan") || other.tag == "wolf") && !npcs.Contains(other.GetComponent<NpcControl>()))
        {
            npcs.Add(other.GetComponent<NpcControl>());
        }
        if (other.name.StartsWith("Boss"))
        {
            bossInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (player.registerNpcsForDashDamage)
        {
            if (other.name.StartsWith("Boss"))
            {
                bossInRange = false;
            }
            if (isObstacle(other.tag))
            {
                obstacleFound = false;
            }
            npcs.Remove(other.GetComponent<NpcControl>());
        }
    }

    public IEnumerator damageNpc()
    {
        if (bossInRange)
        {
            boss.healthDeduction = 5;
            float x = boss.transform.position.x;
            if (player.dir == 1)
            {
                if (x > playerPreDashX && x < player.dashFinalX)
                {
                    boss.damageRoutine();
                }
            }
            else if (x < playerPreDashX && x > player.dashFinalX)
            {
                boss.damageRoutine();
            }
        }

        while (npcs.Count > 0)
        {
            NpcControl npcControl = (NpcControl)npcs[0];
            try
            {
                npcControl.healthDeduction = 5;
                float x2 = npcControl.transform.position.x;
                if (player.dir == 1)
                {
                    if (x2 > playerPreDashX && x2 < player.dashFinalX)
                    {
                        npcControl.damageRoutine();
                    }
                }
                else if (x2 < playerPreDashX && x2 > player.dashFinalX)
                {
                    npcControl.damageRoutine();
                }
                npcs.Remove(npcControl);
            }
            catch (MissingReferenceException)
            {
                npcs.Remove(npcControl);
            }
        }

        npcs.Clear();
        Object.FindObjectOfType<AudioManager>().Play("swordDash");

        yield return new WaitForSeconds(0.7f);

        player.registerNpcsForDashDamage = true;
        bossInRange = false;
        player.isDashing = false;
        dashStarted = false;

        player.GetComponent<Animator>().SetBool("isAttacking", false);
    }

    private bool isObstacle(string obstacleTag)
    {
        string[] array = new string[9] { "npc", "Collectible", "checkPoint", "pzp", "floor", "zombie", "wolf", "platform", "misc" };
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(obstacleTag))
            {
                return false;
            }
        }
        return true;
    }
}