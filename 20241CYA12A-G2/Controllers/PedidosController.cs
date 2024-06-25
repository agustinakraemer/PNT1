using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _20241CYA12A_G2.Models;
using Microsoft.AspNetCore.Identity;

namespace _20241CYA12A_G2.Controllers
{
    public class PedidosController : Controller
    {
        private readonly DbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PedidosController(DbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

		public async Task<IActionResult> CreateItem()
		{

			var user = await _userManager.GetUserAsync(User);
			var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.Email.ToUpper() == user.NormalizedEmail);

			// Buscar carrito Cliente
			// consultar carrito cliente

			var carritoCliente = await _context.Pedido
	            .Include(p => p.Carrito)
	            .FirstOrDefaultAsync(p => p.Carrito.ClienteId == cliente.Id && p.Estado == 1);

			if (carritoCliente != null)
			{
				return NotFound();
			}


			//Obtener pedidos confirmados de los ultimos 30 dias
			// Consultar pedidos de los ultimos 30 dias

			var pedidosUltimos30 = await _context.Pedido
				.Include(p => p.Carrito)
				.Where(p => p.Carrito.ClienteId == cliente.Id && p.Estado == 2  && carritoCliente.FechaCompra.Date <= (DateTime.Now.Date).AddDays(-30))
				.ToListAsync();


            double gastoEnvio = 80;

			// obtenemos costo de envio:
			// validar cantidad
			if (pedidosUltimos30.Count > 10)
			{
                gastoEnvio = 0;
			}

			//TODO: consultar api del clima

			var temp = 1;
            var llueve = false;
            var temperaturaMinima = 5;
            if ( llueve || temp < temperaturaMinima)
            {
                gastoEnvio *= 1.5;
            }

            // generar detalle pedido
            carritoCliente.GastoEnvio = (decimal)gastoEnvio;
			_context.Update(carritoCliente);
			await _context.SaveChangesAsync();

			// redirigir a la vista del detalle del carrito
			return RedirectToAction("Index", "CarritoItems/Details/" + carritoCliente.CarritoId);

		}

		public async Task<IActionResult> GenerarPedido()
		{

			var user = await _userManager.GetUserAsync(User);
			var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.Email.ToUpper() == user.NormalizedEmail);

			// Buscar pedido Cliente

			var carritoCliente = await _context.Pedido
				.Include(p => p.Carrito)
				.FirstOrDefaultAsync(p => p.Carrito.ClienteId == cliente.Id && p.Estado == 1);

			if (carritoCliente != null)
			{
				return NotFound();
			}

            //generar numero de pedido
            var nroPedido = await GenerarNumeroPedido();

            // generar detalle pedido
            carritoCliente.Estado = 2;
            carritoCliente.NroPedido = nroPedido;
			_context.Update(carritoCliente);

			// guardar pedido
			await _context.SaveChangesAsync();

			//mostar detalle y numero pedido
			return RedirectToAction("Index", "Pedidos/Details/" + nroPedido);

		}

        public async Task<int> GenerarNumeroPedido()
        {
			var ultimoPedido = await _context.Pedido.FirstOrDefaultAsync();

			var nroPedido = 0;

			if (ultimoPedido == null)
            {
                nroPedido = 30000;
            }
            else
            {
                nroPedido = ultimoPedido.NroPedido + 5;
            }

            return nroPedido;

		}

		// GET: Pedidos
		public async Task<IActionResult> Index()
        {
            var pedidos = await _context.Pedido 
                .Include(p=>p.Carrito)
                .Include(p=>p.Carrito.Cliente)
                .ToListAsync();

            var usuario = await _userManager.GetUserAsync(User);

            var pedidosCliente = pedidos.Where(p=>p.Carrito.Cliente.Email==usuario.Email).ToList();

            return View(pedidosCliente);
        }

        // GET: Pedidos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pedido == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido
                .Include(p => p.Carrito)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }


        // GET: Pedidos/Create
        public IActionResult Create()
        {
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id");
            return View();
        }

        // POST: Pedidos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NroPedido,FechaCompra,Subtotal,GastoEnvio,Total,Estado,CarritoId")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", pedido.CarritoId);
            return View(pedido);
        }

        // GET: Pedidos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pedido == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", pedido.CarritoId);
            return View(pedido);
        }

        // POST: Pedidos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NroPedido,FechaCompra,Subtotal,GastoEnvio,Total,Estado,CarritoId")] Pedido pedido)
        {
            if (id != pedido.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.Id))
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
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", pedido.CarritoId);
            return View(pedido);
        }

        // GET: Pedidos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pedido == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido
                .Include(p => p.Carrito)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Pedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pedido == null)
            {
                return Problem("Entity set 'DbContext.Pedido'  is null.");
            }
            var pedido = await _context.Pedido.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedido.Remove(pedido);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(int id)
        {
          return (_context.Pedido?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
