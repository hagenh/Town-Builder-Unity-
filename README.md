# 🏰 Unity Town Builder

_A 2D top-down medieval survival RPG where you chop, mine, hunt, explore — and build a thriving town from the wilds._

## 🎮 About the Game

In **Unity Town Builder**, you are a lone settler forging a life in a vast, randomly generated medieval wilderness. Gather resources, survive harsh conditions, meet wandering strangers, and recruit them to automate tasks and expand your settlement.

> 🌲 Chop wood, ⛏️ mine stone, 🐗 hunt wildlife, and 🧭 explore a living, breathing world.

## ✨ Features

- 🧱 **Survival & Resource Gathering**
  - Chop trees, mine rocks, hunt animals — and manage your hunger and health.
  
- 🏘️ **Town Building**
  - Build houses, workshops, and assign jobs to NPCs to create your own medieval town.
  
- 🤝 **NPC Recruitment**
  - Meet strangers in the wilderness. Gain their trust, and bring them into your town as workers or companions.

- 🌍 **Procedural Map Generation**
  - Explore a vast, randomly generated world filled with hidden treasures, dangers, and surprises.

- 🎯 **Automation & Progression**
  - Assign recruited NPCs to tasks like woodcutting or mining, and watch your town grow — even while you're off on adventures.

## 📷 Screenshots

![image](https://github.com/user-attachments/assets/8012a95f-0fc6-428e-a5f2-6ee24dd7cb0b)
![image](https://github.com/user-attachments/assets/d1f24e30-7185-4c71-b157-e95361f26e12)


## 🚧 Current Status

The game is in **active development**. Major systems like resource gathering, procedural world generation, and NPC recruitment are being implemented.

## 🛠️ Built With

- **Unity** (2D URP)
- **C#**
- Custom **tile-based world system**
- Future plans: Dialogue system, questing, inventory & crafting

## 🗨️ Dialogue & Recruitment

A basic dialogue system is now included. Create `Dialogue` scriptable objects with your conversation lines and assign them to NPCs via the `RecruitableNpc` component. Attach `NpcDialogueInteractor` to a trigger around the NPC and the player can press **E** to talk. Press **F** in the example script to increase how much the NPC likes the player.

Once the `like` value reaches the `recruitThreshold`, interacting will recruit the NPC and future conversations will use the `recruitedDialogue` asset instead.

NPCs can gain affection when the player helps them or gives them items. In the example component pressing **F** simulates giving a gift and calls `IncreaseLike(1)`.
