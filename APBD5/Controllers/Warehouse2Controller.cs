using APBD5.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APBD5.Controllers
{
    [Route("api/warehouse")]
    [ApiController]
    public class Warehouse2Controller : ControllerBase
    {
        private IDbService dbService;

        public Warehouse2Controller(IDbService dbService)
        {
            this.dbService = dbService;
        }

        [HttpPost]
        public IActionResult CreateProductWarehouse(ProductWarehouse productWarehouse)
        {
            return Ok(dbService.CreateProductWarehouseWithProcedure(productWarehouse.IdProduct, productWarehouse.IdWarehouse, productWarehouse.Amount, productWarehouse.CreatedAt));
        }
    }
}
