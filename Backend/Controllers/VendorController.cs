using Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Net.Mime;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly DBContext context;
        private readonly IConfiguration configuration;
        public VendorController(DBContext _context, IConfiguration configuration)
        {
            context = _context;
            this.configuration = configuration;
        }

        [HttpGet("Get All Vendors")]
        public IActionResult Get_All_Vendors()
        {
            try
            {
                var vendors = context.Vendors.Where(x=>x.Status=="Active").ToList();
                if (vendors.Any())
                {
                    return Ok(vendors);
                }
                return StatusCode(404, "Not Found");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Get Vendor/{id}")]
        public IActionResult Get_Vendor(int id)
        {
            try
            {
                var vendor = context.Vendors.Find(id);
                if (vendor == null)
                    return NotFound();
                else
                    return Ok(vendor);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Add Vendor")]
        public IActionResult Add_Vendor([FromBody] Vendor vendor)
        {
            try
            {
                context.Vendors.Add(vendor);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Delete Vendor/{id}")]
        public IActionResult Delete_Vendor(int id)
        {
            try
            {
                var vendor = context.Vendors.Find(id);
                if (vendor == null)
                {
                    return StatusCode(404, "Not Found");

                }
                else
                {
                    vendor.Status = "Inactive";
                    vendor.Isapproved = true;
                    var products = context.Products.Where(x => x.VendorId == id).ToList();
                    if(products.Any())
                    {
                     

                        foreach(var p in products)
                        {
                            p.Status = "Inactive"; 
                            List<Inventory> inventory=context.Inventories.Where(x=>x.ProductId==p.Id).ToList();
                            if(inventory.Any())
                            {
                                context.Inventories.RemoveRange(inventory);
                            }
                        }
                       
                    }

                    context.SaveChanges();
                    return StatusCode(200,"Done");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update Vendor/{id}")]
        public IActionResult Update_Vendor(int id, [FromBody] Vendor vendor)
        {
            try
            {
                var Updatedvendor = context.Vendors.Find(id);
                if (Updatedvendor == null)
                {
                    return StatusCode(404, "Vendor Not Found");
                }
                else
                {
                    Updatedvendor.Name = vendor.Name;
                    Updatedvendor.Mobile = vendor.Mobile;
                    Updatedvendor.Email = vendor.Email;
                    Updatedvendor.Password = vendor.Password;
                    Updatedvendor.Address = vendor.Address;
                    Updatedvendor.City = vendor.City;
                    Updatedvendor.State = vendor.State;
                    Updatedvendor.Pincode = vendor.Pincode;
                    Updatedvendor.Isapproved = vendor.Isapproved;
                    Updatedvendor.Status = vendor.Status;

                    context.SaveChanges();
                    return Ok();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPatch("ChangePassword/{id}")]
        public IActionResult ChangePassword(int id, string password)
        {
            var vendor = context.Vendors.Find(id);
            if (vendor == null)
            {
                return StatusCode(404, "Vendor Not Found");
            }
            else
            {
                vendor.Password = password;
                context.SaveChanges();
                return Ok(vendor);
            }

        }


    }
}
