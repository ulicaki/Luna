using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialJumpArea : MonoBehaviour
{


    // all this maded becouse regular Animator.Play("Pointer") not worked well, so i came to this solution


    /// <summary>
    ///
    ///     i set 2d collider under the player so every time player touches if, player make a jump and also pointer + Tap text do animation
    ///
    /// after first tap (exit from turorial mode) this object disable 
    /// 
    /// </summary>


    [SerializeField] GameObject Player;
    [SerializeField] Animator PointerAnim;
    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            PointerAnim.Play("Pointer");
        }
    }

    private void Update()
    {
        transform.position = new Vector2(Player.transform.position.x, transform.position.y);
    }
}
