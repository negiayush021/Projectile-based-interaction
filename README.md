# Projectile-Based Interaction
**Web3Task Pvt. Ltd. | Game Developer Intern Assignment | Unity 2D | Android**

---

## 📱 Controls

| Action | Input |
|---|---|
| Aim | Press and hold on the ball |
| Pull | Drag in opposite direction of launch |
| Launch | Release finger / mouse |
| Retry | Tap the Retry button after success or failure |

> The further you pull the slingshot, the more force is applied.
> Dotted trajectory preview shows the ball's predicted path before launch.

---

## 🛠️ Implementation Approach

### Projectile System
The player interacts with a slingshot mechanic. When the player presses on the ball, they can drag it backwards to set both the direction and force of the shot. The pull distance is clamped to a `maxPullDistance` value to prevent excessive force.

On release, the launch force is calculated as:
```
launchDir  = centerPoint.position - pullPosition
force      = launchDir.magnitude × launchForceMultiplier
velocity   = launchDir.normalized × force / ball.mass
```
This mirrors Unity's internal `ForceMode2D.Impulse` behavior (`velocity += force / mass`), ensuring the trajectory preview accurately matches the actual ball path.

### Rubber Band Visual
Two `LineRenderer` components connect the left and right slingshot fork tips to the ball's current pull position. Both bands update every frame during drag, giving a realistic slingshot stretch effect. A squish and stretch scale effect is also applied to the ball sprite based on pull distance:
```
squishX = 1 + stretchAmount × 0.2
squishY = 1 - stretchAmount × 0.1
```

### Trajectory Preview
A `LineRenderer` with a custom dotted HLSL shader displays the predicted arc in real time before launch. The preview uses the standard projectile motion formula:
```
position = startPos + velocity × t + 0.5 × gravity × t²
```
The dotted effect is achieved via a custom shader that tiles a circle shape along the line using UV coordinates, with no texture required.

---

## 🏔️ Surface Interaction Logic

The scene contains two curved ramp setups built using `EdgeCollider2D`:

- **Level 1 (Easy):** Gentle curve with wide angle tolerance. Ball slides smoothly into the goal box on correct shots.
- **Level 2 (Hard):** Steeper curve with narrow angle tolerance. Requires more precise aim and force control.

On `OnCollisionEnter2D`, the ball checks two conditions:
1. **Impact angle** — angle between ball velocity and surface contact normal
2. **Impact speed** — current velocity magnitude at moment of contact

```csharp
float angle = Vector2.Angle(ball_rb.linearVelocity, contactNormal);
float speed = ball_rb.linearVelocity.magnitude;

if (angle < maxAngle && speed < maxSpeed)
    // SUCCESS — ball slides along ramp to goal box
else
    // FAILURE — ball bounces off or breaks
```

If both values are within defined thresholds → ball smoothly follows the surface to the goal.
If either value is out of range → ball bounces off or triggers failure state.

---

## ✨ Motion Feedback Implemented

| Feedback | Implementation |
|---|---|
| **Launch Feedback** | Ball squishes on pull (scale X increases, Y decreases) and snaps back to normal on release |
| **Impact Feedback** | Camera shake on failure using `Mathf.Sin()` wave with smooth fade-out over time |
| **Trajectory Preview** | Dotted arc LineRenderer with custom shader updates in real time while aiming |

---

## ⚠️ Challenges Faced

- **Trajectory mismatch** — Initial trajectory was inaccurate because gravity multiplier was `0.05f` instead of `0.5f` in the projectile motion formula. Fixed by correctly applying `0.5 × gravity × t²`.

- **Ball freezing in Unity 6** — Unity 6 removed `isKinematic` from `Rigidbody2D`. Solved by using `RigidbodyType2D.Static` while dragging (ball moves via `transform.position`) and switching to `RigidbodyType2D.Dynamic` on release.

- **Rubber band positioning** — LineRenderer positions were defaulting to `Vector2(0,0)` because fork tip positions were never assigned at runtime. Fixed by referencing actual `Transform` positions in `Start()` and updating both positions every frame during drag.

- **Camera shake offset** — Initial implementation used `Time.time` inside `Mathf.Sin()` which caused the camera to teleport to wrong world positions. Fixed by using `elapsed` time and always offsetting from `camOriginalPos`.

- **Input compatibility** — Handled both mouse (editor testing) and touch (Android) input using Unity's New Input System with unified pointer helper methods (`PointerPressedThisFrame`, `PointerHeld`, `PointerReleasedThisFrame`).

- **Unwanted Unity folders in Git** — Unity auto-generated `basketball_BackUpThisFolder_ButDontShipItWithYourGame` and `BurstDebugInformation_DoNotShip` folders kept appearing in the repo. Fixed by removing them from git tracking with `git rm -r --cached` and updating `.gitignore`.

---

## 🚀 Improvements Given More Time

- **Physics-based sliding** — Replace angle/speed threshold logic with dynamic `PhysicsMaterial2D` that smoothly transitions the ball between bouncing and sliding states based on impact.
- **Particle effects** — Add particle burst on impact, dust trail while sliding, and shatter effect on failure for better game feel.
- **Scoring system** — Award points based on accuracy of angle and speed at impact, with combo multipliers for consecutive successful shots.
- **More level variations** — Add levels with moving surfaces, multiple ramps, and environmental obstacles.
- **Sound effects** — Add audio feedback for launch, slide, success, and failure states.
- **Adjustable difficulty** — Expose force sensitivity and angle tolerance as difficulty settings selectable from a menu screen.
- **Trajectory improvement** — Account for surface collisions in trajectory preview so the dotted line stops at the ramp instead of passing through it.

---

## 📦 Project Structure

```
Assets/
  ├── Scripts/
  │     ├── ProjectileLauncher.cs   — slingshot input, launch, trajectory preview
  │     ├── BallScript.cs           — collision detection, scoring, camera shake
  │     ├── GameManager.cs          — success/fail/retry game loop
  │     └── CameraShake.cs          — sin-wave camera shake effect
  ├── Scenes/
  │     ├── Level1.unity            — easy ramp setup
  │     └── Level2.unity            — hard ramp setup
  ├── Shaders/
  │     └── DottedTrajectory.shader — custom dotted line shader
  └── Prefabs/
        └── Ball.prefab
```

---

## 🔧 Build Info

- **Engine:** Unity 6
- **Platform:** Android (APK)
- **Input System:** Unity New Input System (Mouse + Touch supported)
- **Minimum Android Version:** Android 8.0+

---

## 📬 Submission

- **GitHub:** https://github.com/negiayush021/Projectile-based-interaction
- **APK:** Included in submission email

---

*Submitted by: Ayush Negi*
*Role: Game Developer Intern*
*Company: Web3Task Pvt. Ltd.*
