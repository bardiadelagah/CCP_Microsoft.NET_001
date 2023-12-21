﻿using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAcess.Data;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyBookWeb.Areas.Admin.Controllers;
[Area("Admin")]
public class ProductController : Controller
{
    // private readonly ApplicationDbContext _db;
    // public ProductController(ApplicationDbContext db)
    
    private readonly IUnitOfWork _unitOfWork; 
    private readonly IWebHostEnvironment _webHostEnvironment; 
    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        //_db = db;
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }
    public IActionResult Index()
    {
        List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList(); //_db.Categories
        return View(objProductList);
    }
    public IActionResult Upsert(int? id)
    {
        ProductVM productVM = new()
        {
            CategoryList = _unitOfWork.Category.GetAll().Select(u=> new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            Product = new Product()
        };
        if (id == null || id == 0)
        {
            //create
            return View(productVM);
        }
        else
        {
            //update
            productVM.Product = _unitOfWork.Product.Get(u=>u.Id==id);
            return View(productVM); 
        }
    }
    [HttpPost]
    public IActionResult Upsert(ProductVM productVM, IFormFile? file)
    {   
        if (ModelState.IsValid)
        {   
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(wwwRootPath, @"images\product");
                
                if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                {
                    //delete the old image
                    var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(productPath, fileName),FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                productVM.Product.ImageUrl = @"\images\product\" + fileName;
            }
            
            //_db.Categories.Add(obj);
            //_db.SaveChages();
            if (productVM.Product.Id == 0)
            {
                _unitOfWork.Product.Add(productVM.Product);
            }
            else
            {
                _unitOfWork.Product.Update(productVM.Product);
            }
            _unitOfWork.Save();
            TempData["success"] =  "Product created successfully";
            return RedirectToAction("Index");
        }
        else 
        {
            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u=> new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(productVM);
        }
    }

    // public IActionResult Edit(int? id)
    // {
    //     if(id==null || id == 0)
    //     {
    //         return NotFound();
    //     }
    //     // Product? productFromDb = _db.Products.Find(id);
    //     Product? productFromDb = _unitOfWork.Product.Get(u=>u.Id==id);
    //     // Product? productFromDb1 = _db.Products.FirstOrDefault(u=>u.Id==id);
    //     // Product? productFromDb2 = _db.Products.Where(u=>u.Id==id).FirstOrDefault();
    //     if (productFromDb == null)
    //     {
    //         return NotFound();
    //     }
    //     return View(productFromDb);
    // }
    // [HttpPost]
    // public IActionResult Edit(Product obj)
    // {   
    //     if (ModelState.IsValid){
    //         //_db.Categories.Add(obj);
    //         //_db.SaveChages();
    //         _unitOfWork.Product.Update(obj);
    //         _unitOfWork.Save();
    //         TempData["success"] =  "Product update successfully";
    //         return RedirectToAction("Index");
    //     }
    //     return View();
        
    // }


    // public IActionResult Delete(int? id)
    // {
    //     if(id==null || id == 0)
    //     {
    //         return NotFound();
    //     }
    //     // Product? productFromDb = _db.Categories.Find(id);
    //     Product? productFromDb = _unitOfWork.Product.Get(u=>u.Id==id);
    //     // Product? productFromDb1 = _db.Categories.FirstOrDefault(u=>u.Id==id);
    //     // Product? productFromDb2 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();
    //     if (productFromDb == null)
    //     {
    //         return NotFound();
    //     }
    //     return View(productFromDb);
    // }
    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePOST(int? id)
    {   
        // Product? obj = _db.Categories.Find(id);
        Product? obj  = _unitOfWork.Product.Get(u=>u.Id==id);
        if (obj == null)
        {
            return NotFound();
        }
        //_db.Categories.Add(obj);
        //_db.SaveChages();
        _unitOfWork.Product.Remove(obj);
        _unitOfWork.Save();
        TempData["success"] =  "Product delete successfully";
        return RedirectToAction("Index");
        
    }


    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
        return Json(new { data = objProductList});
    }

    // [HttpDelete]
    public IActionResult Delete(int? Id)
    {
        var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == Id);
        if (productToBeDeleted == null)
        {
            return Json(new {success = false, message = "Error while deleting"});
        }
        var oldImagePath = 
                        Path.Combine(_webHostEnvironment.WebRootPath,
                         productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
 
        _unitOfWork.Product.Remove(productToBeDeleted);
        _unitOfWork.Save();

        return Json(new {success = true, message = "Delete Successful"});

    }

    #endregion
}
