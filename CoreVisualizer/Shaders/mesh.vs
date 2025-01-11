#version 330 core
#define USE_MATERIAL
#define TANGENT_SPACE
layout (location = 0) in vec3 position;
layout (location = 1) in vec4 color;
layout (location = 2) in vec2 uvs;
layout (location = 3) in vec3 normal;
layout (location = 4) in vec3 tangent;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;
uniform mat3 normalMatrix;

uniform vec3 lightDir;
uniform vec4 lightColor;
uniform vec3 viewPos;

uniform vec4 ambientColor;
uniform vec4 diffuseColor;
uniform vec4 specularColor;
uniform vec4 emmisiveColor; 
uniform float shinStrength;

out VS_OUT
{
    vec4 inColor;
    vec2 inUvs;
    vec3 tangentLightDir;
    vec3 tangentViewPos;
    vec3 tangentFragPos;
    #if defined(MODEL_SPACE)
        vec3 tNormal;
    #endif
}vs_out;

void main()
{
    vec3 fragPos = vec3(model * vec4(position, 1.0));  
    vs_out.inUvs = uvs;
    #if defined(USE_MATERIAL)
        vec3 ambient = vec3(lightColor * ambientColor);

        vec3 norm = normalize(normal);
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = vec3(lightColor * (diff * diffuseColor));

        vec3 viewDir = normalize(viewPos - fragPos);
        vec3 reflectDir = reflect(-lightDir, norm);  
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), round(shinStrength * 0.5));
        vec3 specular = vec3(lightColor * (spec * specularColor));  

        vec3 result = ambient + diffuse + specular;
        vs_out.inColor = vec4(result,1);
    #else
        vec3 N = normalize(normalMatrix * normal);
        #if defined(TANGENT_SPACE)
            vec3 T = normalize(normalMatrix * tangent);
            T = normalize(T - dot(T, N) * N);
            vec3 B = cross(N, T);
            mat3 TBN = transpose(mat3(T, B, N));
    
            vs_out.tangentLightDir = TBN * lightDir;
            vs_out.tangentViewPos  = TBN * viewPos;
            vs_out.tangentFragPos  = TBN * fragPos;
        #else
            vs_out.tNormal = N;
            vs_out.tangentLightDir = lightDir;
            vs_out.tangentViewPos  = viewPos;
            vs_out.tangentFragPos  = fragPos;
        #endif
    #endif
    gl_Position = projection * view * model * vec4(position, 1.0);
}