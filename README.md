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
- When standing on an acidic block, rate at which ant health decreases is doubled.
- Ants can eat mulch blocks to gain health if they are on top of the block. Mulch blocks are removed if they are eaten.
- Ants can also share health with eachother if they are on the same block. This is a zero-sum exchange.
- However, when on the same block, ants cannont eat the block.
- Ants cannot move to blocks with a height difference greater than 2.
- Ants can dig blocks under them (except container blocks). Blocks are removed from world if dug.
- No new ants are created per evaluation/generation phase.

## Methods

Tried three different ways of making a neural network to maximize nest block production. 

- The first way was by creating one neural network that controlled the entire simulation. Ants had no "awareness" of a neural networkd. This idea is similar to the third and final idea.
- The second was by allowing each ant to have its own independent neural network. 
- The third was to have again one neural network, but each ant runs that one network. 

## Results

The final version of the project has a simple UI with the generation count, the current nest block count, and the high score of most nest blocks acheived. 

## Future directions
