using UnityEngine;
using System.Collections;

public class SpineDemo : MonoBehaviour
{
    private Animator animator;

    // Use this for initialization
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            animator.SetInteger("state", 1);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            animator.SetInteger("state", 0);
        }
    }
}
