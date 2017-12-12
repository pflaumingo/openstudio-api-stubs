require 'openstudio'
require 'json'

def classes_to_json(classes, mod = nil)
  json_out = {}
  json_out['moduleName'] = mod
  json_out['classes'] = []
  classes.each do |c|
    json_out["classes"].push(c.to_s)
  end

  json_out
end

def main
  utility_class_names = %w(Workspace WorkspaceObject IdfObject)
  utility_classes = OpenStudio.constants.select {|c| utility_class_names.include? c.to_s}

  model_module = OpenStudio::Model
  model_classes = model_module.constants.select {|c| model_module.const_get(c).is_a? Class}

  classes_json = {'modules' => []}
  classes_json['modules'].push(classes_to_json(utility_classes))
  classes_json['modules'].push(classes_to_json(model_classes, 'Model'))

  File.new('//Mac/Home/Documents/Visual Studio 2013/Projects/OSStubsGenerator/OSStubsGenerator/resources/classes.json', 'w').puts(classes_json.to_json)
end

if __FILE__ == $0
  main
end
