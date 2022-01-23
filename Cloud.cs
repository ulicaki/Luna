using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] float TimeToDestroy;
    float Speed;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        Speed = Random.Range(5, 6);
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(Timer());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = new Vector3(Speed, 0, 0);
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(TimeToDestroy);
        Destroy(gameObject);
    }


}
