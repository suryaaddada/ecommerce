using Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly DBContext context;
        public OrderController(DBContext _context)
        {
            context = _context;
        }

        [HttpGet("Get All Orders Based On User Id/{id}")]
        public IActionResult Get_All_OrderDetails(int id)
        {
            try
            {
                var user = context.Users.Find(id);
                if (user != null)
                {


                    var orderid = context.Orders.Where(x => x.UserId == id).Select(x => x.Id).ToList();
                    if (orderid.Any())
                    {
                        var items = (from oi in context.OrderedItems
                                     join p in context.Products
                                   on oi.ProductId equals p.Id
                                     where orderid.Contains((int)oi.OrderId)
                                     select new
                                     {
                                         p.BrandName,
                                         p.Image,
                                         p.Description,
                                         p.Category,
                                         p.SubCategory,
                                         p.Color,
                                         oi.Quantity
                                     }).ToList();
                       
                            return Ok(items);
                       
                    }
                    else
                    {
                        return StatusCode(404, "No orders found");
                    }
                }
                return StatusCode(404, "User Not Found");
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("Get Ordered Items BY orderId/{id}")]
        public IActionResult Get_OrderedItems_By_Order_Id(int id)
        {
            try
            {
                var ordered_items = (from oi in context.OrderedItems join p in context.Products
                                   on oi.ProductId equals p.Id where oi.OrderId == id
                                     select new
                                     {
                                         p.BrandName, p.Image, p.Description, p.Category,
                                         p.SubCategory, p.Color, oi.Quantity
                                     }).ToList();

                if (ordered_items.Any())
                {
                    return Ok(ordered_items);
                }
                else
                    return StatusCode(404, "No orders Found");
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Get Orders By userId/{id}")]
        public IActionResult Get_Orders_By_UserId(int id)
        {
            try
            {
                var orders = context.Orders.Where(x => x.UserId == id).ToList();
                if(orders.Any())
                {
                    return Ok(orders);
                }
                return StatusCode(404, "Orders NOt Found");
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
