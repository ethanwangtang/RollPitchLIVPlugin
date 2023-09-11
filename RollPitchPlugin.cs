/*
Copyright 2019 LIV inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// User defined settings which will be serialized and deserialized with Newtonsoft Json.Net.
// Only public variables will be serialized.
public class RollPitchPluginSettings : IPluginSettings {
    public float fov = 90f;
    public float distance = 3f;
    public float speed = .5f;
    public float rollSensitivity = 0.75f;
    public float pitchSensitivity = 0.50f;
    public float smoothingFactor = 0.25f;
}

// The class must implement IPluginCameraBehaviour to be recognized by LIV as a plugin.
public class RollPitchPlugin : IPluginCameraBehaviour {

    // Store your settings localy so you can access them.
    RollPitchPluginSettings _settings = new RollPitchPluginSettings();

    // Provide your own settings to store user defined settings .   
    public IPluginSettings settings => _settings;

    // Invoke ApplySettings event when you need to save your settings.
    // Do not invoke event every frame if possible.
    public event EventHandler ApplySettings;

    // ID is used for the camera behaviour identification when the behaviour is selected by the user.
    // It has to be unique so there are no plugin collisions.
    public string ID => "RollPitchPlugin";
    // Readable plugin name "Keep it short".
    public string name => "Roll&Pitch Cam";
    // Author name.
    public string author => "***REMOVED***";
    // Plugin version.
    public string version => "1.0";
    // Localy store the camera helper provided by LIV.
    PluginCameraHelper _helper;
    float _elaspedTime;
    Vector3 lockedPosition, originalForward;
    Quaternion previousFrameRotation;

    // Constructor is called when plugin loads
    public RollPitchPlugin() { }

    // OnActivate function is called when your camera behaviour was selected by the user.
    // The pluginCameraHelper is provided to you to help you with Player/Camera related operations.
    public void OnActivate(PluginCameraHelper helper)
    {
        _helper = helper;

        Transform headTransform = _helper.playerHead;

        //obtaining origional forward/backward axis. Not in use for now
        /*originalForward = headTransform.forward;*/

        //lock the position in based on startup positioning.
        Vector3 rotationVector = headTransform.forward * _settings.distance;
        lockedPosition = headTransform.position - rotationVector;

        //initialize previousFrameRotation to player head
        previousFrameRotation = headTransform.rotation;
    }

    // OnSettingsDeserialized is called only when the user has changed camera profile or when the.
    // last camera profile has been loaded. This overwrites your settings with last data if they exist.
    public void OnSettingsDeserialized()
    {

    }

    // OnFixedUpdate could be called several times per frame. 
    // The delta time is constant and it is ment to be used on robust physics simulations.
    public void OnFixedUpdate()
    {

    }

    // OnUpdate is called once every frame and it is used for moving with the camera so it can be smooth as the framerate.
    // When you are reading other transform positions during OnUpdate it could be possible that the position comes from a previus frame
    // and has not been updated yet. If that is a concern, it is recommended to use OnLateUpdate instead.
    public void OnUpdate()
    {
        _elaspedTime += Time.deltaTime * _settings.speed;
        Transform headTransform = _helper.playerHead;

        //attempting to copy forward/backward motion. Unneccessary for now.
        /*
        Vector3 newPosition;
        newPosition = Vector3.Project(headTransform.position, originalForward);*/

        //camera always points at head position.
        Vector3 newRotation;
        newRotation = headTransform.position - lockedPosition; //gets angle between camera and head.
        newRotation.y += headTransform.forward.y * _settings.pitchSensitivity; //adding slight pitch following

        //acquires rollRotation by taking the up value of the vector3 and multiplying it by the sensitivity.
        Vector3 rollRotation = headTransform.up; //multiplying the whole vector3 doesn't change the angle...
        rollRotation.x = rollRotation.x * _settings.rollSensitivity;
        rollRotation.z = rollRotation.z * _settings.rollSensitivity;

        //compiles pitch/yaw in newRotation and roll in rollRotation into Quaternion.
        Quaternion targetCameraRotation = Quaternion.LookRotation(newRotation, rollRotation);

        // Smooth out the camera rotation. I need the value of the previous frame's rotation for this.
        Quaternion smoothedRotation = Quaternion.Slerp(previousFrameRotation, targetCameraRotation, smoothingFactor);

        _helper.UpdateCameraPose(lockedPosition, targetCameraRotation);
        _helper.UpdateFov(_settings.fov);

        previousFrameRotation = targetCameraRotation;
    }

    // OnLateUpdate is called after OnUpdate also everyframe and has a higher chance that transform updates are more recent.
    public void OnLateUpdate()
    {

    }

    // OnDeactivate is called when the user changes the profile to other camera behaviour or when the application is about to close.
    // The camera behaviour should clean everything it created when the behaviour is deactivated.
    public void OnDeactivate()
    {
        ApplySettings?.Invoke(this, EventArgs.Empty);
    }

    // OnDestroy is called when the users selects a camera behaviour which is not a plugin or when the application is about to close.
    // This is the last chance to clean after your self.
    public void OnDestroy()
    {

    }
}
