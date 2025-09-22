using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //State Machine
    public PlayerBaseState currentState;
    public PlayerNormalState NormalState = new PlayerNormalState();

    public Rigidbody2D rb;
    public Animator animator;
    public PlayerInput input;
    public Collider2D col;

    [Header(" Collision ")]
    [SerializeField] private float collisionRadius = 0.25f;
    public Vector2 bottomOffest, rightOffset, leftOffset;

    //Grab Edge
    public float redXOffset, redYOffset, redXSize, redYSize;
    public float greenXOffset, greenYOffset, greenXSize, greenYSize;
    public bool greenBox, redBox;

    [Header(" Move ")]
    public float moveSpeed = 5f;
    public bool canMove = true;
    public AudioSource runAduioSrc;

    [Header(" Jump ")]
    public LayerMask groundLayer;
    public float jumpForce;

    [Header(" Controll ")]
    public bool canControll = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        SwitchToState(NormalState);
    }

    private void Update()
    {
        if (canControll && !DialogueManager.Instance.isShowing)
        {
            currentState.OnUpdate(this);
        }
        else
        {
            runAduioSrc.Stop();
        }
    }

    public void SwitchToState(PlayerBaseState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit(this);
        }

        currentState = newState;

        if (currentState != null)
        {
            currentState.OnEnter(this);
        }
    }

    public bool isGround()
    {
        //RaycastHit hit;
        return Physics2D.Raycast(transform.position, Vector2.down, 1.4f, groundLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("RescueTalk"))
        {
            canControll = false;
            this.rb.velocity = Vector2.zero;
            this.animator.SetFloat("Speed", 0f);
            DialogueManager.Instance.StartDialogueThisState();
        }
        else if (collision.CompareTag("NextPoint"))
        {
            DialogueManager.Instance.NextLevel();
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.4f);

        Gizmos.color = Color.red;
    }
}
