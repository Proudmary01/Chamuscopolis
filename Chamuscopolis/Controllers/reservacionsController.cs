using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using Chamuscopolis.Models;
using Newtonsoft.Json.Linq;
// using SendGrid's C# Library
// https://github.com/sendgrid/sendgrid-csharp
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Chamuscopolis.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("")]
    public class reservacionsController : ApiController
    {
        private prosgEntities db = new prosgEntities();

        // GET: api/reservacions
        [Route]
        public IQueryable<reservacion> Getreservacions()
        {
            return db.reservacions;
        }

        // GET: api/reservacions/cancha/
        [ResponseType(typeof(reservacion))]
        [Route]
        public async Task<IHttpActionResult> Getreservacion(int id)
        {
            reservacion reservacion = await db.reservacions.FindAsync(id);
            if (reservacion == null)
            {
                return NotFound();
            }

            return Ok(reservacion);
        }

        // GET: api/reservacions/5
        [ResponseType(typeof(reservacion))]
        [Route]
        public IHttpActionResult GetHorariosReservados(int idCancha, string fecha)
        {
            List<string> horas = new List<string>();
            List<reservacion> reservaciones = db.reservacions.Where(reser => reser.CANCHA_idCANCHA == idCancha ).ToList();
            List<reservacion> resFiltro = new List<reservacion>();

            foreach (reservacion reservacion in reservaciones)
            {
                if (reservacion.fecha.Substring(0, 10) == fecha)
                { 
                    resFiltro.Add(reservacion);
                }
            }

            if (resFiltro.Count == 0)
            {
                return Ok(horas);
            }

            foreach (reservacion r in resFiltro)
            {
                horas.Add(r.hora.ToString());
            }
            return Ok(horas);
        }

        // PUT: api/reservacions/5
        [ResponseType(typeof(void))]
        [Route]
        public async Task<IHttpActionResult> Putreservacion(int id, reservacion reservacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reservacion.idRESERVACION)
            {
                return BadRequest();
            }

            db.Entry(reservacion).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!reservacionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/reservacions
        [ResponseType(typeof(reservacion))]
        [Route]
        public async Task<IHttpActionResult> Postreservacion(string json)
        {
            JObject o = JObject.Parse(json);
            reservacion reservacion = new reservacion();

            try
            {
                reservacion.monto = (decimal)o["monto"];
                reservacion.CANCHA_idCANCHA = (int)o["idCancha"];
                reservacion.cancha = db.canchas.FirstOrDefault(x => x.idCANCHA == reservacion.CANCHA_idCANCHA);
                reservacion.USUARIO_idUSUARIO = (int)o["idUsuario"];
                reservacion.usuario = db.usuarios.FirstOrDefault(x => x.idUSUARIO == reservacion.USUARIO_idUSUARIO);
                reservacion.hora = (string)o["fecha"];
                reservacion.fecha = (string)o["fecha"];
            }
            catch (Exception e)
            {
                return BadRequest(ModelState);
            }

            db.reservacions.Add(reservacion);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var n = ex;
                if (reservacionExists(reservacion.idRESERVACION))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            //Aqui va el envio de un correo.
            Execute().Wait();
            return CreatedAtRoute("DefaultApi", new { id = reservacion.idRESERVACION }, reservacion);
        }

        // send email method
        static async Task Execute()
        {
            var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("wil.25_r@yahoo.com", "Williams Yahoo");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("willy.kira_25@hotmail.com", "Williams Hotmail");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

        // DELETE: api/reservacions/5
        [ResponseType(typeof(reservacion))]
        [Route]
        public async Task<IHttpActionResult> Deletereservacion(int id)
        {
            reservacion reservacion = await db.reservacions.FindAsync(id);
            if (reservacion == null)
            {
                return NotFound();
            }

            db.reservacions.Remove(reservacion);
            await db.SaveChangesAsync();

            return Ok(reservacion);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool reservacionExists(int id)
        {
            return db.reservacions.Count(e => e.idRESERVACION == id) > 0;
        }
    }
}