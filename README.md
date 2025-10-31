# 🎮 Fake3D

A 2D endless runner that creates the illusion of depth using layered visuals and perspective-based movement.  
Dodge falling obstacles across three “lanes” and survive as long as possible — all while keeping your HP intact.

---

## 🕹️ Gameplay Overview

The player controls a character running forward in a pseudo-3D environment.  
Obstacles fall down each lane, and the player must **move left, right, or jump** to avoid getting hit.

- **Goal:** Avoid incoming obstacles to survive as long as possible.  
- **Lose Condition:** HP reaches 0 → triggers Game Over.  
- **Perspective:** Entirely 2D with a layered “fake-3D” illusion.

---

## 🎯 Controls

| Action | Key |
|--------|-----|
| Move Left | `A` or `←` |
| Move Right | `D` or `→` |
| Jump | `Space` |

---

## 🩸 Health System

- Player HP gradually regenerates over time.  
- Taking damage from obstacles reduces HP.  
- When HP reaches 0, the **Game Over panel** is displayed.

---

## 🧠 Technical Notes

- The illusion of depth is achieved entirely through **2D layering** and position offsets — no 3D models are used.  
- Movement logic uses `transform.position` updates for precise control.  
- Collision is based on **lane index** and **Y-axis distance**, matching the fake 3D movement.

---

## 🔗 Sample Video

👉 [**Google Drive Link**](https://drive.google.com/file/d/1K1pyzntX5uFtajb7Kh9VTKCNzx5mKZFB/view?usp=sharing)

---

## 🛠️ Made With

- **Unity Engine**  
- **C# (MonoBehaviour)**  
- **TextMeshPro** for UI  
- **Animator** for player transitions

---

## 💬 Credits

Developed by **Iñigo Tapales**  
A minimalist runner that blends 2D simplicity with 3D depth illusions.

