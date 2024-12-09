using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using OdevDagitim.Models;
using OdevDagitim.Repositories;
using OdevDagitim.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace OdevDagitim.Controllers
{
  
    public class AssignmentController : Controller
    {
        private readonly AssignmentRepository _assignmentRepository;
        private readonly ClassRepository _classRepository;
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly INotyfService _notyf;
        private readonly AssignmentSubmissionRepository _submissionRepository;

        public AssignmentController(
            AssignmentRepository assignmentRepository,
            ClassRepository classRepository,
            UserRepository userRepository,
            IMapper mapper,
            INotyfService notyf,
            AssignmentSubmissionRepository submissionRepository)
        {
            _assignmentRepository = assignmentRepository;
            _classRepository = classRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _notyf = notyf;
            _submissionRepository = submissionRepository;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRol = User.FindFirst(ClaimTypes.Role).Value; 
            if(userRol == "admin")
            {
                var adminss = await _assignmentRepository.GetAssignmentsWithDetailsAsync();

                var viewModele = _mapper.Map<List<AssignmentViewModel>>(adminss);
                return View(viewModele);
            }


            if (int.TryParse(userId, out int parsedUserId))
            {
                var deneme = await _assignmentRepository.GetMyList(parsedUserId);
                var model = _mapper.Map<List<AssignmentViewModel>>(deneme);
                return View(model);
            }

           
                // Öğretmen sayısını al
                var teacherCount = _userRepository.Where(u => u.Role == "Teacher").Count();
            ViewBag.Teachers = teacherCount;

            // Sınıf sayısını al
            var classes = await _classRepository.GetAllAsync();
            ViewBag.Classes = classes.Count();

            // Ödevleri al
            var assignments = await _assignmentRepository.GetAssignmentsWithDetailsAsync();
        
            var viewModel = _mapper.Map<List<AssignmentViewModel>>(assignments);
            return View(viewModel);
        }
        [Authorize(Roles = "admin,teacher")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View(new AssignmentViewModel());
        }
        [Authorize(Roles ="admin,teacher")]
        [HttpPost]
        public async Task<IActionResult> Create(AssignmentViewModel model, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                var assignment = _mapper.Map<Assignment>(model);
                assignment.Created = DateTime.Now;
                assignment.Updated = DateTime.Now;

                // Dosyaları kaydet
                if (files != null && files.Count > 0)
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", "assignments");
                    
                    // Klasör yoksa oluştur
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            // Güvenli dosya adı oluştur
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(uploadPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                        }
                    }
                }

                await _assignmentRepository.AddAsync(assignment);
                _notyf.Success("Ödev başarıyla oluşturuldu.");
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdowns();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(id);
            if (assignment == null)
            {
                _notyf.Error("Ödev bulunamadı.");
                return RedirectToAction(nameof(Index));
            }

            var viewModel = _mapper.Map<AssignmentViewModel>(assignment);
            await LoadDropdowns();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AssignmentViewModel model, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                var assignment = await _assignmentRepository.GetByIdAsync(model.Id);
                if (assignment == null)
                {
                    _notyf.Error("Ödev bulunamadı.");
                    return RedirectToAction(nameof(Index));
                }

                // Mevcut özellikleri güncelle
                assignment.Title = model.Title;
                assignment.Description = model.Description;
                assignment.DueDate = model.DueDate;
                assignment.ClassId = model.ClassId;
                assignment.TeacherId = model.TeacherId;
                assignment.Updated = DateTime.Now;

                // Yeni dosyaları kaydet
                if (files != null && files.Count > 0)
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", "assignments");
                    
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(uploadPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                        }
                    }
                }

                await _assignmentRepository.UpdateAsync(assignment);
                _notyf.Success("Ödev başarıyla güncellendi.");
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdowns();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(id);
            if (assignment == null)
            {
                _notyf.Error("Ödev bulunamadı.");
                return RedirectToAction(nameof(Index));
            }

            // Ödev dosyalarını sil
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", "assignments");
            var assignmentPath = Path.Combine(uploadPath, id.ToString());
            if (Directory.Exists(assignmentPath))
            {
                Directory.Delete(assignmentPath, true);
            }

            await _assignmentRepository.DeleteAsync(id);
            _notyf.Success("Ödev başarıyla silindi.");
            return RedirectToAction(nameof(Index));
        }

      
        [HttpGet]
        public async Task<IActionResult> Submit(int id)
        {
            var assignment = await _assignmentRepository.GetAssignmentWithDetailsAsync(id);
            if (assignment == null)
            {
                _notyf.Error("Ödev bulunamadı.");
                return RedirectToAction("Index", "Home");
            }

            if (DateTime.Now > assignment.DueDate)
            {
                _notyf.Warning("Bu ödevin teslim süresi dolmuştur!");
            }

            var viewModel = new AssignmentSubmitViewModel
            {
                AssignmentId = id,
                AssignmentTitle = assignment.Title,
                DueDate = assignment.DueDate
            };

            return View(viewModel);
        }

    
        [HttpPost]
        public async Task<IActionResult> Submit(AssignmentSubmitViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var assignment = await _assignmentRepository.GetByIdAsync(model.AssignmentId);
            if (assignment == null)
            {
                _notyf.Error("Ödev bulunamadı.");
                return RedirectToAction("Index", "Home");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError("File", "Lütfen bir dosya seçin.");
                return View(model);
            }

            //// Dosya uzantı kontrolü
            //var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".zip", ".rar" };
            var fileExtension = Path.GetExtension(model.File.FileName).ToLowerInvariant();
            //if (!allowedExtensions.Contains(fileExtension))
            //{
            //    ModelState.AddModelError("File", "Sadece PDF, DOC, DOCX, ZIP ve RAR dosyaları yükleyebilirsiniz.");
            //    return View(model);
            //}

            // Dosya kaydetme işlemi
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", "submissions", userId.ToString());
            
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            var submission = new AssignmentSubmission
            {
                AssignmentId = model.AssignmentId,
                UserId = userId,
                Description = model.Description,
                SubmissionPath = $"/files/submissions/{userId}/{fileName}",
                SubmissionDate = DateTime.Now,
                IsLate = DateTime.Now > assignment.DueDate,
                Created = DateTime.Now,
                Updated = DateTime.Now
            };

            await _submissionRepository.AddAsync(submission);
            _notyf.Success("Ödeviniz başarıyla teslim edildi.");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var assignment = await _assignmentRepository.GetAssignmentWithDetailsAsync(id);
            if (assignment == null)
            {
                _notyf.Error("Ödev bulunamadı.");
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new AssignmentDetailViewModel
            {
                Id = assignment.Id,
                Title = assignment.Title,
                Description = assignment.Description,
                DueDate = assignment.DueDate,
                TeacherName = assignment.Teacher.Username,
                ClassName = assignment.Class.ClassName,
                FilePath = assignment.FilePath,
                Submissions = assignment.Submissions.Select(s => new SubmissionDetailViewModel
                {
                    Id = s.Id,
                    StudentName = s.User.Username,
                    SubmissionDate = s.SubmissionDate,
                    IsLate = s.IsLate,
                    Description = s.Description,
                    SubmissionPath = s.SubmissionPath
                }).ToList()
            };

            // Eğer öğrenci ise kendi teslimini bul
            if (User.IsInRole("Ogrenci"))
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var userSubmission = assignment.Submissions.FirstOrDefault(s => s.UserId == userId);
                if (userSubmission != null)
                {
                    viewModel.UserSubmission = new SubmissionDetailViewModel
                    {
                        Id = userSubmission.Id,
                        StudentName = userSubmission.User.Username,
                        SubmissionDate = userSubmission.SubmissionDate,
                        IsLate = userSubmission.IsLate,
                        Description = userSubmission.Description,
                        SubmissionPath = userSubmission.SubmissionPath
                    };
                }
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadSubmission(int submissionId)
        {
            var submission = await _submissionRepository.GetByIdAsync(submissionId);
            if (submission == null)
            {
                _notyf.Error("Dosya bulunamadı.");
                return RedirectToAction(nameof(Index));
            }

            // Güvenlik kontrolü - sadece admin veya dosyanın sahibi indirebilir
            if (!User.IsInRole("admin") && !User.IsInRole("Teacher"))
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                if (submission.UserId != userId)
                {
                    _notyf.Error("Bu dosyayı indirme yetkiniz yok.");
                    return RedirectToAction(nameof(Index));
                }
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", submission.SubmissionPath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
            {
                _notyf.Error("Dosya bulunamadı.");
                return RedirectToAction(nameof(Index));
            }

            var fileName = Path.GetFileName(filePath);
            var mimeType = GetMimeType(filePath);

            return PhysicalFile(filePath, mimeType, fileName);
        }

        private string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            switch (extension)
            {
                case ".pdf":
                    return "application/pdf";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".zip":
                    return "application/zip";
                case ".rar":
                    return "application/x-rar-compressed";
                default:
                    return "application/octet-stream";
            }
        }

        private async Task LoadDropdowns()
        {
            var classes = await _classRepository.GetAllAsync();
            ViewBag.Classes = new SelectList(classes, "Id", "ClassName");

            var teachers = await _userRepository.GetTeachersAsync();
            ViewBag.Teachers = new SelectList(teachers, "Id", "Username");
        }

        // Diğer action'lar (Edit, Delete vs.) eklenecek...
    }
} 