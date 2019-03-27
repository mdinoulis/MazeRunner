using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class mazeAgent : Agent
{

    public GameObject ViewCamera = null;
    public AudioClip JumpSound = null;
    public AudioClip HitSound = null;
    public AudioClip CoinSound = null;

    // public MazeSpawner mazeInstance;

    private Rigidbody mRigidBody = null;
    private AudioSource mAudioSource = null;
    private bool mFloorTouched = false;

    private RayPerception rayPerception;

    public bool done = false;
    public int numCoins = 0;

    public void setNumCoins(int number)
    {
        numCoins = number;
    }

    Rigidbody rBody;
    public override void InitializeAgent()
    {
        rBody = GetComponent<Rigidbody>();
        mAudioSource = GetComponent<AudioSource>();
        rayPerception = GetComponent<RayPerception>();
        done = false;
    }

    public Transform Coin;
    public override void AgentReset()
    {

        Debug.Log("Resetting");

        Debug.Log("Destroy Agent on Reset");
        Destroy(gameObject, 1);

    }


    public override void CollectObservations()
    {

        // Add raycast perception observations for pillars and walls
        float rayDistance = 20f;
        float[] rayAngles = { 30f };
        string[] detectableObjects = { "pillar", "wall", "Coin" };
        AddVectorObs(rayPerception.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));

        // Target and Agent positions
        AddVectorObs(this.transform.position);

        // Agent velocity
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.z);

    }


    public float speed = 10;
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);

        if (ViewCamera != null)
        {
            Vector3 direction = (Vector3.up * 2 + Vector3.back) * 2;
            RaycastHit hit;
            Debug.DrawLine(transform.position, transform.position + direction, Color.red);
            if (Physics.Linecast(transform.position, transform.position + direction, out hit))
            {
                ViewCamera.transform.position = hit.point;
            }
            else
            {
                ViewCamera.transform.position = transform.position + direction;
            }
            ViewCamera.transform.LookAt(transform.position);
        }

    }

    void OnCollisionEnter(Collision coll)
    {

        if (coll.gameObject.tag.Equals("Floor"))
        {
            mFloorTouched = true;
            if (mAudioSource != null && HitSound != null && coll.relativeVelocity.y > .5f)
            {
                mAudioSource.PlayOneShot(HitSound, coll.relativeVelocity.magnitude);
            }
        }
        else
        {
            AddReward(-0.01f);
            if (mAudioSource != null && HitSound != null && coll.relativeVelocity.magnitude > 2f)
            {
                mAudioSource.PlayOneShot(HitSound, coll.relativeVelocity.magnitude);
            }
        }

    }

    void OnCollisionExit(Collision coll)
    {

        if (coll.gameObject.tag.Equals("Floor"))
        {
            mFloorTouched = false;
        }

    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag.Equals("Coin"))
        {

            AddReward(1.0f);
            numCoins -= 1;

            Debug.Log("Coins =");
            Debug.Log(numCoins);

            if (numCoins == 0)
            {
                AddReward(3.0f);
                Debug.Log("Score =");
                Debug.Log(GetCumulativeReward());
                if (mAudioSource != null && JumpSound != null)
                {
                    mAudioSource.PlayOneShot(JumpSound);
                }
                done = true;
                Done();
            }
            else
            {
                if (mAudioSource != null && CoinSound != null)
                {
                    mAudioSource.PlayOneShot(CoinSound);
                }
            }

            Destroy(other.gameObject);

        }

    }

    public override void AgentOnDone()
    {
        Debug.Log("Destroy Agent");
        //Destroy(gameObject);
    }


}



