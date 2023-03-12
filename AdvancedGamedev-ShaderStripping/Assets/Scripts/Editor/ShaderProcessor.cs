using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace ShaderStripping
{
    public class ShaderProcessor : IPreprocessShaders
    {
        public int callbackOrder => 0;

        private ShaderKeyword[] keywordsToRemove;

        public ShaderProcessor()
        {
            this.keywordsToRemove = new ShaderKeyword[3];
            
            this.keywordsToRemove[0] = new ShaderKeyword("GREEN");
            this.keywordsToRemove[1] = new ShaderKeyword("YELLOW");
            this.keywordsToRemove[2] = new ShaderKeyword("SEMI_TRANSPARENT");
        }

        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {

            for(int i = data.Count - 1; i >= 0; i--)
            {
                var keywords = data[i].shaderKeywordSet.GetShaderKeywords();

                for(int j = 0; j < this.keywordsToRemove.Length; j++)
                {
                    if(keywords.Contains(this.keywordsToRemove[j]))
                    {
                        data.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}
