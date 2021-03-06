using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIScript : MonoBehaviour
{

	public float levelTime = 99;

	public static int bombMaxModifier = 3;

	GameObject player;

	Text timerTextMinutes, timerTextSeconds, livesText;
	 
	static Text bombLengthText;

	static Text bombTotalText;

	int playerLives;

	// Use this for initialization
	void Start ()
    {
		//Gets player information
		player = GameObject.Find("Player");

		//Gets text information
		timerTextMinutes = GameObject.Find("TimeCountdown").transform.GetChild(0).GetComponent<Text>();
		timerTextSeconds = GameObject.Find("TimeCountdown").transform.GetChild(1).GetComponent<Text>();
		livesText = GameObject.Find("Lives").transform.GetChild(0).GetComponent<Text>();
		bombLengthText = GameObject.Find("Explosion_Distance").transform.GetChild(0).GetComponent<Text>();
		bombTotalText = GameObject.Find("Bomb_Total").transform.GetChild(0).GetComponent<Text>();
	}

    void GetBombTotal()
    {
        int bombNumber = 0;
        Transform bombParent = GameObject.Find("Bombs").transform;
        for(int i = 0; i < bombParent.childCount; ++i)
        {
            if (bombParent.GetChild(i).GetComponent<BombControl>()) { ++bombNumber; }
        }

        int bombMax = player.GetComponent<PlayerControl>().bombMaxCount;
        bombNumber = bombMax - bombNumber;
        bombTotalText.text = bombNumber.ToString();
    }

	public void ExplosionDistance()
    {
		bombLengthText.text = player.GetComponent<PlayerControl>().bombRange.ToString();
	}

	//Changes the life total in the heads up display when the player takes damage from an enemy or bomb
	void Lives()
    {
		playerLives = player.GetComponent<Health>().health;
		livesText.text = playerLives.ToString();
	}

	void World()
    {
        /* method that updates world number is located in the RoomHudUpdater script which should be 
         * attached to every room in each scene
         */
	}

	void TimeCountdown()
    {
		levelTime -= Time.deltaTime;
		timerTextMinutes.text = Mathf.Floor(levelTime / 60).ToString("0");

		if (levelTime <= 0)
        {
			timerTextMinutes.text = 0.ToString("0");
		}

		timerTextSeconds.text = Mathf.Floor(levelTime % 60).ToString("00");

		if (levelTime <= 0)
        {
			timerTextSeconds.text = 0.ToString("00");
		}

		if (levelTime <= 0)
        {
            Debug.Log("You Ran Out of Time");
			LevelManager levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
			levelManager.LoadLevel("Game Over Screen");
		}
	}

	void Update ()
    {
		Lives();
		TimeCountdown();
        GetBombTotal();
	}
}
