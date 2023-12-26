using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class OutputFormatter
{

    public static string CreateGameObjectHierarchyString(List<UnityGameObject> gameObjects, List<UnityTransform> transforms)
    {
        Dictionary<string, UnityGameObject> gameObjectDictionary = gameObjects.ToDictionary(item => item.id, item => item);
        Dictionary<string, UnityTransform> transformDictinary = transforms.ToDictionary(item => item.id, item => item);
        Dictionary<string, UnityTransform> gameObjectTransformDictionary = transforms.ToDictionary(item => item.gameObjectId ?? "-1", item => item);

        string sceneHierarchyString = "";
        foreach (UnityGameObject sceneObject in gameObjects)
        {
            UnityTransform? transform = null;
            gameObjectTransformDictionary.TryGetValue(sceneObject.id, out transform);
            if (transform != null && transform.parent == "0")
            {
                if (transform.children != null)
                {
                    Stack<Tuple<UnityGameObject, string>> childObjects = new Stack<Tuple<UnityGameObject, string>>();
                    childObjects.Push(Tuple.Create(sceneObject, ""));
                    while (childObjects.Count > 0)
                    {
                        Tuple<UnityGameObject, string> childObject = childObjects.Pop();
                        UnityGameObject currentObject = childObject.Item1;
                        string prefix = childObject.Item2;

                        sceneHierarchyString += String.Format("{0}{1}\n", prefix, currentObject.name);

                        UnityTransform? currentObjectTransform = null;
                        gameObjectTransformDictionary.TryGetValue(currentObject.id, out currentObjectTransform);
                        if (currentObjectTransform != null && currentObjectTransform.children != null)
                        {
                            List<string> sortedChildren = currentObjectTransform.children
                                .ToList().OrderByDescending(item => item).ToList();
                            foreach (string child in sortedChildren)
                            {
                                UnityTransform? childTransform;
                                transformDictinary.TryGetValue(child, out childTransform);
                                if (childTransform != null && childTransform.gameObjectId != null)
                                {
                                    childObjects.Push(Tuple.Create(
                                        gameObjectDictionary[childTransform.gameObjectId], prefix + "--"));
                                }
                            }
                        }
                    }
                }
                else
                {
                    sceneHierarchyString += String.Format("{0}{1}\n", "", sceneObject.name);
                }
            }
        }

        return sceneHierarchyString;
    }

    public static string CreateUnusedScriptsString(Dictionary<string, string> scriptMapping,
                                                    List<UnityMonoBehaviour> allUsedScripts,
                                                    string sourceDirectoryName)
    {
        List<string> allScripts = scriptMapping.Keys.ToList();
        HashSet<string> usedScripts = new HashSet<string>(allUsedScripts.Select(usedScript => usedScript.guid));

        string unusedScriptsString = "Relative Path,GUID\n";
        List<string> unusedScripts = allScripts.Where(script => !usedScripts.Contains(script)).ToList();
        foreach (string unusedScript in unusedScripts)
        {
            unusedScriptsString += String.Format("{0},{1}\n", Path.GetRelativePath(sourceDirectoryName, scriptMapping[unusedScript]), unusedScript);
        }

        return unusedScriptsString;
    }
}

