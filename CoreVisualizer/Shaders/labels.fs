#version 330 core
in vec2 texCoords;
in float layer;
uniform sampler2DArray labels;
void main()
{
	vec3 color[3];
    color[0] = vec3(1.0, 0.0, 0.0);
    color[1] = vec3(0.0, 1.0, 0.0);
    color[2] = vec3(0.0, 0.0, 1.0);
    float scale = texture(labels, vec3(texCoords.x, 1 - texCoords.y, layer)).r;
    gl_FragColor = vec4(scale * color[int(layer)], scale);
}