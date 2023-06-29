extends Spatial

export(float) var start_x = 306090.0
export(float) var start_y = 5678033.9
export(float) var end_x = 329877.9
export(float) var end_y = 5692033.0
export(int) var radius = 64
export(int) var max_features = 1000
export var dataBase = ""
var las_file = "/media/data/Projects/masterprojekt/Downloads/lidar-tests/lsc_33316_5690_3_sn.laz"
var street_scene = preload("res://Scenes/Street.tscn")
var generate_streets_coroutine
var generate_ground_coroutine
var generate_veg_coroutine
var current_features = 0
var dataset


func _ready():
	dataset = Geodot.get_dataset(dataBase)
	generate_ground_coroutine = generate_ground()
	generate_veg_coroutine = generate_veg()
	generate_streets_coroutine = generate_streets()


func generate_ground():
	#var ground_set = Geodot.get_dataset(las_file)
	#var blue = ground_set.get_feature_layers()
	#print(ground_set)
	yield(get_tree(), "idle_frame")

func generate_veg():
	var vegetation = dataset.get_feature_layer("veg03_f")
	# veg01_f	AX_Landwirtschaft
	# veg02_f	AX_Wald
	# veg03_f	AX_Gehoelz
	# veg03_f	AX_Heide
	# veg03_f	AX_Moor
	# veg03_f	AX_Sumpf
	# veg03_f	AX_UnlandVegetationsloseFlaeche
	# veg04_f	AX_Vegetationsmerkmal
	yield(get_tree(), "idle_frame")

func generate_streets():
	var streets = dataset.get_feature_layer("ver01_l")
	# ver01_f	AX_Platz
	# ver01_f	AX_Strassenverkehr
	# ver01_l	AX_Strassenachse
	# ver02_l	AX_Fahrwegachse
	# ver02_l	AX_WegPfadSteig
	for offset_x in range(0, end_x - start_x, radius):
		for offset_y in range(0, end_y - start_y, radius):
			print("x: ", start_x + offset_x, " y: ", start_y + offset_y)
			var ver01_l = streets.get_features_near_position(
				transform.origin.x + start_x + offset_x,
				transform.origin.z + start_y + offset_y,
				radius,
				max_features - current_features
			)
			for line in ver01_l:
				var street = street_scene.instance()
				street.curve = line.get_offset_curve3d(-(start_x), 0, -(start_y))
				print(line)
				var width = float(line.get_attribute("WIDTH"))
				width = max(width, 3)  # It's sometimes -1 in the data

				street.get_node("PathFollow/CSGPolygon").polygon[0] = Vector2(width, 0)
				street.get_node("PathFollow/CSGPolygon").polygon[1] = Vector2(width, 1)
				street.get_node("PathFollow/CSGPolygon").polygon[2] = Vector2(0, 1)
				street.get_node("PathFollow/CSGPolygon").polygon[3] = Vector2(-width, 1)
				street.get_node("PathFollow/CSGPolygon").polygon[4] = Vector2(-width, 0)

				add_child(street)
				current_features += 1
				if current_features >= max_features:
					print("enough features")
					return
				yield(get_tree(), "idle_frame")
			yield(get_tree(), "idle_frame")
		yield(get_tree(), "idle_frame")


func _process(_delta):
#	if generate_ground_coroutine.is_valid():
#		generate_ground_coroutine.resume()
	if generate_veg_coroutine.is_valid():
		generate_veg_coroutine.resume()
	if generate_streets_coroutine.is_valid():
		generate_streets_coroutine.resume()
