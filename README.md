<h1 align="center">MyBuddy-Unity</h1>

<p align="center">
  <strong>The Unity avatar runtime for MyBuddy, responsible for rendering, animation, audio playback, and lip-sync, and exported for Android embedding.</strong>
</p>

<p align="center">
  <a href="#overview">Overview</a> &bull;
  <a href="#related-repositories">Related Repositories</a> &bull;
  <a href="#features">Features</a> &bull;
  <a href="#project-layout">Project Layout</a> &bull;
  <a href="#requirements">Requirements</a> &bull;
  <a href="#opening-the-project">Opening the Project</a> &bull;
  <a href="#running-in-the-editor">Running in the Editor</a> &bull;
  <a href="#building-as-an-android-library">Building as an Android Library</a> &bull;
  <a href="#integration-contracts">Integration Contracts</a> &bull;
  <a href="#troubleshooting">Troubleshooting</a>
</p>

---

## Overview

MyBuddy-Unity is the companion Unity project used by the MyBuddy Flutter application. It is not the source of truth for product logic. Its job is to:

- render the avatar and scene
- play synthesized speech audio
- drive lip-sync from audio playback
- run animation transitions triggered by Flutter
- export cleanly as an Android library that can be embedded into the Flutter host app

The current project targets Unity `2022.3.62f3` and uses the Universal Render Pipeline.

---

## Related Repositories

This Unity project is part of the MyBuddy product surface:

- **Flutter app**: https://github.com/newnonsick/MyBuddy
- **Unity avatar runtime**: https://github.com/newnonsick/MyBuddy-Unity
- **Model catalogs**: https://github.com/newnonsick/MyBuddy-cfg

This repository exports into the Flutter app's Android project, where the resulting `unityLibrary` module is consumed by the host application.

---

## Features

- real-time avatar rendering inside an embedded Unity scene
- audio-driven lip-sync using blend shapes
- animator-triggered body animations
- Flutter-triggered speech playback and stop commands
- Android library export workflow compatible with Flutter host integration
- URP-based rendering pipeline with a fixed 60 FPS limiter in scene runtime

---

## Project Layout

```text
Assets/
  Audio/            Runtime audio assets
  Materials/        Scene and avatar materials
  Scenes/           Unity scenes, including the main sample scene
  Scripts/          Bridge, animation, lip-sync, and frame-rate control
  Settings/         Render pipeline and project assets

Packages/
  manifest.json     Unity package dependencies

ProjectSettings/
  ProjectVersion.txt
  ProjectSettings.asset
  EditorBuildSettings.asset
```

Important scripts:

- `UnityBridge.cs` - receives Flutter-driven commands for speech and animation
- `BodyController.cs` - keeps speaking state aligned with audio playback
- `LipSyncController.cs` - drives blend shapes from output audio intensity
- `FrameRateLimiter.cs` - sets target frame rate to 60 FPS

---

## Requirements

### Unity and Modules

- Unity Editor `2022.3.62f3`
- Android Build Support for the same Unity version
- Android SDK, NDK, and OpenJDK installed through Unity Hub or configured externally

### Host Integration

- MyBuddy Flutter app repository checked out separately
- Android Studio or Gradle tooling available for verifying the embedded export

---

## Opening the Project

1. Install Unity `2022.3.62f3` through Unity Hub.
2. Clone the repository.
3. Open the project folder in Unity Hub.
4. Let Unity import packages and regenerate the Library folder if needed.
5. Confirm the active scene is `Assets/Scenes/SampleScene.unity`.

---

## Running in the Editor

Use Play Mode to validate scene composition, animation playback, and lip-sync behavior.

Editor helpers built into `UnityBridge.cs` allow manual checks:

- `R`, `F`, `G` trigger sample animations
- `T` attempts to play a hardcoded WAV path for local testing
- `S` stops speaking

Before relying on editor hotkeys, update the hardcoded test audio path to a valid local WAV file on your machine.

---

## Building as an Android Library

