# Assignment3

## Introduction

In nature ants exhibit collective behaviour to accomplish large and complex tasks such as building nests and finding food. This collective behaviour is emergent. 

For this assignment we were given a Unity project with a world already created. That world consists of a container holding a variety of types of blocks. The block types are mulch, grass, stone, air, container, acidic, and nest.

The purpose of the assignment was to create ants and a queen ant that live in this world and try to maximize nest block production. 

The constraints of this system are as follows:
- Only the queen ant can place nest blocks.
- It costs her 1/3 of her maximum health to do so.
- Ants health reduces at a specific rate.
- When health hits 0, the ant dies and is removed from the world.

<img width="700" alt="Screen Shot 2021-03-29 at 8 51 02 PM" src="https://user-images.githubusercontent.com/50717419/112926313-8130fa00-90d0-11eb-9ffa-b1fdf69c7e26.png">

<img width="700" alt="Screen Shot 2021-03-29 at 8 51 05 PM" src="https://user-images.githubusercontent.com/50717419/112926317-82622700-90d0-11eb-8765-aced2b6275c4.png">

- When standing on an acidic block, rate at which ant health decreases is doubled.
- Ants can eat mulch blocks to gain health if they are on top of the block. Mulch blocks are removed if they are eaten.
- Ants can also share health with eachother if they are on the same block. This is a zero-sum exchange.
- However, when on the same block, ants cannont eat the block.
- Ants cannot move to blocks with a height difference greater than 2.
- Ants can dig blocks under them (except container blocks). Blocks are removed from world if dug.
- No new ants are created per evaluation/generation phase.

## Methods

First learned how to make the ants move around the world randomly and meet the constraints listed above (although this was further edited and optimized later in the coding process as well). 

Then tried three different ways of making a neural network to maximize nest block production. Did not complete the first way and did not optimize or fix the second way, since I found the third way was better.  
Used two different sized neural networks as well, but stuck with the one with one hidden layer since this seems to be recommended. Reference in code. 

- The first way was by creating one neural network that controlled the entire simulation. Ants had no "awareness" of a neural network, but it would output values that would give different probabilities for certain things they did, such as if they would move and if the queen would place a nest block. This idea is similar to the third and final idea. These probabilities would change between generations.
- The second was by allowing each ant to have its own independent neural network. Again ants had probabilities of doing certain actions and the outputs of the network were used to dictate the probabilities. 
- The third was to have again one neural network, but each ant runs that one network. Now ants do not have probabilities in their code, but rather the neural network output decides which decision the ants make. So each output neuron represents a different decision, and the one with the highest weight is what the ant does if possible. Networks are compared between generations and if the current one has a higher fitness (more nest blocks were produced) than the previous, it is kept and has an 80% chance of being mutated. 

## Results

The final version of the project has a simple UI with the generation count, the current nest block count, and the high score of most nest blocks acheived (updates after each generation finishes). 

<img width="1036" alt="Screen Shot 2021-03-29 at 8 33 22 PM" src="https://user-images.githubusercontent.com/50717419/112924819-0b2b9380-90ce-11eb-994e-7962673f9474.png">

Ants generate each generation at random coordinates in the world.

<img width="467" alt="Screen Shot 2021-03-29 at 9 23 38 PM" src="https://user-images.githubusercontent.com/50717419/112929123-133b0180-90d5-11eb-9910-e5be859b0b2f.png">

The queen places nest blocks when she has enough health.

![queenPlaceBlock](https://user-images.githubusercontent.com/50717419/112929851-7bd6ae00-90d6-11eb-85d4-bafd5462d329.gif)

When the generation is over, the ants are reset, and the world is also reset with random locations for the blocks inside of it. Queen always starts in the same position.

Training the network over multiple generations should produce a higher nest block count as ants learn. 

<img width="1431" alt="Screen Shot 2021-03-28 at 5 38 13 PM" src="https://user-images.githubusercontent.com/50717419/112926930-6317c980-90d1-11eb-93af-8d6b2cc3d15a.png">

Before a network is mutated, it is written to a file called TestUnityAssingment3IAMHERE.txt  

The neural network can be loaded from this file when the boolean value runFromFile in the WorldManager class is set to true.

A note on file organization:
The behaviour of the ants and the neural network classes are found in the Scripts folder in the Agents folder. The NeuralNet class contains the code to build the neural network and the NeuralNetController contains the code to implement it as well as some methods related to it.
The WorldManager file in the Terrain folder controls the running of the simulation. 

## Discussion

The neural network in the file currently, is not necessarily the best one, as I had one of the best ones (seen in an earlier screenshot) occur before I implemented serialization. I also ran into some issues running the project in Unity as it was intensive for my laptop. I am not sure how effective my neural networks are, and how well they learn. They seem to learn best if there are many ants created at the beginning of a generation, giving a higher probability for one ant to bump into the queen and give her some health causing her to place another nest block. I think they are learning from that interaction to minimize their distance to the queen, but I am not entirely sure since I wasn't able to run the project for too many generations. 

Another thing to note is that by randomly re-creating the world between generations, ants may face new challenges they didn't face in the last world. However, certain things should remain the same to optimize nest block production between worlds, such as maximizing or optimizing health and reducing distance to the queen.


## Future directions

Implementing the use of pheromones in addition to the neural network would be an interesting next step for this project.

Also figuring out the best mutation rate would be a good next step. It was difficult to find a good mutation rate that would create some diversity if the ants weren't making any good decisions, but also not mess up a good neural network. 
