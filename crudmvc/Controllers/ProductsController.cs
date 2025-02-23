using crudmvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace crudmvc.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;
        public ProductsController(ApplicationDbContext context)
        {
            this.context = context;            
        }
        public IActionResult Index()
        {
            var products = context.Products.ToList();
            return View(products);
        }
    }
}
