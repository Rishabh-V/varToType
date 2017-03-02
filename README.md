# var To Type
Visual Studio 2015 Extension to replace var with strong type. 

## Getting started ##
To try the varToType Extension:

1. Download the VSIX from [here](http://vsixgallery.com/extensions/ReplaceVarWithType..7e5909ce-5a1a-42da-a4f9-d9e046e304ea/Replace%20var%20With%20Type%20v1.0.vsix)
2. Install VSIX.
3. Open a new instance of Visual Studio 2015 (tested only in VS 2015 as of now).
4. Open a C# project, which has code files using var.
5. On every usage of var, a light bulb style refactoring should be available.
6. Click on light bulb or Ctrl+ . and it would show the preview of the fix. 
7. Choose to fix that instance, all instances in document, project or solution.
8. On accepting, the fix is applied.var should be replaced with Fully qualified name as discovered.

 ##### Screenshot #####
 
![Replace var with Type](https://github.com/Rishabh-V/varToType/blob/master/TestVar.png "Replace var with type")
![Preview changes](https://github.com/Rishabh-V/varToType/blob/master/TestVarPreview.png "Preview changes")
