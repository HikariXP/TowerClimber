/*
 * Author: CharSui
 * Created On: 2024.07.27
 * Description: 可以后期改造成通用工具
 * 资源管理器，给实际业务做一层抽象隔离
 */

using System;

public class AssetsManager
{
    private static IAssetProvider s_AssetProvider;

    public static void Initialize(Action<bool> callBack)
    {
        if (s_AssetProvider == null)
        {
            return;
        }
    }


}
