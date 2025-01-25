using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private Animator animator;
    private float animationBaseSpeed;

    // Start is called before the first frame update
    void Start()
    {
        this.animator = this.GetComponent<Animator>();
        this.animationBaseSpeed = animator.speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAnimation(string animationName)
    {
        if(animator != null)
        {
            animator.Play(animationName);
        }
    }
}
