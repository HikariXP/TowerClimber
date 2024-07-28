/*
 * Author: CharSui
 * Created On: 2024.07.18
 * Description: 数据库的基础序列化接口
 */

using ExtendTool;
using UnityEngine;

public abstract class ISerialize
{
    public void Deserialize(string jsonData)
    {
        if (jsonData.IsNullOrEmpty())
        {
            Debug.LogError($"[{nameof(ISerialize)}]You are trying to serialize with null string");
            return;
        }

        DeserializeOverride(jsonData);
    }

    protected virtual void DeserializeOverride(string jsonData)
    {
    }

    public virtual string Serialize()
    {
        return string.Empty;
    }
}