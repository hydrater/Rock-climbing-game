using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public enum PLAYER_STATE
    {
        START,
        HANGING,
        JUMP,
        SECONDJUMP,
        DEAD,
    }

    public PLAYER_STATE state;

    public bool onRightWall = true;    // variable to store which wall player is on

    public Rigidbody2D rb2d;
    public BoxCollider2D bc2d;
    public Animator anim;
    public SpriteRenderer sr;

    Sprite defaultSprite;

    // Reference to the walls. ( Need size to perform calculations )
    public BoxCollider2D rightWall, leftWall;

    // Variables for first jump
    public float firstJumpVertForce, firstJumpHoriForce;
    public float firstJumpGravityScale = 1;

    // Variables for second jump
    public float secondJumpVertForce, secondJumpHoriForce;
    public float secondJumpGravityScale = 1;

    // Speed at which the player moves down when in hanging state
    public float hangingGravityForce = 2f;

    // Private variables that store the position when player is stuck to a specific wall
    private float rightWallPos, leftWallPos;
    
    // Store the original scale of the object
    private float originalScaleX = 1.5f;

    public AudioSource WallSlide;
    public AudioSource Jetpack;

    public bool playerIsMale = true;
    
    // Use this for initialization
    void Start () {
        state = PLAYER_STATE.HANGING;   // hanging for now

        PlayerInit();

        defaultSprite = GetComponent<SpriteRenderer>().sprite;

        if (playerIsMale)
        {
            anim.Play("player_male_hang");
        }
        else
        {
            anim.Play("playerHang");
        }
    }

    public void PlayerInit()
    {
        // Calculate the position which the player will hang at the wall and store it
        rightWallPos = rightWall.transform.position.x - rightWall.size.x * 0.5f - bc2d.size.x;
        leftWallPos = leftWall.transform.position.x + leftWall.size.x * 0.5f + bc2d.size.x; 
        
        // Store original scale
        originalScaleX = transform.localScale.x;
    }
	
	// Update is called once per frame
	void Update () 
	{
        // Update hanging state
        if (state == PLAYER_STATE.HANGING)
        {
            // No gravity when hanging
            rb2d.gravityScale = 0;

            // Let the player slide down the wall based on the variable hangingGravityForce ( change in inspector )
            rb2d.velocity = Vector2.down * hangingGravityForce;

            // Make sure the player stick to the walls & do not go beyond
            UpdateHanging();
        }
        else if (state == PLAYER_STATE.JUMP || state == PLAYER_STATE.SECONDJUMP)
        {
            // If player collide with the right wall
            if (transform.position.x >= rightWallPos && rb2d.velocity.x > 0)
            {
                state = PLAYER_STATE.HANGING;
                PlaceOnRightWall();
                WallSlide.Play();
                onRightWall = true;
                rb2d.velocity = Vector2.zero;
                return;
            }
            else if (transform.position.x <= leftWallPos && rb2d.velocity.x < 0)
            {
                state = PLAYER_STATE.HANGING;
                PlaceOnLeftWall();
                WallSlide.Play();
                onRightWall = false;
                rb2d.velocity = Vector2.zero;
                return;
            }
        }

        // Handles the jump
        if(Input.GetMouseButtonDown(0))
        {
            // Only allow the player to perform first jump if he is hanging
            if (state == PLAYER_STATE.HANGING)
            {
                // Set the state
                state = PLAYER_STATE.JUMP;

                if (playerIsMale)
                {
                    anim.Play("player_male_jump");
                }
                else
                {
                    anim.Play("playerJump");
                }

                // Add force depending on inspector values
                // Handle anim direction ( localScale.x )
                if (!onRightWall)
                {
                    rb2d.AddForce(Vector2.up * firstJumpVertForce);
                    rb2d.AddForce(Vector2.right * firstJumpHoriForce);
                    
                }
                else
                {
                    rb2d.AddForce(Vector2.up * firstJumpVertForce);
                    rb2d.AddForce(Vector2.left * firstJumpHoriForce);
                    
                }

                // Change gravity scale based on inspector values
                rb2d.gravityScale = firstJumpGravityScale;
            }
            else if (state == PLAYER_STATE.JUMP)    // only allow the player to perform second jump if player is in first jump
            {
                // Set state 
                state = PLAYER_STATE.SECONDJUMP;

                if (playerIsMale)
                {
                    anim.Play("player_male_second_jump");
                }
                else
                {
                    anim.Play("playerSecondJump");
                }

                Jetpack.Play();

                // Reset velocity
                rb2d.velocity = Vector2.zero;

                // Add force and change gravity scale depending on inspector values
                if (!onRightWall)
                {
                    rb2d.AddForce(Vector2.up * secondJumpVertForce);
                    rb2d.AddForce(Vector2.right * secondJumpHoriForce);
                }
                else
                {
                    rb2d.AddForce(Vector2.up * secondJumpVertForce);
                    rb2d.AddForce(Vector2.left * secondJumpHoriForce);
                }
                rb2d.gravityScale = secondJumpGravityScale;
            }
        }
    }

    // Function to make sure that player does not go beyond the walls when hanging
    void UpdateHanging()
    {
        if (onRightWall)
        {
            if (transform.position.x >= rightWallPos)
            {
                PlaceOnRightWall();
                WallSlide.Play();
            }
        }
        else
        {
            if (transform.position.x <= leftWallPos)
            {
                PlaceOnLeftWall();
                WallSlide.Play();
            }
        }
    }

    // Two functions so that its more clear
    public void PlaceOnRightWall()
    {
        Vector3 oldPos = transform.position;
        oldPos.x = rightWallPos;
        transform.position = oldPos;

        sr.flipX = true;

        if (playerIsMale)
        {
            anim.Play("player_male_hang");
        }
        else
        {
            anim.Play("playerHang");
        }
    }

    public void PlaceOnLeftWall()
    {
        Vector3 oldPos = transform.position;
        oldPos.x = leftWallPos;
        transform.position = oldPos;

        sr.flipX = false;

        if (playerIsMale)
        {
            anim.Play("player_male_hang");
        }
        else
        {
            anim.Play("playerHang");
        }
    }

    // Function to reset player
    public void Reset()
    {
        state = PLAYER_STATE.HANGING;
        int wallSide = Random.Range(0, 2);
        //rb2d.gravityScale = 0;
        
        GetComponent<SpriteRenderer>().sprite = defaultSprite;

        if (wallSide == 0)
        {
            onRightWall = true;
            PlaceOnRightWall();
        }
        else
        {
            onRightWall = false;
            PlaceOnLeftWall();
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("obstacle"))
        {
			if(collision.gameObject.name == "rock(Clone)" || collision.gameObject.name == "stalagmite(Clone)")
            {
            	collision.gameObject.GetComponent<MoveDown>().notHit = false;

                if (playerIsMale)
                {
                    anim.Play("player_male_death_falling_rock");
                }
                else
                {
                    anim.Play("playerDeathFallingRock");
                }

                state = PLAYER_STATE.DEAD;
                Camera.main.GetComponent<GameManager>().gameEnd(2);
            }
            else if(collision.gameObject.name == "spike(Clone)")
            {
                if(state == PLAYER_STATE.HANGING)
                { 
                    // Spike drop
                    StartCoroutine(SpikeDrop(collision.gameObject));
                }
                else
                {
                    if (playerIsMale)
                    {
                        anim.Play("player_male_death_spike");
                    }
                    else
                    {
                        anim.Play("playerDeathSpike");
                    }
                    transform.position = collision.gameObject.transform.GetChild(0).position;

                    // Make the player face the spike
                    if (collision.gameObject.transform.position.x > transform.position.x)
                    {
                        sr.flipX = true;
                    }
                    else
                    {
                        sr.flipX = false;
                    }
                    
                    state = PLAYER_STATE.DEAD;
                    Camera.main.GetComponent<GameManager>().gameEnd(1);
                }
            }

        }
    }

    IEnumerator SpikeDrop(GameObject spike)
    {
        float distance = 0;

        float speed = 5f;

		yield return new WaitForSeconds (0.5f);

        spike.GetComponent<BoxCollider2D>().enabled = false;

        while(distance < 50f)
        {
            distance = Mathf.Abs(transform.position.y - spike.transform.position.y);
            spike.transform.Translate(0, -speed * Time.deltaTime, 0);

            speed += 1f;

            yield return null;
        }

        yield return null;
    }

    // Set the gender of the player. true parametr for male and false for female
    public void SetGender(bool male)
    {
        if(male)
        {
            playerIsMale = true;
        }
        else
        {
            playerIsMale = false;
        }
        Debug.Log("Setting gender to" + male);
    }

    public void SetVolume(float volume)
    {
        WallSlide.volume = volume;
        Jetpack.volume = volume;
    }
}
