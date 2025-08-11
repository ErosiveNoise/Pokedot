using Godot;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;

public class PokemonDownloader
{
    private const string ApiBase = "https://pokeapi.co/api/v2/";
    private System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
    private Dictionary<string, object> pokedex = new();

    public void Main()
    {
        _ = DownloadAllPokemonAsync(); // Fire and forget
    }

    private async Task DownloadAllPokemonAsync()
    {
        for (int id = 1; id <= 149; id++)
        {
            GD.Print($"Scarico Pokémon #{id}...");
            var pokemonJson = await GetJson($"{ApiBase}pokemon/{id}");

            if (pokemonJson == null) continue;

            string name = await GetNameInItalian((string)pokemonJson["species"]["url"]);
            int hp = pokemonJson["stats"][0]["base_stat"].Value<int>();
            int level = 5 + (id % 10);

            List<string> types = new();
            foreach (var t in pokemonJson["types"])
            {
                string typeUrl = t["type"]["url"].ToString();
                string typeName = await GetLocalizedName(typeUrl, "it");
                types.Add(typeName);
            }

            // Prendi 4 mosse con Power valido
            var moves = new List<Attack>();
            foreach (var m in pokemonJson["moves"])
            {
                var details = m["version_group_details"];
                foreach (var d in details)
                {
                    string method = d["move_learn_method"]["name"].ToString();
                    int learnedAt = d["level_learned_at"].Value<int>();
                    string version = d["version_group"]["name"].ToString();

                    if (method == "level-up" && learnedAt <= level && version == "red-blue")
                    {
                        string moveUrl = m["move"]["url"].ToString();
                        var moveData = await GetJson(moveUrl);
                        if (moveData?["power"] != null && moveData["power"].Type != JTokenType.Null)
                        {
                            string moveName = await GetLocalizedName(moveUrl, "it");
                            int power = moveData["power"].Value<int>();
                            int accuracy = moveData["accuracy"]?.Value<int>() ?? 0;
                            int pp = moveData["pp"]?.Value<int>() ?? 0;
                            string type = await GetLocalizedName(moveData["type"]["url"].ToString(), "it");
                            if (!moves.Any<Attack>(m => m.Name == moveName))
                            {
                                moves.Add(new Attack
                                {
                                    Name = moveName,
                                    Power = power,
                                    Accuracy = accuracy,
                                    PP = pp,
                                    CurrentPP = pp,
                                    Type = type
                                });
                            }
                        }
                    }
                }

                if (moves.Count == 4)
                    break;
            }

            // Riempie con placeholder fino a 4 mosse
            while (moves.Count < 4)
            {
                moves.Add(new Attack
                {
                    Name = "—",
                    Power = 0,
                    Accuracy = 0,
                    PP = 0,
                    Type = "—"
                });
            }

            var pokeEntry = new Dictionary<string, object>
            {
                ["Name"] = name,
                ["Hp"] = hp,
                ["CurrentHp"] = hp,
                ["Level"] = level,
                ["Exp"] = 50 + (id * 3),
                ["Type"] = string.Join("/", types),
                ["Attacks"] = moves
            };

            pokedex[id.ToString()] = pokeEntry;
            await Task.Delay(100); // Per non sovraccaricare PokéAPI
        }

        SaveJsonToFile();
        GD.Print("Pokédex generato!");
    }

    private async Task<JObject> GetJson(string url)
    {
        try
        {
            var res = await client.GetStringAsync(url);
            return JObject.Parse(res);
        }
        catch
        {
            GD.PrintErr("Errore nel recupero di: " + url);
            return null;
        }
    }

    private async Task<string> GetNameInItalian(string speciesUrl)
    {
        var data = await GetJson(speciesUrl);
        foreach (var name in data["names"])
        {
            if (name["language"]["name"].ToString() == "it")
                return name["name"].ToString();
        }
        return data["name"].ToString();
    }

    private async Task<string> GetLocalizedName(string url, string lang)
    {
        var data = await GetJson(url);
        foreach (var name in data["names"])
        {
            if (name["language"]["name"].ToString() == lang)
                return name["name"].ToString();
        }
        return data["name"].ToString();
    }

    private void SaveJsonToFile()
    {
        var jsonString = JsonConvert.SerializeObject(pokedex, Formatting.Indented);
        string path = ProjectSettings.GlobalizePath("user://pokedex.json");
        File.WriteAllText(path, jsonString);
        GD.Print($"File salvato in: {path}");
    }
}
