using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _20241CYA12A_G2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace _20241CYA12A_G2.Controllers

{
    public class CarritoItemsController : Controller
    {
        private readonly DbContext _context;
		private readonly UserManager<IdentityUser> _userManager;

		public CarritoItemsController(DbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "CLIENTE")]
		public async Task<IActionResult> CreateOrEditItem(int productoId)
		{

            var producto = await _context.Producto.FindAsync(productoId);
            if (producto.Stock < 1)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
			var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.Email.ToUpper() == user.NormalizedEmail);

            var pedidoPendiente = await _context.Pedido
                .Include(p =>p.Carrito)
                .FirstOrDefaultAsync(p=>p.Carrito.ClienteId == cliente.Id && p.Estado == 1);
            if (pedidoPendiente != null)
            {
                return NotFound();   
            }

            var pedidosDelDia = await _context.Pedido
                .Include(p=>p.Carrito)
                .Where(p=>p.Carrito.ClienteId == cliente.Id && pedidoPendiente.FechaCompra.Date == DateTime.Now.Date)
                .ToListAsync();

            if (pedidosDelDia.Count > 3)
            {
                return NotFound();
            }

            var carrito = await _context.Carrito.Include(c => c.CarritoItems).FirstOrDefaultAsync(c => c.ClienteId == cliente.Id && c.Procesado == false && c.Cancelado == false);
            
            if(carrito == null)
            {
                carrito = new Carrito
                {
                    ClienteId = cliente.Id,
                    Procesado = false,
                    Cancelado = false,
                    CarritoItems = new List<CarritoItem>()
                };

                _context.Add(carrito);
                await _context.SaveChangesAsync();
            }

            var item = carrito.CarritoItems.FirstOrDefault(ci=>ci.ProductoId == productoId);

          
            decimal precioProducto = producto.Precio;

            if(item == null)
            {
                item = new CarritoItem
                {
                    CarritoId = carrito.Id,
                    ProductoId = producto.Id,
                    PrecioUnitarioConDescuento = precioProducto,
                    Cantidad = 1,
                };
                _context.Add(item);
                await _context.SaveChangesAsync();
            }
            else             //buscar descuento
            {
                var descuento = item.Producto.Descuentos.FirstOrDefault(c => c.Producto.Id == productoId);
                if(descuento != null) 
                {
                    decimal porentajeDescuento = (item.PrecioUnitarioConDescuento * (descuento.Porcentaje / 100));
                    item.PrecioUnitarioConDescuento -= porentajeDescuento;



                }
                    item.Cantidad++;
                _context.Update(item);
                await _context.SaveChangesAsync();
            }

            producto.Stock--;
            _context.Update(producto);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Carritos");
		}

		// GET: CarritoItems
		public async Task<IActionResult> Index()
        {
            var dbContext = _context.CarritoItem.Include(c => c.Carrito).Include(c => c.Producto);
            return View(await dbContext.ToListAsync());
        }

        // GET: CarritoItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CarritoItem == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItem
                .Include(c => c.Carrito)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carritoItem == null)
            {
                return NotFound();
            }

            return View(carritoItem);
        }

        // GET: CarritoItems/Create
        public IActionResult Create()
        {
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id");
            ViewData["ProductoId"] = new SelectList(_context.Set<Producto>(), "Id", "Id");
            return View();
        }

        // POST: CarritoItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PrecioUnitarioConDescuento,Cantidad,ProductoId,CarritoId")] CarritoItem carritoItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carritoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", carritoItem.CarritoId);
            ViewData["ProductoId"] = new SelectList(_context.Set<Producto>(), "Id", "Id", carritoItem.ProductoId);
            return View(carritoItem);
        }

        // GET: CarritoItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CarritoItem == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItem.FindAsync(id);
            if (carritoItem == null)
            {
                return NotFound();
            }
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", carritoItem.CarritoId);
            ViewData["ProductoId"] = new SelectList(_context.Set<Producto>(), "Id", "Id", carritoItem.ProductoId);
            return View(carritoItem);
        }

        // POST: CarritoItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PrecioUnitarioConDescuento,Cantidad,ProductoId,CarritoId")] CarritoItem carritoItem)
        {
            if (id != carritoItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carritoItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoItemExists(carritoItem.Id))
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
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", carritoItem.CarritoId);
            ViewData["ProductoId"] = new SelectList(_context.Set<Producto>(), "Id", "Id", carritoItem.ProductoId);
            return View(carritoItem);
        }

        // GET: CarritoItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CarritoItem == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItem
                .Include(c => c.Carrito)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carritoItem == null)
            {
                return NotFound();
            }

            return View(carritoItem);
        }

        // POST: CarritoItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CarritoItem == null)
            {
                return Problem("Entity set 'DbContext.CarritoItem'  is null.");
            }
            var carritoItem = await _context.CarritoItem.FindAsync(id);
            if (carritoItem != null)
            {
                _context.CarritoItem.Remove(carritoItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarritoItemExists(int id)
        {
          return (_context.CarritoItem?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
