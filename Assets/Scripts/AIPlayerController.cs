using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class AIPlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    public Camera mainCamera;
    public PlayerState playerState { get; set; } = PlayerState.Playing;
    public GameObject homeSquare;
    public GameObject ball;
    
    public float rotationSpeed = 5;
        
    private Animator anim;
    private Rigidbody rbody;
    private Rigidbody ballRbody;
    private Vector3 targetLocation;
    private bool justShot = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Game Start for AI Player");
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            Debug.Log("NavMeshAgent could not be found");

        ballRbody = ball.GetComponent<Rigidbody>();
        anim = this.GetComponent<Animator>();
        agent.speed = GetAgentSpeedBasedOnGameLevel(agent.speed);
        agent.acceleration = GetAgentSpeedBasedOnGameLevel(agent.acceleration);
    }

    // Update is called once per frame
    void Update()
    {
        HandleRotatePlayer();
        switch (playerState)
        {
            case PlayerState.Playing:
                MovePlayerDuringPlaying();
                break;
            default:
                // Moving back to center area
                if (agent.enabled)
                {
                    agent.SetDestination(homeSquare.transform.position);
                }

                break;
        }
        
        anim.SetFloat("velx", Math.Min(agent.velocity.x / 10, 1f));
        anim.SetFloat("vely", Math.Min(agent.velocity.z / 10, 1f));
    }
    
    void HandleRotatePlayer()
    {
        Vector3 ballPosition = ballRbody.position;
        Vector3 dir = ballPosition - transform.position;
        dir.y = 0;//This allows the object to only rotate on its y axis
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
    }

    private void MovePlayerDuringPlaying()
    {
        BallThrowing bt = ball.GetComponent<BallThrowing>();

        // ball is hitting to our location
        if (bt._targetSquare != null && homeSquare.CompareTag(bt._targetSquare.tag))
        {
            Vector3 velocity = ball.GetComponent<Rigidbody>().velocity.normalized;
            velocity.y = 0;

            float distance = Vector3.Distance(ball.transform.position, bt.targetLocation);

            Vector3 extraPosition;
            // if (distance > 19f)
            // {
            //     // when ball is far away, AI player would walk through the radius of target location
            //     extraPosition = Quaternion.Euler(0, UnityEngine.Random.Range(-180.0f, 180.0f), 0)
            //                     * new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
            // }
            // else
            // {
                NormalDistribution nd = GetNormalDistributionBasedOnGameLevel();
                extraPosition = velocity * (float)nd.Sample(new System.Random());
            // }

            if (agent.enabled)
            {
                agent.SetDestination(bt.targetLocation + extraPosition);
            }
           
        }
        else
        {
            if (agent.enabled)
            {
                agent.SetDestination(homeSquare.transform.position);
            }
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        // player is holding the ball
        if (ballRbody.isKinematic)
        {
            return;
        }

        switch (playerState)
        {
            case PlayerState.Playing:
                if (!justShot && other.gameObject.CompareTag("Ball"))
                {
                    justShot = true;
                    BallThrowing bt = ball.GetComponent<BallThrowing>();
                    GameObject targetSquare = bt.GetRandomTargetSquare(homeSquare.tag);

                    float flyingTime = GetFlyingTimeBasedOnGameLevel();
                    bt.ShotTheBallToTargetSquare(homeSquare, targetSquare, flyingTime, null);

                    //reset justShot to false
                    StartCoroutine(ResetJustShot());
                }

                break;
        }
    }

    private float GetFlyingTimeBasedOnGameLevel()
    {
        int level = GameManager.Instance.currentLevel;

        float flyingTime = Random.Range(1.2f, 2f);

        // Each level will reduce the flying time to its 70%
        return flyingTime * (float)Math.Pow(0.7f, level - 1);
    }

    private float GetAgentSpeedBasedOnGameLevel(float speed)
    {
        int level = GameManager.Instance.currentLevel;
        return speed * (float)Math.Pow(1.3, level - 1);
    }

    private NormalDistribution GetNormalDistributionBasedOnGameLevel()
    {
        int level = GameManager.Instance.currentLevel;
        switch (level)
        {
            case 1:
                return new NormalDistribution(3.5f, 0.8f);
            case 2:
                return new NormalDistribution(3.5f, 0.5f);
            default:
                return new NormalDistribution(3.5f, 0.2f);
        }
    }
    
    IEnumerator ResetJustShot()
    {
        yield return new WaitForSeconds(0.3f);
        justShot = false;
    }
}
