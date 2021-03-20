using UnityEngine;
using System.Collections;


namespace Antymology.AgentScripts
{
    public class AntHealth : MonoBehaviour
    {

        public float health;

        public bool standingOnAcidicBlock;

        private float maxHealth;

        public bool canEat;

        

        // Use this for initialization
        void Awake()
        {
            standingOnAcidicBlock = false;
            canEat = true;

            // Waits 5s to start then calls function in 1st arg every 5s
            InvokeRepeating("lowerAntHealthFixedAmount", 5f, 5f);

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // Check if ant health at or below 0
            if (!isAntAlive())
            {
                CancelInvoke("lowerAntHealthFixedAmount");
            }
        }


        private void lowerAntHealthFixedAmount()
        {
            if (standingOnAcidicBlock)
            {
                health -= 10f;
            }
            else
            {
                health -= 5f;
            }

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

        #region PUBLIC METHODS 
        public void eatMulchGainHealth()
        {
            if (canEat)
            {
                health += 5;
            }
        }

        public float shareHealth(float otherAntHealth)
        {
            health -= 5;
            
            return 5;

        }

        public void costQueenHealth()
        {
            health -= (maxHealth / 3);
        }

        #endregion


    }

}

