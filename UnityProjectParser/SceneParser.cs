using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class SceneParser
{
    private static UnityGameObject? ParseGameObject(string[] variables)
    {
        string id = variables[0].Substring(variables[0].IndexOf('&') + 1);

        int index = 2;
        while (index < variables.Length && Utils.GetVariableName(variables[index++]) != "m_Name")
        {
        }

        if (index > variables.Length)
        {
            return null;
        }

        string name = Utils.GetVariableValue(variables[--index]);
        return new UnityGameObject(id, name);
    }

    private static UnityTransform ParseTransform(string[] variables)
    {
        string id = variables[0].Substring(variables[0].IndexOf('&') + 1);
        string? parent = null, gameObjectId = null;
        string[]? children = null;

        int index = 2;
        while (index < variables.Length)
        {
            string variable = variables[index++];
            string variableName = Utils.GetVariableName(variable);

            if (variableName == "m_GameObject")
            {
                gameObjectId = Utils.ParseFileId(Utils.GetVariableValue(variable));
            }
            else if (variableName == "m_Father")
            {
                parent = Utils.ParseFileId(Utils.GetVariableValue(variable));
            }
            else if (variableName == "m_Children")
            {
                string rawChildren = Utils.GetVariableValue(variable);
                if (rawChildren == "")
                {
                    List<string> objectChildren = new List<string>();
                    while (index < variables.Length && variables[index].Trim().StartsWith('-'))
                    {
                        objectChildren.Add(Utils.ParseFileId(variables[index++].Trim()));
                    }

                    children = objectChildren.ToArray();
                }
            }
        }

        return new UnityTransform(id, parent, children, gameObjectId);
    }

    private static UnityMonoBehaviour? ParseMonoBehaviour(string[] variables)
    {
        string id = variables[0].Substring(variables[0].IndexOf('&') + 1);

        int index = 2;
        while (index < variables.Length && Utils.GetVariableName(variables[index++]) != "m_Script")
        {
        }

        if (index > variables.Length)
        {
            return null;
        }

        string guid = Utils.ParseGuid(Utils.GetVariableValue(variables[--index]));
        return new UnityMonoBehaviour(id, guid);
    }

    public static Tuple<List<UnityGameObject>, List<UnityTransform>, List<UnityMonoBehaviour>> ParseScene(string sceneFilename)
    {
        List<UnityGameObject> sceneGameObjects = new List<UnityGameObject>();
        List<UnityTransform> sceneTransforms = new List<UnityTransform>();
        List<UnityMonoBehaviour> sceneScripts = new List<UnityMonoBehaviour>();

        string scene = File.ReadAllText(sceneFilename);

        string[] rawSceneObjects = scene.Split("---");

        foreach (string sceneObject in rawSceneObjects)
        {
            string[] objectVariables = sceneObject.Split('\n');
            UnityObjectType type = Utils.ToUnityObjectEnum(objectVariables[1].Substring(0, objectVariables[1].IndexOf(':')));

            switch (type)
            {
                case UnityObjectType.GameObject:
                    UnityGameObject? gameObject = ParseGameObject(objectVariables);
                    if (gameObject != null)
                    {
                        sceneGameObjects.Add(gameObject);
                    }
                    break;
                case UnityObjectType.Transform:
                    UnityTransform transform = ParseTransform(objectVariables);
                    if (transform.gameObjectId != null)
                    {
                        sceneTransforms.Add(transform);
                    }
                    break;
                case UnityObjectType.MonoBehaviour:
                    UnityMonoBehaviour? script = ParseMonoBehaviour(objectVariables);
                    if (script != null)
                    {
                        sceneScripts.Add(script);
                    }
                    break;
                default:
                    continue;
            }

        }

        return Tuple.Create(sceneGameObjects, sceneTransforms, sceneScripts);
    }
}

