using System.Diagnostics;
using CrudMVC.Data;
using CrudMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace CrudMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataAccess _dataAccess;
        public HomeController(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public IActionResult Index()
        {
            try
            {
                var usuarios = _dataAccess.listarUsuarios();
                return View(usuarios);
            }
            catch (Exception ex)
            {
                return View();
            }

        }

        public IActionResult Cadastrar()
        {
            return View();
        }

        public IActionResult Detalhes(Guid id)
        {
            var usuario = _dataAccess.BuscarUsuarioPorId(id);
            return View(usuario);
        }

        public IActionResult Editar(Guid id)
        {
            var usuario = _dataAccess.BuscarUsuarioPorId(id);
            return View(usuario);
        }

        public IActionResult Remover(Guid id)
        {
            var result = _dataAccess.RemoverUsuario(id);
            if (result)
            {
                TempData["MensagemSucesso"] = "Usuário removido com sucesso!";
            }
            else
            {
                TempData["MensagemErro"] = "Ocorreu um erro ao tentar remover o usuário";            
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Cadastrar(Usuario usuario)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    _dataAccess.CadastrarUsuario(usuario);
                    TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao cadastrar usuário: " + ex.Message);
                    return View(usuario);
                }
            }
            else
            {
                return View(usuario);
            }
        }

        [HttpPost]
        public IActionResult Editar(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = _dataAccess.EditarUsuario(usuario);
                    if (result) 
                    {
                        TempData["MensagemSucesso"] = "Usuário editado com sucesso!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["MensagemErro"] = "Ocorreu um erro ao tentar editar o usuário";
                        return View(usuario);
                    }
    
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao editar usuário: " + ex.Message);
                    return View(usuario);
                }
            }
            else
            {
                return View(usuario);
            }
        }
    }
}
