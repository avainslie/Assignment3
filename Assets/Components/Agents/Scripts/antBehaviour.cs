using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Antymology.Helpers;


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
        // Waits 5 s to start then calls function in 1st arg every 5 s
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
        if(!isAntAlive())
        {
            CancelInvoke("lowerAntHealthFixedAmount");
        }

        moveAnt();
        
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

    private void checkWhatBlockAntIsOn()
    {
        int[] pos = AntPosition.getAntCurrentPosition(transform.position);

        

        // Line below is not helpful
        //Debug.Log(pos.ToString());

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
