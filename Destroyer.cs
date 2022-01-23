using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{

    /// <summary>
    ///
    /// destroy that object after he reaching 88.9 distance in X axis
    /// 
    /// </summary>

    GameObject Player;
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (Player.transform.position.x - transform.position.x > 88.9f)
        {
            Destroy(gameObject);
        }
    }


}
