/*
    # -------------------------------------------------------------------------
    This script is for initializing Lab Streaming Layer. We can then use this
    in experiment tasks to push_sample for integrating Unity tasks with our
    EEG data acquisition.

    WRITTEN BY AINSLEY K. BONIN AND DEREK J. HUFFMAN (FEBRUARY 2022).
    # -------------------------------------------------------------------------
*/

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using LSL;

public class Initialize_LSL : ExperimentTask
{
    // import the relevant LSL functions --------------------------------------
    [DllImport ("liblsl.1.15.2")] private static extern float push_sample ();
    [DllImport ("liblsl.1.15.2")] private static extern float StreamInfo ();
    [DllImport ("liblsl.1.15.2")] private static extern float StreamOutlet ();

    [Header("Task-specific Properties")]
    public string StreamName = "Unity.UprightInvertedStream";
    public string StreamType = "Unity.String";
    public string StreamId = "MyStreamID-Unity1234";

    [HideInInspector] public StreamOutlet outlet;
    [HideInInspector] public float[] currentSample;


    public override void startTask()
    {
        TASK_START();
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();
        StreamInfo streamInfo = new StreamInfo(StreamName, StreamType, 1, Time.deltaTime * 1000, LSL.channel_format_t.cf_float32);
        XMLElement chans = streamInfo.desc().append_child("channels");
        chans.append_child("channel").append_child_value("label", "Z");
        outlet = new StreamOutlet(streamInfo);
    }


    public override bool updateTask()
    {
        return true;
    }


    public override void endTask()
    {
        TASK_END();
    }


    public override void TASK_END()
    {
        base.endTask();
    }


    public void lsl_push_sample()
    {
        outlet.push_sample(currentSample);
    }
}
