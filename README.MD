# openstudio-api-stubs
Stub generator for the OpenStudio API to provide intellisense in RubyMine.

## How RubyMine Intellisense Works
RubyMine provides intellisense for ruby within the project load paths and from gems that have been installed and required in a given file. OpenStudio is written in C++ and made accessible in Ruby via SWIG, but this prevents it from being recognized via intellisense in RubyMine. However, stubs with no implementation can be generated to mimic the effect of having a library/API written natively in Ruby from a RubyMine intellisense perspective. A "stub" is simply a module, class or method that mimics the signature of a specific library but provides no implementation logic, for example:
```ruby
module OpenStudio
  module Model
    class AirLoopHVAC < Loop
      def removeBranchForZone(thermalZone)
      end
    end
  end
end
```
## How This Project Works
The project works by first calling a pre-build ruby script to extract all the classes from the desired modules of the OpenStudio API and creating a json file of each class and which module it belongs to. It then uses the OpenStudio C# bindings to reflexively look at all methods for each class and generate a signature. There is no method overloading in Ruby so any method with multiple signatures will simply take *args.
### Why Not Just Use Reflection Solely in Ruby?
OpenStudio being written in C++ generates Ruby bindings via SWIG, which prevents reflection from being able to examine the method signatures correctly. The C# bindings on the other hand do allow for the method signatures to be examined because it's statically typed and therefore can reflexively gather the information needed to generate the method signatures.
### Why Not Only Use the C# Bindings?
All classes in the C# bindings live in one 'OpenStudio' namespace where as in Ruby there are classes that live directly in the OpenStudio module (i.e. Workspace), but other objects in the Model submodule (OpenStudio::Model). Therefore, the ruby bindings are needed to generate the classes.json file which provides a mapping between each class and it's Ruby specific path.
## How Enable OpenStudio Intellisense in RubyMine?
In the project's gemfile, add the openstudio-api-stubs gem:
```ruby
gem 'openstudio-api-stubs'
```