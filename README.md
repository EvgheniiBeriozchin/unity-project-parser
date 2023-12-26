# Unity Project Parser
This parser was written as part of an assessment for a JetBrains internship.  
The project creates the hierarchies of game objects from all scenes and collects the names and guids of all unused scripts in the Unity project. The scene parsing is optimized by parallelization.

## Run Application
You can run the application by first navigating to `UnityProjectParser/bin/Release/net6.0/` and then running:   
```./UnityProjectParser.exe unity_project_path output_folder_path```

## Run Project Code
To run the code, open the `.sln` in Visual Studio 2022 with the C#/.NET packages installed and run it from there.
