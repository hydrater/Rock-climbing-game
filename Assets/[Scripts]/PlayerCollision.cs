using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {

	public GameManager GM;
    
	void Start () {
		
	}
	

	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.name == "monster")
		{
			other.GetComponent<Monster>().attack(0);
			GM.gameEnd(0);
		}
	}
}
