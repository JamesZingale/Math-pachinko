# Pinball Launcher Implementation Guide

This guide will help you set up the enhanced PinballLauncher in your 3D math pinball game.

## 1. Setting Up the Launcher GameObject

1. Create a new empty GameObject in your scene and name it "PinballLauncher"
2. Position it at the bottom of your play area where you want the ball to launch from
3. Add the `PinballLauncher.cs` script to this GameObject

## 2. Creating the Launcher Visual

1. Create a 3D model for your launcher (or use a simple cylinder/cube for testing)
2. Make it a child of the PinballLauncher GameObject
3. Position it so that its forward direction points in the direction balls will launch
4. Assign this child object to the `launcherVisual` field in the inspector

## 3. Setting Up the Launch Point

1. Create an empty GameObject as a child of the PinballLauncher
2. Name it "LaunchPoint"
3. Position it at the point where balls should spawn from
4. Assign this to the `launchPoint` field in the inspector

## 4. Creating the Trajectory Line

1. Add a LineRenderer component to the PinballLauncher GameObject
2. Configure the LineRenderer:
   - Set Width to around 0.1
   - Create a material with a transparent shader (e.g., "Particles/Standard Unlit")
   - Set the material color to something visible (e.g., white with alpha 0.5)
   - Assign this material to the LineRenderer
3. Create a Gradient for the trajectory:
   - Start with a bright color (e.g., white)
   - End with a transparent color (e.g., white with alpha 0)
4. Assign this gradient to the `trajectoryGradient` field in the inspector

## 5. Setting Up the Ball Prefab

1. Make sure your ball prefab has:
   - A Sphere Collider
   - A Rigidbody component
   - Appropriate physics material (bouncy)
2. Assign this prefab to the `ballPrefab` field in the inspector

## 6. Setting Up Input Layers

1. Create a new Layer called "AimPlane"
2. Create a large invisible plane in your scene for mouse/touch aiming
3. Assign it to the "AimPlane" layer
4. In the PinballLauncher component, set the `aimPlane` field to include only the "AimPlane" layer

## 7. Adding UI Elements (Optional)

For the power slider:
1. Create a UI Slider element in your canvas
2. Position it at the bottom of the screen
3. Assign it to the `powerSlider` field

For the aim indicator:
1. Create a UI Image element in your canvas
2. Make it a small arrow or dot
3. Position it at the bottom of the screen
4. Assign it to the `aimIndicator` field

## 8. Adding Effects (Optional)

For launch particles:
1. Add a ParticleSystem component as a child of the launcher
2. Configure it to emit a burst of particles
3. Disable "Play On Awake"
4. Assign it to the `launchParticles` field

For launch sound:
1. Add an AudioSource component to the launcher
2. Assign a sound effect to it
3. Disable "Play On Awake"
4. Assign it to the `launchSound` field

## 9. Configuring Launcher Settings

Adjust these settings in the inspector to match your game's feel:
- `minPower` and `maxPower`: Control how hard the ball can be launched
- `powerChargeRate`: How quickly power increases when charging
- `aimSpeed`: How fast the launcher rotates with keyboard controls
- `minAngle` and `maxAngle`: Limit the rotation range of the launcher
- `trajectoryPoints`: More points = smoother trajectory line
- `trajectoryTimeStep`: Smaller = more accurate physics prediction
- `maxTrajectoryTime`: How far ahead to predict the trajectory
- `launcherStretchFactor`: How much the launcher visual compresses when charging

## 10. Testing

1. Enter Play mode
2. Test all input methods:
   - Keyboard: Arrow keys to aim, Space to charge and launch
   - Mouse: Point to aim, click and hold to charge, release to launch
   - Touch (on mobile): Tap and drag to aim, release to launch
3. Adjust settings as needed for the best feel

## Troubleshooting

- If the trajectory line doesn't appear, check that the LineRenderer is properly configured
- If mouse/touch aiming doesn't work, verify the AimPlane layer is set up correctly
- If the ball doesn't launch with enough force, increase the `maxPower` value
- If the launcher visual doesn't animate, check that it's assigned in the inspector
