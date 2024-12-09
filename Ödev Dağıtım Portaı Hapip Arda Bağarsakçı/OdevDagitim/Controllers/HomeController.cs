using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using OdevDagitim.Models;
using OdevDagitim.Repositories;
using OdevDagitim.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace OdevDagitim.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AssignmentRepository _assignmentRepository;
        private readonly UserRepository _userRepository;
        private readonly AssignmentSubmissionRepository _submissionRepository;
        private readonly IMapper _mapper;
        private readonly INotyfService _notyf;

        public HomeController(
            ILogger<HomeController> logger,
            AssignmentRepository assignmentRepository,
            UserRepository userRepository,
            AssignmentSubmissionRepository submissionRepository,
            IMapper mapper,
            INotyfService notyf)
        {
            _logger = logger;
            _assignmentRepository = assignmentRepository;
            _userRepository = userRepository;
            _submissionRepository = submissionRepository;
            _mapper = mapper;
            _notyf = notyf;
        }

        [Authorize(Roles = "Ogrenci")]
        public async Task<IActionResult> Index()
        {
            // Giriş yapmış kullanıcının ID'sini al
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "User");
            }

            var userId = int.Parse(userIdClaim.Value);
            var user = await _userRepository.GetByIdAsync(userId);

            if (user.ClassId == null)
            {
                _notyf.Warning("Henüz bir sınıfa atanmamışsınız.");
                return View(new List<StudentAssignmentViewModel>());
            }

            // Öğrencinin sınıfına ait ödevleri getir
            var assignments = await _assignmentRepository.GetAssignmentsByClassAsync(user.ClassId.Value);
            
            // Öğrencinin teslim ettiği ödevleri getir
            var submissions = await _submissionRepository.GetSubmissionsByUserAsync(userId);

            // ViewModel'e dönüştür
            var viewModel = assignments.Select(a => new StudentAssignmentViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                DueDate = a.DueDate,
                TeacherName = a.Teacher.Username,
                IsSubmitted = submissions.Any(s => s.AssignmentId == a.Id),
                IsLate = DateTime.Now > a.DueDate,
                SubmissionDate = submissions.FirstOrDefault(s => s.AssignmentId == a.Id)?.SubmissionDate
            }).ToList();

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
