/*
 * Author: CharSui
 * Created On: 2024.07.27
 * Description: 资源管理器的接口，在使用具体某个资源框架进行资源读取的时候，需要基于这个规则进行重写
 */
using System;


public interface IAssetProvider
{
    /// <summary>
    /// 初始化函数
    /// </summary>
    public void Initialize(Action<bool> callBack);

    public abstract T GetAsset<T>() where T : UnityEngine.Object;
}
