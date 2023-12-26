using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum UnityObjectType
{
    GameObject,
    Transform,
    MonoBehaviour,
    Other,
}

public class UnityGameObject
{

    public string id;
    public string name;

    public UnityGameObject(string id, string name)
    {
        this.id = id;
        this.name = name;
    }

    override public string ToString()
    {
        return String.Format("id: {0}\nname: {1}\n", this.id, this.name);
    }
}

public class UnityTransform
{

    public string id;
    public string? parent = null;
    public string? gameObjectId = null;
    public string[]? children = null;

    public UnityTransform(string id, string? parent, string[]? children, string? gameObjectId)
    {
        this.id = id;
        this.parent = parent;
        this.children = children;
        this.gameObjectId = gameObjectId;
    }

    override public string ToString()
    {
        return String.Format("id: {0}\nparent: {1}\nchildren: {2}\n",
                              this.id, this.parent, this.children);
    }
}

public class UnityMonoBehaviour
{

    public string id;
    public string guid;

    public UnityMonoBehaviour(string id, string guid)
    {
        this.id = id;
        this.guid = guid;
    }

    override public string ToString()
    {
        return String.Format("id: {0}\nguid: {1}\n",
                              this.id, this.guid);
    }
}

