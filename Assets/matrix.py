import json

distance_set = [2, 2.5, 3, 3.5]
distance_repeat_count = 2

matrix = []
count = 0
for distance in distance_set:
    for border in [0, 1]:
        for i in range(distance_repeat_count):
            matrix.append([str(count), str(distance), str(border)])
            count += 1

with open("matrix.json","w") as matrixfile:
    data = {
        "matrix": matrix
    }
    matrixfile.write(json.dumps(data))
