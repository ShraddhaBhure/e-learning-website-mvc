﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using C_Models;
using C_Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace E_LearningMVC.Controllers
{
    public class HomeProjectsController : Controller
    {
        private readonly IHomeProjectsRepository _repository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly INotyfService _notyf;

        public HomeProjectsController(IHomeProjectsRepository repository, INotyfService notyf, IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
            _notyf = notyf;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _repository.GetAllProjects();
            return View(projects);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HomeProjects project, IFormFile coverImage)
        {
            if (ModelState.IsValid)
            {
                if (coverImage != null && coverImage.Length > 0)
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + coverImage.FileName;
                    var filePath = Path.Combine(imagePath, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await coverImage.CopyToAsync(fileStream);
                    }
                    project.CoverImage = uniqueFileName;
                }

                await _repository.AddProject(project);
                _notyf.Success("Project Added Sucessfully");
                return RedirectToAction(nameof(Index));
            }

            return View(project);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var project = await _repository.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HomeProjects project, IFormFile coverImage)
        {
            if (id != project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (coverImage != null && coverImage.Length > 0)
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + coverImage.FileName;
                    var filePath = Path.Combine(imagePath, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await coverImage.CopyToAsync(fileStream);
                    }
                    project.CoverImage = uniqueFileName;
                }

                await _repository.UpdateProject(project);
                _notyf.Information("Project Edited Sucessfully ");
                return RedirectToAction(nameof(Index));
            }

            return View(project);
        }

        //[HttpGet]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var project = await _repository.GetProjectById(id);
        //    if (project == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(project);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var project = await _repository.GetProjectById(id);
        //    if (project == null)
        //    {
        //        return NotFound();
        //    }

        //    await _repository.DeleteProject(id);
        //    return RedirectToAction(nameof(Index));
        //}


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _repository.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int projectId)
        {
            var project = await _repository.GetProjectById(projectId);
            if (project == null)
            {
                return NotFound();
            }

            await _repository.DeleteProject(projectId);
            _notyf.Warning("Project Deleted Sucessfully");

            return RedirectToAction(nameof(Index));
        }



    }
}
