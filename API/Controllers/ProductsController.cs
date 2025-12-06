using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //public class ProductsController(IGenericRepository<Product> repo) : BaseApiController
    public class ProductsController(IUnitOfWork unit) : BaseApiController
    {
        //private StoreContext context;

        //public ProductsController(StoreContext context)
        //{
        //    this.context = context;
        //}

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(
            [FromQuery]ProductSpecParams specParams)
        {
            //return await context.Products.ToListAsync();

            var spec = new ProductSpecification(specParams);

            //return await CreatePagedResult(repo,spec, specParams.PageIndex,specParams.PageSize);
            return await CreatePagedResult(unit.Repository<Product>(),spec, specParams.PageIndex,specParams.PageSize);
        }

        [HttpGet("{id:int}")] //api/products/2
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            //var product = await context.Products.FindAsync(id);
            var product = await unit.Repository<Product>().GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            //context.Products.Add(product);
            //await context.SaveChangesAsync();
            unit.Repository<Product>().Add(product);
            if (await unit.Complete())
            {
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }

            return BadRequest("Problem creating product");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if (product.Id != id || !ProductExists(id))
            {
                return BadRequest("Can't update this product!");
            }

            //context.Entry(product).State = EntityState.Modified;

            //await context.SaveChangesAsync();

            unit.Repository<Product>().Update(product);
            if (await unit.Complete()) 
            {
                return NoContent();
            }

            return BadRequest("Problem in updating the product!");

        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            //var product = await context.Products.FindAsync(id);
            var product = await unit.Repository<Product>().GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            //context.Products.Remove(product);
            //await context.SaveChangesAsync();

            unit.Repository<Product>().Remove(product);    
            if (await unit.Complete())
            {
                return NoContent();
            }

            return BadRequest("Problem in deleting the product!");

        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            //return await context.ProductBrands.ToListAsync();
            //TODO - implement in GenericRepositoryd

            var spec = new BrandListSpecification();

            return Ok(await unit.Repository<Product>().ListAsync(spec));
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            //return await context.ProductBrands.ToListAsync();
            //TODO - implement in GenericRepository

            var spec = new TypeListSpecification();
            var types = await unit.Repository<Product>().ListAsync(spec);
            return Ok(types);
        }

        private bool ProductExists(int id)
        {
            //return context.Products.Any(x => x.Id == id);
            return unit.Repository<Product>().Exists(id);
        }
    }
}
