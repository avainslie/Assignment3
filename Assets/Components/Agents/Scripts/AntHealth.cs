using UnityEngine;
using System.Collections;


namespace Antymology.AgentScripts
{
    /// <summary>
    /// Manages all methods related to the ants health
    /// </summary>
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

        // Ant health reduces at a fixed rate
        private void lowerAntHealthFixedAmount()
        {
            if (!gameObject.tag.Equals("queen"))
            {
                // Double the amount that is reduced if on acidic block
                if (standingOnAcidicBlock)
                {
                    health -= 4f;
                }
                else
                {
                    health -= 2f;
                }
            }
        }


        // Checks if ant is dead and should be removed
        private bool isAntAlive()
        {
            if (health <= 0f && !gameObject.tag.Equals("queen")) // Queen never dies
            {
                gameObject.SetActive(false);
                return false;
            }
            return true;
        }

        #region PUBLIC METHODS

        public void eatMulchGainHealth()
        {
            if (canEat)
            {
                health += 50;
            }
        }

        // Decides whether to share
        public void shareHealthToAntWithLess(string otherTag, AntHealth otherAntHealth)
        {
            if (otherAntHealth.health < health)
            {
                otherAntHealth.health += shareHealth(otherAntHealth.health, otherTag);
            }
        }

        // Calculates how much to share
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

