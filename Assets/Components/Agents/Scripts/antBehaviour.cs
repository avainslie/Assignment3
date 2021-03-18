using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Antymology.Helpers;
using Antymology.Terrain;

// TODO: RESTRICT ONLY ONE ANT ON A BLOCK AT A TIME

/* 
 * Manages ants actions
 * References at bottom of file
 */
public class antBehaviour : MonoBehaviour
{
    [SerializeField] float health;

    private Rigidbody rigidbody;

    private System.Random RNG;

  
    [SerializeField] float _timeToWaitInbetween;
    private float _waitTimer;


    private void Awake()
    {
        // Waits 5s to start then calls function in 1st arg every 5s
        InvokeRepeating("lowerAntHealthFixedAmount", 5f, 5f);

        // Generate new random number generator
        RNG = new System.Random(ConfigurationManager.Instance.Seed);

    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        // Check if ant health at or below 0
        if(!isAntAlive())
        {
            CancelInvoke("lowerAntHealthFixedAmount");
        }


        
        // TODO: SHOULD ONLY CHECK BLOCK DIRECTLY UNDER ANT
        checkWhatBlockAntIsOn();
        
    }

    // Only moves ants every X seconds, set in Inspector
    private void moveAnt()
    {

        if (_waitTimer >= _timeToWaitInbetween)
        {
            // TEMPORARY CODE TO MOVE ANTS AROUND AND BUILD OTHER METHODS
            double r = RNG.NextDouble() * 100;

            if (r >= 60)
            {
                transform.Translate(Vector3.forward, Space.World);
            }
            else if (r < 60 && r >= 5)
            {
                //transform.Rotate(0, 90, 0);
                transform.Translate(Vector3.forward, Space.World);
            }
            else
            {
                //transform.Rotate(0, 180, 0);
                transform.Translate(Vector3.forward, Space.World);
            }


            _waitTimer = 0f;

        }
        else
        { 
            _waitTimer += 1 * Time.deltaTime;
        }





        // Everytime ant moves, check where he is
        checkWhatBlockAntIsOn();
    }



    private void OnCollisionEnter(Collision collision)
    {
       
        transform.Translate(Vector3.up, Space.World);


        //transform.Rotate(0, 90, 0);

    }

    private void OnTriggerEnter(Collider other)
    {
        // Why does this give the same height no matter where you are? Height of chunk??
        Debug.Log(other.GetComponent<Renderer>().bounds.size);
    }

    private void checkWhatBlockAntIsOn()
    {
        int[] pos = AntPosition.getAntCurrentPosition(transform.position);

        int x = pos[0];
        int y = pos[1];
        int z = pos[2];

        AbstractBlock ab = WorldManager.Instance.GetBlock(x, y, z);

        // Gets an air block since its getting y value of a bit higher off the ground,
        // not right below. so should subtract a value each time to account for this
        //Debug.Log("The type of block is " + ab.GetType()); // The type of block is Antymology.Terrain.AirBlock


        //Debug.Log("And at 0, 0, 0 it is: " + WorldManager.Instance.GetBlock(0, 0, 0).GetType());


        if (ab.GetType().Equals(typeof(Antymology.Terrain.AirBlock)))
        {

            // Do nothing
        }
        else if (ab.GetType().Equals(typeof(Antymology.Terrain.MulchBlock)))
        {
            consumeMulch(x ,y ,z);
       
        }
        else if (ab.GetType().Equals(typeof(Antymology.Terrain.AcidicBlock)))
        {

            // Do nothing
        }


    }



    private void lowerAntHealthFixedAmount()
    {
        health -= 5f;
    }

    // Checks if ant is dead and should be removed
    private bool isAntAlive()
    {
        if (health <= 0f)
        {
            Destroy(gameObject);
            return false;
        }
        return true;
    }

    private void consumeMulch(int xMulchBlock, int yMulchBlock, int zMulchBlock)
    {
        health += 5;
        Debug.Log("ON A MULCH");
        // Replace mulch block with airblock
        WorldManager.Instance.SetBlock(xMulchBlock, yMulchBlock, zMulchBlock, new AirBlock());
        
    }




}

/* REFERENCES
 * 
 * InvokeRepeating:
 * https://answers.unity.com/questions/17131/execute-code-every-x-seconds-with-update.html
 * 
 * Wait timer:
 * https://youtu.be/j-28BbzvgGk
 * 
 * 
 * 
 * 
 */
