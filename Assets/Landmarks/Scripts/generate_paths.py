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
    route1 = [1, 0, 5, 6, 7]
    route2 = [3, 2, 7, 8, 9]
    route3 = [9, 4, 3, 8, 13, 14]
    route4 = [12, 7, 6, 11, 16, 17, 18]
    route5 = [19, 14, 13, 18, 23, 24]
    route6 = [21, 20, 15, 16, 17, 22]
    route7 = [15, 10, 11, 16, 21, 20]

    rdj_arr = [[5,7], [7,9], [3, 13], [6, 16], [13, 23], [20, 22], [10, 20]]

    doors_arr = [["a", "e", "j", "k"], ["c", "g", "l", "m"], 
                ["i", "d", "h", "q", "v"], ["p", "k", "o", "x", "cc", "dd"], 
                ["aa", "v", "z", "ii", "nn"], ["kk", "ff", "bb", "cc", "hh"], 
                ["w", "s", "x", "gg", "kk"]]

    paths = [route1, route2, route3, route4, route5, route6, route7]

    paths_for_subj = list()
    rdj_for_subj = list()
    doors_for_subj = list()

    for i in range(3):
        ind = np.random.choice(7, 7, replace=False)
        for index in ind:
            doors_for_subj.append(doors_arr[index])
            paths_for_subj.append(paths[index])
            rdj_for_subj.append(rdj_arr[index])

        # Should handle edge case where duplicates at 3?

    path_name = "../TextFiles/ParticipantFiles/"
    path_filename = path_name + "s" + str(participant_number) + "_paths.txt"
    rdj_name = path_name + "s" + str(participant_number) + "_rdj.txt"
    doors_name = path_name + "s" + str(participant_number) + "_doors.txt"

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


def generate_adj(participant_number):
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
    route1 = [1, 0, 5, 6, 7]
    route2 = [3, 2, 7, 8, 9]
    route3 = [9, 4, 3, 8, 13, 14]
    route4 = [12, 7, 6, 11, 16, 17, 18]
    route5 = [19, 14, 13, 18, 23, 24]
    route6 = [21, 20, 15, 16, 17, 22]
    route7 = [15, 10, 11, 16, 21, 20]
    paths = [route1, route2, route3, route4, route5, route6, route7]

    # Making an adjacency matrix of all rooms connected by a path
            #    0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 24
    adj_list = [[2, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], #0
                [1, 2, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], #1
                [0, 0, 2, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], #2
                [0, 0, 1, 2, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], #3
                [0, 0, 0, 1, 2, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], #4
                [1, 1, 0, 0, 0, 2, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], #5
                [1, 1, 0, 0, 0, 1, 2, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0], #6
                [1, 1, 1, 1, 0, 1, 1, 2, 1, 1, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0], #7
                [0, 0, 1, 1, 1, 0, 0, 1, 2, 1, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], #8
                [0, 0, 1, 1, 1, 0, 0, 1, 1, 2, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], #9
                [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0], #10
                [0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 2, 1, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0], #11
                [0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 2, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0], #12
                [0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 2, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1], #13
                [0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 2, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1], #14
                [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 2, 1, 1, 0, 0, 1, 1, 1, 0, 0], #15
                [0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 2, 1, 1, 0, 1, 1, 1, 0, 0], #16
                [0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 1, 1, 2, 1, 0, 1, 1, 1, 0, 0], #17
                [0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 2, 1, 0, 0, 0, 1, 1], #18
                [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 2, 0, 0, 0, 1, 1], #19
                [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 2, 1, 1, 0, 0], #20
                [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 2, 1, 0, 0], #21
                [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 2, 0, 0], #22
                [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 2, 1], #23
                [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 2]] #24
    
    adj_matrix = np.array(adj_list)
    # select a random starting point
    cond2starts = np.random.choice(25, 7, replace=False)
    condition2 = list()
    for start in cond2starts:
        row = adj_matrix[start, :]
        choices = np.argwhere(row == 0)
        end = np.random.choice(np.squeeze(choices), 1)
        condition2.append([start, end[0]])
    condition2 = np.array(condition2)

    # CONDITION 3
    condition3 = np.array([[5,10], [1,2], [9,14], [12,13], [19,24], [22,23], [21,22]])

    # CONDITION 1
    condition1 = list()
    rand1 = np.random.choice(7, 7, replace=False)
    for i in range(7):
        # check edge cases where we have overlap btw conditions 1 and 3??
        trial_path = paths[rand1[i]]
        rand2 = np.random.choice(len(trial_path), 2, replace=False)
        trial_arr = np.array([trial_path[rand2[0]], trial_path[rand2[1]]])
        condition1.append(trial_arr)

    all_trials = np.row_stack((condition1, condition2, condition3))
    np.random.shuffle(all_trials)

    path_name = "../TextFiles/ParticipantFiles/"
    adj_filename = path_name + "s" + str(participant_number) + "_adj.txt"

    with open(adj_filename, "w") as output_adj:
        for trial in all_trials:
            for room in trial:
                output_adj.write("%i\n" %room)
            output_adj.write("\n")
    output_adj.close()

    
    return
    
    
# generate_paths(0)
generate_adj(0)
