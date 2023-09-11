# RollPitchLIVPlugin

This is a camera plugin for LIV that mimics the roll and pitch of the player's head.
The camera's spawn location is determined by the rotation of your head.

This plugin will spawn a folder and JSON settings file on first activation, found where you put the DLL.
Settings options:
\nfov: Field of view
\ndistance: distance it spawns behind you
\nspeed: doesn't do anything.
\nrollSensitivity: how much the camera matches your head roll rotation
\npitchSensitivity: how much the camera matches your head pitch rotation
\nsmoothingFactor: how much interpolation on rotation. 0 is no rotation, 1 is perfect match.
