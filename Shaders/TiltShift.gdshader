shader_type canvas_item;

uniform float limit: hint_range(0.0,0.5) = 0.2;
uniform float blur: hint_range(0.0,5.0) = 2.0;
uniform float intensity: hint_range (0.0, 1) = 0.19;
uniform bool debug = true;

void fragment()
{
    vec4 color = textureLod(SCREEN_TEXTURE, SCREEN_UV, blur);
    if (debug)
    {
        color = vec4(1.0, 1.0, 1.0, 1.0);
    }
    if (distance(UV, vec2(0.5,0.5)) > limit)
    {
        color.a = smoothstep(distance(UV, vec2(0.5,0.5)),limit,intensity);
    }
    else
    {
        color.a = 0.0f;
    }
    COLOR = color;
}