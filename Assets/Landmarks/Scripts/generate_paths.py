"""
@author: ainsleybonin
"""
import numpy as np

def generate_paths():
    '''
    This function creates the randomized paths (3 repetitions of 7 paths = 21 total)
    for one participant.

    Returns
    -------
    paths_for_subj : np array - shape=(21,7)
        Array of paths for one participant
        
    Written by Ainsley (11/16/2023)

    Maze coded as follows:
    0  1  2  3  4 
    5  6  7  8  9 
    10 11 12 13 14 
    15 16 17 18 19 
    20 21 22 23 24
    '''
    # To keep all of the arrays the same size, 99 is added to the end of
    # each path that is less than 7 rooms long
    route1 = [1, 0, 5, 6, 7, 99, 99]
    route2 = [3, 2, 7, 8, 9, 99, 99]
    route3 = [9, 4, 3, 8, 13, 14, 99]
    route4 = [12, 7, 6, 11, 16, 17, 18]
    route5 = [19, 14, 13, 18, 23, 24, 99]
    route6 = [21, 20, 15, 16, 17, 22, 99]
    route7 = [15, 10, 11, 12, 17, 99, 99]

    paths = np.vstack((route1, route2, route3, route4, route5, route6, route7))
    all_paths = np.tile(paths, (3,1))

    paths_for_subj = np.zeros((21,7))
    for i in range(3):
        ind = np.random.choice(7, 7, replace=False)
        if i > 0:
            ind = (7*i)+ind
        paths_for_subj[ind] = all_paths[:7, :]

        # Should handle edge case where duplicates at 3?

    return paths_for_subj