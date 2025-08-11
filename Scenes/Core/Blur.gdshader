shader_type canvas_item;

uniform sampler2D mask;
uniform float blur_radius = 8.0;

void fragment() {
    vec2 uv = UV;
    vec2 tex_size = vec2(textureSize(TEXTURE, 0)); // Cast corretto
    vec4 col = vec4(0.0);
    float total = 0.0;

    int radius = int(blur_radius);

    for (int x = -radius; x <= radius; x++) {
        for (int y = -radius; y <= radius; y++) {
            vec2 offset = vec2(float(x),float(y)) / tex_size; // Ora valido
            float weight = 1.0 - length(vec2(float(x),float(y))) / blur_radius;
            weight = max(weight, 0.0);
            col += texture(TEXTURE, uv + offset) * weight;
            total += weight;
        }
    }

    vec4 blur_color = col / total;
    float mask_val = texture(mask, uv).r;
    COLOR = mix(blur_color, texture(TEXTURE, uv), mask_val);
}
