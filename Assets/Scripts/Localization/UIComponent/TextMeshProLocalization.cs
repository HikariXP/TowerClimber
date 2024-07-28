/*
 * Author: CharSui
 * Created On: 2024.07.14
 * Description: 基于TMP的多语言处理
 */
using TMPro;
using UnityEngine;

namespace Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextMeshProLocalization : UILocalizationBase
    {
        private TextMeshProUGUI _textMeshProUGUI;

        protected override void Init()
        {
            _textMeshProUGUI = gameObject.GetComponent<TextMeshProUGUI>();
            base.Init();
        }

        protected override void RefreshLocalization(string localizationText)
        {
            _textMeshProUGUI.text = localizationText;
        }
    }
}
