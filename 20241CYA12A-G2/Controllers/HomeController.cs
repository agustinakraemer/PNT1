using _20241CYA12A_G2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace _20241CYA12A_G2.Controllers
{
    public class HomeController : Controller
       
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbContext _context;

		public HomeController(ILogger<HomeController> logger, DbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
			var numeroDia = (int)DateTime.Today.DayOfWeek;

			string dia = System.Globalization.CultureInfo.CurrentCulture.
                DateTimeFormat.GetDayName((DayOfWeek)numeroDia);

			var descuento = await _context.Descuento.Include(d => d.Producto).
                FirstOrDefaultAsync(d => d.Dia == numeroDia && d.Activo);

            HomeViewModel homeViewModel = new();

			if (descuento == null)
			{

                homeViewModel.MensajePromo = "Hoy es " + dia + ". Disfruta del mejor sushi #EnCasa con amigos.";
			}
			else
			{
				homeViewModel.Dia = dia;
				homeViewModel.Descuento = descuento.Porcentaje.ToString() + "%";
				homeViewModel.Producto = descuento.Producto.Nombre;
			}
 
			return View(homeViewModel);

			 
        }
		// Borramos Privacy de la vistaHome
		/*public IActionResult Privacy()
        {
            return View();
        }*/

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
