using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] Animator PointerAnim;
    [SerializeField] Animator TapTextAnimation;
    // Start is called before the first frame update
public void PointerClickedAnimation ()
    {
        Player.GetComponent<Player>().Jump();
        TapTextAnimation.Play("Tap");
    }

    public void EndAnimation ()
    {
        PointerAnim.SetBool("Point", false);
    }
}
