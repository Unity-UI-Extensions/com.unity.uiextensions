/// Credit SimonDarksideJ

using System.Collections.Generic;
using UnityEditor;

namespace UnityEngine.UI.Extensions
{
    [InitializeOnLoadAttribute]
    public static class ShaderLibrary
    {
        public static Dictionary<string, Shader> shaderInstances = new Dictionary<string, Shader>();
        public static Shader[] preLoadedShaders;

        public static Shader GetShaderInstance(string shaderName)
        {
            if (shaderInstances.ContainsKey(shaderName))
            {
                return shaderInstances[shaderName];
            }

            var newInstance = Shader.Find(shaderName);
            if (newInstance != null)
            {
                shaderInstances.Add(shaderName, newInstance);
            }
            return newInstance;
        }
    }
}
