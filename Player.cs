using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luna;
using TMPro;


public class Player : MonoBehaviour
{

    enum EndCardTurnOnAfter { Dies, TimeAndTouches, }


    [Header("Endcard Configuration")]
    [LunaPlaygroundField("Show Endcard Depends On:" , 1 ,"End Game")]
    [SerializeField] EndCardTurnOnAfter EndCardMode;
    [LunaPlaygroundField("On mode Dies, Show EndCard After This Ammount Of Dies:", 2, "End Game")]
    [SerializeField] int ShowEndcardAfterThisCountOfDies;
    [LunaPlaygroundField("On mode Dies, Show EndCard After This Ammount Of Seconds:", 3, "End Game")]
    [SerializeField] float TimeToLoadEndcardOnDieMode;

    [SerializeField] GameObject GM;

    [Header("Player Configuration")]
    [LunaPlaygroundField("Player Speed:", 1, "Player Configuration")]
    [SerializeField] float Speed;
    [LunaPlaygroundField("JumpForce:", 0, "Player Configuration")]
    [SerializeField] float JumpForce;
    [SerializeField] GameObject FlyEffect;
    [SerializeField] Sprite PlayerDeadSprite;


    [Header("UI")]
    [LunaPlaygroundField("Time GameOver Loading:", 2, "General")]
    [SerializeField] float TimeForLoadingGameOverPanel;
    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] Animator ScoreAnim;
    [SerializeField] GameObject TapTextObj;
    [SerializeField] Animator GameOverTextAnim;
    [SerializeField] Animator ScorePanelAnim;
    [SerializeField] TextMeshProUGUI GameOverScore;
    int score = 0;
    int NumberOfClickes;
    int GameOverPanelOnce = 0;

    [Header("Camera")]
    [SerializeField] Transform CameraObj;
    [SerializeField] float ShakeDuration;
    [SerializeField] float ShakeMag;
    float CameraStartPosX;

    [Header("Tutorial")]
    [SerializeField] GameObject TutorialObjects;


    [Header("Sounds")]
    [SerializeField] AudioSource AS;
    [SerializeField] AudioClip JumpSound;
    [SerializeField] AudioClip PointSound;
    [SerializeField] AudioClip GameOverSound;



    Rigidbody2D rb;
    Animator Anim;
    bool GameIsRunning = false;
    bool TutorialMode = true;
    bool EndcardIsOn = false;


    // Start is called before the first frame update
    void Start()
    {    
        //offset camera pos for movment
        CameraStartPosX = CameraObj.transform.position.x;
        // setup and reset score text
        rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        ScoreText.text = "" + score;

        // check if showing endcard after dies or time
        CheckEndCardMode();
    }


    void CheckEndCardMode ()
    {
        GameObject DontDestroyObject = GameObject.FindGameObjectWithTag("DontDestroy");

        if (EndCardMode == EndCardTurnOnAfter.TimeAndTouches)
        {          
            DontDestroyObject.GetComponent<DontDestroyGM>().GetEndCardMod(1);       // 0 - Dies     1 - Taps and Time
        }
      else if (EndCardMode == EndCardTurnOnAfter.Dies)
        {

            DontDestroyObject.GetComponent<DontDestroyGM>().GetEndCardMod(0);       // 0 - Dies     1 - Taps and Time
        }

    }


    // Update is called once per frame
    void Update()
    {
        OnPlayerTouchScreen();

        PlayerRotateByAngle();
    }


    // for Endcard
    public void TurnOffUI ()
    {
        GameIsRunning = false;
        ScoreText.gameObject.SetActive(false);
        TapTextObj.SetActive(false);
        GameOverTextAnim.gameObject.SetActive(false);
        ScorePanelAnim.gameObject.SetActive(false);
    }


    // on touch check if its first touch (exit tutorial)
    // so turn off tutorial ad set the gameplay and after that jump
    void OnPlayerTouchScreen ()
    {
        if (Input.GetMouseButtonDown(0) && GameIsRunning || Input.GetMouseButtonDown(0) && !GameIsRunning && TutorialMode)
        {
            if (NumberOfClickes == 0)
            {
                NumberOfClickes++;
                TutorialMode = false;
                TutorialObjects.SetActive(false);
                TapTextObj.SetActive(false);
                ScoreText.gameObject.SetActive(true);
                GameIsRunning = true;
                GM.GetComponent<GM>().GameStarted();
                ScoreText.gameObject.SetActive(true);

                //Send event that player started game
                Luna.Unity.Analytics.LogEvent("Game_Started", 1);
            }

            Jump();
        }
    }


    // like in the original game, rotate player by angle of movement
    void PlayerRotateByAngle ()
    {
        if (GameIsRunning || TutorialMode)
        {
            // calc angle that player needs to look
            float angle = Vector3.Angle(Vector3.right, rb.velocity);

            //becouse Vector3.Angle return only positive numbs, so if bird falling just inverse angle
            if (rb.velocity.y < 0)
                angle = -angle;

            transform.eulerAngles = new Vector3(0, 0, angle / 2);
        }
    }



    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, JumpForce);
        Instantiate(FlyEffect, gameObject.transform.position, Quaternion.identity);
        AS.PlayOneShot(JumpSound);
        Anim.Play("Fly");
    }


    // move player forward
    private void FixedUpdate()
    {
        if(GameIsRunning || TutorialMode)
            rb.velocity = new Vector2(Speed, rb.velocity.y);

    }


    //move camera with the player
    private void LateUpdate()
    {
        if(GameIsRunning || TutorialMode)
         CameraObj.position = new Vector3(transform.position.x + CameraStartPosX, CameraObj.position.y, CameraObj.position.z);
    }


    // on player collision with something, end game and turn on UI
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!EndcardIsOn)
        {
            if (collision.gameObject.tag != "Tutorial")
            {
                //error improver
                GetComponent<Animator>().enabled = false;
                // change player sprite to dead
                gameObject.GetComponent<SpriteRenderer>().sprite = PlayerDeadSprite;
                // end game in all methods
                GameIsRunning = false;
                // stop spawn new pipes
                GM.GetComponent<GM>().GameOverFun();
                // shake camera on collision
                StartCoroutine(CameraShake());

              
                

                //only on first collision turn on UI
                if (GameOverPanelOnce == 0)
                {
                        AS.PlayOneShot(GameOverSound);
                        GameOverPanelOnce++;
                    StartCoroutine(EndGameUIPopUp());
                }
            }
        }
    }

    IEnumerator EndGameUIPopUp ()
    {
        // Turn off Score ui that showed in the gameplay
        ScoreText.gameObject.SetActive(false);

        //turn on Game over UI with animations
        GameOverTextAnim.gameObject.SetActive(true);
        GameOverTextAnim.Play("GameOver");

        //If Endcard mode set to Dies so start checking if needs now to show Endcard
        //If Endcard mode set to Time and Touches so do regular Restart UI
        if (EndCardMode == EndCardTurnOnAfter.TimeAndTouches)
        {
            // do littel delay between GameOver text and score panel
            yield return new WaitForSeconds(TimeForLoadingGameOverPanel);
            ScorePanelAnim.gameObject.SetActive(true);
            GameOverScore.text = "" + score;
            ScorePanelAnim.Play("ScorePanel");
        }
        else if (EndCardMode == EndCardTurnOnAfter.Dies)
        {
            Debug.Log("Dies Mode Work");
            // Get data about witch num of dead is right now
            GameObject DontDestroyObj = GameObject.FindGameObjectWithTag("DontDestroy");
            int TimesThatDies = DontDestroyObj.GetComponent<DontDestroyGM>().NumberOfRestarts;

            // trasnlate UI from playground to code count (exmple user typed: show end card after 1 dead, but in the game
            // there wasnt pressed Restart button so int gonna be 0)
            TimesThatDies++;


            if(TimesThatDies == ShowEndcardAfterThisCountOfDies)
            {
                Debug.Log("ShowEndCard");
                yield return new WaitForSeconds(TimeToLoadEndcardOnDieMode);
                TurnOffUI();
                DontTurnOnUIAfterEndCard();
                DontDestroyObj.GetComponent<DontDestroyGM>().EndCardModeOnDieShowEndCard();
            }
            else
            {
                yield return new WaitForSeconds(TimeForLoadingGameOverPanel);
                ScorePanelAnim.gameObject.SetActive(true);
                GameOverScore.text = "" + score;
                ScorePanelAnim.Play("ScorePanel");
            }

        }
    }

  
    IEnumerator CameraShake ()
    {
        //Save camara pos before Shake for reseting
        Vector3 OriginalPos = CameraObj.transform.position;

        //timer reset
        float ElapsedTime = 0;

        // while timer is on, add random float to camera pos
        while (ElapsedTime < ShakeDuration)
        {
            float XOffset = Random.Range(-0.5f, 0.5f) * ShakeMag;
            float YOffset = Random.Range(-0.5f, 0.5f) * ShakeMag;

            CameraObj.transform.localPosition = new Vector3(OriginalPos.x + XOffset, OriginalPos.y + YOffset, OriginalPos.z);

            ElapsedTime += Time.deltaTime;

            yield return null;
        }

        // reset camara start pos
        CameraObj.transform.position = OriginalPos;
    }


    // Add score on eneter pipes and do random animation
    public void AddScore ()
    {
        if (GameIsRunning)
        {
            score++;
            ScoreText.text = "" + score;
            int rand = Random.Range(0, 1);
            switch (rand)
            {
                case 0: ScoreAnim.Play("PopRight");
                    break;

                case 1: ScoreAnim.Play("PopLeft");
                    break;
            }

            AS.PlayOneShot(PointSound);


            // Send Relevant Event

            switch(score)
            {
                case 1: Luna.Unity.Analytics.LogEvent("First_Pipe", 1);
                    break;
                case 2:
                    Luna.Unity.Analytics.LogEvent("Second_Pipe", 1);
                    break;
                case 3:
                    Luna.Unity.Analytics.LogEvent("Third_Pipe", 1);
                    break;
            }
        }
    }

    public void DontTurnOnUIAfterEndCard ()
    {
        EndcardIsOn = true;
    }

}