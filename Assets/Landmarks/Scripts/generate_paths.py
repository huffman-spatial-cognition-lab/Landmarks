"""
@author: ainsleybonin
"""
import numpy as np

def generate_paths(participant_number):
    '''
    This function creates the randomized paths (3 repetitions of 7 paths = 21 total)
    for one participant.

    Arguments:
    ----------
    participant_number: int, subject number for saving the txt file

    Returns:
    --------
    paths_for_subj : array
        Array of paths for one participant
        
    Written by Ainsley (11/16/2023)

    Maze coded as follows:
    0  1  2  3  4 
    5  6  7  8  9 
    10 11 12 13 14 
    15 16 17 18 19 
    20 21 22 23 24

    Doors coded as (rooms are numbers, doors are letters):
    0   a   1   b   2   c   3   d   4 
    e       f       g       h       i
    5   j   6   k   7   l   8   m   9 
    n       o       p       q       r
    10  s  11   t   12  u   13  v   14 
    w       x       y        z      aa
    15  bb 16   cc  17  dd  18  ee  19 
    ff     gg       hh      ii      jj   
    20  kk 21  ll   22  mm  23  nn  24

    '''
    route1 = [6,11,10,5,0,1,2]
    route2 = [10,5,6,11,16,15]
    route3 = [21,20,15,16,17,22]
    route4 = [12,7,6,11,16,17,18]
    route5 = [8,13,12,7,2,3,4]
    route6 = [9,4,3,8,13,14]
    route7 = [18,13,14,19,24,23]

    rdj1 = [0,2]
    rdj2 = [6,16]
    rdj3 = [15,17]
    rdj4 = [16,18]
    rdj5 = [2,4]
    rdj6 = [3,13]
    rdj7 = [14,24]
    
    doors1 = ["o", "s", "n", "e", "a", "b"]
    doors2 = ["n", "j", "o", "x", "bb"]
    doors3 = ["kk", "ff", "bb", "cc", "hh"]
    doors4 = ["p", "k", "o", "x", "cc", "dd"]
    doors5 = ["q", "u", "p", "g", "c", "d"]
    doors6 = ["i", "d", "h", "q", "v"]
    doors7 = ["z", "v", "aa", "jj", "nn"]

    rdj_arr = [rdj1, rdj2, rdj3, rdj4, rdj5, rdj6, rdj7, rdj1, rdj2, rdj3, rdj4, rdj5, rdj6, rdj7]

    doors_arr = [doors1, doors2, doors3, doors4, doors5, doors6, doors7, doors1, doors2, doors3, doors4, doors5, doors6, doors7]

    paths = [route1, route2, route3, route4, route5, route6, route7, 
             route1[::-1], route2[::-1], route3[::-1], route4[::-1], route5[::-1], route6[::-1], route7[::-1]]

    paths_for_subj = list()
    rdj_for_subj = list()
    doors_for_subj = list()

    for i in range(5):
        ind = np.random.choice(14, 14, replace=False)
        for index in ind:
            doors_for_subj.append(doors_arr[index])
            paths_for_subj.append(paths[index])
            # randomly switch which object will be on the right vs. left
            coin = np.random.choice(1, 1)
            if coin == 0:
                rdj_for_subj.append(rdj_arr[index])
            else:
                rdj_for_subj.append(rdj_arr[index][::-1])

    path_filename = "../TextFiles/ParticipantFiles/" + "s" + str(participant_number) + "_paths.txt"
    rdj_name = "../TextFiles/ParticipantFiles/" + "s" + str(participant_number) + "_rdj.txt"
    doors_name = "../TextFiles/ParticipantFiles/" + "s" + str(participant_number) + "_doors.txt"

    with open(path_filename, "w") as output_paths:
        for trial in paths_for_subj:
            for room in trial:
                output_paths.write("%i\n" %room)
            output_paths.write("\n")
    output_paths.close()

    with open(rdj_name, "w") as output_rdj:
        for trial in rdj_for_subj:
            for obj in trial:
                output_rdj.write("%i\n" %obj)
            output_rdj.write("\n")
    output_rdj.close()

    with open(doors_name, "w") as output:
        for trial in doors_for_subj:
            for door in trial:
                output.write("%s\n" %door)
            output.write("\n")
    
    return


