extends Node

# Import classes
const HTerrain = preload("res://addons/zylann.hterrain/hterrain.gd")
const HTerrainData = preload("res://addons/zylann.hterrain/hterrain_data.gd")
const HTerrainTextureSet = preload("res://addons/zylann.hterrain/hterrain_texture_set.gd")
const XYZFormat = preload("res://addons/zylann.hterrain/util/xyz_format.gd")

# You may want to change paths to your own textures
var grass_texture = load("res://addons/zylann.hterrain_demo/textures/ground/grass_albedo_bump.png")
var sand_texture = load("res://addons/zylann.hterrain_demo/textures/ground/sand_albedo_bump.png")
var leaves_texture = load("res://addons/zylann.hterrain_demo/textures/ground/leaves_albedo_bump.png")


func _ready():
	return
	# Create terrain resource and give it a size.
	# It must be either 513, 1025, 2049 or 4097.
	var terrain_data = HTerrainData.new()

	var fpath = "/media/data/Sythelux/Projects/citygml/Downloads/Großpösna/Downloads/333185676_dgm1.xyz"
	print(fpath)

	var f := File.new()
	var err := f.open(fpath, File.READ)
	if err != OK:
		return false

	var bounds := XYZFormat.load_bounds(f)
	var res := HTerrainData.get_adjusted_map_size(bounds.image_width, bounds.image_height)

	var width := res
	var height := res

	terrain_data.resize(res)

	# Get access to terrain maps
	var heightmap: Image = terrain_data.get_image(HTerrainData.CHANNEL_HEIGHT)
	var normalmap: Image = terrain_data.get_image(HTerrainData.CHANNEL_NORMAL)
	var splatmap: Image = terrain_data.get_image(HTerrainData.CHANNEL_SPLAT)

	heightmap.lock()
	normalmap.lock()
	splatmap.lock()

	heightmap.fill(Color(0,0,0))

	f.seek(0)
	XYZFormat.load_heightmap(f, heightmap, bounds)

	heightmap.flip_y()

	heightmap.unlock()
	normalmap.unlock()
	splatmap.unlock()

	# Commit modifications so they get uploaded to the graphics card
	var modified_region = Rect2(Vector2(), heightmap.get_size())
	terrain_data.notify_region_change(modified_region, HTerrainData.CHANNEL_HEIGHT)
	terrain_data.notify_region_change(modified_region, HTerrainData.CHANNEL_NORMAL)
	terrain_data.notify_region_change(modified_region, HTerrainData.CHANNEL_SPLAT)

	# Create texture set
	# NOTE: usually this is not made from script, it can be built with editor tools
	var texture_set = HTerrainTextureSet.new()
	texture_set.set_mode(HTerrainTextureSet.MODE_TEXTURES)
	texture_set.insert_slot(-1)
	texture_set.set_texture(0, HTerrainTextureSet.TYPE_ALBEDO_BUMP, grass_texture)
	texture_set.insert_slot(-1)
	texture_set.set_texture(1, HTerrainTextureSet.TYPE_ALBEDO_BUMP, sand_texture)
	texture_set.insert_slot(-1)
	texture_set.set_texture(2, HTerrainTextureSet.TYPE_ALBEDO_BUMP, leaves_texture)

	# Create terrain node
	var terrain = HTerrain.new()
	terrain.set_shader_type(HTerrain.SHADER_CLASSIC4_LITE)
	terrain.set_data(terrain_data)
	terrain.set_texture_set(texture_set)
	add_child(terrain)

	# No need to call this, but you may need to if you edit the terrain later on
	#terrain.update_collider()
