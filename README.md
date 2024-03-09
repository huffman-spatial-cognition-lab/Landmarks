# Landmarks
This branch is a collaboration between Sinan Yumurtaci '23, Prof. Derek J. Huffman, and Paisley Annes '26 at Colby College.

# Important information
- The PTO (point to origin) task is only currently working for the Oculus Quest or Quest 2.
- We (Sinan, Paisley, Derek) created the trials for the task in Python (see pythonTrialConfiguration).
- While this is slightly non-Landmarkian, Sinan designed the Relocation and Pointing Tasks to update the target objects within the update function. We are keeping it this way for now since the task is working, but we could consider changing this in the future.
- The door was occasionally acting funny when it inactivated when it was not fully shut. To fix this issue, DJH wrote code to check the initial transform position and rotation and then to reset the door each time it is instantiated. DJH set the door to be inactive at the start because it can interact with the collider, etc. to open it at the start of the experiment, but we want the transform information as it was actually set in the Editor.

# To-do
## Unity -- Experiment Code
- [x] During relocation task, turn on/off the "boundary" walls based on the task condition
- [x] During pointing task, turn off the "boundary" walls
- [x] Extend the floor indefinitely
- [x] Add a door and have this work with the boundary as well

## Bugs
- [x] Pressing the trigger on the Oculus hand controller during navigation (not supposed to, trigger is for pointing only), the app goes into undefined behavior, where all navigation tasks are immediately skipped, and only relocation and pointing tasks happen
  - [x] Investigate: It looks like Sinan must have fixed this issue
