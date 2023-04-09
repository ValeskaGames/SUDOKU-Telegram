extends Control

var following = false
var dragging_start_position = Vector2i()
var win = Window

func _ready():
	win = get_window()

func _on_gui_input(event):
	if event is InputEventMouseButton:
		if event.get_button_index() == 1:
			following = !following
			dragging_start_position = get_local_mouse_position() as Vector2i

func _process(_delta):
	
	if following:
		var conv = get_global_mouse_position() as Vector2i
		win.position = win.position + conv - dragging_start_position
		

func _on_close_pressed():
	get_tree().quit()

func _on_minimize_pressed():
	win.set_mode(1)
