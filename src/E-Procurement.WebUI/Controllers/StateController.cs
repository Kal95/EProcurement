﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Web.Mvc.Alerts;
using AutoMapper;
using E_Procurement.Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using E_Procurement.Repository.StateRepo;
using E_Procurement.WebUI.Models.StateModel;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Microsoft.IdentityModel.Tokens;
using static E_Procurement.WebUI.Enums.Enums;
using Microsoft.AspNetCore.Authorization;

namespace E_Procurement.WebUI.Controllers
{
    [Authorize]
    public class StateController : BaseController
    {
        private readonly IStateRepository _stateRepository;
        private readonly IMapper _mapper;

        public StateController(IStateRepository stateRepository, IMapper mapper)
        {
            _stateRepository = stateRepository;
            _mapper = mapper;
        }


        // GET: State
        public ActionResult Index()
        {
            //List<State> model = new List<State>();
            try
            {
               var model = _stateRepository.GetStates().ToList();

                List<StateModel> smodel = _mapper.Map<List<StateModel>>(model);
                return View(smodel);
            }
            catch (Exception )
            {
                return View("Error");
            }
        }

        // GET: State/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: State/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: State/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StateModel model)
        {
            try
            {
                string message;
                model.CreatedBy = User.Identity.Name;

                if (ModelState.IsValid)
                { 

                    var status = _stateRepository.CreateState(model, out message);
                    
                    if (status == true)
                    {

                        Alert("State Created Successfully", NotificationType.success);

                    }

                    else
                    {
                        Alert("This State Already Exists", NotificationType.info); 
                        return View(model);
                    }

                    return RedirectToAction("Index", "State");
                }
                else
                {
                    ViewBag.StatusCode = 2;
                    Alert("State Wasn't Created", NotificationType.error);

                    return View(model);

                }
            }

            catch (Exception)
            {
               
                return View("Error");
            }
        }

        // GET: State/Edit/5
        public ActionResult Edit(int StateId)
        {
          

            StateModel Model = new StateModel();

            try
            {
               
                var state = _stateRepository.GetStates().Where(u => u.Id == StateId).FirstOrDefault();

                if (state == null)
                {
                    Alert("This State Doesn't Exist", NotificationType.warning);

                    return RedirectToAction("Index", "State");
                }

                Model.Id = state.Id;

                Model.StateName = state.StateName;


                Model.IsActive = state.IsActive;

                return View(Model);
            }
            catch (Exception)
            {
                
                return View("Error");
            }
           
        }

        // POST: State/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StateModel Model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    string message;

                    var state = _stateRepository.GetStates().FirstOrDefault(u => u.Id == Model.Id);

                    if (state == null) { Alert("This State Doesn't Exist", NotificationType.warning); return RedirectToAction("Index", "State"); }

                    Model.UpdatedBy = User.Identity.Name;

                    var status = _stateRepository.UpdateState(Model, out message);
                    

                    if (status == true)
                    {
                        Alert("State Updated Successfully", NotificationType.success);
                      
                    }

                    else
                    {
                        Alert("This State Already Exists", NotificationType.info);
                        return View(Model);
                    }

                    return RedirectToAction("Index", "State");
                }
                else
                {
                    ViewBag.StatusCode = 2;

                    Alert("State Wasn't Updated", NotificationType.error);

                    return View(Model);
                }
            }
            catch (Exception)
            {
                
                return View("Error");
            }
        }

        // GET: State/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: State/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}