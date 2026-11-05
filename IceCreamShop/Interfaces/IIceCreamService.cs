using IceCreamShop.Models;

namespace IceCreamShop.Interfaces;

public interface IIceCreamService
{
    List<IceCream> GetAll();
    IceCream? Get(int id);
    void Add(IceCream iceCream);
    void Delete(int id);
    void Update(IceCream iceCream);
    int Count { get; }
}
