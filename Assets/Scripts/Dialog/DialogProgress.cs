/*
 * Author: CharSui
 * Created On: 2024.07.17
 * Description: 对话进度结构
 */

using Newtonsoft.Json;

namespace Dialog
{
    public struct DialogProgress
    {
        [JsonProperty("progress")]
        public int progress;
        
        [JsonProperty("dialog_summary")]
        public string dialogSummary;

        [JsonProperty("dialog_context")]
        public Dialog[] dialogContext;
    }
}