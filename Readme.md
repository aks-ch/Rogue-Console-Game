# Rogue Console Game
This game is a basic console implementation of the game "Rogue" and is used as a learning experience for me in C#.

# Gameplay
The game has multiple maps (tied to the difficulty you choose).
To unlock a hallway to any given deeper map, you must defeat all enemies on the map that has said hallway.
Some hallways may even require you to collect `Keys` which look like `!` (colored yellow) in game.
The higher the difficulty, the more the chance of a hallway requiring a key.
To win the game, you must find and collect the item `%` (colored magenta).
Items (including keys) are hidden by default and can be discovered through walls in a very small range.
The empty spaces that have been "scanned" change their color to reflect being scanned.

Arrow keys for movement in game.

Enemies spawn on each map. Deep maps have less enemies but said enemies will be stronger.
In order to attack an enemy, you have to "move into" the enemy.
The enemy will attempt to attack the player when the player is in within a defined range.
The enemy will attack the player if it tries to move into a space that is occupied by the player.

To heal, the player must not interact with an enemy for `HealCooldown` turns.
After the `HealCooldown` reaches `0`, the player will heal `HealFactor` health every turn.
Interacting with an enemy resets the cooldown.

The player changes color depending on player health status.
- `DarkBlue`: Health is full.
- `DarkGreen`: Health is less than full.
- `DarkRed`: Health is less than 50% of maximum.
- `DarkYellow`: Player is healing.

A shorthand version of this guide can be viewed in the menu screen.

# Additional Information

Enemy stats can be custom defined in `Data/Enemies.json` using the following format:
```json
[
  { 
    "Symbol": "S",
    "MaxHealth": 3,
    "Strength": 0.2, 
    "Points": 2
  },
  {
    // another enemy
  }
]
```
`Note: Comments are not valid in json`

These are only base stats which are modified (`MaxHealth` & `Strength`) in deeper maps based on difficulty.
The factor for increased stats is as follows `baseStat * (1 + ((MapDepth * Difficulty) / 10))`.

There must be a minimum of `1` enemy defined.
There is no hardcore limit on the amount of enemies that can be defined.
Each enemy declaration must be complete with the four fields as shown.

- The `Symbol` field may not include more than one character.
- The `Points` value dictates the odds of the enemy spawning relative to other enemies.
**`Points` must be a positive integer.**
- The `MaxHealth` value must be a positive integer (for players as well).

For the rest, you *can* declare negative numbers for some number inputs, though it can lead to unintended behavior.
This is deliberate in order to allow freedom to play "crazy".

The player stats can also be modified using `Data/Player.json`.
However, there can only be one player so the declaration can only contain one object.
I suggest you to modify the default values only to modify player stats.

Any number that is represented as an integer in [defaults](#defaults) **must** be an integer.
Decimals, where allowed, will be rounded to single decimal point.

# Defaults
Player:
```json
{
  "Symbol": "P",
  "MaxHealth": 10,
  "Strength": 1,
  "HealCooldown": 5,
  "HealFactor": 0.2
}
```

Enemies:
```json
[
  {
    "Symbol": "S",
    "MaxHealth": 3,
    "Strength": 0.2,
    "Points": 2
  },
  {
    "Symbol": "K",
    "MaxHealth": 5,
    "Strength": 0.5,
    "Points": 3
  },
  {
    "Symbol": "Q",
    "MaxHealth": 8,
    "Strength": 1,
    "Points": 4
  }
]
```