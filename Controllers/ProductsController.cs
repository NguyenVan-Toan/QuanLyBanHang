using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Data;
using QuanLyBanHang.Data.Entities;
using QuanLyBanHang.Services;
using QuanLyBanHang.ViewModels;

namespace QuanLyBanHang.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IValidator<ProductRequest> _validator;
        private readonly ICartService _cartService;
        private readonly IValidator<ProductViewModel> _validatorVM;
        private readonly UserManager<Account> _userManager;
        private readonly IBranchService _branchService;
        public ProductsController(
            ICartService cartService, 
            UserManager<Account> userManager,
            ICategoryService categoryService,
            IProductService productService,
            IValidator<ProductRequest> validator,
            IValidator<ProductViewModel> validatorVM,
            IBranchService branchService
            )
        {
            _cartService = cartService;
            _productService = productService;
            _categoryService = categoryService;
            _validator = validator;
            _validatorVM = validatorVM;
            _userManager = userManager;
            _branchService = branchService;
        }
        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }
        public async Task<IEnumerable<ProductViewModel>> GetProductList()
        {
            var products = await _productService.GetAllProductsAsync();
            return products;
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var Category = await _categoryService.GetByIdAsync(product.CategoryId);
            ViewBag.Category = Category.CategoryName;
            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllAsync(), "Id", "CategoryName");
            ViewData["BranchName"] = new SelectList(await _branchService.GetAllAsync(), "Id", "BranchName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductRequest productRequest)
        {
            ValidationResult result = _validator.ValidateAsync(productRequest).Result;
            if (!result.IsValid)
            {
                result.AddToModelState(this.ModelState);
            }
            if (ModelState.IsValid)
            {
                await _productService.Create(productRequest);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllAsync(), "Id", "CategoryName", productRequest.CategoryId);
            ViewData["BranchName"] = new SelectList(await _branchService.GetAllAsync(), "Id", "BranchName", productRequest.BranchId);
            return View(productRequest);    
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["BranchName"] = new SelectList(await _branchService.GetAllAsync(), "Id", "BranchName");
            ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllAsync(), "Id", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel productViewModel)
        {
            if (id != productViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _productService.Update(productViewModel);
                return RedirectToAction(nameof(Index));
            }
            ViewData["BranchName"] = new SelectList(await _branchService.GetAllAsync(), "Id", "BranchName", productViewModel.BranchId);
            ViewData["CategoryId"] = new SelectList(await _categoryService.GetAllAsync(), "Id", "CategoryName", productViewModel.CategoryId);
            return View(productViewModel);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var Category = await _categoryService.GetByIdAsync(product.CategoryId);
            ViewBag.Category = Category.CategoryName;
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // private bool ProductExists(int id)
        // {
        //     return _context.Products.Any(e => e.Id == id);
        // }
        [AllowAnonymous]
        [Route("xem-san-pham")]
        public async Task<IActionResult> ShowProduct()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }
        [AllowAnonymous]
        [Route("xem-san-pham/{slug}-{id}")]
        public async Task<IActionResult> ShowDetails(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            var Category = await _categoryService.GetByIdAsync(product.CategoryId);
            ViewBag.Category = Category.CategoryName;
            return View(product);
        }
        [AllowAnonymous]
        // [HttpPost]
        [Route("them-san-pham/{slug}-{id}")]
        public async Task<IActionResult> AddProductToCart(int id)
        {
            if (_userManager.GetUserName(User) != null)
            {
                var product = await _productService.GetProductByIdAsync(id);
                var userId = _userManager.GetUserId(User);
                await _cartService.Create(product.Id, userId!);
                var products = await _productService.GetAllProductsAsync();
                return View("ShowProduct", products);
            }
            else return LocalRedirect("/login");
        }
    }
}
