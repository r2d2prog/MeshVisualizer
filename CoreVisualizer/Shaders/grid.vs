#version 330 core
layout (location = 0) in vec3 position;
layout(location = 1) in vec4 color;
uniform mat4 perspective;
uniform mat4 view;
flat out vec4 inColor;
void main()
{
	gl_Position = perspective * view * vec4(position, 1.0);
	inColor = color;
}
