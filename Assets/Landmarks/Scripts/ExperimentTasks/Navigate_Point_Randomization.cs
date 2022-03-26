﻿/*
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
    public int repeats_per_heading = 2;
    public bool shuffle = true;
    public EndListMode EndListBehavior;
    public int current = 0;
    readonly public List<GameObject> currentItem;
    readonly public List<int> heading_index_list = new List<int>();
    readonly public List<int> pointing_index_list = new List<int>();

    private int currentIndex;

    public override void startTask()
    {
        TASK_START();
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        // --------------------------------------------------------------------
        // First, let's gather some information about trial counts, etc. ------
        // --------------------------------------------------------------------
        int obj_count = listToRandomize.objects.Count;


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
        for (int heading_j = 0; heading_j < obj_count; heading_j++)
        {
            heading_template_unshuffled.Add(heading_j);
        }

        // Do the shuffle! ----------------------------------------------------
        int[] heading_template = heading_template_unshuffled.ToArray();
        if (shuffle)
        {
            Experiment.Shuffle(heading_template);
        }

        // Loop over this to repeat the trials within each facing direction ---
        List<int> heading_only = new List<int>();
        foreach (int heading_i in heading_template)
        {
            for (int repeat_i = 0; repeat_i < repeats_per_heading; repeat_i++)
            {
                heading_only.Add(heading_i);
            }
        }

        // Now, loop over the pointing trials for each heading and fill out ---
        // the arrays we will need for the heading and pointing trials --------
        foreach (int heading_i in heading_only)
        {
            List<int> pointing_heading_i_unshuffled = new List<int>();
            for (int pointing_j = 0; pointing_j < obj_count; pointing_j++)
            {
                heading_index_list.Add(heading_i);
                pointing_heading_i_unshuffled.Add(pointing_j);
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
            }
        }


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


