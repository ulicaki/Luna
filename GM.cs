using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Luna;

public class GM : MonoBehaviour
{
    [Header("General")]
    [SerializeField] Transform Player;
    bool GameOver;

    [Header("Pipes")]
    [SerializeField] GameObject Pipe;
    [SerializeField] Transform SpawPosObject;

    [Header("CloudSpawner")]
    [SerializeField] GameObject Cloud1;
    [SerializeField] GameObject Cloud2;
    [SerializeField] GameObject Cloud3;
    [SerializeField] Transform CloudSpawnerObj;
    [SerializeField] float MinYCloudSpawn;
    [SerializeField] float MaxYCloudSpawn;
    [SerializeField] float TimeBetweenCloudSpawn;

    [Header("Level")]
    [SerializeField] GameObject Floor;
    [SerializeField] GameObject Background;
    [SerializeField] Transform FloorSpawnerPos;
    [SerializeField] Transform BackgorundSpawnerPos;
    float PlayerOffsetX;

    [LunaPlaygroundField("Time between Pipe Spaws", 2,"General")]
    [SerializeField] float TimeBetweenSpawns;

    public bool GamePlaying = true;



    private void Start()
    {
        
        StartCoroutine(CloudSpawnTimer());
        

        SpawnFloorAndBackgorund();

        PlayerOffsetX = Player.position.x;
    }

    public void GameStarted ()
    {
        StartCoroutine(PipeSpawnTimer());
    }


    // Spawn Clouds timer
    IEnumerator CloudSpawnTimer ()
    {
        int i = 1;
        while(i > 0)
        {
        if(!GameOver)
          SpawnCloud();
        yield return new WaitForSeconds(TimeBetweenCloudSpawn);
        i++;
        }

    }


    // Random cloud spawn
    void SpawnCloud ()
    {
        float randY = Random.Range(MinYCloudSpawn, MaxYCloudSpawn);
        int randCloud = Random.Range(0, 3);
        switch(randCloud)
        {
            case 0:
                Instantiate(Cloud1, new Vector2(CloudSpawnerObj.position.x, randY), Quaternion.identity);
                break;
            case 1:
                Instantiate(Cloud2, new Vector2(CloudSpawnerObj.position.x, randY), Quaternion.identity);
                break;
            case 2:
                Instantiate(Cloud3, new Vector2(CloudSpawnerObj.position.x, randY), Quaternion.identity);
                break;

        }
        
    }


    private void Update()
    {

        // spawn floor every time player travle 101.3763f on X axis
        if (Player.position.x - PlayerOffsetX >= 101.3763f && !GameOver)
        {
            SpawnFloorAndBackgorund();
            PlayerOffsetX = Player.position.x;
        }
    }

    // Pipe timer spawner
    IEnumerator PipeSpawnTimer ()
    {
        int i = 1;
        while (i > 0)
        {
            if(!GameOver)
            PipeSpawn();
            yield return new WaitForSeconds(TimeBetweenSpawns);
            i++;
        }

    }

    void PipeSpawn ()
    {
        int RandomPosY = Random.Range(0, 15);
        Instantiate(Pipe, new Vector2(SpawPosObject.position.x, RandomPosY), Quaternion.identity);
    }

    public void SpawnFloorAndBackgorund ()
    {
        Instantiate(Floor, FloorSpawnerPos.position, Quaternion.identity);
        Instantiate(Background, BackgorundSpawnerPos.position, Quaternion.identity);

    }

    public void GameOverFun ()
    {
        GameOver = true;
    }

    public void RestartButton()
    {
        Application.LoadLevel(0);
    }


}
