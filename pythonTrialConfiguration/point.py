class Point():
    '''
    convenience point class that provides functions such as distance, creating within random circle or distance, etc.
    '''
    
    def __init__(self, x = None, y = None):
        self.x = x
        self.y = y
        
    def __sub__(self, other):
        if not isinstance(other, Point):
            return NotImplementedError()
        else:
            return Point(self.x - other.x, self.y - other.y)
        
    def __add__(self, other):
        if not isinstance(other, Point):
            return NotImplementedError()
        else:
            return Point(self.x + other.x, self.y + other.y)
        
    def __repr__(self):
        return "P(" + str(round(self.x, 2)) + ", " + str(round(self.y, 2)) + ")"
        
    def dist(self, other):
        diff = self - other
        return diff.magnitude()
    
    def magnitude(self):
        return (self.x**2 + self.y**2) ** 0.5
    
    @staticmethod
    def random_circ(dist = 1):
        '''
        generates a random point with the given distance
        defaults to dist = 1 if no distance (on unit circle)
        '''
        theta = random.random() * 360
        x = math.cos(theta) * dist
        y = math.sin(theta) * dist
        
        return Point(x, y)

    @staticmethod
    def random_rect(w, l):
        x = random.random() * w
        y = random.random() * l
        
        return Point(x, y)