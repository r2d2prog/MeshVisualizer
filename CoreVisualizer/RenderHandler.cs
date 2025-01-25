using GlmSharp;
using System.Collections.Generic;


namespace CoreVisualizer
{
    public class RenderHandler
    {
        public Dictionary<string, ShaderProgramCreator> Programs { get; private set; }
        public DirectionalLight[] DirectionalLights { get; private set; }
        public RenderHandler() 
        {
            Programs = new Dictionary<string, ShaderProgramCreator>();
            DirectionalLights = new DirectionalLight[2];
            DirectionalLights[0] = new DirectionalLight(-vec3.UnitZ, new vec4(0.8f, 0.8f, 0.8f, 1.0f));
        }
    }
}
