extends PathFollow

var traverseTime = 100 # Time it takes to traverse the path
var t = 0 # Active time along the apth
var pathLength = 0 # Length of the path

func _ready():
	pathLength = get_node("..").get_curve().get_baked_length()

func _process(delta):
	if (t > traverseTime):
		t = 0
	t += delta
	set_offset((t/traverseTime)* pathLength)
