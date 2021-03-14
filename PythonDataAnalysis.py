import csv
import numpy as np 
import matplotlib.pyplot as plt 

timestamps = np.empty((0,1))
positions = np.empty((0,3))
with open("example.csv") as logfile:
    logreader = csv.reader(logfile, delimiter='\t')
    for row in logreader:
        if len(row) > 12:
            if row[1] == "Avatar: ":
                time = int(row[0])

                pos = [float(x) for x in row[4:7]]

                rot = [float(x) for x in row[8:11]]

                positions = np.append(positions, np.array([pos]),axis = 0)
                timestamps = np.append(timestamps,  np.array(time))

                print(time, pos, rot)

fig = plt.figure() 
  
# syntax for 3-D projection 
ax = plt.axes(projection ='3d') 

# defining axes 
x = positions[:,0]
y = positions[:,2]
z = positions[:,1]
c = timestamps
ax.scatter(x, y, z, c = c) 
  
# syntax for plotting 
ax.set_title('3d Position of the Headset over Time') 
plt.show() 