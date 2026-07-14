using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public static AnimationController Instance { get; private set; }

    private Animator anim;

    // Expose Animator and current state info
    public Animator Animator => anim;

    public AnimatorStateInfo CurrentStateInfo =>
        anim.GetCurrentAnimatorStateInfo(0);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        PoolParameters();
    }

    private void PoolParameters()
    {
        anim.SetFloat("Speed", PlayerController.Instance.CurrentSpeed);
        anim.SetBool("Grounded", PlayerController.Instance.IsGrounded);
        anim.SetBool(
            "FreeFall",
            MovementFSM.Instance.CurrentStateType == MovementStateType.Fall);
    }

    public void Jump()
    {
        anim.CrossFade("JumpStart", 0.1f);
    }

    public void IdleAndWhatNot()
    {
        anim.CrossFade("Idle, move", 0.1f);
    }

    
    public void Slide()
    {
        anim.CrossFade("Dash", 0.1f);
    }

    public void Land()
    {
        anim.CrossFade("JumpLand", 0.1f);
    }


    public void StopSlide()
    {
        anim.SetBool("Dash", false);
    }
}