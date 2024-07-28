/*
 * Author: CharSui
 * Created On: 2024.07.14
 * Description: 挂载在UI上，并且接收订阅，填入Key值即可。
 */

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Localization
{
    [RequireComponent(typeof(Text))]
    public class TextLocalization : UILocalizationBase
    {
        private Text _text;

        protected override void Init()
        {
            _text = gameObject.GetComponent<Text>();
            base.Init();
        }

        protected override void RefreshLocalization(string localizationText)
        {
            _text.text = localizationText;
        }
    }
}