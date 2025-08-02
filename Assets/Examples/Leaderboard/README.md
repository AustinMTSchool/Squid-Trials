# Leaderboard Example

## Description
This world uses both PlayerData and PlayerObjects to count and persist the number of times a given player has jumped in the world.

## Inspector Parameters

* `string` **Leaderboard Key** - The key name for the value you want to use from PlayerData.
* `int` **Amount of Decimals** - The amount of decimals to show in the leaderboard. 0 will show no decimals. 1 will show 1 decimal. 2 will show 2 decimals and so on.
* `LeaderboardSlot` **Leaderboard Slot Prefab** - The prefab for the leaderboard slot. This is the UI element that will show the player's score.
* `Rect Transform` **Leaderboard Parent** - The parent transform for the leaderboard slots. This is where the leaderboard slots will be instantiated. It should have vertical layout group.


---
## How to Use This Example
1. Add the `Leaderboard` prefab to your scene.
2. Change the `LeaderboardKey` to the PlayerData variable key you want to use for the leaderboard.
3. Run Build & Test!