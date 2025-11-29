**Made by _Aditya Naik_ (22BIT0037)**

**Video recording:https://drive.google.com/file/d/1wv_bScT80pZt5OAYYN1VV8NFHd1aNdoz/view?usp=sharing** 
# Doofus Adventure – Hitwicket Game Assignment

> A Unity game assignment made for **Hitwicket** where Doofus must walk across disappearing platforms and survive long enough to reach **50 pulpits**!

## Controls

| Key | Action |
|-----|-------|
| **W** / **↑ Arrow** | Move Forward |
| **S** / **↓ Arrow** | Move Backward |
| **A** / **← Arrow** | Move Left |
| **D** / **→ Arrow** | Move Right |
| **Space** | Jump |

## Overview

Doofus is an explorer cube that walks across **pulpits (platforms)** that appear and disappear over time.  
The catch? Only **two pulpits can exist at a time**, and each has its **own destruction timer**.

The game reads **timing & speed values** live from a **JSON file hosted on Amazon S3**, as requested in the assignment.


## Features Implemented

| Core Feature | Status |
|--------------|--------|
| Read JSON from S3 (UnityWebRequest) | ✔ Done |
| Doofus movement + jump | ✔ Rigidbody-based |
| Max 2 pulpits active at a time | ✔ Implemented |
| Pulpits spawn adjacent, not overlapping | ✔ Done |
| Random pulpit lifetime (from JSON) | ✔ Done |
| Score increases only once per pulpit | ✔ Fixed with HashSet |
| Win Condition (Score ≥ 50) | ✔ Added |
| Lose Condition (fall/destroy) | ✔ Added |
| Timer shown on each pulpit | ✔ TextMeshPro |
| Footstep audio (looped + fading) | ✔ Done |
| Jump Sound | ✔ Done |
| Background music (persists across scenes) | ✔ Done |
| Death Screen (Dark Souls style) | ✔ YOU DIED screen |
| Win Screen | ✔ YOU WIN screen |
| High Score (saved with PlayerPrefs) | ✔ Shown in Main Menu |
| Main Menu + Quit Button | ✔ Implemented |
| Spike grid background | ✔ Procedural prefabs |


## JSON Used (Loaded from S3)

```json
{
   "player_data" : { "speed" : 3 },
   "pulpit_data" : {
     "min_pulpit_destroy_time" : 4,
     "max_pulpit_destroy_time" : 5,
     "pulpit_spawn_time" : 2.5
   }
}
