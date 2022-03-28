/*
    # -------------------------------------------------------------------------
    
    
    The purpose of this script is to take a list of GameObjects and then to
    make a random order (just the indexes) for the stimuli. This will be
    helpful for general psychology experiments, e.g., in which you want to show
    the same items multiple times.

    Here, this code is set up to iterate

    I modified this script from my other randomization script: 
    LM_RandomOrderStimuli
    
    Copyright (C) 2022 Derek J. Huffman
    # -------------------------------------------------------------------------
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigate_Point_Randomization : ExperimentTask
{
    [Header("Task-specific Properties")]
    public ObjectList listToRandomize;
    public ObjectList navigationObjectList;
    public int repeats_per_heading = 2;
    public bool reset_each_block = true;
    public bool shuffle = true;
    public EndListMode EndListBehavior;
    public int current = 0;
    public List<int> heading_index_list = new List<int>();
    public List<int> pointing_index_list = new List<int>();

    public override void startTask()
    {
        TASK_START();
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        // --------------------------------------------------------------------
        // First, let's reset these arrays so we don't spill over from other --
        // trials. Note: this might not always be the desired behavior. -------
        // --------------------------------------------------------------------
        if (reset_each_block) {
            current = 0;
            heading_index_list = new List<int>();
            pointing_index_list = new List<int>();
        }

        // --------------------------------------------------------------------
        // Next, let's gather some information about trial counts, etc. -------
        // --------------------------------------------------------------------
        int total_obj_count = listToRandomize.objects.Count;
        int nav_location_count = navigationObjectList.objects.Count;
        int obj_count = total_obj_count / nav_location_count;

        int curr_nav = navigationObjectList.current;
        if (curr_nav == 0)
        {
            curr_nav = nav_location_count - 1;
        } else
        {
            curr_nav -= 1;
        }
        Debug.Log("Current navigation iteration: " + curr_nav);


        // --------------------------------------------------------------------
        // Now, let's create the lists with the relevant information: ---------
        // 1) object_heading_list: an index array for headings for each trial -
        //    (0 to n_objects-1)
        // 2) object_pointing_list: an index array for headings for each ------
        //    trial (o to n_objects-1) ----------------------------------------
        // --------------------------------------------------------------------
        List<int> heading_template_unshuffled = new List<int>();
        // Set up the heading template so that we can randomize the facing ----
        // directions for each trial, but we repeat the trials within each ----
        // facing direction below. --------------------------------------------

        // Here, we will set up the minimum and maximum values so that we -----
        // can loop over different center points for each array. --------------
        int min_index = curr_nav * obj_count;
        int less_than_index = (curr_nav + 1) * obj_count;

        for (int heading_j = min_index; heading_j < less_than_index; heading_j++)
        {
            Debug.Log("Current Heading: " + heading_j);
            heading_template_unshuffled.Add(heading_j);
        }

        // Do the shuffle! ----------------------------------------------------
        int[] heading_template = heading_template_unshuffled.ToArray();
        if (shuffle)
        {
            Experiment.Shuffle(heading_template);
        }

        // Loop over each heading ---------------------------------------------
        foreach (int heading_i in heading_template)
        {
            List<int> pointing_heading_i_unshuffled = new List<int>();
            // loop over each target / pointing object ------------------------
            for (int pointing_j = min_index; pointing_j < less_than_index; pointing_j++)
            {
                // loop over the number of repetitions ------------------------
                for (int repeat_i = 0; repeat_i < repeats_per_heading; repeat_i++)
                {
                    heading_index_list.Add(heading_i);
                    pointing_heading_i_unshuffled.Add(pointing_j);
                }

            }
            // Now, let's shuffle the pointing locations within each heading
            int[] pointing_heading_i = pointing_heading_i_unshuffled.ToArray();
            if (shuffle)
            {
                Experiment.Shuffle(pointing_heading_i);
            }
            foreach (int pointing_j in pointing_heading_i)
            {
                pointing_index_list.Add(pointing_j);
                Debug.Log("Current pointing: " + pointing_j);
            }
        }

        Debug.Log("Length of headings: " + heading_index_list.Count);
        Debug.Log("Length of pointings: " + pointing_index_list.Count);

        // --------------------------------------------------------------------
        // Set the listToRandomize.current to the current index (i.e., for ----
        // randomization. This takes care of the first trial; all other -------
        // trials will be set to the random index within the taks script: -----
        // ViewStimuliTimes.cs ------------------------------------------------
        // --------------------------------------------------------------------
        //listToRandomize.current = getCurrentGameObjectIndex();
        listToRandomize.current = getCurrentHeadingIndex();
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


    public void incrementCurrent()
    {
        current++;
        if (current >= heading_index_list.Count && EndListBehavior == EndListMode.Loop)
        {
            current = 0;
        }
    }


    public int getCurrentHeadingIndex()
    {
        return heading_index_list[current];
    }


    public int getCurrentPointingIndex()
    {
        return pointing_index_list[current];
    }


}


