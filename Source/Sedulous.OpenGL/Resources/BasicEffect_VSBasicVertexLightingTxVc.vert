﻿#includeres "Sedulous.OpenGL.Resources.BasicEffectPreamble.glsl" executing

 in vec4 uv_Position0;
 in vec3 uv_Normal0;
 in vec2 uv_TextureCoordinate0;
 in vec4 uv_Color0;

out vec4 vDiffuse;
out vec4 vSpecular;
out vec2 vTexCoord;
out vec4 vPositionPS;

void main()
{
	CommonVSOutput cout = ComputeCommonVSOutputWithLighting(uv_Position0, uv_Normal0, 3);
	SetCommonVSOutputParams;
	
	vTexCoord = FlipTextureCoordinates(uv_TextureCoordinate0);
	vDiffuse *= ConvertColor(uv_Color0);
}