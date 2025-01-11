#version 330 core
#define USE_MATERIAL
#define TANGENT_SPACE

uniform sampler2D diffuse;
uniform sampler2D normal;
uniform sampler2D shininess;

uniform vec4 ambientColor;
uniform vec4 diffuseColor;
uniform vec4 specularColor;
uniform vec4 emmisiveColor; 
uniform float shinStrength;

in VS_OUT
{
    vec4 inColor;
    vec2 inUvs;
    vec3 tangentLightDir;
    vec3 tangentViewPos;
    vec3 tangentFragPos;
    #if defined(MODEL_SPACE)
        vec3 tNormal;
    #endif
}fs_in;

void main()
{
    #if defined(USE_MATERIAL)
        gl_FragColor = fs_in.inColor;
    #else
        #if defined(TANGENT_SPACE)
            vec3 tNormal = texture2D(normal, fs_in.inUvs).rgb;
            tNormal = normalize(tNormal * 2.0 - 1.0); 
        #else
            vec3 tNormal = normalize(fs_in.tNormal);
        #endif
        vec4 color = texture2D(diffuse, fs_in.inUvs);

        float shin = round(texture2D(shininess, fs_in.inUvs).r * 128.0);
        vec3 ambient = 0.5 * color.rgb;
        float diff = max(dot(fs_in.tangentLightDir, tNormal), 0.0);
        vec3 dColor = diff * color.rgb;

        vec3 viewDir = normalize(fs_in.tangentViewPos - fs_in.tangentFragPos);
        vec3 reflectDir = reflect(-fs_in.tangentLightDir, tNormal);
        vec3 halfwayDir = normalize(fs_in.tangentLightDir + viewDir);  
        float spec = pow(max(dot(tNormal, halfwayDir), 0.0), shin);
        vec3 specular = vec3(specularColor) * spec;

        gl_FragColor = vec4(ambient + dColor + specular, 1.0);
    #endif
}