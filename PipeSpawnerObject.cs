using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpawnerObject : MonoBehaviour
{

    /// <summary>
    ///
    ///that script just keep the object the same distace from the player as at start
    ///its needed for spawn Background/pipes/cloud etc position
    /// 
    /// </summary>

    [SerializeField] Transform Player;
    float StartPosX;
    // Start is called before the first frame update
    void Start()
    {
        StartPosX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(Player.position.x + StartPosX, transform.position.y);
    }

 
}
