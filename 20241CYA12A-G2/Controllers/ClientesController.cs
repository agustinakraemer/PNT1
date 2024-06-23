using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _20241CYA12A_G2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SQLitePCL;

namespace _20241CYA12A_G2.Controllers
{
    public class ClientesController : Controller
    {
        private readonly DbContext _context;

        public ClientesController(DbContext context)
        {
            _context = context;
        }

		// GET: Clientes
		[Authorize(Roles = "EMPLEADO,ADMINISTRADOR")]
		public async Task<IActionResult> Index()
        {
              return _context.Cliente != null ? 
                          View(await _context.Cliente.ToListAsync()) :
                          Problem("Entity set 'DbContext.Cliente'  is null.");
        }

		// GET: Clientes/Details/5
		[Authorize(Roles = "EMPLEADO,ADMINISTRADOR")] //min 1:25:51 Clase9: detail para Admin y empleado.
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cliente == null)
            {
                return NotFound();
            }

            var cliente = await _context.Cliente
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create(IdentityUser? user)
        {
            if (user == null) return NotFound();

            Cliente cliente = new()
            {
                Email = user.Email
            };

            return View(cliente);
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Apellido,Direccion,Telefono,FechaNacimiento,Email")] Cliente cliente) // lo que recibimos del formulario
        {
            cliente.NumeroCliente = await GenerarNumeroCliente();
            cliente.FechaAlta = DateTime.Now;
            cliente.Activo = true;

            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

		// GET: Clientes/Edit/5
		[Authorize(Roles = "CLIENTE")]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cliente == null)
            {
                return NotFound();
            }

            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Edit(int id, [Bind("NumeroCliente,Id,Nombre,Apellido,Direccion,Telefono,FechaNacimiento,FechaAlta,Activo,Email")] Cliente cliente) 
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        //Metodos privados
        private bool ClienteExists(int id)
        {
          return (_context.Cliente?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        
        private async Task<int> GenerarNumeroCliente()
        {
            var cliente = await _context.Cliente.OrderByDescending(c =>
            c.NumeroCliente).FirstOrDefaultAsync();

            if(cliente == null)
            {
                return 4200000;
            }

            return (int)cliente.NumeroCliente + 1; // casteamos para evitar que rompa; si el cliente pasado es null
    }
    }

    
}
