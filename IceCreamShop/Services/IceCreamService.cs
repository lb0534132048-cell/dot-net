using System.Text.Json;
using IceCreamShop.Interfaces;
using IceCreamShop.Models;

namespace IceCreamShop.Services;

public class IceCreamService : IIceCreamService
{
    private readonly List<IceCream> _iceCreams;
    private readonly string _filePath;

    public IceCreamService(IWebHostEnvironment webHost)
    {
        _filePath = Path.Combine(webHost.ContentRootPath, "Data", "icecream.json");
        using var jsonFile = File.OpenText(_filePath);
        _iceCreams = JsonSerializer.Deserialize<List<IceCream>>(
            jsonFile.ReadToEnd(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidDataException($"The menu file '{_filePath}' contains no JSON array.");
    }

    public List<IceCream> GetAll() => _iceCreams;

    public IceCream? Get(int id) => _iceCreams.FirstOrDefault(iceCream => iceCream.Id == id);

    public void Add(IceCream iceCream)
    {
        iceCream.Id = _iceCreams.Count == 0 ? 1 : _iceCreams.Max(item => item.Id) + 1;
        _iceCreams.Add(iceCream);
        SaveToFile();
    }

    public void Delete(int id)
    {
        var iceCream = Get(id);
        if (iceCream is null)
        {
            return;
        }

        _iceCreams.Remove(iceCream);
        SaveToFile();
    }

    public void Update(IceCream iceCream)
    {
        var index = _iceCreams.FindIndex(item => item.Id == iceCream.Id);
        if (index < 0)
        {
            return;
        }

        _iceCreams[index] = iceCream;
        SaveToFile();
    }

    public int Count => _iceCreams.Count;

    private void SaveToFile()
    {
        File.WriteAllText(_filePath, JsonSerializer.Serialize(
            _iceCreams,
            new JsonSerializerOptions { WriteIndented = true }));
    }
}
