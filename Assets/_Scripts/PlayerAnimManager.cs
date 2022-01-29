using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer SR;
    [SerializeField] private Animator AC;
    [SerializeField] private PlayerController PC;

    private void FixedUpdate()
    {
        AC.SetBool("Grounded", PC.Grounded);
        AC.SetBool("Moving", PC.XVelocity != 0);
        AC.SetBool("Falling", PC.YVelocity < 0);

        if (PC.XVelocity > 0)
            SR.flipX = false;
        else if (PC.XVelocity < 0)
            SR.flipX = true;
    }
}
