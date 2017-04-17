using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHand : MonoBehaviour {

    public AudioSource MonsterClimb;
    
    void ClimbSound()
    {
        MonsterClimb.Play();
    }
}
