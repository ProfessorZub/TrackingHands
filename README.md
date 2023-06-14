# TrackingHands
Hand Tracking for the Meta Quest 2.

In this project, we're creating a spell casting system in Unity for Virtual Reality applications, specifically designed for the Oculus Quest 2.

![Magic Spells](./TrackingHands-13.jpg)

## Features

- **Hand Gestures**: Utilizes Oculus Integration SDK to detect hand gestures. Gestures are transalted into binary numerical representation.
- **Interactive Spell Effects**: Uses the hand gesture number to generate a unique spell effect. The effect is created using Unity's LineRenderer component and a custom Material, producing a glowing star-like pattern.
- **Real-time Visual Feedback**: The spell effect updates in real time, responding to user's hand gestures. This creates a highly engaging and immersive VR experience.
- **Post-Processing Effects**: Implements Unity's post-processing stack to add a bloom effect, enhancing the visual impact of the spell.
- **Heads Up Display for logging**: Implements a HUD to log debug messages directly to the VR environemt.

## Setup

To get this project up and running, you'll need to:

1. Clone the repository: `git clone https://github.com/yourgithubusername/yourrepositoryname.git`
2. Open the project in Unity.
3. Make sure you have the Oculus Integration package installed. You can download it from [Unity's Asset Store](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022).

## How to Use

Ensure that your Oculus Quest 2 device is connected to a USB 3.0 (or higher) port on your PC, via an appropriate USB-C cable. Launch Quest Link from the Quick Settings bar on your Quest 2 home screen. Press the Oculus button on your right controller if you can’t see the Quick Settings bar. Once Quest Link has loaded, press Play on the Unity editor.

Open and close your hands several times as soon as the app loads to calibrate your hands. Once calibrated, you should see n-pointed stars floating in front of your hands. Move and rotate your hands to see how the shapes follow your movements. Experiment with different hand gestures and compare the hand sum logged in the HUD with the number of points in your stars.

## Step-by-step Guide to make this project from scratch

This repository contains the final project that can be achieved by following the included guide. It is strongly recommended to follow the guide in order to develop your own skills.

[Step-by-Step Guide](./VR-Step-by-Step%20Guide.pdf)

## License

GNU

