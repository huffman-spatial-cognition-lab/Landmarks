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
    public int repetitions_upright = 2;
    //public int repetitions_inverted = 1;
    public bool shuffle = true;
    public EndListMode EndListBehavior;
    public int current = 0;
    readonly public List<GameObject> currentItem;
    readonly public List<int> heading_index_list = new List<int>();
    readonly public List<int> pointing_index_list = new List<int>();
    readonly public List<int> trial_indexer = new List<int>();

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
        // 1) trial_indexer_unshuffled: an index array from 0 to n_trials-1 ---
        // 2) object_heading_list: an index array for headings for each trial -
        //    (0 to n_objects-1)
        // 3) object_pointing_list: an index array for headings for each ------
        //    trial (o to n_objects-1) ----------------------------------------
        // --------------------------------------------------------------------
        List<int> trial_indexer_unshuffled = new List<int>();
        int repeat_blocks = 2;
        int trial_indexer_counter = 0;
        for (int repeat_i = 0; repeat_i < repeat_blocks; repeat_i++)
        {
            for (int heading_j = 0; heading_j < obj_count; heading_j++)
            {
                for (int pointing_k = 0; pointing_k < obj_count; pointing_k++)
                {
                    heading_index_list.Add(heading_j);
                    pointing_index_list.Add(pointing_k);
                    trial_indexer_unshuffled.Add(trial_indexer_counter);
                    trial_indexer_counter += 1;
                }
            }
        }

        // --------------------------------------------------------------------
        // Fill in trial_indexer from the temporary tmp_trial_indexer; e.g., --
        // allowing for shuffling the array here. -----------------------------
        // --------------------------------------------------------------------
        int[] objs = trial_indexer_unshuffled.ToArray();
        if (shuffle)
        {
            Experiment.Shuffle(objs);
        }
        foreach (int trial_i in objs)
        {
            trial_indexer.Add(trial_i);
            Debug.Log("Here is trial:" + trial_i);
        }


        // --------------------------------------------------------------------
        // Set the listToRandomize.current to the current index (i.e., for ----
        // randomization. This takes care of the first trial; all other -------
        // trials will be set to the random index within the taks script: -----
        // ViewStimuliTimes.cs ------------------------------------------------
        // --------------------------------------------------------------------
        //listToRandomize.current = getCurrentGameObjectIndex();
        listToRandomize.current = getCurrentPointingIndex();
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
        if (current >= trial_indexer.Count && EndListBehavior == EndListMode.Loop)
        {
            current = 0;
        }
    }


    public int getCurrentHeadingIndex()
    {
        return heading_index_list[trial_indexer[current]];
    }


    public int getCurrentPointingIndex()
    {
        return pointing_index_list[trial_indexer[current]];
    }
}


