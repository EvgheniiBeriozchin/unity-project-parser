using System.Data;
using System.Collections.Concurrent;

public class RecursiveFileProcessor
{
    public static Tuple<List<string>, List<string>> SearchFiles(string directoryName)
    {
        List<string> sceneFiles = new List<string>();
        List<string> scriptMetaFiles = new List<string>();

        Queue<string> directoryQueue = new Queue<string>();

        directoryQueue.Enqueue(directoryName);
        while (directoryQueue.Count > 0)
        {
            string currentDirectoryName = directoryQueue.Dequeue();
            if (currentDirectoryName.ToLower().Contains("cache"))
            {
                continue;
            }

            sceneFiles.AddRange(Directory.GetFiles(currentDirectoryName, "*.unity"));
            scriptMetaFiles.AddRange(Directory.GetFiles(currentDirectoryName, "*.cs.meta"));

            foreach (string newDirectoryName in Directory.GetDirectories(currentDirectoryName))
            {
                directoryQueue.Enqueue(newDirectoryName);
            }
        }

        return Tuple.Create(sceneFiles, scriptMetaFiles);
    }

    public static void DumpSceneGameObjects(string directoryName, string sceneFilename, string text)
    {
        string trimmedSceneFilename = sceneFilename.Substring(sceneFilename.LastIndexOf("\\") + 1);
        File.WriteAllText(directoryName + "/" + trimmedSceneFilename + ".dump", text);
    }

    public static void DumpUnusedScripts(string directoryName, string text)
    {
        File.WriteAllText(directoryName + "/UnusedScripts.csv", text);
    }

    public static Dictionary<string, string> CreateScriptMapping(List<string> scriptMetaFiles)
    {
        Dictionary<string, string> scriptMapping = new Dictionary<string, string>();
        foreach (string scriptMetaFile in scriptMetaFiles)
        {
            string metaFileText = File.ReadAllText(scriptMetaFile);
            int stringStart = metaFileText.IndexOf("guid: ") + "guid: ".Length;
            int stringLength = metaFileText.Substring(stringStart).IndexOf('\n');
            string guid = metaFileText.Substring(stringStart, stringLength);

            scriptMapping[guid] = scriptMetaFile.Substring(0, scriptMetaFile.Length - 5);
        }

        return scriptMapping;
    }

    public static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("You have to specify the Unity project path and the output path");
            return;
        }

        string sourceDirectoryName = args[0];
        string targetDirectoryName = args[1];
        Tuple<List<string>, List<string>> files = SearchFiles(sourceDirectoryName);
        
        List<string> sceneFiles = files.Item1;
        Dictionary<string, string> scriptMapping = CreateScriptMapping(files.Item2);

        ConcurrentBag<UnityMonoBehaviour> allUsedScriptsBag = new ConcurrentBag<UnityMonoBehaviour>();

        Parallel.ForEach(sceneFiles, (string sceneFilename) =>
        {
            Tuple<List<UnityGameObject>, List<UnityTransform>, List<UnityMonoBehaviour>> parsedScene = SceneParser.ParseScene(sceneFilename);

            List<UnityGameObject> gameObjects = parsedScene.Item1.OrderByDescending(item => item.id).ToList();
            List<UnityTransform> transforms = parsedScene.Item2;
            foreach (UnityMonoBehaviour script in parsedScene.Item3)
            {
                allUsedScriptsBag.Add(script);
            }

            string sceneHierarchyString = OutputFormatter.CreateGameObjectHierarchyString(gameObjects, transforms);
            DumpSceneGameObjects(targetDirectoryName, sceneFilename, sceneHierarchyString);
        });

        List<UnityMonoBehaviour> allUsedScripts = allUsedScriptsBag.ToList();
        string unusedScriptsString = OutputFormatter.CreateUnusedScriptsString(scriptMapping, allUsedScripts, sourceDirectoryName);
        DumpUnusedScripts(targetDirectoryName, unusedScriptsString);
    }
}