using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Basic.Save
{
    public interface SaveHandler
    {
        public void Save();

        public void Load();
        
        public void SetInt();
        
        public int GetInt();

        public void SetFloat();

        public float GetFloat();

        public void SetString();

        public string GetString();
    }
}
