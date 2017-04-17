using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {
	public Transform targetDestiny;
	public Animator anim, monsterHand;
	bool canMove = false;

    public AudioSource MonsterBite;

	void Update()
	{
		if (canMove)
		{
			transform.position =  Vector3.Lerp(transform.position, targetDestiny.position, 0.04f);
		}
		if (targetDestiny.position.y - transform.position.y > 5)
		{
			canMove = true;
			monsterHand.Play("crawl");
			monsterHand.SetBool("stopCrawling", false);
		}
		else if (targetDestiny.position.y - transform.position.y < 1.2f)
		{
			monsterHand.SetBool("stopCrawling", true);
			canMove = false;
		}
	}

	public void attack(byte angle)
	{
		anim.Play("attack");
        MonsterBite.Play();
	}

}
