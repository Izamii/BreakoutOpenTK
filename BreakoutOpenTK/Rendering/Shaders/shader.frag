#version 330

out vec4 outputColor;
in vec2 vTexCoord;

uniform sampler2D tex;
uniform vec3 color;

void main()
{
    outputColor = vec4(color, 1.0f) * texture(tex, vTexCoord);
}