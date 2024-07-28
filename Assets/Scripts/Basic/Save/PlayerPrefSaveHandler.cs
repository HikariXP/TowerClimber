using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Basic.Save
{
    public class PlayerPrefSaveHandler : SaveHandler
    {
        public void Save()
        {
            PlayerPrefs.Save();
        }

        public void Load()
        {
            // PlayerPref Load when game start
        }

        public void SetInt()
        {
            throw new System.NotImplementedException();
        }

        public int GetInt()
        {
            throw new System.NotImplementedException();
        }

        public void SetFloat()
        {
            throw new System.NotImplementedException();
        }

        public float GetFloat()
        {
            throw new System.NotImplementedException();
        }

        public void SetString()
        {
            throw new System.NotImplementedException();
        }

        public string GetString()
        {
            throw new System.NotImplementedException();
        }
    }
}



