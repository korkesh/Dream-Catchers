# Dream-Catchers
GDAP 2016 Capstone Project

<b>Play Intructions:</b>

To run the game from the Unity Editor you must launch from the Intro scene found in the Scenes folder under 
GUIScenes. 

<b>In-Game Intructions:</b>

The options menu can be found upon starting the game. Options include inverting the Camera's axis and 
adjusting the game volume.

<b>NOTE:</b> Controls are set up for use with an Xbox-360 controller

<b>Extra Notes:</b>

<b>Dive Controls:</b>
There are three associated states with diving - dive, slide, and roll.

Dive state is entered by pressing X either in the air, or with some velocity on the ground.
Dive transitions to slide when the player touches the ground in dive state.
Slide will either slow to a stop and transition to a standup into idle, or transition to roll state while 
maintaining current velocity by pressing X.
Roll state has the same movement properties as dive, but will automatically transition to regular fall state 
when the animation completes.

<b>Ground vs Air Dive:</b>
Dive in ground state has static properties, and will always give the player the same initial velocity.
Dive in air state has additive properties.  If the jump button was not held when an air dive was input, any 
positive vertical velocity is immediately zeroed. If the jump button was held upon the initial dive input, 
the dive's vertical movement will mirror that of a regular jump (continual raise to a max height or release 
of the button).
