extends Tree


func set_data(data) -> void:
	
	clear()
	var root := create_item()
	create_from_value(data, root)


func create_from_value(data, parent: TreeItem) -> void:
	
	match typeof(data):
		
		TYPE_DICTIONARY:
			create_from_dictionary(data, parent)
		
		TYPE_ARRAY:
			create_from_array(data, parent)
	
		_:
			var child = create_item(parent)
			child.set_text(0, str(data))


func create_from_dictionary(data: Dictionary , parent: TreeItem) -> void:
	
	for key in data.keys():
		var child := create_item(parent)
		
		var value = data[key]
		
		match typeof(value):
			TYPE_DICTIONARY:
				child.set_text(0, key)
				create_from_dictionary(value, child)
		
			TYPE_ARRAY:
				child.set_text(0, key)
				create_from_array(value, child)
		
			_:
				child.set_text(0, key)
				child.set_text(1, str(value))


func create_from_array(data: Array, parent: TreeItem) -> void:
	
	for value in data:
		create_from_value(value, parent)
