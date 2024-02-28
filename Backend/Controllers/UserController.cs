using Backend.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;


namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DBContext context;
        private readonly IConfiguration configuration;

        public UserController(DBContext _context, IConfiguration configuration)
        {
            context = _context;
            this.configuration = configuration;
        }

        [HttpGet("Get Users")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = context.Users.ToList();
                if (users.Any())
                {
                    return Ok(users);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest("Error");
            }
        }

        [HttpGet("user/{id}")]
        public IActionResult GetUser(int id)
        {
            var user = context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost("Add User")]
        public IActionResult AddUser([FromBody] User user)
        {
            try
            {
                context.Users.Add(user);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Delete User/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    //from wishlist 
                    var wishlist = await context.Wishlists.Where(x => x.UserId==id).ToListAsync(); 
                    if(wishlist.Any())
                    {
                        context.Wishlists.RemoveRange(wishlist);
                       // await context.SaveChangesAsync();
                    }
                    //Cart
                    var cart=await context.Carts.Where(x => x.UserId == id).ToListAsync();
                    if(cart.Any())
                    {  context.Carts.RemoveRange(cart);}

                    //Shipping

                    var shipping=await context.Shippings.Where(x => x.Userid==id).ToListAsync();
                    if(shipping.Any())
                    { context.Shippings.RemoveRange(shipping);}



                    // Orders
                    var orders=await context.Orders.Where(x => x.UserId== id).ToListAsync();
                    
                    if(orders.Any())
                    {
                        //Ordered_Items
                        var orderIds = orders.Select(o => o.Id).ToList();
                        //var ordered_items=await context.OrderedItems.Where(y=>orders.Any(o=>o.Id==y.OrderId)).ToListAsync();
                        var ordered_items=await context.OrderedItems.Where(x=>orderIds.Contains((int)x.OrderId)).ToListAsync(); 
                        if(ordered_items.Any())
                        { context.OrderedItems.RemoveRange(ordered_items);}

                        context.Orders.RemoveRange(orders);
                    }



                    
                    
                    context.Users.Remove(user);
                    await context.SaveChangesAsync();
                    return Ok();
                }
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("Update User/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            try
            {
                var user = context.Users.Find(id);
                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    user.FirstName = updatedUser.FirstName;
                    user.LastName = updatedUser.LastName;
                    user.Email = updatedUser.Email;
                    user.Password = updatedUser.Password;
                    user.Gender = updatedUser.Gender;
                    user.Mobile = updatedUser.Mobile;
                    user.Role = updatedUser.Role;
                    context.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //Shipping Details

        [HttpGet("Get All Shipping/{userid}")]
        public IActionResult Get_AllShipping_Details(int userid)
        {
            try
            {
                var user = context.Users.Find(userid);
                if (user == null)
                { return StatusCode(400, "User details not found"); }
                else
                {
                    var shipping = context.Shippings.Where(x => x.Userid == userid).ToList();
                    if (shipping.Any())
                    {
                        var ids = shipping.Select(x => x.Id).ToList();
                        var result = shipping.Where(x => ids.Contains(x.Id)).Select(x => new
                        {
                            x.Id,
                            x.Name,
                            x.Address,
                            x.City,
                            x.Pincode,x.Country,x.Mobile,x.State,
                        }).ToList();
                        return Ok(result);
                    }
                    else
                        return StatusCode(400, "Shipping address not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetShipping By Id/{id}")]
        public IActionResult Get_Shipping_By_Id(int id)
        {
            try
            {
                var shipping = context.Shippings.Find(id);
                if(shipping == null)
                {
                    return StatusCode(404, "Address Not found");
                }
                return Ok(shipping);

            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Delete Shipping/{id}")]
        public IActionResult Delete_Shipping(int id)
        {
            try
            {
                var shipping = context.Shippings.Find(id);
                if (shipping == null)
                {
                    return StatusCode(404, "Not Found");
                }
                else
                    context.Shippings.Remove(shipping);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Add Shipping")]
        public IActionResult Add_Shipping([FromBody] Shipping shipping)
        {
            try
            {
                context.Shippings.Add(shipping);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update Shipping/{id}")]
        public IActionResult Update_shipping(int id, [FromBody] Shipping shipping)
        {
            try
            {
                var Updated_Shipping = context.Shippings.Find(id);
                if (Updated_Shipping == null)
                {
                    return NotFound();
                }
                else
                {
                    Updated_Shipping.Userid = shipping.Userid;
                    Updated_Shipping.Name = shipping.Name;
                    Updated_Shipping.Address = shipping.Address;
                    Updated_Shipping.City = shipping.City;
                    Updated_Shipping.Pincode = shipping.Pincode;
                    Updated_Shipping.Country = shipping.Country;
                    Updated_Shipping.Mobile = shipping.Mobile;
                    Updated_Shipping.State = shipping.State;
                    context.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    } 

    

}
