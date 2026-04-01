# BTL3 - Flappy Bird Obstacle Course

A challenging 2D flyer game where players navigate a bird through procedurally spawned obstacles while collecting coins in a beautiful parallax-scrolling winter environment.

---

## Game Controls

### Thrust/Rise
- **Spacebar** – Hold to thrust the bird upward
- **Left Mouse Button** – Hold to thrust the bird upward
- **Touch Screen** – Touch to thrust the bird upward

### Menus
- **Restart** – Click the restart button on the Game Over screen to play again
- **Start Game** – Click the start button on the main menu

### Movement Mechanics
- **Falling**: The bird continuously falls due to gravity when no input is active
- **Rising**: Holding any thrust input applies upward force (movement speed capped at a maximum rise speed)
- **Free Fall**: Maximum fall speed is capped to prevent uncontrolled descents

---

## Parallax Scrolling System

The game features a sophisticated infinite parallax scrolling background that creates a sense of continuous forward movement.

### System Architecture

#### **ParallaxLoop.cs** (Background Scrolling)
   - Handles infinite scrolling of the scenic background sprite layer
   - Creates and manages alternating mirrored segments as the player moves
   - Each segment is spawned dynamically when the rightmost segment reaches a threshold
   - Supports optional speed increase over time for difficulty scaling
   - **Key Features:**
     - Dynamic segment cloning with sprite mirroring
     - Sprite rendering optimization
     - Configurable speed, initial segments, and max speed
     - Smooth infinite loop with seamless transitions

#### **How It Works**

1. **Initialization Phase**
   - Stores the width of the source sprite using bounds
   - Creates initial segments (typically 4) positioned horizontally adjacent to each other
   - Sets up alternating mirroring pattern (every other segment is flipped on X-axis)

2. **Movement Phase (Every Frame)**
   - Each segment moves left by `speed × deltaTime` units
   - Checks if a segment has passed the left boundary (destruction threshold)
   - Automatically spawns a new segment at the right edge when needed

3. **Segment Replacement Strategy**
   - When the leftmost segment exits the viewport, it's marked for destruction after `extraLifeSeconds`
   - A new segment is immediately created at the right edge
   - Mirroring alternates to create a seamless tiling pattern
   - Destroyed segments are removed from memory to maintain performance

4. **Infinite Loop Behavior**
   - The system maintains a list of active segments
   - Only segments within view/buffer are rendered and updated
   - Old segments are garbage collected, new ones are instantiated
   - Creates the illusion of infinite horizontal scrolling

#### **Technical Parameters**
- `speed`: Units per second the parallax layer scrolls
- `initialSegments`: Number of segments spawned at start (determines buffer)
- `extraLifeSeconds`: Grace period before destroying off-screen segments
- `increaseSpeed` (ParallaxLoop only): Enables progressive difficulty scaling
- `speedIncreasePerSecond`: How fast the parallax accelerates
- `maxSpeed`: Maximum scrolling speed cap

#### **Performance Optimization**
- Segments outside the viewport are destroyed to reduce draw calls
- Only active segments are updated in the Update loop
- Null checks prevent errors from destroyed objects

---

## Asset Attribution

### Visual Assets

**Glacial Mountains Parallax Background**
- Creator: vnitti
- Source: https://vnitti.itch.io/glacial-mountains-parallax-background
- License: As specified by creator
- Usage: Multi-layer background scenery and parallax scrolling

**Blue Bird Sprite**
- Source: https://inmenus.itch.io/bird
- License: CC0 (Public Domain)
- Usage: Player character sprite and animations

**Crown/Moving Obstacle Asset**
- Creator: aekashics (librarium-static-batch-megapack)
- Source: http://www.akashics.moe/

**Collectable Coin**
- Creator: laredgames
- Source: https://laredgames.itch.io/gems-coins-free
- Usage: In-game collectible coins for scoring

**Statues**
- Creator: odgardian
- Source: https://odgardian.itch.io/medieval-house-set-statues
- Usage: Environmental obstacles and decorative elements

### Audio & Other Assets
- TextMesh Pro: Unity Technologies
- Any other assets used are subject to their respective licenses

---

## Game Architecture Overview

### Core Scripts

- **BirdController.cs**: Handles player movement, collision detection, and death mechanics
- **GameManager.cs**: Manages game state, menus, scoring, and scene management
- **SpawnManager.cs**: Procedurally spawns obstacles (pillars) and collectibles at designated spawn points
- **ParallaxLoop.cs**: Infinite scrolling background parallax system
- **Pillar.cs**: Individual obstacle behavior and coin spawning
- **ScoreUI.cs**: Displays player score and high score
- **GhostTrail.cs**: Visual effect for bird movement
- **SpriteFrameAnimator.cs**: Handles sprite-based animation

### Game Flow

1. **Start**: Main menu appears, game time is paused
2. **Play**: Player holds thrust inputs to navigate through obstacles
3. **Collision**: Hitting an obstacle triggers death animation, 5-second delay, then Game Over
4. **Collection**: Collecting coins increases the score
5. **Game Over**: High score is saved, restart button available
6. **Restart**: All managers reset state, scene reloads with fresh spawn patterns

---

## Development Notes

- Built with Unity
- Uses 2D physics and sprite-based rendering
- Procedural obstacle generation with randomization
- Persistent high score tracking via PlayerPrefs

---

*Last Updated: April 2026*
