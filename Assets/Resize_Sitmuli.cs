using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Resize_Sitmuli : MonoBehaviour {
  public ObjectList startObjects;
    // Start is called before the first frame update

    public void startTask() {

        Vector3 refSize = startObjects.objects[0].GetComponent<Renderer>().bounds.size;

        foreach (GameObject item in startObjects.objects) {
          float resizeX = refSize.x / item.GetComponent<Renderer>().bounds.size.x;
          float resizeY = refSize.y / item.GetComponent<Renderer>().bounds.size.y;
          float resizeZ = refSize.z / item.GetComponent<Renderer>().bounds.size.z;

          resizeX *= item.transform.localScale.x;
          resizeY *= item.transform.localScale.y;
          resizeZ *= item.transform.localScale.z;
        }


        }
      }



        //scales all GameObjects in goResizeList to have the same size as the referenceGO.
