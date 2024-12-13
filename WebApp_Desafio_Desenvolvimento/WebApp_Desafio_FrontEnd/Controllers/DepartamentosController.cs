﻿using AspNetCore.Reporting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using WebApp_Desafio_FrontEnd.ApiClients.Desafio_API;
using WebApp_Desafio_FrontEnd.ViewModels;
using WebApp_Desafio_FrontEnd.ViewModels.Enums;

namespace WebApp_Desafio_FrontEnd.Controllers
{
    public class DepartamentosController : Controller
    {
        private readonly IHostingEnvironment _hostEnvironment;

        public DepartamentosController(IHostingEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Listar));
        }

        [HttpGet]
        public IActionResult Listar()
        {
            // Busca de dados está na Action Datatable()
            return View();
        }

        [HttpGet]
        public IActionResult Datatable()
        {
            try
            {
                var departamentosApiClient = new DepartamentosApiClient();
                var lstDepartamentos = departamentosApiClient.DepartamentosListar();

                var dataTableVM = new DataTableAjaxViewModel()
                {
                    length = lstDepartamentos.Count,
                    data = lstDepartamentos
                };

                return Ok(dataTableVM);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseViewModel(ex));
            }
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            ViewData["Title"] = "Cadastrar Novo Departamento";
            return View("Cadastrar", new DepartamentoViewModel());
        }

        [HttpPost]
        public IActionResult Cadastrar(DepartamentoViewModel departamentoVM)
        {
            try
            {
                var departamentosApiClient = new DepartamentosApiClient();
                var realizadoComSucesso = departamentosApiClient.DepartamentoGravar(departamentoVM);

                if (realizadoComSucesso)
                {
                    return Ok(new ResponseViewModel(
                                $"Departamento gravado com sucesso!",
                                AlertTypes.success,
                                this.RouteData.Values["controller"].ToString(),
                                nameof(Listar)));
                }
                else
                {
                    return BadRequest(new ResponseViewModel("Falha ao gravar o Departamento."));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseViewModel(ex));
            }
        }

        [HttpGet]
        public IActionResult Editar([FromRoute] int id)
        {
            ViewData["Title"] = "Editar Departamento";

            try
            {
                var departamentosApiClient = new DepartamentosApiClient();
                var departamentoVM = departamentosApiClient.DepartamentoObter(id);

                return View("Cadastrar", departamentoVM);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseViewModel(ex));
            }
        }

        [HttpDelete]
        public IActionResult Excluir([FromRoute] int id)
        {
            try
            {
                var departamentosApiClient = new DepartamentosApiClient();
                var realizadoComSucesso = departamentosApiClient.DepartamentoExcluir(id);

                if (realizadoComSucesso)
                {
                    return Ok(new ResponseViewModel(
                                $"Departamento {id} excluído com sucesso!",
                                AlertTypes.success,
                                "Departamentos",
                                nameof(Listar)));
                }
                else
                {
                    throw new ApplicationException($"Falha ao excluir o Departamento {id}.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseViewModel(ex));
            }
        }

        [HttpGet]
        public IActionResult Report()
        {
            string mimeType = string.Empty;
            int extension = 1;
            string contentRootPath = _hostEnvironment.ContentRootPath;
            string path = Path.Combine(contentRootPath, "wwwroot", "reports", "rptDepartamentos.rdlc");

            LocalReport localReport = new LocalReport(path);

            var departamentosApiClient = new DepartamentosApiClient();
            var lstDepartamentos = departamentosApiClient.DepartamentosListar();

            localReport.AddDataSource("dsDepartamentos", lstDepartamentos);

            ReportResult reportResult = localReport.Execute(RenderType.Pdf);

            return File(reportResult.MainStream, "application/octet-stream", "rptDepartamentos.pdf");
        }
    }
}