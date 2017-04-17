using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	public GameObject player, monster, spike, obstacleSpawner;
	public GameObject[] background, UIObj;
	public Sprite[] storePlayerSprites, storeMonster;
	float highestBGRecyclePoint = 96, highestPlayerPoint;
	byte bGSwitchID = 0;   
    public bool gameIsPlaying = false;
	int playerStoreID, highscore = 0;  // 0 male, 1 female
    List<int> purchaseListPlayer = new List<int>();
	static float bgmVol, soundVol;

    public AudioSource UIButtonPress, MainMenuBGM, GameBGM;

	void Start () 
	{
        // If a high score exist in player prefs, put it in
		if (PlayerPrefs.HasKey("highscore"))
        {
            highscore = PlayerPrefs.GetInt("highscore");
        }
		if (PlayerPrefs.HasKey("player"))
        {
			playerStoreID = PlayerPrefs.GetInt("player");
        }
        //Checks player purchases starting from the 3rd element (0 based numerical ID)
        //If more **FREE** characters are added, add a if check in the for loop and update the else clause
		if (PlayerPrefs.HasKey("playerPurchase2"))
		{
			purchaseListPlayer.Add(1);
			purchaseListPlayer.Add(1);
			//Subtract 2 in the loop to remove first 2 characters
			//Adds 2 playerpref name to prevent saving over character 0 and 1
			for (int i = 0; i < storePlayerSprites.Length-2; ++i)
				purchaseListPlayer.Add(PlayerPrefs.GetInt("playerPurchase"+ (2+i).ToString()));
		}
		else //Otherwise, adds 1,1 and 0 for the rest
		{
			purchaseListPlayer.Add(1);
			purchaseListPlayer.Add(1);
			for (int i = 0; i < storePlayerSprites.Length-2; ++i)
				purchaseListPlayer.Add(0);
		}
		UIObj[4].GetComponent<Image>().sprite = storePlayerSprites[playerStoreID];
		if (PlayerPrefs.HasKey("soundVol"))
        {
			soundVol = PlayerPrefs.GetFloat("soundVol");
			bgmVol = PlayerPrefs.GetFloat("bgmVol");
			UIObj[9].GetComponent<Slider>().value = bgmVol;
			UIObj[10].GetComponent<Slider>().value = soundVol;
        }
        else
        {
        	soundVol = 0.5f;
        	bgmVol = 0.2f;
        }

        // Set player gender
        if(playerStoreID == 0)
        {
            player.GetComponent<Player>().SetGender(true);
        }
        else
        {
            player.GetComponent<Player>().SetGender(false);
        }

        MainMenuBGM.Play();
    }
	
	void Update () 
	{
		if (gameIsPlaying)
		{
			if (player.transform.position.y >= highestBGRecyclePoint)
			{
				//Updates the next highestpoint to reach before BG recycling begins
				//Removes spikes that are out of reach
				highestBGRecyclePoint += 48;
				background[bGSwitchID].transform.position = new Vector2(0, highestBGRecyclePoint);
				++bGSwitchID;
				if (bGSwitchID == 3) bGSwitchID = 0;
				createSpikes(highestBGRecyclePoint);
				eraseSpikes();
			}
			if (player.transform.position.y > highestPlayerPoint) highestPlayerPoint = player.transform.position.y;
			transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, highestPlayerPoint, -10), 20);
		}
	}

	List<GameObject> spikeList = new List<GameObject>();
	private void createSpikes(float levelPosition)
	{
		spikeList.Add(GameObject.Instantiate(spike, new Vector3(-9, levelPosition - Random.Range(4, 15),-2), Quaternion.identity) as GameObject);
		spikeList.Add(GameObject.Instantiate(spike, new Vector3(-9, levelPosition + Random.Range(4, 25),-2), Quaternion.identity) as GameObject);
		Quaternion temp = Quaternion.identity;
		temp.eulerAngles = new Vector2 (0, 180);
		spikeList.Add(GameObject.Instantiate(spike, new Vector3(9, levelPosition - Random.Range(4, 15),-2), temp) as GameObject);
		spikeList.Add(GameObject.Instantiate(spike, new Vector3(9, levelPosition + Random.Range(4, 25),-2), temp) as GameObject);
	}

	//Removes last 4 spikes to prevent performance loss over time
	private void eraseSpikes()
	{
		for (int i = 0; i < 4; ++i)
		{
			Destroy(spikeList[0]);
			spikeList.RemoveAt(0);
		}
	}

	private void gameStart()
	{
		foreach (GameObject i in GameObject.FindGameObjectsWithTag("obstacle")) Destroy(i);
		highestBGRecyclePoint = 96;
		bGSwitchID = 0;
		background[0].transform.position = Vector2.zero;
		background[1].transform.position = new Vector2(0, 48);
		background[2].transform.position = new Vector2(0, 96);
		highestPlayerPoint = 0;
		gameIsPlaying = true;
		createSpikes(48);
		createSpikes(96);
		transform.position = new Vector3(0, 0, -10);
		monster.transform.position = new Vector3(2f, -12f, -2f);
		monster.SetActive(true);
		player.transform.position = new Vector3(8.7f, 0f, -1f);
		player.GetComponent<Rigidbody2D>().simulated = true;
        player.SetActive(true);
        player.GetComponent<Player>().PlayerInit();
        player.GetComponent<Player>().Reset();
		obstacleSpawner.SetActive(true);
		obstacleSpawner.GetComponent<ObstacleSpawner>().reset();
	}

	public void gameEnd(byte deathCause)
	{
		gameIsPlaying = false;
		obstacleSpawner.SetActive(false);

		//Switch that handsles animation and deathtimer
		switch(deathCause)
		{
			case 0:
			StartCoroutine(deathByTimer(0.3f));
			break;
			case 1:
			player.GetComponent<Rigidbody2D>().simulated = false;
			popUp();
			break;
			case 2:
			StartCoroutine(deathByTimer(1.5f));
			break;
		}

        // Update the value in player prefs & highscore var
        if (highestPlayerPoint > highscore)
        {
            highscore = (int)highestPlayerPoint;
            PlayerPrefs.SetInt("highscore", highscore);
        }

		UIObj[7].GetComponent<Text>().text = ((int)highestPlayerPoint).ToString();
		UIObj[8].GetComponent<Text>().text = highscore.ToString();
    }

	IEnumerator deathByTimer(float time)
	{
		yield return new WaitForSeconds(time);
		player.SetActive(false);
		popUp();
	}

	private void popUp()
	{
		UIObj[6].GetComponent<Animator>().Play("UIMoveUp");
	}

	//Handles all UI transistions
	public void UI(int id)
	{
		switch(id)
		{
		case -2:
			UIObj[2].SetActive(true);
			UIObj[3].SetActive(false);
			break;
		case -1:
			UIObj[0].SetActive(true);
			UIObj[1].SetActive(false);
			UIObj[2].SetActive(false);
			break;
		case 0:
			UIObj[0].SetActive(false);
			gameStart();
                MainMenuBGM.Stop();
                GameBGM.Play();
			break;
		case 1:
			UIObj[0].SetActive(false);
			UIObj[1].SetActive(true);
			break;
		case 2:
			UIObj[0].SetActive(false);
			UIObj[2].SetActive(true);
			break;
		case 3:
			Application.Quit();
			break;
		case 4:
			UIObj[2].SetActive(false);
			UIObj[3].SetActive(true);
			break;
		case 5:
			UIObj[0].SetActive(true);
			player.SetActive(false);
			monster.SetActive(false);
			UIObj[6].GetComponent<Animator>().Play("UIMoveDown");
			foreach (GameObject i in GameObject.FindGameObjectsWithTag("obstacle")) Destroy(i);
                GameBGM.Stop();
                MainMenuBGM.Play();
			break;
		case 6:
			UIObj[6].GetComponent<Animator>().Play("UIMoveDown");
			gameStart();
			break;
		}

        UIButtonPress.Play();

    }

	public void playerStore(bool dir)
	{
		if (dir)
		{
			++playerStoreID;
            if (playerStoreID == storePlayerSprites.Length)
            {
                playerStoreID = 0;
                player.GetComponent<Player>().SetGender(true);
            }
		}
		else
		{
			--playerStoreID;
            if (playerStoreID == -1)
            {
                playerStoreID = storePlayerSprites.Length - 1;
                player.GetComponent<Player>().SetGender(false);
            }
		}
		UIObj[4].GetComponent<Image>().sprite = storePlayerSprites[playerStoreID];
		if (purchaseListPlayer[playerStoreID] == 1)
		{
			PlayerPrefs.SetInt("player", playerStoreID);
			UIObj[5].SetActive(false);
		}
		else
		{
			UIObj[5].SetActive(true);
		}
	}

	public void playerStorePurchase()
	{
		purchaseListPlayer.Insert(playerStoreID, 1);
		UIObj[5].SetActive(false);
		PlayerPrefs.SetInt("player"+playerStoreID.ToString(), playerStoreID);
	}

	public void changeVolume()
	{
		soundVol = UIObj[10].GetComponent<Slider>().value;
		PlayerPrefs.SetFloat("soundVol", soundVol);
		bgmVol = UIObj[9].GetComponent<Slider>().value;
		PlayerPrefs.SetFloat("bgmVol", bgmVol);

        UIButtonPress.volume = soundVol;
        MainMenuBGM.volume = bgmVol;
        GameBGM.volume = bgmVol;

        player.GetComponent<Player>().SetVolume(soundVol);
	}
}
