using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAcess.Data;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers;

public class CategoryController : Controller
{
    // private readonly ApplicationDbContext _db;
    // public CategoryController(ApplicationDbContext db)
    private readonly IUnitOfWork _unitOfWork; 
    public CategoryController(IUnitOfWork unitOfWork)
    {
        //_db = db;
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index()
    {
        List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList(); //_db.Categories
        return View(objCategoryList);
    }
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Create(Category obj)
    {   
        // if (obj.Name == obj.DisplayOrder.ToString()){
        //     ModelState.AddModelError("name", "the DisplayOrder cannot exactly match the Name");
        // }
        // // ModelOnly All None
        // if (obj.Name.ToLower() == "test"){
        //     ModelState.AddModelError("", "77 is an invalid value");
        // }
        if (ModelState.IsValid){
            //_db.Categories.Add(obj);
            //_db.SaveChages();
            _unitOfWork.Category.Add(obj);
            _unitOfWork.Save();
            TempData["success"] =  "Category created successfully";
            return RedirectToAction("Index");
        }
        return View();
        
    }

    public IActionResult Edit(int? id)
    {
        if(id==null || id == 0)
        {
            return NotFound();
        }
        // Category? categoryFromDb = _db.Categories.Find(id);
        Category? categoryFromDb = _unitOfWork.Category.Get(u=>u.Id==id);
        // Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u=>u.Id==id);
        // Category? categoryFromDb2 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();
        if (categoryFromDb == null)
        {
            return NotFound();
        }
        return View(categoryFromDb);
    }
    [HttpPost]
    public IActionResult Edit(Category obj)
    {   
        if (ModelState.IsValid){
            //_db.Categories.Add(obj);
            //_db.SaveChages();
            _unitOfWork.Category.Update(obj);
            _unitOfWork.Save();
            TempData["success"] =  "Category update successfully";
            return RedirectToAction("Index");
        }
        return View();
        
    }


    public IActionResult Delete(int? id)
    {
        if(id==null || id == 0)
        {
            return NotFound();
        }
        // Category? categoryFromDb = _db.Categories.Find(id);
        Category? categoryFromDb = _unitOfWork.Category.Get(u=>u.Id==id);
        // Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u=>u.Id==id);
        // Category? categoryFromDb2 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();
        if (categoryFromDb == null)
        {
            return NotFound();
        }
        return View(categoryFromDb);
    }
    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePOST(int? id)
    {   
        // Category? obj = _db.Categories.Find(id);
        Category? obj  = _unitOfWork.Category.Get(u=>u.Id==id);
        if (obj == null)
        {
            return NotFound();
        }
        //_db.Categories.Add(obj);
        //_db.SaveChages();
        _unitOfWork.Category.Remove(obj);
        _unitOfWork.Save();
        TempData["success"] =  "Category delete successfully";
        return RedirectToAction("Index");
        
    }
}
