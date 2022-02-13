using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using LSL;

public class Initialize_LSL : MonoBehaviour
{
  [DllImport ("liblsl.1.15.2")] private static extern float push_sample ();
  [DllImport ("liblsl.1.15.2")] private static extern float StreamInfo ();
  [DllImport ("liblsl.1.15.2")] private static extern float StreamOutlet ();
  public StreamOutlet outlet;
  public float[] currentSample;

  public string StreamName = "Unity.UprightInvertedStream";
  public string StreamType = "Unity.String";
  public string StreamId = "MyStreamID-Unity1234";

    // Start is called before the first frame update
    void Start()
    {
      StreamInfo streamInfo = new StreamInfo(StreamName, StreamType, 1, Time.deltaTime * 1000, LSL.channel_format_t.cf_float32);
      XMLElement chans = streamInfo.desc().append_child("channels");
      chans.append_child("channel").append_child_value("label", "Z");
      outlet = new StreamOutlet(streamInfo);
      currentSample = new float[1];
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