def generate_adj(participant_number, shortcut=False):
    '''
    This function creates the randomized paths (3 repetitions of 7 paths = 21 total)
    for one participant.

    Arguments:
    ----------
    participant_number: int, subject number for saving the txt file

    Returns:
    --------
    paths_for_subj : np array - shape=(21,7)
        Array of paths for one participant
        
    Written by Ainsley (11/16/2023)

    Maze coded as follows:
    0  1  2  3  4 
    5  6  7  8  9 
    10 11 12 13 14 
    15 16 17 18 19 
    20 21 22 23 24

    Doors coded as (rooms are numbers, doors are letters):
    0   a   1   b   2   c   3   d   4 
    e       f       g       h       i
    5   j   6   k   7   l   8   m   9 
    n       o       p       q       r
    10  s  11   t   12  u   13  v   14 
    w       x       y        z      aa
    15  bb 16   cc  17  dd  18  ee  19 
    ff     gg       hh      ii      jj   
    20  kk 21  ll   22  mm  23  nn  24

    '''
    route1 = [6,11,10,5,0,1,2]
    route2 = [10,5,6,11,16,15]
    route3 = [21,20,15,16,17,22]
    route4 = [12,7,6,11,16,17,18]
    route5 = [8,13,12,7,2,3,4]
    route6 = [9,4,3,8,13,14]
    route7 = [18,13,14,19,24,23]
    paths = [route1, route2, route3, route4, route5, route6, route7]

    # Making an adjacency matrix of all rooms connected by a path
            #    0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24
    adj_list = [[2, 1, 1, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], #0
                [1, 2, 1, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], #1
                [1, 1, 2, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], #2
                [0, 0, 1, 2, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], #3
                [0, 0, 1, 1, 2, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], #4
                [1, 1, 1, 0, 0, 2, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0], #5
                [1, 1, 1, 0, 0, 1, 2, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0], #6
                [0, 0, 1, 1, 1, 0, 1, 2, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0], #7
                [0, 0, 1, 1, 1, 0, 0, 1, 2, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], #8
                [0, 0, 0, 1, 1, 0, 0, 0, 1, 2, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], #9
                [1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 2, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0], #10
                [1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 2, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0], #11
                [0, 0, 1, 1, 1, 0, 1, 1, 1, 0, 0, 1, 2, 1, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0], #12
                [0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 2, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1], #13
                [0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 2, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1], #14
                [0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 2, 1, 1, 0, 0, 1, 1, 1, 0, 0], #15
                [0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 2, 1, 1, 0, 1, 1, 1, 0, 0], #16
                [0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 1, 1, 2, 1, 0, 1, 1, 1, 0, 0], #17
                [0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 2, 1, 0, 0, 0, 1, 1], #18
                [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 2, 0, 0, 0, 1, 1], #19
                [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 2, 1, 1, 0, 0], #20
                [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 2, 1, 0, 0], #21
                [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 2, 0, 0], #22
                [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 2, 1], #23
                [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 2]] #24
    
    adj_matrix = np.array(adj_list)

    # CONDITION 2 --------------------------------------------------------------------------------------
    # select a random starting point
    cond2starts = np.random.choice(25, 7, replace=False)
    condition2 = list()
    for start in cond2starts:
        row = adj_matrix[start, :]
        choices = np.argwhere(row == 0)
        end = np.random.choice(np.squeeze(choices), 1)
        condition2.append([start, end[0]])
    condition2 = np.array(condition2)


    # CONDITION 3 --------------------------------------------------------------------------------------
    condition3_all = np.array([[1,6], [7,8], [8,9], [9,14], [11,12], [10,15], 
                               [16,21], [12,17], [18,19], [18,23], [21,22], [22,23]])
    np.random.shuffle(condition3_all)
    condition3 = condition3_all[:7, :]


    # CONDITION 4 --------------------------------------------------------------------------------------
    condition4_all = np.array([[0,1], [1,2], [2,3], [3,4], [5,6], [6,7], [10,11], [12,13], 
                               [13,14], [15,16], [16,17], [17,18], [20,21], [23,24],
                               [0,5], [2,7], [3,8], [4,9], [5,10], [6,11], [7,12], [8,13],
                               [11,16], [13,18], [14,19], [15,20], [17,22], [19,24]])
    np.random.shuffle(condition4_all)
    condition4 = condition4_all[:7, :]
    

    # CONDITION 1 --------------------------------------------------------------------------------------
    condition1 = list()
    rand1 = np.random.choice(7, 7, replace=False)
    for i in range(7):
        trial_path = paths[rand1[i]]
        trial_arr = np.random.choice(trial_path, 2)
        condition1.append(trial_arr)

    all_trials = np.row_stack((condition1, condition2, condition3, condition4))
    np.random.shuffle(all_trials)


    # SAVE PARTICIPANT FILE -----------------------------------------------------------------------------
    if shortcut:

        adj_filename = "../TextFiles/ParticipantFiles/" + "s" + str(participant_number) + "_nav2.txt"

        with open(adj_filename, "w") as output_adj:
            for trial in all_trials:
                for room in trial:
                    output_adj.write("%i\n" %room)
                output_adj.write("\n")
        output_adj.close()

    else:

        adj_filename = "../TextFiles/ParticipantFiles/" + "s" + str(participant_number) + "_adj.txt"

        with open(adj_filename, "w") as output_adj:
            for trial in all_trials:
                for room in trial:
                    output_adj.write("%i\n" %room)
                output_adj.write("\n")
        output_adj.close()

    
    return
    
    
generate_paths(0)
generate_adj(0)
generate_adj(0, shortcut=True)
