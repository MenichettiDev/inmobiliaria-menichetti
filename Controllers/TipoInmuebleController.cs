using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaApp.Controllers
{
    public class TipoInmuebleController : Controller
    {
        private readonly TipoInmuebleRepository _tipoRepository;

        public TipoInmuebleController(TipoInmuebleRepository tipoRepository)
        {
            _tipoRepository = tipoRepository;
        }

        public IActionResult Listar()
        {
            var tipos = _tipoRepository.GetAll();
            return View(tipos);
        }

        public IActionResult Detalles(int id)
        {
            var tipo = _tipoRepository.GetById(id);
            if (tipo == null) return NotFound();
            return View(tipo);
        }

        public IActionResult Insertar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Insertar(TipoInmueble tipo)
        {
            if (ModelState.IsValid)
            {
                _tipoRepository.Add(tipo);
                return RedirectToAction("Listar");
            }
            return View(tipo);
        }

        public IActionResult Editar(int id)
        {
            var tipo = _tipoRepository.GetById(id);
            if (tipo == null) return NotFound();
            return View(tipo);
        }

        [HttpPost]
        public IActionResult Editar(int id, TipoInmueble tipo)
        {
            if (id != tipo.IdTipoInmueble) return BadRequest();

            if (ModelState.IsValid)
            {
                _tipoRepository.Update(tipo);
                return RedirectToAction("Listar");
            }
            return View(tipo);
        }

        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var tipo = _tipoRepository.GetById(id);
            if (tipo == null) return NotFound();
            return View(tipo);
        }

        [HttpPost, ActionName("Eliminar")]
        [Authorize(Policy = "Administrador")]
        public IActionResult EliminarConfirmado(int id)
        {
            _tipoRepository.Delete(id);
            return RedirectToAction("Listar");
        }
    }
}
