﻿#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec3 color;
layout (location = 2) in vec3 uvs;
layout (location = 3) in vec3 normal;
layout (location = 4) in vec3 tangent;
uniform mat4 view;
uniform mat4 model;
uniform mat4 projection;
void main()
{
	gl_Position = projection * view * model * vec4(position, 1.0);
}