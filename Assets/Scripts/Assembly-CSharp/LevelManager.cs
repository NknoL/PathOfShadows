using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private Player player;

    public GameObject instOrEscUI;
    public GameObject cam;
    public GameObject gameOverUI;
    public GameObject hudUI;
    public GameObject nlUI;
    public GameObject camBreak;

    public TextMeshProUGUI alert;
    public TextMeshProUGUI gameOverText;

    public bool gameHasEnded;
    public bool checkPointing;
    public bool paused;

    public int yOffset = 5;
    public float gameSpeed;

    // ==================== #6 - NUEVO: Opción de cámara ====================
    public bool followCamera = true;

    private void Start()
    {
        player = Object.FindObjectOfType<Player>();

        // Cargar preferencia guardada (1 = seguir, 0 = estática)
        followCamera = PlayerPrefs.GetInt("cameraFollow", 1) == 1;
    }

    private void Update()
    {
        if (paused && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (paused && Input.GetKeyDown(KeyCode.Q))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("StartScene");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            instructionsMenu();
        }

        if ((!(base.name != "GameOver") && !(base.name != "NextLevel")) || !SceneManager.GetActiveScene().name.StartsWith("Level"))
        {
            return;
        }

        // ==================== #6 - Solo mover cámara si followCamera está activado ====================
        if (followCamera)
        {
            float num = cam.transform.position.x;
            float y = cam.transform.position.y;

            if (SceneManager.GetActiveScene().name.EndsWith("1"))
            {
                if (player.transform.position.x < camBreak.transform.position.x)
                {
                    num += gameSpeed;
                    y = player.transform.position.y + (float)yOffset;
                }
                if (player.rb.velocity.x > 0f && player.transform.position.x >= base.transform.position.x && player.transform.position.x < camBreak.transform.position.x)
                {
                    num = player.transform.position.x;
                }
            }

            if (!player.isDashing && !player.falling)
            {
                cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(num, y, base.transform.position.z), 9f * Time.deltaTime);
            }

            if (intervalCounter(5))
            {
                renderUpdates();
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

    private void renderUpdates()
    {
        float x = Camera.main.transform.position.x;
        float y = Camera.main.transform.position.y;
        float num = Camera.main.aspect * Camera.main.orthographicSize;
        float orthographicSize = Camera.main.orthographicSize;

        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        for (int i = 0; i < rootGameObjects.Length; i++)
        {
            try
            {
                SpriteRenderer[] componentsInChildren = rootGameObjects[i].GetComponentsInChildren<SpriteRenderer>();
                for (int j = 0; j < componentsInChildren.Length; j++)
                {
                    SpriteRenderer component = componentsInChildren[j].GetComponent<SpriteRenderer>();
                    if (componentsInChildren[j].transform.position.x + component.bounds.extents.x < x - num || componentsInChildren[j].transform.position.x - component.bounds.extents.x > x + num)
                    {
                        componentsInChildren[j].enabled = false;
                    }
                    else if (componentsInChildren[j].transform.position.y - component.bounds.extents.y > y + orthographicSize || componentsInChildren[j].transform.position.y + component.bounds.extents.y < y - orthographicSize)
                    {
                        componentsInChildren[j].enabled = false;
                    }
                    else
                    {
                        component.enabled = true;
                    }
                }
            }
            catch (MissingComponentException)
            {
            }
        }
    }

    public void destroyAllNpcs()
    {
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        for (int i = 0; i < rootGameObjects.Length; i++)
        {
            if ((rootGameObjects[i].tag == "wolf" || rootGameObjects[i].tag == "npc" || rootGameObjects[i].tag == "zombie") && rootGameObjects[i].name != "BossMan")
            {
                Object.Destroy(rootGameObjects[i]);
            }
        }
    }

    public void checkPointReached()
    {
        StartCoroutine("checkPointRoutine");
    }

    public IEnumerator checkPointRoutine()
    {
        checkPointing = true;
        alert.text = "CheckPoint Reached";
        yield return new WaitForSeconds(2f);
        alert.text = "";
        checkPointing = false;
    }

    public void instructionsMenu()
    {
        if (!instOrEscUI.activeInHierarchy)
        {
            instOrEscUI.SetActive(value: true);
            Time.timeScale = 0f;
            paused = true;
        }
        else
        {
            instOrEscUI.SetActive(value: false);
            Time.timeScale = 1f;
            paused = false;
        }
    }

    public void goToL2()
    {
        PlayerPrefs.SetInt("level", 2);
        hudUI.SetActive(value: false);
        nlUI.SetActive(value: true);
    }

    public void gameOver()
    {
        if (SceneManager.GetActiveScene().name.EndsWith("2"))
        {
            Object.FindObjectOfType<AudioManager>().Pause("strikeDown");
        }
        if (!gameOverUI.activeInHierarchy)
        {
            Object.FindObjectOfType<AudioManager>().Play("gameOver");
        }
        Camera.main.GetComponent<AudioSource>().Pause();
        hudUI.SetActive(value: false);
        gameOverUI.SetActive(value: true);
    }

    public void postGameOver()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void newGame()
    {
        PlayerPrefs.SetInt("level", 1);
        SceneManager.LoadScene("StoryBreak1");
    }

    public void continueGame()
    {
        if (PlayerPrefs.GetInt("level", 1) == 1)
        {
            SceneManager.LoadScene("StoryBreak1");
        }
        else
        {
            SceneManager.LoadScene("StoryBreak2");
        }
    }

    public void storyBreakContinue()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void gameComplete()
    {
        hudUI.SetActive(value: false);
        gameOverUI.SetActive(value: true);
        gameOverText.text = "Game Complete!!";
    }

    public void quit()
    {
        Application.Quit();
    }

    // ==================== #6 - NUEVO: Método para cambiar modo de cámara ====================
    public void ToggleCameraMode()
    {
        followCamera = !followCamera;
        PlayerPrefs.SetInt("cameraFollow", followCamera ? 1 : 0);

        alert.text = followCamera ? "Cámara: Seguir (actual)" : "Cámara: Estática (mundo abierto)";
        StartCoroutine(ClearAlertText());
    }

    private IEnumerator ClearAlertText()
    {
        yield return new WaitForSeconds(1.8f);
        alert.text = "";
    }
}