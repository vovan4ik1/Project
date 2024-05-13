# GameChekers
## Introduction

This project is a game Chekers on Console Application. The project applies various programming principles, refactoring methodologies, and design patterns to craft code that is tidy, modular, and readily expandable.

## Functionality
1. **Choice regime** This game have two regime singleplayer and multiplayer.Class responsible for this [`Choice Regime`](./Project2/GameUI.cs#153).
2. **Choice board** User can choice size board , three size (small, medium, big). Class responsible for this [`Choice board`](./Project2/GameUI.cs#127).
3. **Create nikname** User can create nikname. Сlasses responsible for this [`Create nikname`](./Project2/Player.cs).
4. **Check Scores** User can check your score . Class responsible for this[`Check scores`](./Project2/Game.cs#288).
   
## Run process locally
1. Clone this repository.
2. Open the project in Visual Studio.
3. Run the project by pressing `Ctrl + F5`.
   
   
## This project uses the following design patterns:
1.**State Patern**  The Checker class has a method InsertPossibleCorrectMovesToList, which selects the valid moves for the current Checker object depending on its type and position on the game board. 
This method is called for each Checker object when a player makes a move, and it determines which moves are available for that particular Checker based on its type.
In this code, several private methods (insertSoldierRegularMovesToList, insertKingRegularMovesToList, insertSoldierEatingMovesToList, insertKingEatingMovesToList) are used, 
each corresponding to a specific state of the Checker object.[`State Patern`](./Project2/Checker.cs)
2.**Template Method** It includes all the necessary functions for user interaction during the game, such as choosing the board size, game mode, getting player names, inputting moves, etc.
The main responsibilities of this code include:
Interacting with the user to choose the board size and game mode.
Getting player names.
Displaying the state of the board during the game.
Handling user input and verifying its correctness.
Notifying the user about the end of the round and providing the option to start a new round.[`Template Patern`](./Project2/GameUI.cs)
3. **Proxy** The main functions of this code include:
Storing move information: The Move struct stores information about the starting and ending positions of a move.
Checking if a move is an eating move: The CheckIfEatingMove() method determines if a move is an eating move, which is used to determine the possibility of making additional moves in the game.
Converting a string to a move object: The QuitRoundOrConvertStringToMove() method converts a string representing a move in the format "start_position>end_position" into a move object. This allows for processing 
user input and performing the corresponding actions in the game.[`Proxy Patern`](./Project2/Move.cs)
