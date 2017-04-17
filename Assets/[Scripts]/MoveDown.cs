using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDown : MonoBehaviour {

	public float minSpeed, maxSpeed, speedUpMultiplier;
	float speed;
	public GameManager GM;
	public bool notHit = true;

	void Start () 
	{
		speed = Random.Range(minSpeed, maxSpeed);
	}
	
	void FixedUpdate () 
	{
		if (!GM.gameIsPlaying && notHit) return;
		transform.Translate(Vector3.down* speed);
		speed *= speedUpMultiplier;
		if (transform.position.y < GM.monster.transform.position.y) Destroy(gameObject);
	}
}
