using UnityEngine;
using System.Collections;


namespace Antymology.AgentScripts
{
    public class AntHealth : MonoBehaviour
    {

        public float health;

        public bool standingOnAcidicBlock;

        public float maxHealth;

        public bool canEat;

        

        // Use this for initialization
        void Awake()
        {
            standingOnAcidicBlock = false;
            canEat = true;
            maxHealth = 1000;

            // Waits 5s to start then calls function in 1st arg every 5s
            InvokeRepeating("lowerAntHealthFixedAmount", 5f, 5f);

        }

        // Update is called once per frame
        void Update()
        {
            // Check if ant health at or below 0
            if (!isAntAlive())
            {
                CancelInvoke("lowerAntHealthFixedAmount");
            }

            // Ensure health doesn't go over max amount
            if (health > maxHealth)
            {
                health = maxHealth;
            }
        }


        private void lowerAntHealthFixedAmount()
        {
            if (!gameObject.tag.Equals("queen"))
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
        }


        // Checks if ant is dead and should be removed
        private bool isAntAlive()
        {
            if (health <= 0f && !gameObject.tag.Equals("queen")) // Queen never dies
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
                health += 10;
            }
        }

        public void shareHealthToAntWithLess(string otherTag, AntHealth otherAntHealth)
        {
            if (otherAntHealth.health < health)
            {
                otherAntHealth.health += shareHealth(otherAntHealth.health, otherTag);
            }
        }

        public float shareHealth(float otherAntHealth, string otherTag)
        {
            health -= 15;
            if (otherTag.Equals("queen"))
            {
                Debug.Log("Sharing health to queen");
                health -= 400;
                return 415;
            }
            return 15;
        }

        public void costQueenHealth()
        {
            health -= (maxHealth / 3);
        }

        public void resetHealth()
        {
            health = maxHealth;
        }

        #endregion


    }

}

