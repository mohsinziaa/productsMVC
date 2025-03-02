using crudmvc.Models;
using crudmvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace crudmvc.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;            
            this.environment = environment;            
        }
        public IActionResult Index()
        {
            var products = context.Products.OrderByDescending(p => p.Id).ToList();
            return View(products);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductDTO productDTO)
        {
            if (productDTO.ImageFile == null)
            { 
                ModelState.AddModelError("ImageFile", "Please select an image file");
            }
            
            if(!ModelState.IsValid)
            {
                return View(productDTO);
            }

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(productDTO.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/products/" + newFileName;

            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDTO.ImageFile.CopyTo(stream);
            }

            Product product = new Product()
            {
                Name = productDTO.Name,
                Brand = productDTO.Brand,
                Category = productDTO.Category,
                Price = productDTO.Price,
                Description = productDTO.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now
            };

            context.Products.Add(product);
            context.SaveChanges();


            return RedirectToAction("Index", "Products");
        }

        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);

            if (product == null) 
            { 
                return RedirectToAction("Index", "Products");            
            }

            var productDTO = new ProductDTO()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
            };

            ViewData["ProductId"] = id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

            return View(productDTO);
        }

        [HttpPost]
        public IActionResult Edit(int id, ProductDTO productDTO)
        {
            var product = context.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");
                return View(productDTO);
            }

            string newFileName = product.ImageFileName;
            if (productDTO.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(productDTO.ImageFile.FileName);
                string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDTO.ImageFile.CopyTo(stream);
                }

                string lastFilePath = environment.WebRootPath + "/products/" + product.ImageFileName;
                System.IO.File.Delete(lastFilePath);
                product.ImageFileName = newFileName;
            }

            product.Name = productDTO.Name;
            product.Brand = productDTO.Brand;
            product.Category = productDTO.Category;
            product.Price = productDTO.Price;
            product.Description = productDTO.Description;
            product.ImageFileName = newFileName;

            context.SaveChanges();

            return RedirectToAction("Index", "Products");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            string lastFilePath = environment.WebRootPath + "/products/" + product.ImageFileName;
            System.IO.File.Delete(lastFilePath);

            context.Products.Remove(product);
            context.SaveChanges(true);

            return RedirectToAction("Index", "Products");
        }
    }
}
