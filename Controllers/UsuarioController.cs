using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories;

namespace InmobiliariaApp.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UsuarioRepository _usuarioRepo;

        public UsuarioController(UsuarioRepository usuarioRepo)
        {
            _usuarioRepo = usuarioRepo;
        }

        public IActionResult Listar()
        {
            var usuarios = _usuarioRepo.GetAll();
            return View(usuarios);
        }

        public IActionResult Detalles(int id)
        {
            var usuario = _usuarioRepo.GetById(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        public IActionResult Insertar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Insertar(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _usuarioRepo.Add(usuario);
                return RedirectToAction("Listar");
            }
            return View(usuario);
        }

        public IActionResult Editar(int id)
        {
            var usuario = _usuarioRepo.GetById(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [HttpPost]
        public IActionResult Editar(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario) return BadRequest();

            if (ModelState.IsValid)
            {
                _usuarioRepo.Update(usuario);
                return RedirectToAction("Listar");
            }
            return View(usuario);
        }

        public IActionResult Eliminar(int id)
        {
            var usuario = _usuarioRepo.GetById(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            _usuarioRepo.Delete(id);
            return RedirectToAction("Listar");
        }
    }
}
