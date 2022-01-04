tool
extends EditorPlugin

func _enter_tree():
    add_custom_type("ConnectedOmniLight", "OmniLight", preload("connected_omniLight.gd"), preload("icon.svg"))
    pass

func _exit_tree():
    remove_custom_type("ConnectedOmniLight")
    pass