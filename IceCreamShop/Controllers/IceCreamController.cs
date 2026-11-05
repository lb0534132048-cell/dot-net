using IceCreamShop.Interfaces;
using IceCreamShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace IceCreamShop.Controllers;

[ApiController]
[Route("[controller]")]
public class IceCreamController : ControllerBase
{
    private readonly IIceCreamService _iceCreamService;

    public IceCreamController(IIceCreamService iceCreamService)
    {
        _iceCreamService = iceCreamService;
    }

    [HttpGet]

    public ActionResult<List<IceCream>> GetAll() 
    {
          
         return _iceCreamService.GetAll();

    
    }     

    [HttpGet("{id:int}")]
    public ActionResult<IceCream> Get(int id)
    {
        
        var iceCream = _iceCreamService.Get(id);
        return iceCream is null ? NotFound() : iceCream;
    }

    [HttpPost]
    public ActionResult<IceCream> Create(IceCream iceCream)
    {
        _iceCreamService.Add(iceCream);
        return CreatedAtAction(nameof(Get), new { id = iceCream.Id }, iceCream);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, IceCream iceCream)
    {
        if (id != iceCream.Id)
        {
            return BadRequest();
        }

        if (_iceCreamService.Get(id) is null)
        {
            return NotFound();
        }

        _iceCreamService.Update(iceCream);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (_iceCreamService.Get(id) is null)
        {
            return NotFound();
        }

        _iceCreamService.Delete(id);
        return NoContent();
    }
}
