# RollPitchLIVPlugin

This is a camera plugin for LIV that mimics the roll and pitch of the player's head.
The camera's spawn location is determined by the rotation of your head.

This plugin will spawn a folder and JSON settings file on first activation, found where you put the DLL.
Settings options:
fov: Field of view
distance: distance it spawns behind you
speed: doesn't do anything.
rollSensitivity: how much the camera matches your head roll rotation
pitchSensitivity: how much the camera matches your head pitch rotation
smoothingFactor: how much interpolation on rotation. 0 is no rotation, 1 is perfect match.
