#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in mat4 model;
uniform mat4 projection;
uniform mat4 view;
flat out vec4 inColor;
void main()
{
	vec4 color[3];
	color[0] = vec4(1.0, 0.0, 0.0, 1.0);
	color[1] = vec4(0.0, 1.0, 0.0, 1.0);
	color[2] = vec4(0.0, 0.0, 1.0, 1.0);
	gl_Position = projection * view * model * vec4(position, 1.0);
	inColor = color[gl_InstanceID];
}
        