This is the most important workflow for production integration.

### 1. Switch to Android

In Unity:

1. Open `File > Build Settings`.
2. Select `Android`.
3. Click `Switch Platform` if the project is not already on Android.

### 2. Verify Player Settings

Review `Project Settings > Player` before exporting:

- product name is currently `AvatarUnity`
- project version is currently `0.1.0`
- current Android application identifier is still template-derived and should be normalized for serious distribution workflows
- Android target architectures are enabled for `armeabi-v7a`, `arm64-v8a`, `x86`, and `x86_64`

### 3. Confirm the Scene List

The build currently includes:

- `Assets/Scenes/SampleScene.unity`

If you add scenes, make sure the intended runtime scene remains included in Build Settings.

### 4. Export the Android Project

In Unity:

1. Open `File > Build Settings`.
2. Ensure `Export Project` is enabled.
3. Choose a clean output directory outside this repository or in a temporary export folder.
4. Click `Build`.

Unity will export an Android Gradle project containing modules such as `unityLibrary` and `launcher`.

### 5. Sync the Export into the Flutter Host App

Copy the exported modules into the Flutter repository:

- copy exported `unityLibrary/` into `MyBuddy/android/unityLibrary/`
- copy exported `launcher/` into `MyBuddy/android/launcher/` if you keep the launcher module in sync

Then verify the Flutter Android project still contains:

- `include(":unityLibrary")` in `android/settings.gradle.kts`
- `implementation(project(":unityLibrary"))` in `android/app/build.gradle.kts`

### 6. Build the Host App

From the Flutter repository:

```bash
flutter build apk
```

Or verify the Android side directly:

```bash
cd android
./gradlew app:assembleDebug
```

On Windows:

```powershell
.\gradlew.bat app:assembleDebug
```

### 7. IL2CPP Notes

The embedded `unityLibrary` Gradle build in the Flutter app is configured to auto-generate IL2CPP libraries if native outputs are missing.

You can force a rebuild from the Flutter Android project with:

```bash
./gradlew :unityLibrary:assembleRelease -PunityRebuildIl2Cpp=true
```

On Windows:

```powershell
.\gradlew.bat :unityLibrary:assembleRelease -PunityRebuildIl2Cpp=true
```

---

## Integration Contracts

The Flutter app currently expects these runtime behaviors:

### Flutter to Unity Channel

- channel name: `unity_bridge`
- Android host forwards Flutter commands into Unity with `UnityPlayer.UnitySendMessage(...)`

### Expected Unity GameObject

- default GameObject name: `UnityBridge`

### Expected Public Methods on the Unity Side

- `Speak(string audioPath)`
- `StopSpeak()`
- `PlayAnimation(string animIndexStr)`

If you rename the GameObject, scripts, or method signatures, you must update the Flutter and Android host integrations together.

---

## Development Notes

- keep Unity focused on presentation and playback, not product business logic
- avoid introducing app-state ownership into Unity scripts
- keep animation triggers stable and explicit
- document any new bridge method or payload expected by the Flutter app

---

## Troubleshooting

### Exported library does not build in Flutter

- confirm the export was done for Android with `Export Project` enabled
- recopy both `unityLibrary` and `launcher` if your exported structure changed
- verify Gradle and Unity versions remain compatible with the host app

### Avatar renders but does not animate

- confirm the `UnityBridge` GameObject still exists in the active scene
- verify animator parameters expected by scripts still exist
- test `PlayAnimation` manually in the editor before re-exporting

### Lip-sync does not move the face

- verify the `SkinnedMeshRenderer` and blend shape names still match the controller expectations
- confirm audio is actually playing through the target `AudioSource`

### Android native libraries are missing

- rebuild the export
- run the host Gradle build with `-PunityRebuildIl2Cpp=true`
- verify Unity Android Build Support and IL2CPP tooling are installed

---

## License

This project is licensed under the Apache License 2.0. See [LICENSE](LICENSE) for details.
