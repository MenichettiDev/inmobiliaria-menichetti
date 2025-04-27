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
            try
            {
                if (ModelState.IsValid)
                {
                    _tipoRepository.Add(tipo);
                    TempData["SuccessMessage"] = "Tipo de inmueble cargado correctamente.";
                    return RedirectToAction("Listar");
                }
                return View(tipo);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error al crear el tipo de inmueble: {ex.Message}";

                return View("Insertar", tipo);
            }
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
            try
            {
                if (id != tipo.IdTipoInmueble) return BadRequest();

                if (ModelState.IsValid)
                {
                    _tipoRepository.Update(tipo);
                    TempData["SuccessMessage"] = "Tipo de inmueble modificado correctamente.";
                    return RedirectToAction("Listar");
                }
                return View(tipo);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error al modificar el tipo de Inmueble: {ex.Message}";

                return View("Editar", tipo);
            }
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
            try
            {
                _tipoRepository.Delete(id);
                TempData["SuccessMessage"] = "Tipo de inmueble eliminado correctamente.";
                return RedirectToAction("Listar");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error al eliminar el tipo de Inmueble: {ex.Message}";

                var tipo = _tipoRepository.GetById(id);
                return View("Eliminar", tipo); // Mostramos la misma vista de confirmación con el error
            }
        }
    }
}
