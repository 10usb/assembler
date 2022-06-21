# Puzzel Game
This is a currently working example for an 8-bit cpu I designed and build in [Turing Complete](https://turingcomplete.game/). I plan to convert the design to real hardware somewere in the future. Which can change some values used for displaying and user input.

The game written in the assembly is a 8x8 map where you control a single unit. With this unit you can move boxes around to there destinations marked by dots. And ofcoure thare are wall which turn it in to a maze. The maps aren't includes as they might have copyright on them. Years ago around 2006, I'd made a clone of a keychain game and copied the maps from the tiny screen to a binary file.

The format I designd for the file to store the map data is described in the table below

| offset | size  | description |
|--------|-------|-------------|
|      0 |     2 | PM          |
|      2 |     1 | version     |
|      3 |     2 | limit       |
|      5 |    64 | map data    |

Saldy back then I didn't knew the benefit of aligning bit's in values so the table below is a value for each valid tile data. So in the code I translate it to a more useful format.
| value | description |
|-------|-------------|
|     0 | Wall        |
|     1 | Box         |
|     2 | Dot         |
|     3 | Unit        |
|     4 | Empty       |
|     5 | Box & Dot   |
|     6 | Unit & Dot  |