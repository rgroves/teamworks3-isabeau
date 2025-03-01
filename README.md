# teamworks3-isabeau
[TEAMWORKS](https://www.udacityvr.com/teamworks) - Udacity VR Group Projects
## Team Ladyhawke's Project (Codename: Isabeau)

# Recap of Game Design Decisions
- This game will be a pattern-based memory skill game
- The platform we are developing for is Google Cardboard

## Things We Stil Need To Do
- We need to pick a game name. In the meantime we are using project code name: Isabeau :) (Note: Isabeau was the lead female character in the movie [Ladyhawke](https://en.wikipedia.org/wiki/Ladyhawke_(film)))
- Agree upon a game story - this will help us with the look and feel of the game and should provide us with a certain direction when creating/finding visual/audio assets
 - One story idea is outlined here: [Doomsday Box](https://udacityvrdeveloper.slack.com/archives/G8501Q007/p1512182934000031)

## Game Mechanics
### Game Play
- At the beginning of the game the player finds themselves in a room where each of the walls and the ceiling have large button like objects embedded in them.
- Each wall of the room is a playable Level of the game
- Each Level will consists of 3 rounds.
- At the start of a round a random pattern will be generated and displayed to the player by activating the button objects in sequence (buttons will light up and play a sound when activated)
- The player must then repeat the pattern by pressing each button in sequence (by targeting and pressing the trigger) being sure to press them in the correct sequence to match the pattern they were shown at the start of the round

### Levels
- Each level will consist of a number of buttons displayed in a "geometric" configuration (*meaning the buttons are positioned in the scene so that if you happened to draw a line from the center of each circle connecting to the next it would form a certain shape*):
 - Level 1 (ceiling): Line - Consists of 2 buttons
 - Level 2 (north wall): Triangle - Consists of 3 buttons
 - Level 3 (east wall): Square - Consists of 4 buttons
 - Level 4 (south wall): Pentagon - Consists of 5 buttons
 - Level 5 (west wall): Hexagon - Consists of 6 buttons
- In addition to there being more buttons and longer patterns, each level will increase the speed at which the winning pattern is shown to the player; making it more difficult since there is less time to memorize it.

### Level Rounds
- Each level will have 3 rounds
- The number of button activations in the generated pattern will be equal to 2 times the number of buttons in the level plus an additional modifier each round
- Each round will increase the number of button activations that will need to be memorized the player

### Examples
- In Level 1 (with 2 buttons):
 - Round One's pattern/sequence will consist of three (2x2-1) button activations
 - Round Two's pattern/sequence will consist of four (2x2+0) button activations
 - Round Three's pattern/sequence will consist of five (2x2+1) button activations
- In Level 2 (with 3 buttons):
 - Round One's pattern/sequence will consist of five (2x3-1) button activations
 - Round Two's pattern/sequence will consist of six (2x3+0) button activations
 - Round Three's pattern/sequence will consist of seven (2x3+1) button activations
- In Level 3 (with 4 buttons):
 - Round One's pattern/sequence will consist of seven (2x4-1) button activations
 - Round Two's pattern/sequence will consist of eight (2x4+0) button activations
 - Round Three's pattern/sequence will consist of nine (2x4+1) button activations
- Etc.

### Assets
- 3 Scenes
 - Intro/Title Scene
 - Main Gameplay Scene 
 - Outro/Credits Scene
- A skybox (optional if we are keeping the play area an enclosed space with no view to the outside)
- A texture for the walls/ceiling/floor of the play area
- 6 unique objects & textures to be used as the push buttons
- Music
 - Intro/Title Screen music
 - Level Music - 5 unique looping clips we can use for each level
 - Outro/Credit Screen music
- Sound Effects
 - 6 "activation sounds" that are unique to each push button
 - Failure sound effect (for if player does not match the pattern)
 - Win sound effect (for if player matches the pattern)
 - Invalid action sound (for when player hits a button on an inactive wall/level)

### Enhancements
If time allows for feature enhancements here are some ideas:

- Create a cheat layer for testing (or for having a cheat mode in game)
 - The layer would reveal the next color in sequence that should be pressed
 - This would help during testing so we can easily test out the success/win condition of each level without having to memorize or write down the pattern while testing
- Instead of being statically positioned when the level starts, the button objects can be animated to fly into view and land into the current level's shape pattern maybe with a unique sound effect for this animation
- If player gets the pattern wrong the button objects can be animated to fall away from the scene and a "fail" sound effect play
- Player can get 3 lives/misses so they can continue guessing the pattern before losing the level
- It would be cool for the buttons to animate with a glow/spark (possibly using the particle system) to give them a great visual effect

# Scripts
## GameState.cs
This script manages the game state and is the main driver for the game. It should be attached as a component to an empty game object named GameState in the Project Hierarchy.

This script exposes a reference named Push Button Surfaces in the Inspector. It is an array of the game objects that represent the surfaces of the game play areas that contain the push buttons for a level. You'll need to set the array size to the number of play surfaces in the scene and then set the array entries to the game objects that represent the playable surfaces. The order which you add the game objects to the array in the Inspector will be used as the Level order (first object in the array is Level 1, second object is Level 2, etc.).

## PushButtonSurface.cs
- This script should be added as a component to each of the playable surfaces in the game.
- This script expects the game objects that represent the "push buttons" (that the player will interact with) to be child objects of each surface game object. 
- It also expects the button game objects to be tagged with the tag PushButton. This is so that the script can identify the game objects that are to be used as the buttons.

# Scenes
## PlayAreaExample
- This scene is currently setup with just one plane and five button objects.
- You can see there is a GameState object that has the GameState script component added to it.
- The plane has the PushButtonSurface script attached to it.
- You can play with this scene to add/remove buttons or add additional surfaces with more buttons. Just be sure to adjust the GameState settings and add the appropriate script to the additional surfaces.
