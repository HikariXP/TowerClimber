/*
 * Author: CharSui
 * Created On: 2024.07.18
 * Description: 一些基础的字符串类拓展
 */

namespace ExtendTool
{
    public static class StringExtend
    {
        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        public static bool IsNullOrWhiteSpace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }
    }
}