using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {

	public float minSpawnTime, maxSpawnTime;
	public GameObject rock, stalagmite;
	public GameManager GM;

	void Start () 
	{
			
	}

	public void reset()
	{
		StartCoroutine(spawnObstacle());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator spawnObstacle()
	{
		do
		{
			yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
			GameObject temp = Instantiate(Random.Range(0,2) == 0? rock : stalagmite, new Vector3(Random.Range(-4f, 4f), transform.position.y, -1), Quaternion.identity) as GameObject;
			temp.GetComponent<MoveDown>().GM = GM;
		}
		while (GM.gameIsPlaying);
	}
}
