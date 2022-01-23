using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luna;



public class DontDestroyGM : MonoBehaviour
{


    [LunaPlaygroundField("Overall time until EndCard", 5, "End Game")]
    [SerializeField] float TimeUntilEndCard;

    [LunaPlaygroundField("Number Tap Until Endcard", 4, "End Game")]
    [SerializeField] int NumberOfClickes;

    [Header("Sounds")]
    [SerializeField] AudioSource AS;
    [SerializeField] AudioClip PopUpSound;

    int EndCardMode;            // 0 = Dies, 1 - Taps + Time
    public int NumberOfRestarts;
    bool EndCardOn;
    GameObject Player;

    public void GetEndCardMod (int EndCardModeGeten)
    {
        EndCardMode = EndCardModeGeten;
    }


    // dont destroy that object
    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("DontDestroy");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    public void RestartButton()
    {
        NumberOfRestarts++;
        //Luna.Unity.Analytics
        switch(NumberOfRestarts)
        {
            case 1:
                Luna.Unity.Analytics.LogEvent("First_Restart", 1);
                break;
            case 2:
                Luna.Unity.Analytics.LogEvent("Second_Restart", 1);
                break;
            case 3:
                Luna.Unity.Analytics.LogEvent("Third_Restart", 1);
                break;
        }
        Application.LoadLevel(0);
    }

    private void Update()
    {
        if(EndCardMode == 1)            // Do it only if choosen to show end card after time + Touches
        CheckEndCardForTime();


        if(EndCardMode == 1)            // Do it only if choosen to show end card after time + Touches
        CheckEndCardForClicks();

    }




    // This is timer so EndCard will turn on before the ad will ends
    void CheckEndCardForTime ()
    {
        TimeUntilEndCard -= Time.deltaTime;
        if (TimeUntilEndCard <= 0 && !EndCardOn)
        {
            ShowEndCard();
        }

    }


    // after some amount of clicks on the screen game gonna show end card
    void CheckEndCardForClicks ()
    {
        Debug.Log("Touches");
        if (Input.GetMouseButtonDown(0))
            NumberOfClickes--;

        if (NumberOfClickes <= 0 && !EndCardOn)
        {
            ShowEndCard();
        }
    }



    public void ShowEndCard ()
    {
        EndCardOn = true;
        Player.GetComponent<Player>().TurnOffUI();
        Player.GetComponent<Player>().DontTurnOnUIAfterEndCard();

        AS = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        AS.PlayOneShot(PopUpSound);
        EndCardController.Instance.OpenEndCard();

        //send event for showing Endcard
        Luna.Unity.Analytics.LogEvent(Luna.Unity.Analytics.EventType.EndCardShown);
    }

    public void EndCardModeOnDieShowEndCard ()
    {
        AS = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        AS.PlayOneShot(PopUpSound);
        EndCardController.Instance.OpenEndCard();

        //send event for showing Endcard
        Luna.Unity.Analytics.LogEvent(Luna.Unity.Analytics.EventType.EndCardShown);
    }



    public void InstallGame ()
    {
        Luna.Unity.Playable.InstallFullGame();
    }
}
