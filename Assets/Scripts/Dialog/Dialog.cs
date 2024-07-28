/*
 * Author: CharSui
 * Created On: 2024.07.03
 * Description: 注意，这里是台词的最小单位。
 * 台词只用于战局中的对话系统
 * 而且不直接存储多语言的
 */

using Newtonsoft.Json;

namespace Dialog
{
    public struct Dialog
    {
        /// <summary>
        /// 显示的角色立绘
        /// TODO:这里的用法不是很好，但是可以通过提前约束立绘Index来实现表情差分，比如0-9是Lalako
        /// </summary>
        [JsonProperty("role_index")]
        public int roleIndex;
        
        /// <summary>
        /// 台词多语言Key
        /// </summary>
        [JsonProperty("dialog_key")]
        public string dialogContentKey;
    }
}
