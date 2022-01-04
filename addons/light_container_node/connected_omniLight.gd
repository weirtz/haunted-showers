tool
extends OmniLight

# class member variables go here, for example:
# var a = 2
# var b = "textvar"
var parent = get_parent()

func _enter_tree():
	set_process(true)

func _process(delta):
	light_energy = get_parent().lightEnergy
