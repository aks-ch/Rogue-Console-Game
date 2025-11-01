# Rogue Console Game (Under Construction 🚧)
This game is a basic console implementation of the game "Rogue" and is used as a learning experience for me in C#.

# Information
The game offers 2 options to play:
1. Random Settings
   - Seed is randomly chosen.
2. Custom Settings
   - Seed can be manually chosen.
   - Game size can be manually chosen.
   - Enemy count can be manually chosen.

The stats of the enemy can be custom defined in Data/Enemies.json using the following format:
```json
[
  { 
    "Symbol": "S",
    "MaxHealth": 3,
    "Strength": 0.2
  },
  {
    "Symbol": "K",
    "MaxHealth": 5,
    "Strength": 0.5
  },
  {
    "Symbol": "Q",
    "MaxHealth": 8,
    "Strength": 1
  },
  {
    // another enemy
  }
]
```
`Note: Comments are not valid in json`

There must be a minimum of `1` enemy defined.
There is no hardcore limit on the amount of enemies that can be defined.
Each enemy declaration must be complete with the three fields as shown.
The `Symbol` field may not include more than one character.
You can declare negative numbers for `MaxHealth` and `Strength` thought it can lead to unintended behavior.

Unlike the enemy stats, the player stats cannot be modified as of now.

The following features use the seed for game initialization where applicable:
- Game size
- Enemy count
- Type of Enemy
- Player location on spawn
- Enemy location on spawn

A shorthand version of this guide can be viewed in the menu screen.

# Controls
Arrow keys for movement in game.

In order to attack an enemy, you have to "move into" the enemy.
The enemy will attack the player if it tries to move into a space that is occupied by the player.

# Defaults
Player:
- `Symbol - "P"`
- `MaxHealth - 10`
- `Strength - 1`

Enemies:
```json
[
  {
    "Symbol": "S",
    "MaxHealth": 3,
    "Strength": 0.2
  },
  {
    "Symbol": "K",
    "MaxHealth": 5,
    "Strength": 0.5
  },
  {
  "Symbol": "Q",
    "MaxHealth": 8,
    "Strength": 1
  }
]
```