/*
	六角形縁 by あるる（きのもと 結衣）
	Hex Line Shader by Yui Kinomoto @arlez80

	MIT License
*/

shader_type spatial;
render_mode unshaded;

uniform vec2 hex_size = vec2( 0.05, 0.08 );
uniform vec4 scan_color : source_color = vec4( 0.0, 1.0, 0.3, 1.0 );
uniform float scan_line_size : hint_range( 0.0, 0.025 ) = 0.001;

vec2 get_hex_uv( vec2 uv )
{
	bool half = mod( uv.y / 2.0, hex_size.y ) / hex_size.y < 0.5;
	vec2 half_shift_uv = uv + vec2( hex_size.x * 0.5 * float( half ), 0.0 );
	vec2 hex_uv = floor( half_shift_uv / hex_size ) * hex_size;
	vec2 norm_uv = mod( half_shift_uv, hex_size ) / hex_size;

	return hex_uv + mix(
		vec2( 0.0, 0.0 )
	,	mix(
			mix(
				vec2( hex_size.x, -hex_size.y )
			,	vec2( 0.0, -hex_size.y )
			,	float( norm_uv.x < 0.5 )
			)
		,	mix(
				vec2( 0.0, -hex_size.y )
			,	vec2( -hex_size.x, -hex_size.y )
			,	float( norm_uv.x < 0.5 )
			)
		,	float( half )
		)
	,	float( norm_uv.y < 0.3333333 ) * float( norm_uv.y / 0.3333333 < ( abs( norm_uv.x - 0.5 ) * 2.0 ) )
	);
}

void fragment( )
{
	vec2 hex_uv[9];

	for( int y=0; y<3; y ++ ) {
		for( int x=0; x<3; x ++ ) {
			hex_uv[y*3+x] = get_hex_uv( UV + vec2( scan_line_size * float( x - 1 ), scan_line_size * float( y - 1 ) ) );
		}
	}

	vec2 sobel_src_x = (
		hex_uv[0] * -1.0
	+	hex_uv[3] * -2.0
	+	hex_uv[6] * -1.0
	+	hex_uv[2] * 1.0
	+	hex_uv[5] * 2.0
	+	hex_uv[8] * 1.0
	);
	vec2 sobel_src_y = (
		hex_uv[0] * -1.0
	+	hex_uv[1] * -2.0
	+	hex_uv[2] * -1.0
	+	hex_uv[6] * 1.0
	+	hex_uv[7] * 2.0
	+	hex_uv[8] * 1.0
	);
	vec2 sobel = sqrt( sobel_src_x * sobel_src_x + sobel_src_y * sobel_src_y );

	ALBEDO = scan_color.rgb;
	ALPHA = scan_color.a * float( 0.01 < length( sobel ) );
}

	/*
		中心位置からの距離を出すけど、普通に二次元距離なので役にたたない
		六角形を三次元で扱う方法でやったほうがいい
	float hex_x1 = abs( mod( UV.x / size.x + 0.5, 1.0 ) - 0.5 );
	float hex_x2 = abs( mod( UV.x / size.x, 1.0 ) - 0.5 );
	vec2 dist_uv = vec2(
		mix(
			hex_x1
		,	hex_x2
		,	float( mod( UV.y / 2.0, size.y ) / size.y - hex_x1 * 0.3333333 < 0.0 )
		+	float( 0.0 < mod( UV.y / 2.0, size.y ) / size.y - 0.5 - hex_x2 * 0.3333333 )
		) * 1.666666
	,	abs( hex_uv.y - half_shift_uv.y + size.y * 0.65 ) / size.y
	);
	*/
