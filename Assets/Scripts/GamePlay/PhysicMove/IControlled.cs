/*
 * Author: CharSui
 * Created On: 2024.08.19
 * Description: 实现内容来作为可被控制的内容
 */

using UnityEngine;

public abstract class IControlled : MonoBehaviour
{
    private ISListener _inputListener;

    public void RegisterInputListenner(ISListener inputListener)
    {
        if(inputListener == null)return;
        _inputListener = inputListener;
        RegisterInputEvent(_inputListener);
    }

    public void UnregisterInputListener()
    {
        UnregisterInputEvent();
        _inputListener = null;
    }

    /// <summary>
    /// 注册输入
    /// </summary>
    protected abstract void RegisterInputEvent(ISListener inputListener);

    /// <summary>
    /// 注销订阅
    /// </summary>
    protected abstract void UnregisterInputEvent();
}
