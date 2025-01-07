#version 330 core
layout (location = 0) in vec2 position;
layout (location = 1) in mat4 model;
uniform mat4 projection;
uniform mat4 view;
out vec2 texCoords;
out float layer;
void main()
{
	vec3 viewRight = vec3(view[0][0], view[1][0], view[2][0]);
    vec3 viewUp = vec3(view[0][1], view[1][1], view[2][1]);
    vec3 newPos = viewRight * position.x + viewUp * position.y;
    gl_Position = projection * view * model * vec4(newPos, 1.0);
    texCoords = position;
    layer = float(gl_InstanceID);
}