using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DriverManagementWebApp.Models;

namespace DriverManagementWebApp.Controllers
{
    public class ParcelController : BaseController
    {
        private Entities db = new Entities();

        // GET: Parcel
        public ActionResult Index()
        {
            var deliveries = db.deliveries.Include(d => d.account).Include(d => d.account1);
            return View(deliveries.ToList());
        }

        // GET: Parcel/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            delivery delivery = db.deliveries.Find(id);
            if (delivery == null)
            {
                return HttpNotFound();
            }
            return View(delivery);
        }

        // GET: Parcel/Create
        public ActionResult Create()
        {
            ViewBag.Delivery_Driver_ID = new SelectList(db.accounts.Where(x => !x.IsDispatchManager), "UserID", "Name");
            DeliveryViewModel model = new DeliveryViewModel();
            model.Delivery = new delivery();
            model.NotifyMe = false;
            return View(model);
        }

        // POST: Parcel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DeliveryViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.NotifyMe)
                {
                    model.Delivery.Delivery_DispatchManager_ID = UserInfoModel.UserID;
                }
                model.Delivery.Status = "Not Delivered";
                model.Delivery.Delivery_Driver_ID = model.Delivery_Driver_ID;
                db.deliveries.Add(model.Delivery);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Delivery_Driver_ID = new SelectList(db.accounts.Where(x => !x.IsDispatchManager), "UserID", "Name", model.Delivery.Delivery_Driver_ID);
            return View(model);
        }

        // GET: Parcel/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            delivery delivery = db.deliveries.Find(id);
            if (delivery == null)
            {
                return HttpNotFound();
            }
            ViewBag.Delivery_DispatchManager_ID = new SelectList(db.accounts, "UserID", "UserName", delivery.Delivery_DispatchManager_ID);
            ViewBag.Delivery_Driver_ID = new SelectList(db.accounts, "UserID", "UserName", delivery.Delivery_Driver_ID);
            return View(delivery);
        }

        // POST: Parcel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DeliveryID,ParcelID,CustomerContactNo,CustomerName,Delivery_Driver_ID,Delivery_DispatchManager_ID,DeliveryDateTime,Address,Status")] delivery delivery)
        {
            if (ModelState.IsValid)
            {
                db.Entry(delivery).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Delivery_DispatchManager_ID = new SelectList(db.accounts, "UserID", "UserName", delivery.Delivery_DispatchManager_ID);
            ViewBag.Delivery_Driver_ID = new SelectList(db.accounts, "UserID", "UserName", delivery.Delivery_Driver_ID);
            return View(delivery);
        }

        // GET: Parcel/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            delivery delivery = db.deliveries.Find(id);
            if (delivery == null)
            {
                return HttpNotFound();
            }
            return View(delivery);
        }

        // POST: Parcel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            delivery delivery = db.deliveries.Find(id);
            db.deliveries.Remove(delivery);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
