using Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using System;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DBContext context;
        public ProductController(DBContext _context)
        {
            context = _context;
        }

        [HttpGet("GetAllProducts")]
        public IActionResult GetAllProducts()
        {
            try
            {
                var products = context.Products.Where(x=>x.Status=="Active").ToList();
                if (products.Any())
                {
                    return Ok(products);
                }
                return StatusCode(404, "Not Found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetProduct/{id}")]


        public IActionResult GetProduct(int id)
        {
            try
            {
                var product = context.Products.Find(id);
                if (product != null)
                {
                    var productDetails = (from p in context.Products
                                          join i in context.Inventories on p.Id equals i.ProductId
                                          where p.Id == id
                                          select new
                                          {
                                              p.Id,p.BrandName,p.Image,
                                              p.Description,p.Category,
                                              p.SubCategory, p.Color,
                                              i.ProductSize,i.Price,i.Stock
                                          }).ToList();

                    
                        return Ok(productDetails);
                    
                }
                else
                {
                    return StatusCode(404, "Product Not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("Get Grouped Product/{id}")]
        public IActionResult GetGroupedProduct(int id)
        {
            try
            {
                var product = context.Products.Find(id);
                if (product != null)
                {
                    var productDetails = (from p in context.Products
                                          join i in context.Inventories on p.Id equals i.ProductId
                                          where p.Id == id
                                          select new
                                          {
                                              p.Id,
                                              p.BrandName,
                                              p.Image,
                                              p.Description,
                                              p.Category,
                                              p.SubCategory,
                                              p.Color,
                                              ProductDetails = new
                                              {
                                                  i.Id,
                                                  i.ProductSize,
                                                  i.Price,
                                                  i.Stock
                                              }
                                          })
                                         .GroupBy(p => new
                                         {
                                             p.Id,
                                             p.BrandName,
                                             p.Image,
                                             p.Description,
                                             p.Category,
                                             p.SubCategory,
                                             p.Color
                                         })
                                         .Select(g => new
                                         {
                                             g.Key.Id,
                                             g.Key.BrandName,
                                             g.Key.Image,
                                             g.Key.Description,
                                             g.Key.Category,
                                             g.Key.SubCategory,
                                             g.Key.Color,
                                             ProductDetails = g.Select(p => p.ProductDetails).ToList()
                                         })
                                         .FirstOrDefault();

                    return Ok(productDetails);
                }
                else
                {
                    return StatusCode(404, "Product Not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //public IActionResult GetProduct(int id)
        //{
        //    try
        //    {
        //        var product = context.Products.SingleOrDefault(x => x.Id == id);
        //        if (product != null)
        //        {
        //            return Ok(product);
        //        }
        //        else
        //            return StatusCode(404, "Not found");

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpPost("Add Product")]
        public IActionResult AddProduct([FromBody] Product product)
        {
            try
            {
                var vendor = context.Vendors.Find(product.VendorId);
                if(vendor == null)
                {
                    return StatusCode(404, "Vendor Not Found");
                }
                context.Products.Add(product);
                context.SaveChanges();

                return Ok(product.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                var product = context.Products.Find(id);
                if (product == null)
                {
                    return NotFound();
                }
                else
                {
                    product.Status = "Inactive";
                    var inventory=context.Inventories.Where(x=>x.ProductId==id).ToList();
                    if(inventory.Any())
                    {
                        context.Inventories.RemoveRange(inventory);
                    }

                    context.SaveChanges();
                    return Ok("Done");
                }
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("UpdateProduct/{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            try
            {
                var product = context.Products.Find(id);
                if (product == null)
                {
                    return NotFound();
                }
                else
                {
                    product.BrandName = updatedProduct.BrandName;
                    product.Description = updatedProduct.Description;
                    product.Category = updatedProduct.Category;
                    product.SubCategory = updatedProduct.SubCategory;
                    product.Image = updatedProduct.Image;
                    product.Color = updatedProduct.Color;
                    product.VendorId = updatedProduct.VendorId;
                    product.Status = updatedProduct.Status;
                    context.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //wishlist  


        [HttpGet("Get Wishlist By userId/{id}")]
        public IActionResult Get_Wishlist(int id)
        {
            try { 
                var user = context.Users.Find(id);
                if (user == null)
                {
                    return StatusCode(404, "User Not Found");
                }else
                {
                    var wishlist=context.Wishlists.Where(x=>x.UserId==id).ToList(); 
                    var pid=wishlist.Select(x=>x.ProductId).ToList();
                    if (wishlist.Any())
                    {
                        return Ok(pid);
                    }
                    else
                        return StatusCode(404, "Wishlist Not Found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Get Wishlisted Products By Userid/{id}")]
        public IActionResult Get_Wishlist_Product_Details(int id)
        {
            try
            {
                var user = context.Users.Find(id);
                if (user == null)
                {
                    return StatusCode(404, "User Not Found");
                }
                else
                {
                    var wishlist = context.Wishlists.Where(x => x.UserId == id).ToList();
                    if (wishlist.Count > 0)
                    {
                        var productIds = wishlist.Select(w => w.ProductId).ToList();
                        // var products=context.Products.Where(y=>wishlist.Any(w=>w.ProductId==y.Id)).ToList(); 

                        var products = context.Products.Where(y => productIds.Contains(y.Id) && y.Status == "Active").Select(p => new
                        {
                            p.Id,
                            p.BrandName,
                            p.Image,
                            p.Description,
                            p.Category,
                            p.SubCategory,
                            p.Color,
                            p.VendorId,
                            p.Status
                        }).ToList();

                        if (products.Any())
                        {
                            return Ok(products);
                        }
                        else
                        {
                            return StatusCode(404, "Product Not Found");
                        }
                    }
                    else
                    {
                        return StatusCode(404, "Wishlist Not Found");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Add Wishlist")]
        public IActionResult Add_Product_Wishlist([FromBody] Wishlist wishlist)
        {
            try
            {
                context.Wishlists.Add(wishlist);
                context.SaveChanges();
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("Deleted from Wishlist/{uid,pid}")]
        public IActionResult Delete_From_Wishlist(int uid,int pid)
        {
            try {
                var selectedUser = context.Wishlists.Where(x=>x.UserId==uid);
                if(selectedUser.Any())
                {
                    var SelectedProduct = selectedUser.Where(x => x.ProductId == pid).First();
                    if(SelectedProduct!=null)
                    {
                        context.Wishlists.Remove(SelectedProduct);
                        context.SaveChanges();
                        return Ok(SelectedProduct);
                    } 
                    else
                    {
                        return StatusCode(404, "Product Not found in wishlist");
                    }
                }
                else
                {
                    return StatusCode(404, "User Not Found in WishList");
                }
            }
            catch (Exception ex) 
            { 
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("Delete Wishlist/{id}")]
        public IActionResult Delete_Product_Wishlist(int id)
        {
            try
            {
                var wishlist = context.Wishlists.Find(id);
                if (wishlist == null)
                {
                    return NotFound();
                }
                context.Wishlists.Remove(wishlist);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        //cart 
        [HttpGet("Get Cart Items By User Id/{id}")]

        public IActionResult Get_Cart(int id)
        {
            try
            {
                var user = context.Users.Find(id);
                if (user == null)
                    return StatusCode(404, "User not Found");
                else
                {
                    var cart = context.Carts.Where(x => x.UserId == id).ToList();

                    if (cart.Count == 0)
                    { return StatusCode(404, "Cart is Empty"); }
                    else
                    {
                        var productId = cart.Select(x => x.ProductId).ToList();

                        var products = context.Products.Where(x => productId.Contains(x.Id) && x.Status == "Active").ToList().
                            Join(cart, product => product.Id,
                        cartItem => cartItem.ProductId,
                        (product, cartItem) => new
                        {
                            product.Id,
                            product.BrandName,
                            product.Image,
                            product.Description,
                            product.Category,
                            product.SubCategory,
                            product.Color,
                            product.VendorId,
                            product.Status,
                            cartItem.Quantity
                        }).ToList();
                        if (products.Count == 0)
                            return StatusCode(404, "Product Not Found");
                        else
                            return Ok(products);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("Add Cart")]
        public IActionResult Add_product_cart([FromBody] Cart cart)
        {
            try
            {
                var check=context.Carts.Where(x=>x.ProductId==cart.ProductId).ToList();
                if (check.Any())
                {
                    return StatusCode(200, "Already in Cart");
                }
                else
                {
                    context.Carts.Add(cart);
                    context.SaveChanges();
                    return Ok();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Delete Cart/{id}")]
        public IActionResult Delete_Product_Cart(int id)
        {
            try
            {
                var cart = context.Carts.Find(id);
                if (cart == null)
                {
                    return NotFound();
                }
                context.Carts.Remove(cart);
                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update Cart/{id}")]
        public IActionResult Update_Product_Cart(int id, [FromBody] Cart cart)
        {
            try
            {
                var Updatedcart = context.Carts.Find(id);
                if (Updatedcart == null)
                { return NotFound(); }
                else
                {
                    Updatedcart.ProductId = cart.ProductId;
                    Updatedcart.UserId = cart.UserId;
                    Updatedcart.Quantity = cart.Quantity;
                    Updatedcart.InventoryId = cart.InventoryId;
                    context.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //Inventory

        [HttpGet("Inventory Details")]
        public IActionResult Inventory_Details()
        {
            try
            {
                var inventories = context.Inventories.ToList();
                if (inventories.Any())
                    return Ok(inventories);
                else
                    return StatusCode(404, "Out of Stock");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Add Items into Inventory")]
        public IActionResult Add_items([FromBody] Inventory inventory)
        {
            try
            {
                var product = context.Products.Find(inventory.ProductId);
                if (product == null)
                { return StatusCode(404, "Product not Found"); }
                else
                {
                    context.Inventories.Add(inventory);
                    context.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update Inventory Items/{id}")]
        public IActionResult Update_Items(int id, [FromBody] Inventory inventory)
        {
            try
            {
                var updated_inventory = context.Inventories.Find(id);
                if (updated_inventory == null)
                {
                    return StatusCode(404, "Inventory not found");
                }
                else
                {
                    var product = context.Products.Find(inventory.ProductId);
                    if (product == null)
                    { return StatusCode(404, "Product not found"); }
                    else
                    {
                        updated_inventory.ProductId = inventory.ProductId;
                        updated_inventory.ProductSize = inventory.ProductSize;
                        updated_inventory.Price = inventory.Price;
                        updated_inventory.Stock = inventory.Stock;

                        context.SaveChanges();
                        return Ok();
                    }
                }
            }
            catch (Exception ex)
            { return BadRequest(ex.Message); }
        }

        [HttpDelete("Delete Inventory Items/{id}")]
        public IActionResult Delete_Items(int id)
        {
            try
            {
                var inventory = context.Inventories.Find(id);
                if(inventory == null)
                {
                    return StatusCode(404, "Inventory Not Found");
                }
                else
                {
                    context.Inventories.Remove(inventory);
                    context.SaveChanges();
                    return Ok();
                }
            }catch(Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("Get Inventory Details By Vendor iD/{id}")]



        //public IActionResult Get_Inventory(int id)
        //    {
        //        try
        //        {
        //            var vendor = context.Vendors.Find(id);
        //            if (vendor == null)
        //            {
        //                return StatusCode(404, "Vendor Not Found");
        //            }
        //            else
        //            {
        //                var products = context.Products.Where(x => x.VendorId == id).ToList();
        //                if (products.Any())
        //                {
        //                    var inventory = context.Inventories
        //                        .Where(x => products.Select(p => p.Id).Contains((int)x.ProductId))
        //                        .ToList();

        //                    var inventoryGrouped = inventory
        //                        .GroupBy(
        //                            inv => new
        //                            {
        //                                inv.ProductId,
        //                                inv.Stock,
        //                                inv.Price,
        //                                inv.ProductSize
        //                            },
        //                            inv => inv.Product,
        //                            (key, group) => new
        //                            {
        //                                ProductDetails = group.FirstOrDefault(),
        //                                InventoryDetails = new
        //                                {
        //                                    key.Stock,
        //                                    key.Price,
        //                                    key.ProductSize
        //                                }
        //                            }
        //                        )
        //                        .ToList();

        //                    var options = new JsonSerializerOptions
        //                    {
        //                        ReferenceHandler = ReferenceHandler.Preserve,
        //                        // Other options as needed
        //                    };

        //                    return Ok(JsonSerializer.Serialize(inventoryGrouped, options));
        //                }
        //                else
        //                {
        //                    return StatusCode(404, "Products Not Found");
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }


        public IActionResult Get_Inventory(int id)
        {
            try
            {
                var vendor = context.Vendors.Find(id);
                if (vendor == null)
                {
                    return StatusCode(404, "Vendor Not Found");
                }
                else
                {
                    var products = context.Products.Where(x => x.VendorId == id).ToList();
                    if (products.Any())
                    {
                        var productIds = products.Select(x => x.Id).ToList();

                        var inventory = context.Inventories.Where(x => productIds.Contains((int)x.ProductId)).ToList()
                            .Join(products, inv => inv.ProductId,
                            pro => pro.Id, (invent, p) => new
                            {
                                p.Id,
                                p.BrandName,
                                p.Image,
                                p.Description,
                                p.Category,
                                p.SubCategory,
                                p.Color,
                                p.Status,
                                InventoryId= invent.Id,
                                invent.Stock,
                                invent.Price,
                                invent.ProductSize,
                                
                            }).ToList();

                        return Ok(inventory);
                    }
                    else
                        return StatusCode(404, "Products Not Found");
                }

            }
            catch (Exception ex)
            { return BadRequest(ex.Message); }
        }


    }
}
