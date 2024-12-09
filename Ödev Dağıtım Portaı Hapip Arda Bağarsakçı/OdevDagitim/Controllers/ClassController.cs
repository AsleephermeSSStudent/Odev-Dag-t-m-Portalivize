using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using OdevDagitim.Models;
using OdevDagitim.Repositories;
using OdevDagitim.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OdevDagitim.Controllers
{
    [Authorize(Roles = "admin,teacher")]
    public class ClassController : Controller
    {
        private readonly ClassRepository _classRepository;
        private readonly IMapper _mapper;
        private readonly INotyfService _notyf;

        public ClassController(ClassRepository classRepository, IMapper mapper, INotyfService notyf)
        {
            _classRepository = classRepository;
            _mapper = mapper;
            _notyf = notyf;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetClasses()
        {
            var classes = await _classRepository.GetAllAsync();
            var classViewModels = _mapper.Map<List<ClassViewModel>>(classes);
            return Json(classViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> AddClass([FromBody] ClassViewModel model)
        {
            if (ModelState.IsValid)
            {
                var classEntity = _mapper.Map<Class>(model);
                await _classRepository.AddAsync(classEntity);
                _notyf.Success("Sınıf başarıyla eklendi.");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateClass([FromBody] ClassViewModel model)
        {
            if (ModelState.IsValid)
            {
                var classEntity = _mapper.Map<Class>(model);
                await _classRepository.UpdateAsync(classEntity);
                _notyf.Success("Sınıf başarıyla güncellendi.");
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteClass(int id)
        {
            await _classRepository.DeleteAsync(id);
            _notyf.Success("Sınıf başarıyla silindi.");
            return Json(new { success = true });
        }
    }
} 