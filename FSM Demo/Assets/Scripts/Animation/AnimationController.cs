using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public static AnimationController Instance { get; private set; }

    private Animator anim;

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
        anim.SetTrigger("Jump");
    }
}