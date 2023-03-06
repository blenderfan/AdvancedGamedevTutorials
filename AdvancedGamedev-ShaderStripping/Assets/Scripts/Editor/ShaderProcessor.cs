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
                        //It's a weird way to loop through it, when you could just go backwards?
                        //Well, I copied it from Unity's documentation, so maybe there is a reason, or everybody
                        //just always copies it like that ^^
                        data.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }
        }
    }
}
