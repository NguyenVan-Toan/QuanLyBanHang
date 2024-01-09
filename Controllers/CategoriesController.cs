using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Data;
using QuanLyBanHang.Data.Entities;
using QuanLyBanHang.Services;
using QuanLyBanHang.ViewModels;

namespace QuanLyBanHang.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        // private readonly QuanLyBanHangDbContext _context;
        private readonly ICategoryService _categoryService;
        private readonly IValidator<CategoryRequest> _validator;
        private readonly IValidator<CategoryViewModel> _validatorVM;

        public CategoriesController(ICategoryService categoryService, IValidator<CategoryViewModel> validatorVM, IValidator<CategoryRequest> validator)
        {
            _validator = validator;
            _categoryService = categoryService;
            _validatorVM = validatorVM;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        // GET: Categories/Details/5
        [Route("Categories/{slug}-{id}")]
        public async Task<IActionResult> Details(int id)
        {

            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryRequest categoryRequest)
        {

            ValidationResult validationResult = await _validator.ValidateAsync(categoryRequest);
            if (!validationResult.IsValid)
                validationResult.AddToModelState(this.ModelState);
            if (ModelState.IsValid)
            {
                await _categoryService.CreateAsync(categoryRequest);
                return RedirectToAction(nameof(Index));
            }
            return View(categoryRequest);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel categoryViewModel)
        {
            if (id != categoryViewModel.Id)
            {
                return NotFound();
            }
            ValidationResult result = await _validatorVM.ValidateAsync(categoryViewModel);
            if (!result.IsValid)
                result.AddToModelState(this.ModelState);
            if (ModelState.IsValid)
            {
                await _categoryService.UpdateAsync(categoryViewModel);
                return RedirectToAction(nameof(Index));
            }
            return View(categoryViewModel);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _categoryService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // private bool CategoryExists(int id)
        // {
        //     return _context.Categories.Any(e => e.Id == id);
        // }
    }
}
