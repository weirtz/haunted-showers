extends Node

var dict = {}

func _ready():
	# Called when the node is added to the scene for the first time.
	# Initialization here
	pass

func get_save_dict(var key, var value):
	dict.clear()
	dict[key] = value
	return dict

func get_new_dict():
	dict.clear()
	return dict

func append_dict(var dict, var key, var value):
	dict[key] = value;
	return dict
