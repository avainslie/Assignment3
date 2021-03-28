using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Antymology.Helpers;
using Antymology.Terrain;
using System.Linq;

namespace Antymology.AgentScripts
{
    public static class DirectionFinder
    {
        // Returns valid directions ant can move in, forward or backward
        public static int[][] getPossibleForwardORBackwardDirections(int xCoord, int yCoord, int zCoord, string dir)
        {
            int[][] possibleForwardORBackwardDirections = new int[3][];

            int[] neighbourYCoords = new int[3];

            int[][] neighbourXZCoords = new int[3][];

            if (dir.Equals("F"))
            {
                neighbourXZCoords = new int[][]
                {
                new int[] { xCoord - 1, zCoord + 1 },
                new int[] { xCoord, zCoord + 1 },
                new int[] { xCoord + 1, zCoord+1 },
                };
            }
            else 
            {
                neighbourXZCoords = new int[][]
                {
                new int[] { xCoord - 1, zCoord - 1 },
                new int[] { xCoord, zCoord - 1 },
                new int[] { xCoord + 1, zCoord - 1 },
                };
            }
            

            for (int i = 0; i < 3; i++)
            {
                possibleForwardORBackwardDirections[i] = new int[3];
            }

            // Find y coordinate of each neighbour
            for (int i = 0; i < 3; i++)
            {
                neighbourYCoords[i] = WorldManager.Instance.getHeightAt(
                    neighbourXZCoords[i][0], neighbourXZCoords[i][1]);

                if (Mathf.Abs(neighbourYCoords[i] - yCoord) < 2 &&
                    !WorldManager.Instance.checkIfCoordinatesAreNotInWorld(
                        neighbourXZCoords[i][0], neighbourYCoords[i],
                        neighbourXZCoords[i][1]))
                {
                    // x coord of the neighbour
                    possibleForwardORBackwardDirections[i][0] = neighbourXZCoords[i][0];
                    // y coord of the neighbour
                    possibleForwardORBackwardDirections[i][1] = neighbourYCoords[i];
                    // z coord of the neighbour
                    possibleForwardORBackwardDirections[i][2] = neighbourXZCoords[i][1];
                }
            }

            // Now we have a jagged array of all the possible moves ant can make.
            // If array not full of moves, than the "empty" spots are occupied by zeros
            int[][] possibleDirectionsNoZeros = sliceTheZerosOut(possibleForwardORBackwardDirections);

            return possibleDirectionsNoZeros;
        }

        public static bool validXNeighbourHeight(int xToCheck, int y, int z)
        {
            int neighbourYCoord = WorldManager.Instance.getHeightAt(xToCheck, z);

            if (Mathf.Abs(neighbourYCoord - y) < 2 && !WorldManager.Instance.
                checkIfCoordinatesAreNotInWorld(xToCheck, neighbourYCoord, z))
                return true;

            return false;
        }
      
        // Some code learned from office hours w/Cooper
        // When moving from one black to another, ants are not allowed to move to a
        // block that is greater than 2 units in height difference
        // Could have also used 2D arrays instead of jagged
        public static int[][] getPossibleRandomDirections(int xCoord, int yCoord, int zCoord)
        {
            int[] neighbourYCoords = new int[8];

            int[][] neighbourXZCoords = new int[][]
            {
                new int[] { xCoord - 1, zCoord + 1 },
                new int[] { xCoord, zCoord + 1 },
                new int[] { xCoord + 1, zCoord+1 },
                new int[] { xCoord - 1, zCoord },
                new int[] { xCoord + 1, zCoord },
                new int[] { xCoord - 1, zCoord - 1 },
                new int[] { xCoord, zCoord - 1 },
                new int[] { xCoord + 1, zCoord - 1 }
            };

            // Max 8 directions, 3 int in each direction
            // When initializing an array in c#, all values will be 0.
            // So we will need to remove those later.
            int[][] possibleDirections = new int[8][];

            for (int i = 0; i < 8; i++)
            {
                possibleDirections[i] = new int[3];
            }


            // Find y coordinate of each neighbour
            for (int i = 0; i < 8; i++)
            {
                neighbourYCoords[i] = WorldManager.Instance.getHeightAt(
                    neighbourXZCoords[i][0], neighbourXZCoords[i][1]);

                // If the difference between the neighbouring block and current
                // block is less than 2 units save the coordinates as a possible
                // direction to move in. And check if valid world coords
                if (Mathf.Abs(neighbourYCoords[i] - yCoord) < 2 &&
                    !WorldManager.Instance.checkIfCoordinatesAreNotInWorld(
                        neighbourXZCoords[i][0], neighbourYCoords[i],
                        neighbourXZCoords[i][1]))
                {
                    // x coord of the neighbour
                    possibleDirections[i][0] = neighbourXZCoords[i][0];
                    // y coord of the neighbour
                    possibleDirections[i][1] = neighbourYCoords[i];
                    // z coord of the neighbour
                    possibleDirections[i][2] = neighbourXZCoords[i][1];
                }
            }


            // Now we have a jagged array of all the possible moves ant can make.
            // If array not full of moves, than the "empty" spots are occupied by zeros
            int[][] possibleDirectionsNoZeros = sliceTheZerosOut(possibleDirections);


            return possibleDirectionsNoZeros;
        }

        private static int[][] sliceTheZerosOut(int[][] possibleDirections)
        {

            // Default where to "slice" the jagged array
            int whereToSlice = 3;

            // Set whereToSlice to the first all zero sub array in jagged array
            for (int i = 0; i < possibleDirections.Length; i++)
            {
                if (possibleDirections[i].Sum() == 0)
                {
                    whereToSlice = i;
                    break;
                }
            }

            // Deal with case where there is nowhere to move
            if (whereToSlice == 0)
            {
                return null;
            }
            // whereToSlice can = 1 through 8
            else
            {
                int[][] possibleDirectionsNoZeros = new int[whereToSlice][];

                for (int i = 0; i < possibleDirectionsNoZeros.Length; i++)
                {
                    possibleDirectionsNoZeros[i] = possibleDirections[i];
                }
                return possibleDirectionsNoZeros;
            }
            
        }
    }
}