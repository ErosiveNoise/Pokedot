shader_type canvas_item;

uniform vec2 player_position; // In screen space (pixels)
uniform float radius = 150.0;
uniform float blur_strength = 4.0;

void fragment() {
    vec2 uv = SCREEN_UV;
    vec2 player_uv = player_position / vec2(textureSize(SCREEN_TEXTURE, 0));
    float dist = distance(uv, player_uv);

    vec4 col = vec4(0.0);
    int samples = 4;

    if (dist > radius / textureSize(SCREEN_TEXTURE, 0).x) {
        // Apply blur
        for (int x = -samples; x <= samples; x++) {
            for (int y = -samples; y <= samples; y++) {
                vec2 offset = vec2(x, y) * blur_strength / textureSize(SCREEN_TEXTURE, 0);
                col += texture(SCREEN_TEXTURE, uv + offset);
            }
        }
        col /= pow((samples * 2 + 1), 2.0);
    } else {
        // No blur near the player
        col = texture(SCREEN_TEXTURE, uv);
    }

    COLOR = col;
}
