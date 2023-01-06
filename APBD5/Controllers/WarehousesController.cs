using APBD5.Models;
using APBD5.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD5.Controllers
{
    [ApiController]
    [Route("api/warehouses")]
    public class WarehousesController : ControllerBase
    {
        private IDbService dbService;

        public WarehousesController(IDbService dbService)
        {
            this.dbService = dbService;
        }

        [HttpPost]
        public IActionResult CreateProductWarehouse(ProductWarehouse pw)
        {
            if (dbService.ProductExists(pw.IdProduct) && dbService.WarehouseExists(pw.IdWarehouse) && pw.Amount > 0)
            {
                if (dbService.OrderExists(pw.IdProduct, pw.Amount) && dbService.IsDateCorrect(pw.CreatedAt, pw.IdProduct, pw.Amount))
                {
                    if (dbService.OrderCompleted(pw.IdProduct, pw.Amount))
                    {
                        dbService.UpdateTime(pw.IdProduct, pw.Amount);
                        return Ok(dbService.CreateProductWarehouse(pw.IdProduct, pw.IdWarehouse, pw.Amount, pw.CreatedAt));
                    }
                    else return BadRequest("Already completed order");
                }
                else return NotFound("Order doesn't exist");
            }
            else return NotFound("The product/warehouse doesn't exist");
        }
    
}
}
