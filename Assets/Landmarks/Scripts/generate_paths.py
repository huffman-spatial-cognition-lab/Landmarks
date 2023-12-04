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
    # To keep all of the arrays the same size, 99 is added to the end of
    # each path that is less than 7 rooms long
    route1 = [1, 0, 5, 6, 7, 99, 99, 99]
    route2 = [3, 2, 7, 8, 9, 99, 99, 99]
    route3 = [9, 4, 3, 8, 13, 14, 99, 99]
    route4 = [12, 7, 6, 11, 16, 17, 18, 99]
    route5 = [19, 14, 13, 18, 23, 24, 99, 99]
    route6 = [21, 20, 15, 16, 17, 22, 99, 99]
    route7 = [15, 10, 11, 16, 21, 20, 99, 99]

    rdj_arr = np.array([[5,7], [7,9], [3, 13], [6, 16], [13, 23], [20, 22], [10, 20]], dtype=int)

    doors_arr = [["a", "e", "j", "k"], ["c", "g", "l", "m"], 
                ["i", "d", "h", "q", "v"], ["p", "k", "o", "x", "cc", "dd"], 
                ["aa", "v", "z", "ii", "nn"], ["kk", "ff", "bb", "cc", "hh"], 
                ["w", "s", "x", "gg", "kk"]]

    paths = np.vstack((route1, route2, route3, route4, route5, route6, route7))

    paths_for_subj = np.zeros((21,8), dtype=int)
    rdj_for_subj = np.zeros((21,2), dtype=int)
    doors_for_subj = list()

    for i in range(3):
        ind = np.random.choice(7, 7, replace=False)
        for index in ind:
            doors_for_subj.append(doors_arr[index])
        if i > 0:
            ind = (7*i)+ind
        paths_for_subj[ind] = paths
        rdj_for_subj[ind] = rdj_arr

        # Should handle edge case where duplicates at 3?

    path_name = "../TextFiles/ParticipantFiles/"
    path_filename = path_name + "s" + str(participant_number) + "_paths.txt"
    rdj_name = path_name + "s" + str(participant_number) + "_rdj.txt"
    doors_name = path_name + "s" + str(participant_number) + "_doors.txt"

    np.savetxt(path_filename, paths_for_subj, fmt="%.0f",delimiter="\n")
    np.savetxt(rdj_name, rdj_for_subj, fmt="%.0i", delimiter="\n")

    with open(doors_name, "w") as output:
        for trial in doors_for_subj:
            for door in trial:
                output.write("%s\n" %door)
    
    return

generate_paths(0)
